using Newtonsoft.Json;
using System.Collections.Generic;

namespace Alejacma.TraktTv.Model
{
    public class UnwatchedItems
    {
        public class UnwatchedEpisode
        {
            [JsonProperty("ids")]
            public Episode.EpisodeIds Ids { get; set; }
        }

        [JsonProperty("episodes")]
        public List<UnwatchedEpisode> Episodes { get; set; }
    }
}
