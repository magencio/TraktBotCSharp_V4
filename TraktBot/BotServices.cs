using Microsoft.Bot.Configuration;
using TraktBot.Model;

namespace TraktBot
{
    /// <summary>
    /// Represents references to external services.
    /// </summary>
    public class BotServices : Alejacma.Bot.Services.BotServices
    {
        private const string TraktTvName = "traktTv";
        private const string ClientIdProperty = "clientId";
        private const string ClientSecretProperty = "clientSecret";

        /// <summary>
        /// Initializes a new instance of the <see cref="BotServices"/> class.
        /// </summary>
        /// <param name="botConfiguration">Bot configuration.</param>
        /// <param name="isProduction">Production environment.</param>
        public BotServices(BotConfiguration botConfiguration, bool isProduction)
            : base(botConfiguration, isProduction)
        {
            foreach (var service in botConfiguration.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.Generic
                    when service is GenericService generic && generic.Name == TraktTvName:
                        TraktTv = new TraktTv(generic.Configuration[ClientIdProperty], generic.Configuration[ClientSecretProperty]);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the trakt.tv client.
        /// </summary>
        /// <value>
        /// A <see cref="ITraktTv"/> instance created based on configuration.
        /// </value>
        public ITraktTv TraktTv { get; }
    }
}