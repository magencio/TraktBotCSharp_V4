using Microsoft.Bot.Builder;

namespace TraktBot.Dialogs.Options
{
    /// <summary>
    /// Options for <see cref="ChooseShowDialog"/>.
    /// </summary>
    public class ChooseShowDialogOptions
    {
        public string Question { get; set; }

        public RecognizerResult RecognizerResult { get; set; }

        public bool GetSeasons { get; set; }
    }
}
