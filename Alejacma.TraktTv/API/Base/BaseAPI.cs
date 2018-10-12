using Alejacma.TraktTv.Extensions;
using Alejacma.TraktTv.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Alejacma.TraktTv.API.Base
{
    public abstract class BaseAPI
    {
        protected TraktTvConfiguration configuration;

        private string baseApiUri = "https://api-v2launch.trakt.tv";

        private string apiVersion = "2";

        protected BaseAPI(TraktTvConfiguration configuration)
            => this.configuration = configuration;

        private HttpClient CreateTraktTvHttpClient()
        {
            var httpClient = new HttpClient() { BaseAddress = new Uri(baseApiUri) };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-key", configuration.ClientId);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-version", apiVersion);
            if (configuration.AccessToken is string accessToken)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            }
            return httpClient;
        }

        protected async Task<T> ExecuteGetRequestAsync<T>(
            string api,
            ExtendedInfoLevel? extendedInfoLevel = null,
            int? page = null,
            int? limit = null,
            List<KeyValuePair<string, string>> extraParameters = null)
        {
            var parameters = new List<KeyValuePair<string, string>>();

            if (extendedInfoLevel.HasValue)
            {
                parameters.Add(new KeyValuePair<string, string>("extended", extendedInfoLevel.Value.GetString()));
            }
            if (page.HasValue)
            {
                parameters.Add(new KeyValuePair<string, string>("page", page.Value.ToString()));
            }
            if (limit.HasValue)
            {
                parameters.Add(new KeyValuePair<string, string>("limit", limit.Value.ToString()));
            }
            if (extraParameters != null)
            {
                parameters.AddRange(extraParameters);
            }

            using (var content = new FormUrlEncodedContent(parameters))
            {
                var query = await content.ReadAsStringAsync();
                return await ExecuteGetRequestAsync<T>(api, query);
            }
        }

        private async Task<T> ExecuteGetRequestAsync<T>(string api, string query)
        {
            using (var httpClient = CreateTraktTvHttpClient())
            using (var response = await httpClient.GetAsync($"{api}?{query}"))
            {
                return await response
                    .EnsureSuccessStatusCode()
                    .Content
                    .ReadAsStringAsync()
                    .DeserializeAsync<T>();
            }
        }

        protected Task<T> ExecutePostRequestAsync<T>(string api, List<KeyValuePair<string, string>> parameters)
            => ExecutePostRequestAsync<T>(api, new FormUrlEncodedContent(parameters));

        protected Task<T> ExecutePostRequestAsync<T>(string api, object parameters)
            => ExecutePostRequestAsync<T>(api, new StringContent(parameters.Serialize(), Encoding.UTF8, "application/json"));

        private async Task<T> ExecutePostRequestAsync<T>(string api, HttpContent parameters)
        {
            using (var httpClient = CreateTraktTvHttpClient())
            using (var response = await httpClient.PostAsync(api, parameters))
            {
                return await response
                    .EnsureSuccessStatusCode()
                    .Content
                    .ReadAsStringAsync()
                    .DeserializeAsync<T>();
            }
        }

        protected async Task ExecuteDeleteRequestAsync(string api)
        {
            using (var httpClient = CreateTraktTvHttpClient())
            using (var response = await httpClient.DeleteAsync(api))
            {
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
