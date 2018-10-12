using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Alejacma.Bot.Dialogs;
using Alejacma.Bot.Telemetry;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using TraktBot.Dialogs.Options;
using TraktBot.NLP;

namespace TraktBot.Dialogs
{
    /// <summary>
    /// A dialog to ask questions with Yes/No answers using NLP.
    /// </summary>
    public class ConfirmationDialog : IntentDialog
    {
        private readonly List<CardAction> suggestedActions = new List<CardAction>()
        {
            new CardAction() { Title = "Yes", Type = ActionTypes.ImBack, Value = "Yes" },
            new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No" },
            new CardAction() { Title = "Cancel", Type = ActionTypes.ImBack, Value = "Cancel" },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmationDialog"/> class.
        /// </summary>
        /// <param name="recognizer">Intent recognizer.</param>
        /// <param name="telemetry">Telemetry client.</param>
        public ConfirmationDialog(IRecognizer recognizer, IBotTelemetry telemetry)
            : base(nameof(ConfirmationDialog), recognizer)
            => Begin(OnBeginAsync).
                Matches(TraktBotNLP.IntentYes, OnYesAsync).
                Matches(TraktBotNLP.IntentNo, OnNoAsync).
                Matches(TraktBotNLP.IntentHelp, OnHelpAsync).
                Matches(TraktBotNLP.IntentCancel, OnCancelAsync).
                Default(OnUnknownAsync);

        private async Task<DialogTurnResult> OnBeginAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var options = stepContext.Options as ConfirmationDialogOptions ?? throw new ArgumentException(nameof(stepContext.Options));

            await stepContext.Context.SendSuggestedActionsAsync(options.Question, suggestedActions, cancellationToken);
            return EndOfTurn;
        }

        private async Task<DialogTurnResult> OnYesAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            => await stepContext.EndDialogAsync(true, cancellationToken);

        private async Task<DialogTurnResult> OnNoAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            => await stepContext.EndDialogAsync(false, cancellationToken);

        private async Task<DialogTurnResult> OnHelpAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var options = stepContext.Options as ConfirmationDialogOptions;
            await stepContext.Context.SendSuggestedActionsAsync($"Answer Yes or No to the question: {options.Question}", suggestedActions, cancellationToken);
            return EndOfTurn;
        }

        private async Task<DialogTurnResult> OnCancelAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            => await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

        private async Task<DialogTurnResult> OnUnknownAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendSuggestedActionsAsync("Sorry, I didn't understand that.  Please, just answer the question", suggestedActions, cancellationToken);
            return EndOfTurn;
        }
    }
}
