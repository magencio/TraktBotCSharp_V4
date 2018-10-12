using Alejacma.TraktTv.API.Base;
using Alejacma.TraktTv.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alejacma.TraktTv.API
{
    public class ShowsAPI : BaseAPI
    {
        public ShowsAPI(TraktTvConfiguration configuration) : base(configuration) {}

        /// <summary>
        /// Shows / Popular / Get popular shows
        /// http://docs.trakt.apiary.io/#reference/shows/popular/get-popular-shows 
        /// </summary>
        /// <param name="extendedInfoLevel">Can be Min or Full. Min by default</param>
        /// <param name="page">Number of page of results to be returned. 1 by default</param>
        /// <param name="limit">Number of results to return per page. 10 by default</param>
        /// <returns>Returns the most popular shows. Popularity is calculated using the rating percentage and the number of ratings.</returns>
        public Task<List<Show>> GetPopularAsync(ExtendedInfoLevel? extendedInfoLevel = null, int? page = null, int? limit = null)
            => ExecuteGetRequestAsync<List<Show>>("shows/popular", extendedInfoLevel, page, limit);

        /// <summary>
        /// Shows / Trending / Get trending shows
        /// http://docs.trakt.apiary.io/#reference/shows/trending/get-trending-shows
        /// </summary>
        /// <param name="extendedInfoLevel">Can be Min or Full. Min by default</param>
        /// <param name="page">Number of page of results to be returned. 1 by default</param>
        /// <param name="limit">Number of results to return per page. 10 by default</param>
        /// <returns>Returns all shows being watched right now. Shows with the most users are returned first.</returns>
        public Task<List<TrendingShow>> GetTrendingAsync(ExtendedInfoLevel? extendedInfoLevel = null, int? page = null, int? limit = null)
            => ExecuteGetRequestAsync<List<TrendingShow>>("shows/trending", extendedInfoLevel, page, limit);

        /// <summary>
        /// Shows / Summary / Get a single show
        /// http://docs.trakt.apiary.io/#reference/shows/summary/get-a-single-show 
        /// </summary>
        /// <param name="traktId">Trakt ID of the show</param>
        /// <param name="extendedInfoLevel">Can be Min or Full. Min by default</param>
        /// <returns>Returns a single shows's details. If you get extended info, the airs object is relative to the show's country. 
        /// You can use the day, time, and timezone to construct your own date then convert it to whatever timezone your user is in.
        /// Note: When getting full extended info, the status field can have a value of returning series (airing right now), 
        /// in production (airing soon), canceled, or ended.</returns>
        public Task<Show> GetSummaryAsync(int traktId, ExtendedInfoLevel? extendedInfoLevel = null)
            => ExecuteGetRequestAsync<Show>($"shows/{traktId}", extendedInfoLevel);

        /// <summary>
        /// Shows / Watched Progress / Get show watched progress
        /// http://docs.trakt.apiary.io/#reference/shows/watched-progress/get-show-watched-progress 
        /// </summary>
        /// <param name="traktId">Trakt ID of the show</param>
        /// <returns>Returns watched progress for show including details on all seasons and episodes. 
        /// The next episode will be the next episode the user should watch, if there are no upcoming episodes it will be set to null.</returns>
        public Task<ShowWatchedProgress> GetWatchedProgressAsync(int traktId)
            => ExecuteGetRequestAsync<ShowWatchedProgress>($"shows/{traktId}/progress/watched");

        /// <summary>
        /// Shows / People / Get all people for a show
        /// http://docs.trakt.apiary.io/#reference/shows/people/get-all-people-for-a-show 
        /// </summary>
        /// <param name="traktId">Trakt ID of the show</param>
        /// <returns>Returns all cast and crew for a show. Each cast member will have a character and a standard person object. 
        /// The crew oject will be broken up into production, art, crew, costume & make-up, directing, writing, sound, and camera 
        /// (if there are people for those crew positions). Each of those members will have a job and a standard person object.</returns>
        public Task<People> GetPeopleAsync(int traktId)
            => ExecuteGetRequestAsync<People>($"shows/{traktId}/people");

        /// <summary>
        /// Shows / Related / Get related shows
        /// http://docs.trakt.apiary.io/#reference/shows/related/get-related-shows 
        /// </summary>
        /// <param name="traktId">Trakt ID of the show</param>
        /// <param name="extendedInfoLevel">Can be Min or Full. Min by default</param>
        /// <param name="page">Number of page of results to be returned. 1 by default</param>
        /// <param name="limit">Number of results to return per page. 10 by default</param>
        /// <returns>Returns related and similar shows. By default, 10 related shows will returned. You can send a limit to get up to 100 items.
        /// Note: We are continuing to improve this algorithm.</returns>
        public Task<List<Show>> GetRelatedAsync(int traktId, ExtendedInfoLevel? extendedInfoLevel = null, int? page = null, int? limit = null)
            => ExecuteGetRequestAsync<List<Show>>($"shows/{traktId}/related", extendedInfoLevel, page, limit);
    }
}
