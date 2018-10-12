using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace TraktBot.State
{
    public class BotAccessors : IBotAccessors
    {
        public BotAccessors(UserState userState, ConversationState conversationState)
        {
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));

            DialogState = conversationState.CreateProperty<DialogState>(nameof(DialogState));
        }

        public UserState UserState { get; }

        public ConversationState ConversationState { get; }

        public IStatePropertyAccessor<DialogState> DialogState { get; set; }
    }
}
