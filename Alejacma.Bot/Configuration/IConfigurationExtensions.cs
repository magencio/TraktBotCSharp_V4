using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;
using System;

namespace Alejacma.Bot.Configuration
{
    public static class IConfigurationExtensions
    {
        /// <summary>
        /// Loads the bot configuration from a .bot file.
        /// </summary>
        /// <param name="configuration">App configuration used to find the .bot file.</param>
        /// <returns>Bot configuration</returns>
        public static BotConfiguration LoadBotConfiguration(this IConfiguration configuration)
        {
            var secretKey = configuration.GetSection("botFileSecret")?.Value;
            var botFilePath = configuration.GetSection("botFilePath")?.Value;
            var botConfig = BotConfiguration.Load(botFilePath ?? @".\BotConfiguration.bot", secretKey);
            return botConfig ?? throw new InvalidOperationException($"The .bot config file could not be loaded. ({botConfig})");
        }
    }
}
