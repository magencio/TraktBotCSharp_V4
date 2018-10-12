using Alejacma.TraktTv.API.Base;
using Alejacma.TraktTv.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alejacma.TraktTv.API
{
    public class OAuthAPI : BaseAPI
    {
        public OAuthAPI(TraktTvConfiguration configuration) : base(configuration) {}

        /// <summary>
        /// Authentication - OAuth / Token / Exchange code for access_token
        /// http://docs.trakt.apiary.io/#reference/authentication-oauth/token/exchange-code-for-access_token
        /// Use the authorization code GET parameter sent back to your redirect uri to get an access token. 
        /// </summary>
        /// <param name="code">Authorization code from trakt.tv that we can use to request the access token.</param>
        /// <returns>Returns the access token that we can use to authenticate the user in other calls to the API.</returns>
        public async Task<Authorization> GetTokenAsync(string code, string redirectUri, string state)
            => await ExecutePostRequestAsync<Authorization>("oauth/token", new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", configuration.ClientId),
                new KeyValuePair<string, string>("client_secret", configuration.ClientSecret),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("state", "state")
            });
    }
}
