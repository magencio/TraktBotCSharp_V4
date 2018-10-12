using Alejacma.TraktTv.API.Base;
using Alejacma.TraktTv.Model;
using System.Threading.Tasks;

namespace Alejacma.TraktTv.API
{
    public class EpisodesAPI : BaseAPI
    {
        public EpisodesAPI(TraktTvConfiguration configuration) : base(configuration) {}

        /// <summary>
        /// Episodes / Summary / Get a single episode for a show
        /// http://docs.trakt.apiary.io/#reference/episodes/summary/get-a-single-episode-for-a-show
        /// </summary>
        /// <param name="traktId">Trakt ID of the show</param>
        /// <param name="season">Season number</param>
        /// <param name="episode">Episode number</param>
        /// <param name="extendedInfoLevel">Can be Min or Full. Min by default</param>
        /// <returns>Returns a single episode's details. 
        /// All date and times are in UTC and were calculated using the episode's air_date and show's country and air_time.
        /// Note: If the first_aired is unknown, it will be set to null.</returns>
        public Task<Episode> GetSummaryAsync(int traktId, int season, int episode, ExtendedInfoLevel? extendedInfoLevel = null)
            => ExecuteGetRequestAsync<Episode>($"shows/{traktId}/seasons/{season}/episodes/{episode}", extendedInfoLevel);
    }
}
