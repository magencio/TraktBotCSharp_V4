using Newtonsoft.Json;
using System;

namespace Alejacma.TraktTv.Model
{
    public class CustomListItem
    {
        [JsonProperty("listed_at")]
        public DateTime? ListedAt { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("show")]
        public Show Show { get; set; }
    }
}
