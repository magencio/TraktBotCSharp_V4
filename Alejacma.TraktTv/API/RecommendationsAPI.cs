using Alejacma.TraktTv.API.Base;
using Alejacma.TraktTv.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alejacma.TraktTv.API
{
    public class RecommendationsAPI : BaseAPI
    {
        public RecommendationsAPI(TraktTvConfiguration configuration) : base(configuration) {}

        /// <summary>
        /// Recommendations / Shows / Get show recommendations
        /// http://docs.trakt.apiary.io/#reference/recommendations/shows/get-show-recommendations
        /// </summary>
        /// <param name="extendedInfoLevel">Can be Min or Full. Min by default</param>
        /// <returns>Personalized show recommendations for a user. Results returned with the top recommendation first</returns>
        public Task<List<Show>> GetShowsAsync(ExtendedInfoLevel? extendedInfoLevel = null)
            => ExecuteGetRequestAsync<List<Show>>("recommendations/shows", extendedInfoLevel);
    }
}
