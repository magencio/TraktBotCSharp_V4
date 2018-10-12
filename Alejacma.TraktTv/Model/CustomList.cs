using Newtonsoft.Json;
using System;

namespace Alejacma.TraktTv.Model
{
    public class CustomList
    {
        public class CustomListIds
        {
            [JsonProperty("trakt")]
            public int Trakt { get; set; }

            [JsonProperty("slug")]
            public string Slug { get; set; }
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("privacy")]
        public string Privacy { get; set; }

        [JsonProperty("display_numbers")]
        public bool DisplayNumbers { get; set; }

        [JsonProperty("allow_comments")]
        public bool AllowComments { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("item_count")]
        public int ItemCount { get; set; }

        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }

        [JsonProperty("likes")]
        public int Likes { get; set; }

        [JsonProperty("ids")]
        public CustomListIds Ids { get; set; }
    }
}
