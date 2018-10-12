using Alejacma.TraktTv.API.Base;
using Alejacma.TraktTv.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alejacma.TraktTv.API
{
    public class GenresAPI : BaseAPI
    {
        public GenresAPI(TraktTvConfiguration configuration) : base(configuration) { }

        /// <summary>
        /// Genres / List / Get genres
        /// http://docs.trakt.apiary.io/#reference/genres/list/get-genres
        /// One or more genres are attached to all movies and shows. 
        /// Some API methods allow filtering by genre, so it's good to cache this list in your app.
        /// </summary>
        /// <returns>Returns a list of all genres, including names and slugs</returns>
        public Task<List<Genre>> GetListAsync()
            => ExecuteGetRequestAsync<List<Genre>>("genres/shows");
    }
}
