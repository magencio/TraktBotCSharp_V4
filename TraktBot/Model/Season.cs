namespace TraktBot.Model
{
    using System.Collections.Generic;

    public class Season
    {
        // Min
        public int Number { get; set; }

        public int IdsTrakt { get; set; }

        // Full
        public int EpisodeCount { get; set; }

        // Episodes
        public List<Episode> Episodes { get; set; } = new List<Episode>();
    }
}
