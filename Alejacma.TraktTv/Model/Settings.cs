using Newtonsoft.Json;
using System;

namespace Alejacma.TraktTv.Model
{
    public class Settings
    {
        public class UserSettings
        {
            public class UserImages
            {
                public class UserAvatar
                {
                    [JsonProperty("full")]
                    public string Full { get; set; }
                }

                [JsonProperty("avatar")]
                public UserAvatar Avatar { get; set; }
            }

            [JsonProperty("username")]
            public string UserName { get; set; }

            [JsonProperty("private")]
            public bool Private { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("vip")]
            public bool Vip { get; set; }

            [JsonProperty("joined_at")]
            public DateTime? JoinedAt { get; set; }

            [JsonProperty("location")]
            public string Location { get; set; }

            [JsonProperty("about")]
            public string About { get; set; }

            [JsonProperty("gender")]
            public string Gender { get; set; }

            [JsonProperty("age")]
            public int? Age { get; set; }

            [JsonProperty("images")]
            public UserImages Images { get; set; }
        }

        public class AccountSettings
        {
            [JsonProperty("timezone")]
            public string Timezone { get; set; }

            [JsonProperty("time_24hr")]
            public bool Time24hr { get; set; }

            [JsonProperty("cover_image")]
            public string CoverImage { get; set; }
        }

        public class ConnectionsSettings
        {
            [JsonProperty("facebook")]
            public bool Facebook { get; set; }

            [JsonProperty("twitter")]
            public bool Twitter { get; set; }

            [JsonProperty("google")]
            public bool Google { get; set; }

            [JsonProperty("tumblr")]
            public bool Tumblr { get; set; }
        }

        public class SharingTextSettings
        {
            [JsonProperty("watching")]
            public string Watching { get; set; }

            [JsonProperty("watched")]
            public string Watched { get; set; }
        }

        [JsonProperty("user")]
        public UserSettings User { get; set; }

        [JsonProperty("account")]
        public AccountSettings Account { get; set; }

        [JsonProperty("connections")]
        public ConnectionsSettings Connections { get; set; }

        [JsonProperty("sharing_text")]
        public SharingTextSettings SharingText { get; set; }
    }
}
