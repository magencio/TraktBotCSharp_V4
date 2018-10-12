using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace TraktBot.State
{
    public interface IBotAccessors
    {
        /// <summary>
        /// User state.
        /// </summary>
        UserState UserState { get; }

        /// <summary>
        /// Conversation state.
        /// </summary>
        ConversationState ConversationState { get; }

        /// <summary>
        /// Dialog state accessor.
        /// </summary>
        IStatePropertyAccessor<DialogState> DialogState { get; set; }
    }
}