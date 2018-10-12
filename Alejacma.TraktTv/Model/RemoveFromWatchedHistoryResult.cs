using Newtonsoft.Json;
using System.Collections.Generic;

namespace Alejacma.TraktTv.Model
{
    public class RemoveFromWatchedHistoryResult
    {
        public class DeletedResults 
        {
            [JsonProperty("episodes")]
            public int Episodes { get; set; }
        }

        public class NotFoundResults
        {
            public class NotFoundEpisode
            {
                [JsonProperty("ids")]
                public Episode.EpisodeIds Ids { get; set; }
            }

            [JsonProperty("episodes")]
            public List<NotFoundEpisode> Episodes { get; set; }
        }

        [JsonProperty("deleted")]
        public DeletedResults Deleted { get; set; }

        [JsonProperty("not_found")]
        public NotFoundResults NotFound { get; set; }
    }
}
