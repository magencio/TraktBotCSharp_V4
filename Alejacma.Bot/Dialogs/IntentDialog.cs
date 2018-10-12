using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Reflection;
using Alejacma.Bot.Recognizers;

namespace Alejacma.Bot.Dialogs
{
    /// <summary>
    /// Dialog to handle intents.
    ///
    /// Code and comments based on https://github.com/Microsoft/botbuilder-dotnet/blob/master/libraries/Microsoft.Bot.Builder.Dialogs/WaterfallDialog.cs
    /// and https://github.com/Microsoft/botbuilder-dotnet/blob/master/libraries/Microsoft.Bot.Builder.Dialogs/Dialog.cs
    ///
    /// Note: WaterfallStepContext inherits from DialogContext, which has an internal constructor. This means I can't create my own IntentDialog context.
    /// So I'm creating a WaterfallStepContext via reflection, and for that to work, IntentDialog must inherit from WaterfallDialog instead of Dialog.
    /// </summary>
    public class IntentDialog : WaterfallDialog
    {
        /// <summary>
        /// The index to access the last recognizer result from the Values collection in WaterfallStepContext.
        /// </summary>
        public const string RecognizerResult = "recognizerResult";

        private const string PersistedOptions = "options";
        private const string PersistedValues = "values";
        private const string CurrentSteps = "steps";
        private const string CurrentStepIndex = "stepIndex";

        private const string BeginSteps = "begin";
        private const string IntentStepsPrefix = "intent:";
        private const string DefaultSteps = "default";

        private readonly IRecognizer recognizer;
        private readonly Dictionary<string, List<WaterfallStep>> steps = new Dictionary<string, List<WaterfallStep>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="IntentDialog"/> class.
        /// </summary>
        /// <param name="dialogId">The id of the dialog.</param>
        /// <param name="recognizer">The intent recognizer.</param>
        public IntentDialog(string dialogId, IRecognizer recognizer)
            : base(dialogId)
            => this.recognizer = recognizer ?? throw new ArgumentNullException(nameof(recognizer));

        /// <summary>
        /// Sets the waterfall to execute when the dialog begins.
        /// If this waterfall doesn't exists, the dialog will run the recognizer against the current activity text
        /// to find a suitable intent handler.
        /// </summary>
        /// <param name="step">The step execute.</param>
        /// <returns>This dialog.</returns>
        public IntentDialog Begin(WaterfallStep step)
            => Begin(new WaterfallStep[] { step ?? throw new ArgumentNullException(nameof(step)) });

        /// <summary>
        /// Sets the waterfall to execute when the dialog begins.
        /// If this waterfall doesn't exists, the dialog will run the recognizer against the current activity text
        /// to find a suitable intent handler.
        /// </summary>
        /// <param name="steps">The steps to execute.</param>
        /// <returns>This dialog.</returns>
        public IntentDialog Begin(IEnumerable<WaterfallStep> steps)
        {
            this.steps[BeginSteps] = new List<WaterfallStep>(steps ?? throw new ArgumentNullException(nameof(steps)));
            return this;
        }

        /// <summary>
        /// Sets the waterfall to execute when an intent is matched.
        /// </summary>
        /// <param name="intent">The intent name.</param>
        /// <param name="step">The step to execute.</param>
        /// <returns>This dialog.</returns>
        public IntentDialog Matches(string intent, WaterfallStep step)
            => Matches(intent, new WaterfallStep[] { step ?? throw new ArgumentNullException(nameof(step)) });

        /// <summary>
        /// Sets the waterfall to execute when an intent is matched.
        /// </summary>
        /// <param name="intent">The intent name.</param>
        /// <param name="steps">The steps to execute.</param>
        /// <returns>This dialog.</returns>
        public IntentDialog Matches(string intent, IEnumerable<WaterfallStep> steps)
        {
            this.steps[$"{IntentStepsPrefix}{intent}"] = new List<WaterfallStep>(steps ?? throw new ArgumentNullException(nameof(steps)));
            return this;
        }

        /// <summary>
        /// Sets the default waterfall to execute when no intent is matched.
        /// </summary>
        /// <param name="step">The step to execute.</param>
        /// <returns>This dialog.</returns>
        public IntentDialog Default(WaterfallStep step)
            => Default(new WaterfallStep[] { step ?? throw new ArgumentNullException(nameof(step)) });

        /// <summary>
        /// Sets the default waterfall to execute when no intent is matched.
        /// </summary>
        /// <param name="steps">The steps to execute.</param>
        /// <returns>This dialog.</returns>
        public IntentDialog Default(IEnumerable<WaterfallStep> steps)
        {
            this.steps[DefaultSteps] = new List<WaterfallStep>(steps ?? throw new ArgumentNullException(nameof(steps)));
            return this;
        }

        /// <summary>
        /// Method called when a new dialog has been pushed onto the stack and is being activated.
        /// </summary>
        /// <param name="dc">The dialog context for the current turn of conversation.</param>
        /// <param name="options">(Optional) arguments that were passed to the dialog during `begin()` call that started the instance.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public override async Task<DialogTurnResult> BeginDialogAsync(
            DialogContext dc,
            object options = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            dc = dc ?? throw new ArgumentNullException(nameof(dc));

            // Initialize dialog state
            var state = dc.ActiveDialog.State;
            state[PersistedOptions] = options;
            state[PersistedValues] = new Dictionary<string, object>();

            // Run first step
            return await RunStepAsync(dc, BeginSteps, 0, DialogReason.BeginCalled, dc.Context.Activity.Text, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Method called when an instance of the dialog is the "current" dialog and the
        /// user replies with a new activity. The dialog will generally continue to receive the users
        /// replies until it calls either `DialogSet.end()` or `DialogSet.begin()`.
        /// </summary>
        /// <param name="dc">The dialog context for the current turn of conversation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public override async Task<DialogTurnResult> ContinueDialogAsync(
            DialogContext dc,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            dc = dc ?? throw new ArgumentNullException(nameof(dc));

            // Don't do anything for non-message activities
            if (dc.Context.Activity.Type != ActivityTypes.Message)
            {
                return EndOfTurn;
            }

            // Ensure we are done with previous waterfall. Then run next step with the message text as the result.
            var state = dc.ActiveDialog.State;
            var steps = state[CurrentSteps] as string;
            var index = steps.Length;
            return await RunStepAsync(dc, steps, index, DialogReason.ContinueCalled, dc.Context.Activity.Text, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Method called when an instance of the dialog is being returned to from another
        /// dialog that was started by the current instance using `DialogSet.begin()`.
        /// Any result passed from the called dialog will be passed to the current dialogs parent.
        /// </summary>
        /// <param name="dc">The dialog context for the current turn of conversation.</param>
        /// <param name="reason">Reason why the dialog resumed.</param>
        /// <param name="result">(Optional) value returned from the dialog that was called. The type of the value returned is dependant on the dialog that was called.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public override async Task<DialogTurnResult> ResumeDialogAsync(
            DialogContext dc,
            DialogReason reason,
            object result = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            dc = dc ?? throw new ArgumentNullException(nameof(dc));

            // Increment step index and run step
            var state = dc.ActiveDialog.State;
            var steps = state[CurrentSteps] as string;
            var index = Convert.ToInt32(state[CurrentStepIndex]) + 1;
            return await RunStepAsync(dc, steps, index, reason, result, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes one step in a waterfall.
        /// </summary>
        /// <param name="currentSteps">Current waterfall being executed.</param>
        /// <param name="stepContext">Context of the step.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        protected virtual async Task<DialogTurnResult> OnStepAsync(
            string currentSteps,
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
            => await steps[currentSteps][stepContext.Index](stepContext, cancellationToken).ConfigureAwait(false);

        private async Task<DialogTurnResult> RunStepAsync(
            DialogContext dc,
            string currentSteps,
            int currentStepIndex,
            DialogReason reason,
            object result,
            CancellationToken cancellationToken)
        {
            dc = dc ?? throw new ArgumentNullException(nameof(dc));

            // Do we know which waterfall to execute? Did we already finish executing current waterfall?
            RecognizerResult recognizerResult = null;
            if (!steps.ContainsKey(currentSteps) || currentStepIndex >= steps[currentSteps].Count())
            {
                // If so, decide on the next waterfall to execute
                recognizerResult = await recognizer.RecognizeAsync(dc.Context, cancellationToken).ConfigureAwait(false);
                var topIntent = recognizerResult.GetTopIntent();
                currentSteps = $"{IntentStepsPrefix}{topIntent}";
                if (!steps.ContainsKey(currentSteps))
                {
                    if (steps.ContainsKey(DefaultSteps))
                    {
                        currentSteps = DefaultSteps;
                    }
                    else
                    {
                        // There is no suitable waterfall, so just return any result to parent
                        return await dc.EndDialogAsync(result, cancellationToken).ConfigureAwait(false);
                    }
                }

                currentStepIndex = 0;
            }

            // Update persisted step info
            var state = dc.ActiveDialog.State;
            state[CurrentSteps] = currentSteps;
            state[CurrentStepIndex] = currentStepIndex;

            // Create step context
            var options = state[PersistedOptions];
            var values = (IDictionary<string, object>)state[PersistedValues];
            if (recognizerResult != null)
            {
                values[RecognizerResult] = recognizerResult;
            }

            var stepContext = CreateInstance<WaterfallStepContext>(this, dc, options, values, currentStepIndex, reason, result);

            // Execute step
            return await OnStepAsync(currentSteps, stepContext, cancellationToken).ConfigureAwait(false);
        }

        private T CreateInstance<T>(params object[] args)
        {
            var type = typeof(T);
            var instance = type.Assembly.CreateInstance(
                type.FullName,
                false,
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                args,
                null,
                null);
            return (T)instance;
        }
    }
}