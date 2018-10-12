using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Alejacma.TraktTv.Model
{
    public class Episode
    {
        public class EpisodeIds
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

        // Min 

        [JsonProperty("season")]
        public int Season { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("ids")]
        public EpisodeIds Ids { get; set; }

        // Full
        [JsonProperty("number_abs")]
        public int? NumberAbs { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("first_aired")]
        public DateTime? FirstAired { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("votes")]
        public int Votes { get; set; }

        [JsonProperty("available_translations")]
        public List<string> AvailableTranslations { get; set; }
    }
}
