/// <summary>
/// Code based on https://github.com/Azure/botbuilder-instrumentation-cs/blob/master/botbuilder-instumentation/Telemetry/TelemetryLogger.cs.
/// </summary>
namespace Alejacma.Bot.Telemetry
{
    /// <summary>
    /// Properties that are stored within the telemetry service events.
    /// </summary>
    public static class TelemetryEventProperties
    {
        public const string TimeStampProperty = "timestamp";
        public const string TypeProperty = "type";
        public const string ActivityIdProperty = "id";
        public const string ChannelIdProperty = "channel";
        public const string ConversationIdProperty = "conversationId";
        public const string UserIdProperty = "userId";
        public const string UserNameProperty = "userName";
        public const string TextProperty = "text";
        public const string LocaleProperty = "locale";
        public const string IntentProperty = "intent";
        public const string ScoreProperty = "score";
        public const string EntitiesProperty = "entities";
        public const string GoalNameProperty = "goalName";
        public const string ExceptionProperty = "exception";
    }
}
