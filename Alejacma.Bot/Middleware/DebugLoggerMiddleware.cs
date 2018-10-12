using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Alejacma.Bot.Middleware
{
    /// <summary>
    /// Middleware that shows all text messages in and out of the bot in the debug console.
    /// </summary>
    public class DebugLoggerMiddleware : IMiddleware
    {
        /// <summary>
        /// Records incoming and outgoing text messages to the debug console.
        /// </summary>
        /// <param name="context">The <see cref="ITurnContext"/> object for this turn.</param>
        /// <param name="next">The delegate to call to continue the bot middleware pipeline.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(
            ITurnContext turnContext,
            NextDelegate next,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            BotAssert.ContextNotNull(turnContext);

            if (turnContext.Activity.Text is string text)
            {
                Debug.WriteLine(text);
            }

            turnContext.OnSendActivities(OnSendActivitiesAsync);

            if (next != null)
            {
                await next(cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<ResourceResponse[]> OnSendActivitiesAsync(
            ITurnContext turnContext,
            List<Microsoft.Bot.Schema.Activity> activities,
            Func<Task<ResourceResponse[]>> next)
        {
            var responses = await next().ConfigureAwait(false);
            activities
                .Where(a => a.Text is string text)
                .ToList()
                .ForEach(a => Debug.WriteLine(a.Text));
            return responses;
        }
    }
}
