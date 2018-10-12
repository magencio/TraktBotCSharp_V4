using Newtonsoft.Json;
using System.Collections.Generic;

namespace Alejacma.TraktTv.Model
{
    public class Season
    {
        public class SeasonIds
        {
            [JsonProperty("trakt")]
            public int Trakt { get; set; }

            [JsonProperty("tvdb")]
            public int? Tvdb { get; set; }

            [JsonProperty("tmdb")]
            public int? Tmdb { get; set; }

            [JsonProperty("tvrage")]
            public int? Tvrage { get; set; }
        }

        // Min

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("ids")]
        public SeasonIds Ids { get; set; }

        // Full

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("votes")]
        public int Votes { get; set; }

        [JsonProperty("episode_count")]
        public int EpisodeCount { get; set; }

        [JsonProperty("aired_episodes")]
        public int AiredEpisodes { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        // Episodes

        [JsonProperty("episodes")]
        public List<Episode> Episodes { get; set; }


    }
}
