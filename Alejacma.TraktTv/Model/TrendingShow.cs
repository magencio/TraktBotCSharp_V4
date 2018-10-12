using Newtonsoft.Json;

namespace Alejacma.TraktTv.Model
{
    public class TrendingShow
    {
        [JsonProperty("watchers")]
        public int Watchers { get; set; }

        [JsonProperty("show")]
        public Show Show { get; set; }
    }
}
