using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alejacma.TraktTv;
using Alejacma.TraktTv.Model;
using AutoMapper;

namespace TraktBot.Model
{
    /// <summary>
    /// Access to trakt.tv services.
    ///
    /// Trakt.tv API apps settings: https://trakt.tv/oauth/applications.
    /// </summary>
    public class TraktTv : ITraktTv
    {
        private readonly string clientId;
        private readonly string clientSecret;

        private readonly IMapper mapper;

        public TraktTv(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;

            mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Alejacma.TraktTv.Model.Show, Show>();
                cfg.CreateMap<Alejacma.TraktTv.Model.Season, Season>();
                cfg.CreateMap<Alejacma.TraktTv.Model.Episode, Episode>();
            }).CreateMapper();
        }

        public async Task<string> GetUserNameAsync(string token)
            => (await CreateClient(token).Users.GetSettingsAsync())
                .User.UserName;

        public async Task<List<Show>> GetTrendingShowsAsync(int? page = null, int? limit = null)
            => mapper.Map<List<Show>>(
                (await CreateClient().Shows.GetTrendingAsync(ExtendedInfoLevel.Full, page, limit))
                .Select(ts => ts.Show));

        public async Task<List<Show>> GetPopularShowsAsync(int? page = null, int? limit = null)
            => mapper.Map<List<Show>>(
                await CreateClient().Shows.GetPopularAsync(ExtendedInfoLevel.Full, page, limit));

        public async Task<Show> GetShowSummaryAsync(int traktId, bool fullDetails = false)
            => mapper.Map<Show>(
                await CreateClient().Shows.GetSummaryAsync(traktId, fullDetails ? ExtendedInfoLevel.Full : ExtendedInfoLevel.Min));

        public async Task<List<Show>> GetRecommendedShowsAsync(string token)
            => mapper.Map<List<Show>>(
                await CreateClient(token).Recommendations.GetShowsAsync(ExtendedInfoLevel.Full));

        public async Task<List<Show>> SearchShowsAsync(string query)
            => mapper.Map<List<Show>>(
                (await CreateClient().Search.GetTextQueryAsync(query))
                .Select(sr => sr.Show));

        public async Task<List<Season>> GetSeasonsAsync(int traktId, bool fullDetails = false)
            => mapper.Map<List<Season>>(
                await CreateClient().Seasons.GetSummaryAsync(traktId, fullDetails ? ExtendedInfoLevel.FullWithEpisodes : ExtendedInfoLevel.Episodes));

        public async Task<bool> AddWatchedEpisodeAsync(string token, int traktId)
        {
            var result = await CreateClient(token).Sync.AddToWatchedHistoryAsync(
                new WatchedItems()
                {
                    Episodes =
                        new List<WatchedItems.WatchedEpisode>()
                        {
                            new WatchedItems.WatchedEpisode()
                            {
                                Ids = new Alejacma.TraktTv.Model.Episode.EpisodeIds() { Trakt = traktId },
                            },
                        },
                });

            return result.Added.Episodes == 1;
        }

        private TraktTvAPIClient CreateClient(string token = null)
            => new TraktTvAPIClient(
                new TraktTvConfiguration(clientId, clientSecret, token));
    }
}