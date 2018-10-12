using Newtonsoft.Json;
using System.Collections.Generic;

namespace Alejacma.TraktTv.Model
{
    public class RemoveItemsFromCustomListResult
    {
        public class DeletedResults
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

        [JsonProperty("deleted")]
        public DeletedResults Deleted { get; set; }

        [JsonProperty("not_found")]
        public NotFoundResults NotFound { get; set; }
    }
}
