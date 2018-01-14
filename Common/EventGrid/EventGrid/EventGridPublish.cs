using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static DuraShop.EventGrid.Conf;

namespace DuraShop.EventGrid
{
    public static class Conf
    {
        public enum Subject { MAIL, SMS }
        public enum EventType { REMINDERITEMSINCART, MFAVERIFICATION, WELCOMENEWUSER, GENERIC }
    }

    public static class PublishCommunication
    {
        static string topicEndpoint = Environment.GetEnvironmentVariable("topicEndpoint");
        static string sasKey = Environment.GetEnvironmentVariable("sasKey");

        public static async void Push(string eventData, string id, Subject subject, EventType eventType)
        {
            var notif = JsonConvert.DeserializeObject<NotifData>(eventData);
            List<GridData> eventList = new List<GridData>();

            var eventItem = new GridData
            {
                Subject = subject.ToString(),
                EventType = "durashop.notification." + eventType.ToString(),
                EventTime = DateTime.UtcNow,
                Id = id,
                Data = notif
            };

            eventList.Add(eventItem);
            await PostToEventGridAsync(eventList);
        }

        static async Task PostToEventGridAsync(List<GridData> data)
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

                var response = await client.SendAsync(request);
            }
        }
    }
}
