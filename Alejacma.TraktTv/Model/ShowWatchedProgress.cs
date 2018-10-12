using Newtonsoft.Json;
using System.Collections.Generic;

namespace Alejacma.TraktTv.Model
{
    public class ShowWatchedProgress
    {
        public class SeasonWatchedProgress
        {
            public class EpisodeWatchedProgress
            {
                [JsonProperty("number")]
                public int Number { get; set; }

                [JsonProperty("completed")]
                public bool Completed { get; set; }
            }

            [JsonProperty("number")]
            public int Number { get; set; }

            [JsonProperty("aired")]
            public int Aired { get; set; }

            [JsonProperty("completed")]
            public int Completed { get; set; }

            [JsonProperty("episodes")]
            public List<EpisodeWatchedProgress> Episodes { get; set; }
        }

        public class ShowNextEpisode
        {
            public class ShowNextEpisodeIds
            {
                [JsonProperty("trakt")]
                public int Trakt { get; set; }

                [JsonProperty("tvdb")]
                public int? Tvdb { get; set; }

                [JsonProperty("imdb")]
                public string Imdb { get; set; }

                [JsonProperty("tmdb")]
                public int? Tmdb { get; set; }

                [JsonProperty("tvrage")]
                public int? Tvrage { get; set; }
            }

            [JsonProperty("season")]
            public int Season { get; set; }

            [JsonProperty("number")]
            public int Number { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("ids")]
            public ShowNextEpisodeIds Ids { get; set; }
        }

        [JsonProperty("aired")]
        public int Aired { get; set; }

        [JsonProperty("completed")]
        public int Completed { get; set; }

        [JsonProperty("seasons")]
        public List<SeasonWatchedProgress> Seasons { get; set; }

        [JsonProperty("next_episode")]
        public ShowNextEpisode NextEpisode { get; set; }
    }
}
