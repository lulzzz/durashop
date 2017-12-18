using durashoppingcart.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace durashoppingcart.Utils
{
    public static class EventGridReminder
    {
        static string topicEndpoint = "https://durashop-cartreminder.westus2-1.eventgrid.azure.net/api/events";
        static string sasKey = "oF1lQoeDI/H/AmVlDTPMqjKye5jafkO8Zga3tM8vuwM=";

        public static async void Add(CartInstance ci)
        {
            List<GridEvent<object>> eventList = new List<GridEvent<object>>();
            GridEvent<object> eventCartReminder = new GridEvent<object>
            {
                Subject = $"CART.REMINDER",
                EventType = $"REMINDER.ITEMSINCART",
                EventTime = DateTime.UtcNow,
                Id = ci.input.FirstOrDefault().CartId,
                Data = ci
            };

            eventList.Add(eventCartReminder);
            await PostToEventGridAsync(eventList);
        }

        private static async Task PostToEventGridAsync(List<GridEvent<object>> data)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("aeg-sas-key", sasKey);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("DuraShoppingCartReminder");

                string json = JsonConvert.SerializeObject(data);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, topicEndpoint)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                HttpResponseMessage response = await client.SendAsync(request);
            }
        }
    }
}
