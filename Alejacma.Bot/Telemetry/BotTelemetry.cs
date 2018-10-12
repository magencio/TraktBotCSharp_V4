using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

/// <summary>
/// Code based on https://github.com/Azure/botbuilder-instrumentation-cs/blob/master/botbuilder-instumentation/Telemetry/TelemetryLogger.cs.
/// </summary>
namespace Alejacma.Bot.Telemetry
{
    /// <summary>
    /// Sends bot related events to Application Insights.
    /// </summary>
    public class BotTelemetry : IBotTelemetry
    {
        private readonly TelemetryClient telemetryClient;
        private readonly bool logOriginalMessage;
        private readonly bool logUserName;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotTelemetry"/> class.
        /// </summary>
        /// <param name="instrumentationKey">Instrumentation key.</param>
        /// <param name="logOriginalMessage">(Optional) Enable/Disable logging original message name within telemetry service.</param>
        /// <param name="logUserName">(Optional) Enable/Disable logging user name within telemetry service.</param>
        public BotTelemetry(
            string instrumentationKey,
            bool logOriginalMessage,
            bool logUserName)
        {
            telemetryClient = new TelemetryClient(new TelemetryConfiguration(instrumentationKey));
            this.logOriginalMessage = logOriginalMessage;
            this.logUserName = logUserName;
        }

        public void TrackActivity(
            IActivity activity,
            IDictionary<string, string> customProperties = null)
        {
            var et = BuildEventTelemetry(activity, customProperties);
            telemetryClient.TrackEvent(et);
        }

        public void TrackIntent(
            IActivity activity,
            RecognizerResult recognizerResult)
        {
            var (intent, score) = recognizerResult.GetTopScoringIntent();
            var properties = new Dictionary<string, string>
            {
                { TelemetryEventProperties.IntentProperty, intent },
                { TelemetryEventProperties.ScoreProperty, score.ToString("N2") },
                { TelemetryEventProperties.EntitiesProperty, JsonConvert.SerializeObject(recognizerResult.Entities) },
            };
            var et = BuildEventTelemetry(activity, properties);
            et.Name = TelemetryEventTypes.Intent;
            telemetryClient.TrackEvent(et);

            // TODO: Track sentiment if included in intent data.
        }

        public void TrackCustomEvent(
            IActivity activity,
            string eventName = TelemetryEventTypes.CustomEvent,
            IDictionary<string, string> customProperties = null)
        {
            var et = BuildEventTelemetry(activity, customProperties);
            et.Name = string.IsNullOrWhiteSpace(eventName) ? TelemetryEventTypes.CustomEvent : eventName;
            telemetryClient.TrackEvent(et);
        }

        public void TrackGoalTriggeredEvent(
            IActivity activity,
            string goalName,
            IDictionary<string, string> customProperties = null)
        {
            customProperties = customProperties ?? new Dictionary<string, string>();
            customProperties.Add(TelemetryEventProperties.GoalNameProperty, goalName);

            var et = BuildEventTelemetry(activity, customProperties);
            et.Name = TelemetryEventTypes.GoalTriggeredEvent;
            telemetryClient.TrackEvent(et);
        }

        public void TrackException(
            IActivity activity,
            Exception exception,
            IDictionary<string, string> customProperties = null)
        {
            customProperties = customProperties ?? new Dictionary<string, string>();
            customProperties.Add(TelemetryEventProperties.ExceptionProperty, JsonConvert.SerializeObject(exception));

            var et = BuildEventTelemetry(activity, customProperties);
            et.Name = TelemetryEventTypes.Exception;
            telemetryClient.TrackEvent(et);
        }

        private EventTelemetry BuildEventTelemetry(
            IActivity activity,
            IDictionary<string, string> customProperties = null)
        {
            // Add generic activity properies.
            var et = new EventTelemetry();
            if (activity.Timestamp != null)
            {
                et.Properties.Add(TelemetryEventProperties.TimeStampProperty, DateTimeOffsetToIso8601(activity.Timestamp.Value));
            }

            et.Properties.Add(TelemetryEventProperties.TypeProperty, activity.Type);
            et.Properties.Add(TelemetryEventProperties.ActivityIdProperty, activity.Id);
            et.Properties.Add(TelemetryEventProperties.ChannelIdProperty, activity.ChannelId);
            et.Properties.Add(TelemetryEventProperties.ConversationIdProperty, activity.Conversation.Id);

            // Add properties depending on activity type.
            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    var messageActivity = activity.AsMessageActivity();
                    string userName;
                    if (messageActivity.ReplyToId == null)
                    {
                        et.Name = TelemetryEventTypes.MessageReceived;
                        et.Properties.Add(TelemetryEventProperties.UserIdProperty, messageActivity.From.Id);
                        userName = messageActivity.From.Name;
                    }
                    else
                    {
                        et.Name = TelemetryEventTypes.MessageSent;
                        et.Properties.Add(TelemetryEventProperties.UserIdProperty, messageActivity.Recipient.Id);
                        userName = messageActivity.Recipient.Name;
                    }

                    if (logUserName && !string.IsNullOrWhiteSpace(userName))
                    {
                        et.Properties.Add(TelemetryEventProperties.UserNameProperty, userName);
                    }

                    if (logOriginalMessage && !string.IsNullOrWhiteSpace(messageActivity.Text))
                    {
                        et.Properties.Add(TelemetryEventProperties.TextProperty, messageActivity.Text);
                    }

                    et.Properties.Add(TelemetryEventProperties.LocaleProperty, messageActivity.Locale);
                    break;
                case ActivityTypes.ConversationUpdate:
                    et.Name = TelemetryEventTypes.ConversationUpdate;
                    break;
                case ActivityTypes.EndOfConversation:
                    et.Name = TelemetryEventTypes.EndOfConversation;
                    break;
                default:
                    et.Name = TelemetryEventTypes.OtherActivity;
                    break;
            }

            // Add additional properties.
            if (customProperties != null)
            {
                foreach (var property in customProperties)
                {
                    et.Properties.Add(property);
                }
            }

            return et;
        }

        private string DateTimeOffsetToIso8601(DateTimeOffset dateTimeOffset)
        {
            var s = JsonConvert.SerializeObject(dateTimeOffset.ToUniversalTime());
            return s.Substring(1, s.Length - 2);
        }
    }
}
