using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Alejacma.TraktTv.Model
{
    public class Show
    {
        public class ShowIds
        {
            [JsonProperty("trakt")]
            public int Trakt { get; set; }

            [JsonProperty("slug")]
            public string Slug { get; set; }

            [JsonProperty("tvdb")]
            public int? Tvdb { get; set; }

            [JsonProperty("imdb")]
            public string Imdb { get; set; }

            [JsonProperty("tmdb")]
            public int? Tmdb { get; set; }

            [JsonProperty("tvrage")]
            public int? Tvrage { get; set; }
        }

        public class ShowAirs
        {
            [JsonProperty("day")]
            public string Day { get; set; }

            [JsonProperty("time")]
            public string Time { get; set; }

            [JsonProperty("timezone")]
            public string TimeZone { get; set; }
        }

        // Min

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("ids")]
        public ShowIds Ids { get; set; }

        // Full

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("first_aired")]
        public DateTime? FirstAired { get; set; }

        [JsonProperty("airs")]
        public ShowAirs Airs { get; set; }

        [JsonProperty("runtime")]
        public int Runtime { get; set; }

        [JsonProperty("certification")]
        public string Certification { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("trailer")]
        public string Trailer { get; set; }

        [JsonProperty("homepage")]
        public string HomePage { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("votes")]
        public int Votes { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("available_translations")]
        public List<string> AvailableTranslations { get; set; }

        [JsonProperty("genres")]
        public List<string> Genres { get; set; }

        [JsonProperty("aired_episodes")]
        public int AiredEpisodes { get; set; }
    }
}
