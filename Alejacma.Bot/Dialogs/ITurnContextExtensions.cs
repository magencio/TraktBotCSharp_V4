using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Alejacma.Bot.Dialogs
{
    /// <summary>
    /// Extension methods for ITurnContext.
    /// </summary>
    public static class ITurnContextExtensions
    {
        /// <summary>
        /// Sends an adaptive card to user.
        /// </summary>
        /// <param name="context">Bot Turn Context.</param>
        /// <param name="embeddedResourceName">Name of the embedded resource containing the json of the adaptive card.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public static async Task<ResourceResponse> SendAdaptiveCardAsync(
            this ITurnContext context,
            string embeddedResourceName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var cardContent = GetEmbeddedResourceContents(embeddedResourceName);
            var reply = context.Activity.CreateReply();
            reply.Attachments = new List<Attachment>()
            {
                new Attachment()
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(cardContent),
                },
            };
            return await context.SendActivityAsync(reply, cancellationToken);
        }

        /// <summary>
        /// Sends a reply with suggested actions to user.
        /// </summary>
        /// <param name="context">Bot Turn Context.</param>
        /// <param name="textReplyToSend">Reply.</param>
        /// <param name="suggestedActions">Suggested actions.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public static async Task<ResourceResponse> SendSuggestedActionsAsync(
            this ITurnContext context,
            string textReplyToSend,
            IEnumerable<CardAction> suggestedActions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var reply = context.Activity.CreateReply(textReplyToSend);
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>(suggestedActions),
            };
            return await context.SendActivityAsync(reply, cancellationToken);
        }

        /// <summary>
        /// Sends a reply with a carousel of hero cards to user.
        /// </summary>
        /// <param name="context">Bot Turn Context.</param>
        /// <param name="textReplyToSend">Reply.</param>
        /// <param name="cards">Hero cards.</param>
        /// <param name="suggestedActions">Suggested actions.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public static async Task<ResourceResponse> SendHeroCardsAsync(
            this ITurnContext context,
            string textReplyToSend,
            IEnumerable<HeroCard> cards,
            IEnumerable<CardAction> suggestedActions = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var reply = context.Activity.CreateReply(textReplyToSend);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = new List<Attachment>(cards.Select(c => c.ToAttachment()));
            if (suggestedActions != null)
            {
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = new List<CardAction>(suggestedActions),
                };
            }

            return await context.SendActivityAsync(reply, cancellationToken);
        }

        private static string GetEmbeddedResourceContents(string embeddedResourceName)
        {
            var assembly = Assembly.GetEntryAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(embeddedResourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
