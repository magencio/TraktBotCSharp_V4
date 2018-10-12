using Newtonsoft.Json;
using System;

namespace Alejacma.TraktTv.Model
{
    public class Person
    {
        public class PersonIds
        {
            [JsonProperty("trakt")]
            public int Trakt { get; set; }

            [JsonProperty("slug")]
            public string Slug { get; set; }

            [JsonProperty("imdb")]
            public string Imdb { get; set; }

            [JsonProperty("tmdb")]
            public int? Tmdb { get; set; }

            [JsonProperty("tvrage")]
            public int? Tvrage { get; set; }
        }

        // Min

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ids")]
        public PersonIds Ids { get; set; }

        // Full

        [JsonProperty("biography")]
        public string Biography { get; set; }

        [JsonProperty("birthday")]
        public DateTime? Birthday { get; set; }

        [JsonProperty("death")]
        public DateTime? Death { get; set; }

        [JsonProperty("birthplace")]
        public string Birthplace { get; set; }

        [JsonProperty("homepage")]
        public string Homepage { get; set; }
    }
}
