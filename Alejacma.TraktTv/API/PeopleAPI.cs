using Alejacma.TraktTv.API.Base;
using Alejacma.TraktTv.Model;
using System.Threading.Tasks;

namespace Alejacma.TraktTv.API
{
    public class PeopleAPI : BaseAPI
    {
        public PeopleAPI(TraktTvConfiguration configuration) : base(configuration) {}

        /// <summary>
        /// People / Summary / Get a single person
        /// http://docs.trakt.apiary.io/#reference/people/summary/get-a-single-person
        /// </summary>
        /// <param name="traktId">Trakt ID of the person</param>
        /// <param name="extendedInfoLevel">Can be Min or Full. Min by default</param>
        /// <returns>Returns a single person's details</returns>
        public Task<Person> GetSummaryAsync(int traktId, ExtendedInfoLevel? extendedInfoLevel = null)
            => ExecuteGetRequestAsync<Person>($"people/{traktId}", extendedInfoLevel);
    }
}
