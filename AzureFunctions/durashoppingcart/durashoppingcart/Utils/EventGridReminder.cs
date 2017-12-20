using durashoppingcart.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace durashoppingcart.Utils
{
    public static class EventGridReminder
    {
        static string topicEndpoint = ConfigurationManager.AppSettings["topicEndpoint"];
        static string sasKey = ConfigurationManager.AppSettings["sasKey"];

        public static async void Add(CartInstance ci)
        {
            List<GridEvent<object>> eventList = new List<GridEvent<object>>();
            GridEvent<object> eventCartReminder = new GridEvent<object>
            {
                Subject = $"CART.REMINDER",
                EventType = $"REMINDER.ITEMSINCART.MAIL",
                EventTime = DateTime.UtcNow,
                Id = ci.input.FirstOrDefault().CartId,
                Data = ci
            };

            eventList.Add(eventCartReminder);
            await PostToEventGridAsync(eventList);
        }

        static async Task PostToEventGridAsync(List<GridEvent<object>> data)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("aeg-sas-key", sasKey);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("DuraShoppingCartReminder");

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
