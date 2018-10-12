using Alejacma.TraktTv.API.Base;
using Alejacma.TraktTv.Model;
using System.Threading.Tasks;

namespace Alejacma.TraktTv.API
{
    public class SyncAPI : BaseAPI
    {
        public SyncAPI(TraktTvConfiguration configuration) : base(configuration) {}

        /// <summary>
        /// Sync / Add to History / Add items to watched history
        /// http://docs.trakt.apiary.io/#reference/sync/add-to-history
        /// Add items to a user's watch history. Send a WatchedAt UTC datetime to mark items as watched in the past.
        /// </summary>
        /// <param name="watchedItems">Watched items</param>
        /// <returns>Number of items added</returns>
        public Task<AddToWatchedHistoryResult> AddToWatchedHistoryAsync(WatchedItems watchedItems)
            => ExecutePostRequestAsync<AddToWatchedHistoryResult>("sync/history", watchedItems);

        /// <summary>
        /// Sync / Remove from History / Remove items from history
        /// http://docs.trakt.apiary.io/#reference/sync/remove-from-history
        /// Remove items from a user's watch history including all watches, scrobbles and checkins.
        /// </summary>
        /// <param name="unwatchedItems">Unwatched items</param>
        /// <returns>Number of items removed</returns>
        public Task<RemoveFromWatchedHistoryResult> RemoveFromWatchedHistoryAsync(UnwatchedItems unwatchedItems)
            => ExecutePostRequestAsync<RemoveFromWatchedHistoryResult>("sync/history/remove", unwatchedItems);
    }
}
