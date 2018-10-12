using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Alejacma.Bot.Middleware
{
    /// <summary>
    /// Middleware that randomly selects a reply from a string containing a list of replies separated by '|'.
    /// </summary>
    public class RandomizeReplyMiddleware : IMiddleware
    {
        public Task OnTurnAsync(
            ITurnContext turnContext,
            NextDelegate next,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            turnContext.OnSendActivities(OnSendActivitiesAsync);
            return next(cancellationToken);
        }

        private async Task<ResourceResponse[]> OnSendActivitiesAsync(
            ITurnContext turnContext,
            List<Activity> activities,
            Func<Task<ResourceResponse[]>> next)
        {
            activities
                .Where(a => a.Type == ActivityTypes.Message && a.Text != null)
                .ToList()
                .ForEach(a => a.Text = PickOneReplyRandomly(a.Text));
            return await next();
        }

        private string PickOneReplyRandomly(string text)
        {
            var replies = text.Split('|');
            return replies[new Random().Next(0, replies.Length)];
        }
    }
}
