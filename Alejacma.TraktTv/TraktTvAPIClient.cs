using Alejacma.TraktTv.API;

namespace Alejacma.TraktTv
{
    /// <summary>
    /// Client of Trakt.tv API
    /// http://docs.trakt.apiary.io/#
    /// </summary>
    public class TraktTvAPIClient
    {
        private TraktTvConfiguration configuration;

        private OAuthAPI oAuth;
        public OAuthAPI OAuth => oAuth = oAuth ?? new OAuthAPI(configuration);

        private GenresAPI genres;
        public GenresAPI Genres => genres = genres ?? new GenresAPI(configuration);

        private PeopleAPI people;
        public PeopleAPI People => people = people ?? new PeopleAPI(configuration);

        private RecommendationsAPI recommendations;
        public RecommendationsAPI Recommendations => recommendations = recommendations ?? new RecommendationsAPI(configuration);

        private SearchAPI search;
        public SearchAPI Search => search = search ?? new SearchAPI(configuration);

        private ShowsAPI shows;
        public ShowsAPI Shows => shows = shows ?? new ShowsAPI(configuration);

        private SeasonsAPI seasons;
        public SeasonsAPI Seasons => seasons = seasons ?? new SeasonsAPI(configuration);

        private EpisodesAPI episodes;
        public EpisodesAPI Episodes => episodes = episodes ?? new EpisodesAPI(configuration);

        private SyncAPI sync;
        public SyncAPI Sync => sync = sync ?? new SyncAPI(configuration);

        private UsersAPI users;
        public UsersAPI Users => users = users ?? new UsersAPI(configuration);

        public TraktTvAPIClient(TraktTvConfiguration configuration)
            => this.configuration = configuration;
    }
}
