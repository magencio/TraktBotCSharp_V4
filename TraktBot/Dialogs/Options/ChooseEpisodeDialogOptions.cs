using Microsoft.Bot.Builder;
using TraktBot.Model;

namespace TraktBot.Dialogs.Options
{
    /// <summary>
    /// Options for <see cref="ChooseEpisodeDialog"/>.
    /// </summary>
    public class ChooseEpisodeDialogOptions
    {
        public string Question { get; set; }

        public RecognizerResult RecognizerResult { get; set; }

        public Season Season { get; set; }
    }
}
