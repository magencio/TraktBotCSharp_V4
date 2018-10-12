using Alejacma.Bot.Configuration;
using Alejacma.Bot.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TraktBot.State;

namespace TraktBot
{
    /// <summary>
    /// The Startup class configures services and the app's request pipeline.
    /// </summary>
    public class Startup
    {
        private readonly bool isProduction = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </summary>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-2.1"/>
        public Startup(IHostingEnvironment env)
        {
            isProduction = env.IsProduction();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        /// <summary>
        /// Gets the configuration that represents a set of key/value application configuration properties.
        /// </summary>
        /// <value>
        /// The <see cref="IConfiguration"/> that represents a set of key/value application configuration properties.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application Builder.</param>
        /// <param name="env">Hosting Environment.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to create logger object for tracing.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Specifies the contract for a <see cref="IServiceCollection"/> of service descriptors.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Load external services.
            var botConfig = Configuration.LoadBotConfiguration();
            var botServices = new BotServices(botConfig, isProduction);
            var (appId, appPassword) = botServices.AppCredentials;
            var conversationState = new ConversationState(botServices.Storage);
            var userState = new UserState(botServices.Storage);
            var botAccessors = new BotAccessors(userState, conversationState);
            var telemetry = botServices.Telemetry;

            // Register the external services.
            services.AddSingleton(sp => botServices);

            // Register the bot's state and state property accessor objects.
            services.AddSingleton(sp => botAccessors);

            // Register and configure the bot.
            services.AddBot<Bot>(options =>
            {
                options.CredentialProvider = new SimpleCredentialProvider(appId, appPassword);

                options.OnTurnError = async (context, exception) =>
                {
                    telemetry.TrackException(context.Activity, exception);
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };

                options.Middleware.Add(new RandomizeReplyMiddleware());
                options.Middleware.Add(new DebugLoggerMiddleware());
                options.Middleware.Add(new TelemetryLoggerMiddleware(telemetry));

                options.State.Add(conversationState);
                options.State.Add(userState);
            });
        }
    }
}
