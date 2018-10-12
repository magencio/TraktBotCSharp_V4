using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Alejacma.TraktTv.Model
{
    public class WatchedItems
    {
        public class WatchedEpisode
        {
            [JsonProperty("watched_at")]
            public DateTime? WatchedAt { get; set; }

            [JsonProperty("ids")]
            public Episode.EpisodeIds Ids { get; set; }
        }

        [JsonProperty("episodes")]
        public List<WatchedEpisode> Episodes { get; set; }
    }
}
