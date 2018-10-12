using System;
using System.Threading;
using System.Threading.Tasks;
using Alejacma.Bot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using TraktBot.Dialogs;
using TraktBot.NLP;
using TraktBot.State;

namespace TraktBot
{
    /// <summary>
    /// TraktBot.
    /// </summary>
    public class Bot : Alejacma.Bot.Bot
    {
        public Bot(BotServices services, BotAccessors accessors)
            : base(accessors, nameof(RootDialog))
        {
            var recognizer = services.Recognizers[TraktBotNLP.ModelName];
            var traktTv = services.TraktTv;
            var telemetry = services.Telemetry;

            dialogs.Add(new RootDialog(recognizer, traktTv, telemetry));
            dialogs.Add(new ConfirmationDialog(recognizer, telemetry));
            dialogs.Add(new ChooseShowDialog(recognizer, traktTv, telemetry));
            dialogs.Add(new ChooseSeasonDialog(recognizer, telemetry));
            dialogs.Add(new ChooseEpisodeDialog(recognizer, telemetry));
            dialogs.Add(new TraktTvLoginPrompt());
        }

        protected override async Task OnMemberAddedToConversationAsync(DialogContext dc, Activity activity, CancellationToken cancellationToken)
        {
            await dc.Context.SendAdaptiveCardAsync("TraktBot.Dialogs.AdaptiveCards.WelcomeCard.json", cancellationToken);
        }
    }
}
