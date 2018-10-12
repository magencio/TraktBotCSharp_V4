using Microsoft.Bot.Builder;

namespace Alejacma.Bot.Recognizers
{
    /// <summary>
    /// Extension methods for RecognizerResult.
    /// </summary>
    public static class RecognizerResultExtensions
    {
        /// <summary>
        /// Ignore any intent with a score below this minimum.
        /// </summary>
        public const double MinScore = 0.5;

        /// <summary>
        /// Gets the top intent returned by an intent recognizer.
        /// </summary>
        /// <param name="results">Intent recognizer results.</param>
        /// <returns>Intent name.</returns>
        public static string GetTopIntent(this RecognizerResult results)
        {
            var scoringIntent = results?.GetTopScoringIntent();
            return scoringIntent.HasValue && scoringIntent.Value.score >= MinScore
                ? scoringIntent.Value.intent
                : null;
        }

        /// <summary>
        /// Gets the value of an entity returned by an intent recognizer.
        /// </summary>
        /// <param name="results">Intent recognizer results.</param>
        /// <param name="entityName">Entity name.</param>
        /// <returns>Value of the entity.</returns>
        public static string GetEntityValue(this RecognizerResult results, string entityName)
            => results?.Entities[entityName]?.First?.ToString();
    }
}
