using System.Collections.Generic;
using System.Threading.Tasks;

namespace TraktBot.Model
{
    /// <summary>
    /// Access to trakt.tv services.
    /// </summary>
    public interface ITraktTv
    {
        /// <summary>
        /// Add an episode to the watched list of a show.
        /// </summary>
        /// <param name="token">Trakt.tv authentication token for the user.</param>
        /// <param name="traktId">Id of the episode in Trakt.tv.</param>
        /// <returns>Returns if the operation was successful.</returns>
        Task<bool> AddWatchedEpisodeAsync(string token, int traktId);

        /// <summary>
        /// Gets a list of popular shows.
        /// </summary>
        /// <param name="page">(Optional) Page number.</param>
        /// <param name="limit">(Optional) Number of results per page.</param>
        /// <returns>The list of shows.</returns>
        Task<List<Show>> GetPopularShowsAsync(int? page = null, int? limit = null);

        /// <summary>
        /// Gets a list of recommended shows.
        /// </summary>
        /// <param name="token">Trakt.tv authentication token for the user.</param>
        /// <returns>The list of shows.</returns>
        Task<List<Show>> GetRecommendedShowsAsync(string token);

        /// <summary>
        /// Gets all the seasons of a show.
        /// </summary>
        /// <param name="traktId">Id of the show in Trakt.tv.</param>
        /// <param name="fullDetails">(Optional) Include full season details or not.</param>
        /// <returns>The list of seasons.</returns>
        Task<List<Season>> GetSeasonsAsync(int traktId, bool fullDetails = false);

        /// <summary>
        /// Gets details from a show.
        /// </summary>
        /// <param name="traktId">Id of the show in Trakt.tv.</param>
        /// <param name="fullDetails">(Optional) Include full show details or not.</param>
        /// <returns>Show details.</returns>
        Task<Show> GetShowSummaryAsync(int traktId, bool fullDetails = false);

        /// <summary>
        /// Gets a list of trending shows.
        /// </summary>
        /// <param name="page">(Optional) Page number.</param>
        /// <param name="limit">(Optional) Number of results per page.</param>
        /// <returns>The list of shows.</returns>
        Task<List<Show>> GetTrendingShowsAsync(int? page = null, int? limit = null);

        /// <summary>
        /// Gets the user name.
        /// </summary>
        /// <param name="token">Trakt.tv authentication token for the user.</param>
        /// <returns>User name.</returns>
        Task<string> GetUserNameAsync(string token);

        /// <summary>
        /// Queries trakt.tv for a list of shows.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <returns>The list of shows.</returns>
        Task<List<Show>> SearchShowsAsync(string query);
    }
}