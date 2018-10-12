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
    /// Dialog to choose an episode number within a specific season of a show.
    /// </summary>
    public class ChooseEpisodeDialog : IntentDialog
    {
        private readonly List<CardAction> suggestedActions = new List<CardAction>()
        {
            new CardAction() { Type = ActionTypes.ImBack, Title = "Cancel", Value = "Cancel" },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseEpisodeDialog"/> class.
        /// </summary>
        /// <param name="recognizer">Intent recognizer.</param>
        /// <param name="telemetry">Telemetry client.</param>
        public ChooseEpisodeDialog(IRecognizer recognizer, IBotTelemetry telemetry)
            : base(nameof(ChooseEpisodeDialog), recognizer)
            => Begin(OnBeginAsync).
                Matches(TraktBotNLP.IntentHelp, OnHelpAsync).
                Matches(TraktBotNLP.IntentCancel, OnCancelAsync).
                Default(OnAnythingElseAsync);

        private async Task<DialogTurnResult> OnBeginAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var options = stepContext.Options as ChooseEpisodeDialogOptions ?? throw new ArgumentException(nameof(stepContext.Options));
            var season = options.Season ?? throw new ArgumentNullException(nameof(options.Season));

            if ((options.RecognizerResult.GetEntityValue(TraktBotNLP.EntityEpisode) is string seasonAndEpisode
                    && this.ExtractEpisode(seasonAndEpisode) is int episodeNumber)
                || (options.RecognizerResult.GetEntityValue(TraktBotNLP.EntityEpisodeNumber) is string episode
                    && int.TryParse(episode, out episodeNumber)))
            {
                return await OnEpisodeNumberAsync(stepContext, episodeNumber, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendSuggestedActionsAsync(options.Question, suggestedActions, cancellationToken);
                return EndOfTurn;
            }
        }

        private async Task<DialogTurnResult> OnHelpAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var maxEpisode = GetMaxEpisodeNumber(GetSeason(stepContext));
            await stepContext.Context.SendSuggestedActionsAsync($"Enter an episode number between 1 and {maxEpisode}", suggestedActions, cancellationToken);
            return EndOfTurn;
        }

        private async Task<DialogTurnResult> OnCancelAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            => await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

        private async Task<DialogTurnResult> OnAnythingElseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (int.TryParse(stepContext.Context.Activity.Text, out int episodeNumber))
            {
                return await OnEpisodeNumberAsync(stepContext, episodeNumber, cancellationToken);
            }
            else
            {
                var maxEpisode = GetMaxEpisodeNumber(GetSeason(stepContext));
                await stepContext.Context.SendSuggestedActionsAsync($"Sorry, '{stepContext.Context.Activity.Text}' is not a valid number. Please, enter an episode number between 1 and {maxEpisode}", suggestedActions, cancellationToken);
                return EndOfTurn;
            }
        }

        private async Task<DialogTurnResult> OnEpisodeNumberAsync(WaterfallStepContext stepContext, int episodeNumber, CancellationToken cancellationToken)
        {
            var season = GetSeason(stepContext);
            var maxEpisode = GetMaxEpisodeNumber(season);
            if (episodeNumber > 0 && episodeNumber <= maxEpisode)
            {
                var episode = GetEpisode(season, episodeNumber);
                return await stepContext.EndDialogAsync(episode, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendSuggestedActionsAsync($"Sorry, episode {episodeNumber} doesn't exist for that season. Please, enter an episode number between 1 and {maxEpisode}", suggestedActions, cancellationToken);
                return EndOfTurn;
            }
        }

        private Season GetSeason(WaterfallStepContext step)
            => (step.Options as ChooseEpisodeDialogOptions).Season;

        private int GetMaxEpisodeNumber(Season season)
            => season.Episodes[season.Episodes.Count - 1].Number;

        private Episode GetEpisode(Season season, int episodeNumber)
            => season.Episodes.First(e => e.Number == episodeNumber);

        private int? ExtractEpisode(string seasonAndEpisode)
            => (Regex.IsMatch(seasonAndEpisode, @"^s\d+e\d+$") || Regex.IsMatch(seasonAndEpisode, @"^\d+x\d+$"))
                && Regex.Matches(seasonAndEpisode, @"\d+") is MatchCollection matches
                && int.TryParse(matches[1].Value, out int episode)
                ? episode
                : default(int?);
    }
}
