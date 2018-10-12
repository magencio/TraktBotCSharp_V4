/// <summary>
/// Code based on https://github.com/Azure/botbuilder-instrumentation-cs/blob/master/botbuilder-instumentation/Telemetry/TelemetryEventTypes.cs
/// </summary>
namespace Alejacma.Bot.Telemetry
{
    /// <summary>
    /// Event types for telemetry service.
    /// </summary>
    public class TelemetryEventTypes
    {
        public const string MessageReceived = "MBFEvent.UserMessage";
        public const string MessageSent = "MBFEvent.BotMessage";
        public const string Intent = "MBFEvent.Intent";
        public const string Sentiment = "MBFEvent.Sentiment";
        public const string ConversationUpdate = "MBFEvent.StartConversation";
        public const string EndOfConversation = "MBFEvent.EndConversation";
        public const string OtherActivity = "MBFEvent.Other";
        public const string CustomEvent = "MBFEvent.CustomEvent";
        public const string GoalTriggeredEvent = "MBFEvent.GoalEvent";
        public const string Exception = "MBFEvent.Exception";
    }
}
