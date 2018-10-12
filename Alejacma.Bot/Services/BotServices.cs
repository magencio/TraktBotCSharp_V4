using System;
using System.Collections.Generic;
using Alejacma.Bot.Recognizers;
using Alejacma.Bot.Telemetry;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Configuration;

namespace Alejacma.Bot.Services
{
    /// <summary>
    /// Represents references to external services.
    ///
    /// For example, LUIS services are kept here as a singleton. This external service is configured
    /// using the <see cref="BotConfiguration"/> class.
    /// </summary>
    public class BotServices : IBotServices
    {
        private const string ProductionName = "production";
        private const string DevelopmentName = "development";
        private const string BotStateName = "botState";

        /// <summary>
        /// Initializes a new instance of the <see cref="BotServices"/> class.
        /// </summary>
        /// <param name="botConfiguration">Bot configuration.</param>
        /// <param name="isProduction">Production environment.</param>
        public BotServices(BotConfiguration botConfiguration, bool isProduction)
        {
            var endpointName = isProduction ? ProductionName : DevelopmentName;

            foreach (var service in botConfiguration.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.Endpoint
                    when service is EndpointService endpoint && endpoint.Name == endpointName:
                        AppCredentials = (endpoint.AppId, endpoint.AppPassword);
                        break;

                    case ServiceTypes.CosmosDB
                    when service is CosmosDbService cosmosDb && cosmosDb.Name == BotStateName:
                        IStorage storage = new CosmosDbStorage(
                            new CosmosDbStorageOptions
                            {
                                DatabaseId = cosmosDb.Database,
                                CollectionId = cosmosDb.Collection,
                                CosmosDBEndpoint = new Uri(cosmosDb.Endpoint),
                                AuthKey = cosmosDb.Key,
                            });
                        break;

                    case ServiceTypes.Luis
                    when service is LuisService luis:
                        var app = new LuisApplication(luis.AppId, luis.SubscriptionKey, luis.GetEndpoint());
                        var recognizer = new TelemetryLuisRecognizer(app);
                        Recognizers.Add(luis.Name, recognizer);
                        break;

                    case ServiceTypes.AppInsights
                    when service is AppInsightsService appInsights:
                        Telemetry = new BotTelemetry(appInsights.InstrumentationKey, logOriginalMessage: true, logUserName: false);
                        break;
                }
            }
        }

        public (string appId, string appPassword) AppCredentials { get; }

        // Note: Memory Storage is for local bot debugging only.When the bot is restarted, everything stored in memory will be gone.
        public IStorage Storage { get; } = new MemoryStorage();

        public Dictionary<string, IRecognizer> Recognizers { get; } = new Dictionary<string, IRecognizer>();

        public IBotTelemetry Telemetry { get; }
    }
}