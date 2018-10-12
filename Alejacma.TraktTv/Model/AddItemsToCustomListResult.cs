using Newtonsoft.Json;
using System.Collections.Generic;

namespace Alejacma.TraktTv.Model
{
    public class AddItemsToCustomListResult
    {
        public class AddedResults
        {
            [JsonProperty("shows")]
            public int Shows { get; set; }
        }

        public class ExistingResults
        {
            [JsonProperty("shows")]
            public int Shows { get; set; }
        }

        public class NotFoundResults
        {
            public class NotFoundShow
            {
                [JsonProperty("ids")]
                public Show.ShowIds Ids { get; set; }
            }

            [JsonProperty("shows")]
            public List<NotFoundShow> Shows { get; set; }
        }

        [JsonProperty("added")]
        public AddedResults Added { get; set; }

        [JsonProperty("existing")]
        public ExistingResults Existing { get; set; }

        [JsonProperty("not_found")]
        public NotFoundResults NotFound { get; set; }
    }
}
