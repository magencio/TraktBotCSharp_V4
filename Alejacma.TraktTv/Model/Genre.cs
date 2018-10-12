using Newtonsoft.Json;

namespace Alejacma.TraktTv.Model
{
    public class Genre
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }
    }
}
