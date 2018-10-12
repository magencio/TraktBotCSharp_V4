using Newtonsoft.Json;
using System.Collections.Generic;

namespace Alejacma.TraktTv.Model
{
    public class CustomListItemIds
    {
        public class ShowItem
        {
            public class ShowItemIds
            {
                [JsonProperty("trakt")]
                public int Trakt { get; set; }
            }

            [JsonProperty("ids")]
            public ShowItemIds Ids { get; set; }

        }

        [JsonProperty("shows")]
        public List<ShowItem> Shows { get; set; }
    }
}
