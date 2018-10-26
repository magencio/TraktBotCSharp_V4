using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Alejacma.Bot.Dialogs;
using Alejacma.Bot.Recognizers;
using Alejacma.Bot.Telemetry;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using TraktBot.Dialogs.Options;
using TraktBot.Model;
using TraktBot.NLP;

namespace TraktBot.Dialogs
{
    /// <summary>
    /// Dialog to choose a season number of a show.
    /// </summary>
    public class ChooseSeasonDialog : IntentDialog
    {
        private readonly List<CardAction> suggestedActions = new List<CardAction>()
        {
            new CardAction() { Type = ActionTypes.ImBack, Title = "Cancel", Value = "Cancel" },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseSeasonDialog"/> class.
        /// </summary>
        /// <param name="recognizer">Intent recognizer.</param>
        /// <param name="telemetry">Telemetry client.</param>
        public ChooseSeasonDialog(IRecognizer recognizer, IBotTelemetry telemetry)
            : base(nameof(ChooseSeasonDialog), recognizer)
            => Begin(OnBeginAsync).
                Matches(TraktBotNLP.IntentHelp, OnHelpAsync).
                Matches(TraktBotNLP.IntentCancel, OnCancelAsync).
                Default(OnAnythingElseAsync);

        private async Task<DialogTurnResult> OnBeginAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var options = stepContext.Options as ChooseSeasonDialogOptions ?? throw new ArgumentException(nameof(stepContext.Options));
            var show = options.Show ?? throw new ArgumentNullException(nameof(options.Show));

            if ((options.RecognizerResult.GetEntityValue(TraktBotNLP.EntityEpisode) is string seasonAndEpisode
                    && this.ExtractSeason(seasonAndEpisode) is int seasonNumber)
                || (options.RecognizerResult.GetEntityValue(TraktBotNLP.EntitySeasonNumber) is string season
                    && int.TryParse(season, out seasonNumber)))
            {
                return await OnSeasonNumberAsync(stepContext, seasonNumber, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendSuggestedActionsAsync(options.Question, suggestedActions, cancellationToken);
                return EndOfTurn;
            }
        }

        private async Task<DialogTurnResult> OnHelpAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var maxSeason = GetMaxSeasonNumber(GetShow(stepContext));
            await stepContext.Context.SendSuggestedActionsAsync($"Enter a season number between 1 and {maxSeason}", suggestedActions, cancellationToken);
            return EndOfTurn;
        }

        private async Task<DialogTurnResult> OnCancelAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            => await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

        private async Task<DialogTurnResult> OnAnythingElseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (int.TryParse(stepContext.Context.Activity.Text, out int seasonNumber))
            {
                return await OnSeasonNumberAsync(stepContext, seasonNumber, cancellationToken);
            }
            else
            {
                var maxSeason = GetMaxSeasonNumber(GetShow(stepContext));
                await stepContext.Context.SendSuggestedActionsAsync($"Sorry, '{stepContext.Context.Activity.Text}' is not a valid number. Please, enter a season number between 1 and {maxSeason}", suggestedActions, cancellationToken);
                return EndOfTurn;
            }
        }

        private async Task<DialogTurnResult> OnSeasonNumberAsync(WaterfallStepContext stepContext, int seasonNumber, CancellationToken cancellationToken)
        {
            var show = GetShow(stepContext);
            var maxSeason = GetMaxSeasonNumber(show);

            if (seasonNumber > 0 && seasonNumber <= maxSeason)
            {
                var season = GetSeason(show, seasonNumber);
                return await stepContext.EndDialogAsync(season, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendSuggestedActionsAsync($"Sorry, season {seasonNumber} doesn't exist for that show. Please, enter a season number between 1 and {maxSeason}", suggestedActions, cancellationToken);
                return EndOfTurn;
            }
        }

        private Show GetShow(WaterfallStepContext stepContext)
            => (stepContext.Options as ChooseSeasonDialogOptions).Show;

        private int GetMaxSeasonNumber(Show show)
            => show.Seasons[show.Seasons.Count - 1].Number;

        private Season GetSeason(Show show, int seasonNumber)
            => show.Seasons.First(s => s.Number == seasonNumber);

        private int? ExtractSeason(string seasonAndEpisode)
            => (Regex.IsMatch(seasonAndEpisode, @"^s\d+e\d+$") || Regex.IsMatch(seasonAndEpisode, @"^\d+x\d+$"))
                && int.TryParse(Regex.Match(seasonAndEpisode, @"\d+").Value, out int season)
                ? season
                : default(int?);
    }
}
