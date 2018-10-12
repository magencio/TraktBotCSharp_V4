using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Alejacma.Bot.Telemetry
{
    /// <summary>
    /// Sends bot related events to telemetry service (e.g. Application Insights)
    /// </summary>
    public interface IBotTelemetry
    {
        /// <summary>
        /// Sends Activity data to telemetry service.
        /// </summary>
        /// <param name="activity">Activity.</param>
        /// <param name="customProperties">(Optional) Additional properties.</param>
        void TrackActivity(IActivity activity, IDictionary<string, string> customProperties = null);

        /// <summary>
        /// Sends Intent data to telemetry service.
        /// </summary>
        /// <param name="activity">Activity associated to the intent data.</param>
        /// <param name="recognizerResult">Recognizer results containing intent data.</param>
        void TrackIntent(IActivity activity, RecognizerResult recognizerResult);

        /// <summary>
        /// Sends a custom event to telemetry service.
        /// </summary>
        /// <param name="activity">Activity associated to event.</param>
        /// <param name="eventName">(Optional) Name of the custom event.</param>
        /// <param name="customProperties">(Optional) Additional properties.</param>
        void TrackCustomEvent(IActivity activity, string eventName = "MBFEvent.CustomEvent", IDictionary<string, string> customProperties = null);

        /// <summary>
        /// Sends a goal triggered event to telemetry service.
        /// </summary>
        /// <param name="activity">Activity associated to event.</param>
        /// <param name="goalName">Goal name.</param>
        /// <param name="customProperties">(Optional) Additional properties.</param>
        void TrackGoalTriggeredEvent(IActivity activity, string goalName, IDictionary<string, string> customProperties = null);

        /// <summary>
        /// Sends Exception data to telemetry service.
        /// </summary>
        /// <param name="activity">Activity associated to event.</param>
        /// <param name="exception">Exception.</param>
        /// <param name="customProperties">(Optional) Additional properties.</param>
        void TrackException(IActivity activity, Exception exception, IDictionary<string, string> customProperties = null);

        // TODO: Track sentiment.
    }
}