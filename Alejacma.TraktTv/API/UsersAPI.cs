using Alejacma.TraktTv.API.Base;
using Alejacma.TraktTv.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alejacma.TraktTv.API
{
    public class UsersAPI : BaseAPI
    {
        public UsersAPI(TraktTvConfiguration configuration) : base(configuration) {}

        /// <summary>
        /// Users / Settings / Retrieve settings
        /// http://docs.trakt.apiary.io/#reference/users/settings/retrieve-settings
        /// </summary>
        /// <returns>Returns the user's settings so you can align your app's experience with what they're used to on the trakt website.</returns>
        public Task<Settings> GetSettingsAsync()
            => ExecuteGetRequestAsync<Settings>("users/settings");

        /// <summary>
        /// Users / Lists / Create custom list
        /// http://docs.trakt.apiary.io/#reference/users/lists/create-custom-list
        /// Create a new custom list.
        /// </summary>
        /// <param name="userName">The name of the user</param>
        /// <param name="listName">The name of the custom list</param>
        /// <param name="description">The description of the custom list</param>
        /// <param name="isPrivate">Is this a private list?</param>
        /// <param name="displayNumbers">Should it display numbers?</param>
        /// <param name="AllowComments">Will it allow comments?</param>
        /// <returns>Returns the details of the custom list</returns>
        public Task<CustomList> CreateCustomListAsync(string userName, string listName, string description, bool isPrivate, bool displayNumbers, bool AllowComments)
            => ExecutePostRequestAsync<CustomList>($"users/{userName}/lists", new CustomList()
            {
                Name = listName,
                Description = description,
                Privacy = isPrivate ? "private" : "public",
                DisplayNumbers = displayNumbers,
                AllowComments = AllowComments
            });

        /// <summary>
        /// Users / Lists / Get a user's custom lists
        /// http://docs.trakt.apiary.io/#reference/users/lists/get-a-user's-custom-lists
        /// </summary>
        /// <param name="userName">The name of the user</param>
        /// <returns>Returns all custom lists for a user</returns>
        public Task<List<CustomList>> GetCustomListsAsync(string userName)
            => ExecuteGetRequestAsync<List<CustomList>>($"users/{userName}/lists");

        /// <summary>
        /// Users / List / Get custom list
        /// http://docs.trakt.apiary.io/#reference/users/list/get-custom-list
        /// </summary>
        /// <param name="userName">The name of the user</param>
        /// <param name="traktId">Trakt ID of the list</param>
        /// <returns>Returns a single custom list</returns>
        public Task<CustomList> GetCustomListAsync(string userName, int traktId)
            => ExecuteGetRequestAsync<CustomList>($"users/{userName}/lists/{traktId}");

        /// <summary>
        /// Users / List / Delete a user's custom list
        /// http://docs.trakt.apiary.io/#reference/users/list/delete-a-user's-custom-list
        /// Remove a custom list and all items it contains
        /// </summary>
        /// <param name="userName">The name of the user</param>
        /// <param name="traktId">Trakt ID of the list</param>
        /// <returns>A task to wait for the completion of the operation</returns>
        public Task DeleteCustomListAsync(string userName, int traktId)
            => ExecuteDeleteRequestAsync($"users/{userName}/lists/{traktId}");

        /// <summary>
        /// Users / List Items / Get items on a custom list
        /// http://docs.trakt.apiary.io/#reference/users/list-items/get-items-on-a-custom-list
        /// </summary>
        /// <param name="userName">The name of the user</param>
        /// <param name="traktId">Trakt ID of the list</param>
        /// <param name="extendedInfoLevel">Can be Min or Full. Min by default</param>
        /// <returns>All items on a custom list</returns>
        public Task<List<CustomListItem>> GetCustomListItemsAsync(string userName, int traktId, ExtendedInfoLevel? extendedInfoLevel = null)
            => ExecuteGetRequestAsync<List<CustomListItem>>($"users/{userName}/lists/{traktId}/items", extendedInfoLevel);

        /// <summary>
        /// Users / List Items / Add items to custom list
        /// http://docs.trakt.apiary.io/#reference/users/list-items/add-items-to-custom-list
        /// Add one or more items to a custom list
        /// </summary>
        /// <param name="userName">The name of the user</param>
        /// <param name="traktId">Trakt ID of the list</param>
        /// <param name="customListItemIds">The ids of the items to add to the list</param>
        /// <returns>Number of items added</returns>
        public Task<AddItemsToCustomListResult> AddItemsToCustomListAsync(string userName, int traktId, CustomListItemIds customListItemIds)
            => ExecutePostRequestAsync<AddItemsToCustomListResult>($"users/{userName}/lists/{traktId}/items", customListItemIds);

        /// <summary>
        /// Users / Remove List Items / Remove items from custom list
        /// http://docs.trakt.apiary.io/#reference/users/remove-list-items/remove-items-from-custom-list
        /// Remove one or more items from a custom list
        /// </summary>
        /// <param name="userName">The name of the user</param>
        /// <param name="traktId">Trakt ID of the list</param>
        /// <param name="customListItemIds">The ids of the items to remove from the list</param>
        /// <returns>Number of items removed</returns>
        public Task<RemoveItemsFromCustomListResult> RemoveItemsFromCustomListAsync(string userName, int traktId, CustomListItemIds customListItemIds)
            => ExecutePostRequestAsync<RemoveItemsFromCustomListResult>($"users/{userName}/lists/{traktId}/items/remove", customListItemIds);
    }
}
