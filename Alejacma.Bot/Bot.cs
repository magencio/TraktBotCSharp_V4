using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alejacma.Bot.Services;
using Alejacma.Bot.Telemetry;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using TraktBot.State;

namespace Alejacma.Bot
{
    /// <summary>
    /// Base class for a bot that provides default handling of activities.
    /// </summary>
    public class Bot : IBot
    {
        protected readonly IBotAccessors accessors;
        protected readonly string rootDialogName;
        protected readonly DialogSet dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bot"/> class.
        /// </summary>
        /// <param name="accessors">Bot state accessors.</param>
        /// <param name="rootDialogName">Name of the root dialog of the bot.</param>
        public Bot(IBotAccessors accessors, string rootDialogName)
        {            
            this.accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));
            this.rootDialogName = rootDialogName ?? throw new ArgumentNullException(nameof(rootDialogName));

            dialogs = new DialogSet(accessors.DialogState);
        }

        /// <summary>
        /// Runs every turn of the conversation. Handles orchestration of messages.
        /// </summary>
        /// <param name="turnContext">Bot Turn Context.</param>
        /// <param name="cancellationToken">Task CancellationToken.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var activity = turnContext.Activity;

            // Create a dialog context
            var dc = await dialogs.CreateContextAsync(turnContext, cancellationToken);

            // Handle activities
            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    await OnMessageAsync(dc, activity, cancellationToken);
                    break;
                case ActivityTypes.Event:
                case ActivityTypes.Invoke:
                    await OnEventAsync(dc, activity, cancellationToken);
                    break;
                case ActivityTypes.ConversationUpdate:
                    await OnConversationUpdateAsync(dc, activity, cancellationToken);
                    break;
            }

            // Persist any changes to storage.
            await accessors.UserState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
            await accessors.ConversationState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Runs when the bot gets an activity of type Message.
        /// </summary>
        /// <param name="dc">Dialog context.</param>
        /// <param name="activity">Activity.</param>
        /// <param name="cancellationToken">Task CancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual async Task OnMessageAsync(DialogContext dc, Activity activity, CancellationToken cancellationToken)
        {
            // Continue the current dialog
            var dialogResult = await dc.ContinueDialogAsync(cancellationToken);

            // if no one has responded,
            if (!dc.Context.Responded)
            {
                // examine results from active dialog
                switch (dialogResult.Status)
                {
                    case DialogTurnStatus.Empty:
                        await dc.BeginDialogAsync(rootDialogName, cancellationToken: cancellationToken);
                        break;

                    case DialogTurnStatus.Waiting:
                        // The active dialog is waiting for a response from the user, so do nothing.
                        break;

                    case DialogTurnStatus.Complete:
                        await dc.EndDialogAsync(cancellationToken: cancellationToken);
                        break;

                    default:
                        await dc.CancelAllDialogsAsync(cancellationToken);
                        break;
                }
            }
        }

        /// <summary>
        /// Runs when the bot gets an activity of type Event.
        /// </summary>
        /// <param name="dc">Dialog context.</param>
        /// <param name="activity">Activity.</param>
        /// <param name="cancellationToken">Task CancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual async Task OnEventAsync(DialogContext dc, Activity activity, CancellationToken cancellationToken)
        {
            // This handles the MS Teams Invoke Activity sent when magic code is not used during authentication.
            // See: https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/authentication/auth-oauth-card#getting-started-with-oauthcard-in-teams
            // The Teams manifest schema is found here: https://docs.microsoft.com/en-us/microsoftteams/platform/resources/schema/manifest-schema
            // It also handles the Event Activity sent from the emulator when the magic code is not used during authentication.
            // See: https://blog.botframework.com/2018/08/28/testing-authentication-to-your-bot-using-the-bot-framework-emulator/
            await dc.ContinueDialogAsync(cancellationToken);
            if (!dc.Context.Responded)
            {
                await dc.BeginDialogAsync(rootDialogName, cancellationToken: cancellationToken);
            }
        }

        private async Task OnConversationUpdateAsync(DialogContext dc, Activity activity, CancellationToken cancellationToken)
        {
            if (activity.MembersAdded.Any())
            {
                // Iterate over all new members added to the conversation.
                foreach (var member in activity.MembersAdded)
                {
                    // Greet anyone that was not the target (recipient) of this message.
                    if (member.Id != activity.Recipient.Id)
                    {
                        await OnMemberAddedToConversationAsync(dc, activity, cancellationToken);
                    }
                }
            }
        }

        /// <summary>
        /// Runs when a user gets added to the conversation.
        /// </summary>
        /// <param name="dc">Dialog context.</param>
        /// <param name="activity">Activity.</param>
        /// <param name="cancellationToken">Task CancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task OnMemberAddedToConversationAsync(DialogContext dc, Activity activity, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
