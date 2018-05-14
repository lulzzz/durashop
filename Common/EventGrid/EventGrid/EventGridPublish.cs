using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static DuraShop.EventGridSend.Conf;

namespace DuraShop.EventGridSend
{
    public static class Conf
    {
        public enum Subject { MAIL, SMS }
        public enum EventType { REMINDERITEMSINCART, MFAVERIFICATION, WELCOMENEWUSER, ORDERCONFIRMATION, GENERIC }
    }

    public static class PublishCommunication
    {
        static readonly string topicEndpoint = Environment.GetEnvironmentVariable("topicEndpoint");
        static readonly string sasKey = Environment.GetEnvironmentVariable("sasKey");

        public static async Task<HttpResponseMessage> Push(NotifData notifData, string id, Subject subject, EventType eventType)
        {
            List<GridData> eventList = new List<GridData>();

            var eventItem = new GridData
            {
                Subject = subject.ToString(),
                EventType = "durashop.notification." + eventType.ToString(),
                EventTime = DateTime.UtcNow,
                Id = id,
                Data = notifData
            };

            eventList.Add(eventItem);
            return await PostToEventGridAsync(eventList);
        }

        static async Task<HttpResponseMessage> PostToEventGridAsync(List<GridData> data)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("aeg-sas-key", sasKey);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("DuraShop");

                var json = JsonConvert.SerializeObject(data);
                var request = new HttpRequestMessage(HttpMethod.Post, topicEndpoint)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                return await client.SendAsync(request);
            }
        }

        private static async Task SendReminderNotificationAsync(string subject, object data)
        {
            var topicEndpoint = Environment.GetEnvironmentVariable("topicEndpoint");
            var sasKey = Environment.GetEnvironmentVariable("sasKey");

            var credentials = new Microsoft.Azure.EventGrid.Models.TopicCredentials(topicEndpoint);
            var client = new Microsoft.Azure.EventGrid.EventGridClient(credentials);
            var eventGridEvent = new Microsoft.Azure.EventGrid.Models.EventGridEvent
            {
                Subject = subject,
                EventType = "REMINDERITEMSINCART",
                EventTime = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString(),
                Data = data,
                DataVersion = "1.0.0",
            };
            var events = new List<Microsoft.Azure.EventGrid.Models.EventGridEvent>();
            events.Add(eventGridEvent);
            await client.PublishEventsWithHttpMessagesAsync(topicHostName, events);
        }
    }
}
