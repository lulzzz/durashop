using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DuraShop.EventGridSend
{
    public static class Event
    {
        static readonly string topicHostName = Environment.GetEnvironmentVariable("topicHostName");
        static readonly string sasKey = Environment.GetEnvironmentVariable("sasKey");

        public static async Task PushNotification(NotifData notifData, string id, string subject, string eventType)
        {
            var credentials = new Microsoft.Azure.EventGrid.Models.TopicCredentials(sasKey);
            var client = new Microsoft.Azure.EventGrid.EventGridClient(credentials);
            var eventGridEvent = new Microsoft.Azure.EventGrid.Models.EventGridEvent
            {
                Subject = subject,
                EventType = eventType,
                EventTime = DateTime.UtcNow,
                Id = id,
                Data = notifData,
                DataVersion = "1.0.0",
            };
            var events = new List<Microsoft.Azure.EventGrid.Models.EventGridEvent>();
            events.Add(eventGridEvent);
            await client.PublishEventsWithHttpMessagesAsync(topicHostName, events);
        }
    }
}
