using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Alejacma.Bot.Middleware;
using Alejacma.Bot.Telemetry;

/// <summary>
/// Code based on https://github.com/Microsoft/BotBuilder-Samples/blob/master/samples/csharp_dotnetcore/21.luis-with-appinsights/AppInsights/MyAppInsightLuisRecognizer.cs
/// & https://raw.githubusercontent.com/Microsoft/BotBuilder-Samples/master/samples/csharp_dotnetcore/21.luis-with-appinsights/AppInsights/MyLuisConstants.cs.
/// </summary>
namespace Alejacma.Bot.Recognizers
{
    /// <summary>
    /// <see cref="TelemetryLuisRecognizer"/> invokes the Luis Recognizer and logs some results into the telemetry service.
    /// Results are logged if <see cref="TelemetryLoggerMiddleware"/> has been added to the bot.
    /// See <see cref="LuisRecognizer"/> for additional information.
    /// </summary>
    public class TelemetryLuisRecognizer : IRecognizer
    {
        private readonly LuisRecognizer recognizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryLuisRecognizer"/> class.
        /// </summary>
        /// <param name="application">The <see cref="LuisApplication"/> to use to recognize text.</param>
        /// <param name="predictionOptions">(Optional) The <see cref="LuisPredictionOptions"/> to use.</param>
        /// <param name="includeApiResults">(Optional) TRUE to include raw LUIS API response.</param>
        public TelemetryLuisRecognizer(LuisApplication application, LuisPredictionOptions predictionOptions = null, bool includeApiResults = false)
        {
            recognizer = new LuisRecognizer(application, predictionOptions, includeApiResults);
        }

        /// <summary>
        /// Analyzes the current message text and return results of the analysis (Suggested actions and intents).
        /// Logs the results to telemetry service if <see cref="TelemetryLoggerMiddleware"/> has been added to the bot.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing information for a single turn of conversation with a user.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The LUIS <see cref="RecognizerResult"/> of the analysis of the current message text in the current turn's context activity."/>.</returns>
        public async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));

            var recognizerResult = await recognizer.RecognizeAsync(turnContext, cancellationToken);

            if (turnContext.TurnState.TryGetValue(TelemetryLoggerMiddleware.TelemetryKey, out var telemetry) && recognizerResult != null)
            {
                ((BotTelemetry)telemetry).TrackIntent(turnContext.Activity, recognizerResult);
            }

            return recognizerResult;
        }

        /// <summary>
        /// Analyzes the current message text and return results of the analysis (Suggested actions and intents).
        /// Logs the results to telemetry service if <see cref="TelemetryLoggerMiddleware"/> has been added to the bot.
        /// </summary>
        /// <typeparam name="T">Type of the recognizer converter that converts from a generic recognizer result to a strongly typed one.</typeparam>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing information for a single turn of conversation with a user.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The strongly typed results of the analysis of the current message text in the current turn's context activity.</returns>
        public async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken)
            where T : IRecognizerConvert, new()
        {
            var result = new T();
            result.Convert(await RecognizeAsync(turnContext, cancellationToken));
            return result;
        }
    }
}
