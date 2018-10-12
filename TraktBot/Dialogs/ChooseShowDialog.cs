using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Dialog to find the details of a show by name.
    /// </summary>
    public class ChooseShowDialog : IntentDialog
    {
        private const string TraktIdPrefix = "traktId:";

        private readonly ITraktTv traktTv;

        private readonly List<CardAction> suggestedActions = new List<CardAction>()
        {
            new CardAction() { Type = ActionTypes.ImBack, Title = "Cancel", Value = "Cancel" },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseShowDialog"/> class.
        /// </summary>
        /// <param name="recognizer">Intent recognizer.</param>
        /// <param name="traktTv">Trakt.tv client.</param>
        /// <param name="telemetry">Telemetry client.</param>
        public ChooseShowDialog(IRecognizer recognizer, ITraktTv traktTv, IBotTelemetry telemetry)
            : base(nameof(ChooseShowDialog), recognizer)
        {
            this.traktTv = traktTv;

            Begin(OnBeginAsync).
            Matches(TraktBotNLP.IntentHelp, OnHelpAsync).
            Matches(TraktBotNLP.IntentCancel, OnCancelAsync).
            Default(OnAnythingElseAsync);
        }

        private async Task<DialogTurnResult> OnBeginAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var options = stepContext.Options as ChooseShowDialogOptions ?? throw new ArgumentException(nameof(stepContext.Options));

            if (options.RecognizerResult.GetEntityValue(TraktBotNLP.EntityShow) is string showName)
            {
                showName = FixShowName(showName);
                return await OnFindShowAsync(stepContext, showName, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendSuggestedActionsAsync(options.Question, suggestedActions, cancellationToken);
                return EndOfTurn;
            }
        }

        private async Task<DialogTurnResult> OnHelpAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var options = stepContext.Options as ChooseShowDialogOptions;
            await stepContext.Context.SendSuggestedActionsAsync(options.Question, suggestedActions, cancellationToken);
            return EndOfTurn;
        }

        private async Task<DialogTurnResult> OnCancelAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            => await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

        private async Task<DialogTurnResult> OnAnythingElseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var showName = stepContext.Context.Activity.Text;

            if (showName.StartsWith(TraktIdPrefix)
                && int.TryParse(showName.Replace(TraktIdPrefix, string.Empty), out int traktId))
            {
                try
                {
                    var show = await traktTv.GetShowSummaryAsync(traktId, true);
                    if (stepContext.Options is ChooseShowDialogOptions options && options.GetSeasons)
                    {
                        show.Seasons = await traktTv.GetSeasonsAsync(show.IdsTrakt);
                    }

                    return await stepContext.EndDialogAsync(show, cancellationToken);
                }
                catch (Exception ex)
                {
                    return await OnTraktTvErrorAsync(stepContext, ex, cancellationToken);
                }
            }

            return await OnFindShowAsync(stepContext, showName, cancellationToken);
        }

        private async Task<DialogTurnResult> OnTraktTvErrorAsync(WaterfallStepContext stepContext, Exception e, CancellationToken cancellationToken)
            => await stepContext.EndDialogAsync(e, cancellationToken);

        private async Task<DialogTurnResult> OnFindShowAsync(WaterfallStepContext stepContext, string showName, CancellationToken cancellationToken)
        {
            showName = showName.ToLower();

            List<Show> shows = null;
            try
            {
                shows = await traktTv.SearchShowsAsync(showName);
            }
            catch (Exception ex)
            {
                return await OnTraktTvErrorAsync(stepContext, ex, cancellationToken);
            }

            if (shows.Where(s => s.Title.ToLower() == showName).FirstOrDefault() is Show show)
            {
                if (stepContext.Options is ChooseShowDialogOptions options && options.GetSeasons)
                {
                    show.Seasons = await traktTv.GetSeasonsAsync(show.IdsTrakt);
                }

                return await stepContext.EndDialogAsync(show, cancellationToken);
            }
            else if (shows.Count() > 0)
            {
                await stepContext.Context.SendHeroCardsAsync("I couldn't find any show with that exact name, but I found shows with similar names. Please select one or enter a new show name:", shows.Select(s => ToHeroCard(s)), suggestedActions, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendSuggestedActionsAsync($"I couldn't find a show with name '{showName}'. Please enter a new name", suggestedActions, cancellationToken);
            }

            return EndOfTurn;
        }

        private string FixShowName(string showName)
            => showName.Replace(" '", "'").Replace("' ", "'"); // LUIS adds spaces to entity values containing apostrophes

        private HeroCard ToHeroCard(Show show)
            => new HeroCard
            {
                Title = show.Title,
                Buttons = new List<CardAction>()
                {
                    new CardAction(ActionTypes.PostBack, title: "Select", value: $"{TraktIdPrefix}{show.IdsTrakt}"),
                },
            };
    }
}
