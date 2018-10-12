namespace TraktBot.NLP
{
    /// <summary>
    /// Intent and Entity names.
    /// </summary>
    public static class TraktBotNLP
    {
        public const string ModelName = "TraktBot.EN";

        public const string IntentHi = "Education_Hi";
        public const string IntentThx = "Education_Thx";
        public const string IntentBye = "Education_Bye";
        public const string IntentTrending = "Shows_Trending";
        public const string IntentPopular = "Shows_Popular";
        public const string IntentRecommendations = "Shows_Recommendations";
        public const string IntentSearch = "Shows_Search";
        public const string IntentStatus = "Shows_Status";
        public const string IntentWatched = "Shows_Watched";
        public const string IntentCancel = "Command_Cancel";
        public const string IntentHelp = "Command_Help";
        public const string IntentLogout = "Command_Logout";
        public const string IntentYes = "Confirmation_Yes";
        public const string IntentNo = "Confirmation_No";

        public const string EntityShow = "Show";
        public const string EntityEpisode = "Episode";
        public const string EntityEpisodeNumber = "EpisodeNumber";
        public const string EntitySeasonNumber = "SeasonNumber";
    }
}
