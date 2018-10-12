using System.Collections.Generic;

namespace TraktBot.Model
{
    public class Show
    {
        // Min
        public string Title { get; set; }

        public int IdsTrakt { get; set; }

        // Full
        public string Overview { get; set; }

        public string Status { get; set; }

        // Seasons
        public List<Season> Seasons { get; set; } = new List<Season>();
    }
}
