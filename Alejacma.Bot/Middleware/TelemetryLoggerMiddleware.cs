using System;
using System.Threading;
using System.Threading.Tasks;
using Alejacma.Bot.Telemetry;
using Microsoft.Bot.Builder;

/// <summary>
/// Code based on https://github.com/Microsoft/BotBuilder-Samples/blob/master/samples/csharp_dotnetcore/21.luis-with-appinsights/AppInsights/MyAppInsightsLoggerMiddleware.cs.
/// </summary>
namespace Alejacma.Bot.Middleware
{
    /// <summary>
    /// Middleware for logging incoming and outgoing Activity messages into telemetry service.
    /// In addition, registers the telemetry client in the context so other telemetry
    /// components like <see cref="TelemetryLuisRecognizer"/> can log telemetry.
    /// If this <see cref="IMiddleware"/> is removed, all the other components don't log (but still operate).
    /// </summary>
    public class TelemetryLoggerMiddleware : IMiddleware
    {
        /// <summary>
        /// The name of the telemetry service in the Context TurnState collection.
        /// </summary>
        public static readonly string TelemetryKey = $"{nameof(TelemetryLoggerMiddleware)}.TelemetryContext";

        private readonly IBotTelemetry telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryLoggerMiddleware"/> class.
        /// </summary>
        /// <param name="telemetry">The Application Insights client.</param>
        public TelemetryLoggerMiddleware(IBotTelemetry telemetry)
        {
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        /// <summary>
        /// Records incoming and outgoing activities to the Application Insights store.
        /// </summary>
        /// <param name="context">The <see cref="ITurnContext"/> object for this turn.</param>
        /// <param name="nextTurn">The delegate to call to continue the bot middleware pipeline.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext context, NextDelegate nextTurn, CancellationToken cancellationToken)
        {
            BotAssert.ContextNotNull(context);

            // Allow other components to log telemetry
            context.TurnState.Add(TelemetryKey, telemetry);

            // Log incoming activity at beginning of turn
            if (context.Activity != null)
            {
                telemetry.TrackActivity(context.Activity);
            }

            // Hook up onSend pipeline and log outgoing activities
            context.OnSendActivities(async (ctx, activities, nextSend) =>
            {
                var responses = await nextSend().ConfigureAwait(false);
                activities.ForEach(a => telemetry.TrackActivity(a));
                return responses;
            });

            if (nextTurn != null)
            {
                await nextTurn(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
