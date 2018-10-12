using Newtonsoft.Json;
using System.Collections.Generic;

namespace Alejacma.TraktTv.Model
{
    public class AddToWatchedHistoryResult
    {
        public class AddedResults
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

        [JsonProperty("added")]
        public AddedResults Added { get; set; }

        [JsonProperty("not_found")]
        public NotFoundResults NotFound { get; set; }
    }
}
