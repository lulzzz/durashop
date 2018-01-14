using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
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

    public static class Publish
    {
        static string topicEndpoint = ConfigurationManager.AppSettings["topicEndpoint"];
        static string sasKey = ConfigurationManager.AppSettings["sasKey"];

        public static async void Push(object eventData, string id, Subject subject, EventType eventType)
        {
            List<GridEvent<object>> eventList = new List<GridEvent<object>>();
            GridEvent<object> eventItem = new GridEvent<object>
            {
                Subject = subject.ToString(),
                EventType = eventType.ToString(),
                EventTime = DateTime.UtcNow,
                Id = id,
                Data = eventData
            };

            eventList.Add(eventItem);
            await PostToEventGridAsync(eventList);
        }

        static async Task PostToEventGridAsync(List<GridEvent<object>> data)
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
