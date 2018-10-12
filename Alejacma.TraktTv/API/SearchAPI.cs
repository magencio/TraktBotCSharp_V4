using Alejacma.TraktTv.API.Base;
using Alejacma.TraktTv.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alejacma.TraktTv.API
{
    public class SearchAPI : BaseAPI
    {
        public SearchAPI(TraktTvConfiguration configuration) : base(configuration) {}

        /// <summary>
        /// Search / Text Query / Get text query results
        /// http://docs.trakt.apiary.io/#reference/search/text-query/get-text-query-results
        /// Perform a text query that searches titles, descriptions, translated titles, aliases, and people.
        /// </summary>
        /// <param name="query">Text query</param>
        /// <param name="page">Number of page of results to be returned. 1 by default</param>
        /// <param name="limit">Number of results to return per page. 10 by default</param>
        /// <returns>Results are ordered by the most relevant score</returns>
        public Task<List<SearchResult>> GetTextQueryAsync(string query, int? page = null, int? limit = null)
            => ExecuteGetRequestAsync<List<SearchResult>>("search", null, page, limit, new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("query", query),
                new KeyValuePair<string, string>("type", "show")
            });
    }
}
