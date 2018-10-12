using Alejacma.TraktTv.API.Base;
using Alejacma.TraktTv.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alejacma.TraktTv.API
{
    public class SeasonsAPI : BaseAPI
    {
        public SeasonsAPI(TraktTvConfiguration configuration) : base(configuration) {}

        /// <summary>
        /// Seasons / Summary / Get all seasons for a show
        /// http://docs.trakt.apiary.io/#reference/seasons/summary/get-all-seasons-for-a-show 
        /// </summary>
        /// <param name="traktId">Trakt ID of the show</param>
        /// <param name="extendedInfoLevel">Can be Min or Full or Episodes. Min by default</param>
        /// <returns>Returns all seasons for a show including the number of episodes in each season. 
        /// If extendedInfoLevel is Episodes, it will return all episodes for all seasons. 
        /// Note: This returns a lot of data, so please only use this method if you need it all!</returns>
        public Task<List<Season>> GetSummaryAsync(int traktId, ExtendedInfoLevel? extendedInfoLevel = null)
            => ExecuteGetRequestAsync<List<Season>>($"shows/{traktId}/seasons", extendedInfoLevel);

        /// <summary>
        /// Seasons / Season / Get a single seasons for a show
        /// http://docs.trakt.apiary.io/#reference/seasons/season/get-single-season-for-a-show 
        /// </summary>
        /// <param name="traktId">Trakt Id of the show</param>
        /// <param name="season">Season number</param>
        /// <returns>Returns all episodes for a specific season of a show.</returns>
        public Task<List<Episode>> GetSeasonAsync(int traktId, int season)
            => ExecuteGetRequestAsync<List<Episode>>($"shows/{traktId}/seasons/{season}");
    }
}
