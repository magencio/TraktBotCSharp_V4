using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alejacma.Bot.Dialogs;
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
    /// The root dialog of the bot.
    /// </summary>
    public class RootDialog : IntentDialog
    {
        private const string Token = "Token";
        private const string ShowTitle = "ShowTitle";

        private readonly ITraktTv traktTv;

        private readonly List<CardAction> suggestedActions = new List<CardAction>()
        {
            new CardAction() { Type = ActionTypes.ImBack, Title = "Trending shows", Value = "Trending shows" },
            new CardAction() { Type = ActionTypes.ImBack, Title = "Popular shows", Value = "Popular shows" },
            new CardAction() { Type = ActionTypes.ImBack, Title = "Recommend shows", Value = "Recommend shows" },
            new CardAction() { Type = ActionTypes.ImBack, Title = "Search show", Value = "Search show" },
            new CardAction() { Type = ActionTypes.ImBack, Title = "Show status", Value = "Show status" },
            new CardAction() { Type = ActionTypes.ImBack, Title = "Episode watched", Value = "Episode watched" },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="RootDialog"/> class.
        /// </summary>
        /// <param name="recognizer">Intent recognizer.</param>
        /// <param name="traktTv">Trakt.tv client.</param>
        /// <param name="telemetry">Telemetry client.</param>
        public RootDialog(IRecognizer recognizer, ITraktTv traktTv, IBotTelemetry telemetry)
            : base(nameof(RootDialog), recognizer)
        {
            this.traktTv = traktTv;

            Matches(TraktBotNLP.IntentHi, OnHiAsync).
            Matches(TraktBotNLP.IntentThx, OnThxAsync).
            Matches(TraktBotNLP.IntentBye, OnByeAsync).
            Matches(TraktBotNLP.IntentTrending, OnTrendingAsync).
            Matches(TraktBotNLP.IntentPopular, OnPopularAsync).
            Matches(TraktBotNLP.IntentRecommendations, OnRecommendations()).
            Matches(TraktBotNLP.IntentSearch, OnSearch()).
            Matches(TraktBotNLP.IntentStatus, OnStatus()).
            Matches(TraktBotNLP.IntentWatched, OnWatched()).
            Matches(TraktBotNLP.IntentHelp, OnHelpAsync).
            Matches(TraktBotNLP.IntentCancel, OnCancelAsync).
            Matches(TraktBotNLP.IntentLogout, OnLogoutAsync).
            Default(OnUnknownAsync);
        }

        private async Task<DialogTurnResult> OnHiAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendSuggestedActionsAsync("Hi there! What can I do for you today?|Hello! How can I help you?", suggestedActions, cancellationToken);
            return EndOfTurn;
        }

        private async Task<DialogTurnResult> OnThxAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("You are welcome|Don't mention it", cancellationToken: cancellationToken);
            return EndOfTurn;
        }

        private async Task<DialogTurnResult> OnByeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Good bye!|Bye, bye!", cancellationToken: cancellationToken);
            return EndOfTurn;
        }

        private async Task<DialogTurnResult> OnTrendingAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                var shows = await traktTv.GetTrendingShowsAsync();
                await SendShowsAsync(stepContext, "These are the trending shows right now:", shows, cancellationToken);
                await SendWhatNowAsync(stepContext, cancellationToken);
                return EndOfTurn;
            }
            catch (Exception ex)
            {
                return await OnTraktTvErrorAsync(stepContext, ex, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> OnPopularAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                var shows = await traktTv.GetPopularShowsAsync();
                await SendShowsAsync(stepContext, "These are the most popular shows at the moment:", shows, cancellationToken);
                await SendWhatNowAsync(stepContext, cancellationToken);
                return EndOfTurn;
            }
            catch (Exception ex)
            {
                return await OnTraktTvErrorAsync(stepContext, ex, cancellationToken);
            }
        }

        private WaterfallStep[] OnRecommendations() => new WaterfallStep[]
        {
            async (stepContext, cancellationToken) =>
            {
                return await stepContext.BeginDialogAsync(
                    nameof(ConfirmationDialog),
                    new ConfirmationDialogOptions()
                    {
                        Question = "Do you want personalized recommendations?",
                    },
                    cancellationToken);
            },
            async (stepContext, cancellationToken) =>
            {
                switch (stepContext.Result)
                {
                    case bool confirmation when confirmation:
                        return await stepContext.BeginDialogAsync(nameof(TraktTvLoginPrompt), cancellationToken: cancellationToken);
                    case bool confirmation when !confirmation:
                        return await OnPopularAsync(stepContext, cancellationToken);
                    default:
                        return await OnChildCancelAsync(stepContext, cancellationToken);
                }
            },
            async (stepContext, cancellationToken) =>
            {
                switch (stepContext.Result)
                {
                    case TokenResponse tokenResponse:
                        try
                        {
                            var user = await traktTv.GetUserNameAsync(tokenResponse.Token);
                            var shows = await traktTv.GetRecommendedShowsAsync(tokenResponse.Token);
                            await SendShowsAsync(stepContext, $"These are the top recommendations for you {user}:", shows, cancellationToken);
                            await SendWhatNowAsync(stepContext, cancellationToken);
                            return EndOfTurn;
                        }
                        catch (Exception ex)
                        {
                            return await OnTraktTvErrorAsync(stepContext, ex, cancellationToken);
                        }

                    default:
                        return await OnAuthenticationErrorAsync(stepContext, cancellationToken);
                }
            },
        };

        private WaterfallStep[] OnSearch() => new WaterfallStep[]
        {
            async (stepContext, cancellationToken) =>
            {
                return await stepContext.BeginDialogAsync(
                    nameof(ChooseShowDialog),
                    new ChooseShowDialogOptions()
                    {
                        Question = "Please, tell me the name of the show you are looking for",
                        RecognizerResult = stepContext.Values[RecognizerResult] as RecognizerResult,
                    },
                    cancellationToken);
            },
            async (stepContext, cancellationToken) =>
            {
                switch (stepContext.Result)
                {
                    case Show show:
                        await SendShowsAsync(stepContext, "Here you have it:", new[] { show }, cancellationToken);
                        await SendWhatNowAsync(stepContext, cancellationToken);
                        return EndOfTurn;
                    case Exception ex:
                        return await OnTraktTvErrorAsync(stepContext, ex, cancellationToken);
                    default:
                        return await OnChildCancelAsync(stepContext, cancellationToken);
                }
            },
        };

        private WaterfallStep[] OnStatus() => new WaterfallStep[]
        {
            async (stepContext, cancellationToken) =>
            {
                return await stepContext.BeginDialogAsync(
                    nameof(ChooseShowDialog),
                    new ChooseShowDialogOptions()
                    {
                        Question = "Please, tell me the name of the show which status you are looking for",
                        RecognizerResult = stepContext.Values[RecognizerResult] as RecognizerResult,
                    },
                    cancellationToken);
            },
            async (stepContext, cancellationToken) =>
            {
                switch (stepContext.Result)
                {
                    case Show show:
                        await SendStatusAsync(stepContext, show, cancellationToken);
                        await SendWhatNowAsync(stepContext, cancellationToken);
                        return EndOfTurn;
                    case Exception ex:
                        return await OnTraktTvErrorAsync(stepContext, ex, cancellationToken);
                    default:
                        return await OnChildCancelAsync(stepContext, cancellationToken);
                }
            },
        };

        private WaterfallStep[] OnWatched() => new WaterfallStep[]
        {
            async (stepContext, cancellationToken) =>
            {
                return await stepContext.BeginDialogAsync(nameof(TraktTvLoginPrompt), cancellationToken: cancellationToken);
            },
            async (stepContext, cancellationToken) =>
            {
                switch (stepContext.Result)
                {
                    case TokenResponse tokenResponse:
                        stepContext.Values[Token] = tokenResponse.Token;
                        return await stepContext.BeginDialogAsync(
                            nameof(ChooseShowDialog),
                            new ChooseShowDialogOptions()
                            {
                                Question = "Please, tell me the name of the show which episode you watched",
                                RecognizerResult = stepContext.Values[RecognizerResult] as RecognizerResult,
                                GetSeasons = true,
                            },
                            cancellationToken);
                    default:
                        return await OnAuthenticationErrorAsync(stepContext, cancellationToken);
                }
            },
            async (stepContext, cancellationToken) =>
            {
                switch (stepContext.Result)
                {
                    case Show show:
                        stepContext.Values[ShowTitle] = show.Title;
                        return await stepContext.BeginDialogAsync(
                            nameof(ChooseSeasonDialog),
                            new ChooseSeasonDialogOptions()
                            {
                                Question = "Please, tell me the season of the episode you watched",
                                RecognizerResult = stepContext.Values[RecognizerResult] as RecognizerResult,
                                Show = show,
                            },
                            cancellationToken);
                    case Exception ex:
                        return await OnTraktTvErrorAsync(stepContext, ex, cancellationToken);
                    default:
                        return await OnChildCancelAsync(stepContext, cancellationToken);
                }
            },
            async (stepContext, cancellationToken) =>
            {
                switch (stepContext.Result)
                {
                    case Season season:
                        return await stepContext.BeginDialogAsync(
                            nameof(ChooseEpisodeDialog),
                            new ChooseEpisodeDialogOptions()
                            {
                                Question = "Please, tell me the episode you watched",
                                RecognizerResult = stepContext.Values[RecognizerResult] as RecognizerResult,
                                Season = season,
                            },
                            cancellationToken);
                    case Exception ex:
                        return await OnTraktTvErrorAsync(stepContext, ex, cancellationToken);
                    default:
                        return await OnChildCancelAsync(stepContext, cancellationToken);
                }
            },
            async (stepContext, cancellationToken) =>
            {
                switch (stepContext.Result)
                {
                    case Episode episode:
                        var token = stepContext.Values[Token] as string;
                        var showTitle = stepContext.Values[ShowTitle] as string;
                        try
                        {
                            var user = await traktTv.GetUserNameAsync(token);
                            if (await traktTv.AddWatchedEpisodeAsync(token, episode.IdsTrakt))
                            {
                                await stepContext.Context.SendActivityAsync($"Ok {user}, I marked this episode as watched: {showTitle} {episode.Season}x{episode.Number}", cancellationToken: cancellationToken);
                            }
                            else
                            {
                                await stepContext.Context.SendActivityAsync($"Sorry {user}, I couldn't mark this episode as watched: {showTitle} {episode.Season}x{episode.Number}", cancellationToken: cancellationToken);
                            }

                            await SendWhatNowAsync(stepContext, cancellationToken);
                            return EndOfTurn;
                        }
                        catch (Exception ex)
                        {
                            return await OnTraktTvErrorAsync(stepContext, ex, cancellationToken);
                        }

                    case Exception ex:
                        return await OnTraktTvErrorAsync(stepContext, ex, cancellationToken);
                    default:
                        return await OnChildCancelAsync(stepContext, cancellationToken);
                }
            },
        };

        private async Task<DialogTurnResult> OnHelpAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendSuggestedActionsAsync("I can answer different questions about tv shows", suggestedActions, cancellationToken);
            return EndOfTurn;
        }

        private async Task<DialogTurnResult> OnCancelAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("There is nothing for me to cancel here", cancellationToken: cancellationToken);
            await SendWhatNowAsync(stepContext, cancellationToken);
            return EndOfTurn;
        }

        private async Task<DialogTurnResult> OnLogoutAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var botAdapter = stepContext.Context.Adapter as BotFrameworkAdapter;
            await botAdapter.SignOutUserAsync(stepContext.Context, TraktTvLoginPrompt.ConnectionName, cancellationToken: cancellationToken);
            await stepContext.Context.SendActivityAsync("Your wish is my command. You are logged out now", cancellationToken: cancellationToken);
            await SendWhatNowAsync(stepContext, cancellationToken);
            return EndOfTurn;
        }

        private async Task<DialogTurnResult> OnChildCancelAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Sure thing!|If you say so...|Of course", cancellationToken: cancellationToken);
            await SendWhatNowAsync(stepContext, cancellationToken);
            return EndOfTurn;
        }

        private async Task<DialogTurnResult> OnUnknownAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendSuggestedActionsAsync("Sorry, I didn't understand that. How can I help you?", suggestedActions, cancellationToken);
            return EndOfTurn;
        }

        private async Task<DialogTurnResult> OnTraktTvErrorAsync(WaterfallStepContext stepContext, Exception e, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync($"Oops! Something went wrong while connecting to trakt.tv service: *{e.Message}*", cancellationToken: cancellationToken);
            await SendWhatNowAsync(stepContext, cancellationToken);
            return EndOfTurn;
        }

        private async Task<DialogTurnResult> OnAuthenticationErrorAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("You failed to authenticate", cancellationToken: cancellationToken);
            await SendWhatNowAsync(stepContext, cancellationToken);
            return EndOfTurn;
        }

        private async Task SendStatusAsync(WaterfallStepContext stepContext, Show show, CancellationToken cancellationToken)
        {
            switch (show.Status)
            {
                case "ended":
                    await stepContext.Context.SendActivityAsync("This show has ended", cancellationToken: cancellationToken);
                    break;
                case "returning series":
                    await stepContext.Context.SendActivityAsync("This show is returning", cancellationToken: cancellationToken);
                    break;
                case "canceled":
                    await stepContext.Context.SendActivityAsync("This show has been canceled", cancellationToken: cancellationToken);
                    break;
                case "in production":
                    await stepContext.Context.SendActivityAsync("This show is in production", cancellationToken: cancellationToken);
                    break;
                default:
                    await stepContext.Context.SendActivityAsync($"The status of this show is: {show.Status}", cancellationToken: cancellationToken);
                    break;
            }
        }

        private async Task SendShowsAsync(WaterfallStepContext stepContext, string textReplyToSend, IEnumerable<Show> shows, CancellationToken cancellationToken)
            => await stepContext.Context.SendHeroCardsAsync(textReplyToSend, shows.Select(s => ToHeroCard(s)), cancellationToken: cancellationToken);

        private HeroCard ToHeroCard(Show show)
            => new HeroCard
            {
                Title = show.Title,
                Text = show.Overview,
                Subtitle = show.Status,
                Buttons = new List<CardAction>()
                {
                    new CardAction(ActionTypes.OpenUrl, title: "Trakt.tv", value: $"https://trakt.tv/shows/{show.IdsTrakt}"),
                },
            };

        private async Task SendWhatNowAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            => await stepContext.Context.SendSuggestedActionsAsync("What else can I do for you?", suggestedActions, cancellationToken);
    }
}
