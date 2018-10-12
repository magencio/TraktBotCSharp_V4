namespace Alejacma.TraktTv
{
    public class TraktTvConfiguration
    {
        public string ClientId { get; }

        public string ClientSecret { get; }

        public string AccessToken { get; }

        public TraktTvConfiguration(string clientId, string clientSecret, string accessToken = null)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            AccessToken = accessToken;
        }
    }
}