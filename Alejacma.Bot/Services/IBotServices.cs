using System.Collections.Generic;
using Alejacma.Bot.Telemetry;
using Microsoft.Bot.Builder;

namespace Alejacma.Bot.Services
{
    public interface IBotServices
    {
        /// <summary>
        /// Gets app credentials.
        /// </summary>
        (string appId, string appPassword) AppCredentials { get; }

        /// <summary>
        /// Gets the set of intent recognizers.
        /// </summary>
        Dictionary<string, IRecognizer> Recognizers { get; }

        /// <summary>
        /// Gets the persistent storage.
        /// </summary>
        IStorage Storage { get; }

        /// <summary>
        /// Gets the telemetry client.
        /// </summary>
        IBotTelemetry Telemetry { get; }
    }
}