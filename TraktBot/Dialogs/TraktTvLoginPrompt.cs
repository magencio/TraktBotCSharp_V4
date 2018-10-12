using Microsoft.Bot.Builder.Dialogs;

namespace TraktBot.Dialogs
{
    /// <summary>
    /// Prompts the user to login on Trakt.tv's via OAuth.
    /// </summary>
    public class TraktTvLoginPrompt : OAuthPrompt
    {
        // The connection name here must match the one from
        // Bot Channels Registration on the settings blade in Azure.
        public const string ConnectionName = "TraktTv";

        public TraktTvLoginPrompt()
            : base(nameof(TraktTvLoginPrompt), new OAuthPromptSettings
            {
                ConnectionName = ConnectionName,
                Text = "First I need to know who you are in Trakt.tv. Please sign in",
                Title = "Sign in",
                Timeout = 300000, // User has 5 minutes to login (1000 * 60 * 5)
            })
        {
        }
    }
}
