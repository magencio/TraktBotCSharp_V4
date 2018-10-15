using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Alejacma.Bot.Tests
{
    public class MockedHttpClientHandler : HttpClientHandler
    {
        private readonly HttpClient client;

        public MockedHttpClientHandler(HttpClient client)
        {
            this.client = client;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var mockedRequest = new HttpRequestMessage()
            {
                RequestUri = request.RequestUri,
                Content = request.Content,
                Method = request.Method
            };

            return client.SendAsync(mockedRequest, cancellationToken);
        }
    }
}
