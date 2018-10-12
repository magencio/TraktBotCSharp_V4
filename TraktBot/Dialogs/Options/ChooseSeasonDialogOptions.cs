using Microsoft.Bot.Builder;
using TraktBot.Model;

namespace TraktBot.Dialogs.Options
{
    /// <summary>
    /// Options for <see cref="ChooseSeasonDialog"/>.
    /// </summary>
    public class ChooseSeasonDialogOptions
    {
        public string Question { get; set; }

        public RecognizerResult RecognizerResult { get; set; }

        public Show Show { get; set; }
    }
}
