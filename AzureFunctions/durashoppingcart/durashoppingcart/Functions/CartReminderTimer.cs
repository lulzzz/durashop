using durashoppingcart.Models;
using DuraShop.EventGrid;
using Microsoft.Azure.WebJobs;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace durashoppingcart.Functions
{
    public class CartReminder
    {
        [FunctionName("SetCartNotificationTimer")]
        public static async Task SetCartNotificationTimer([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            CartInstance cInstance;
            var remData = context.GetInput<CartReminderData>();

            // Set the timer. This means that this function will "sleep" until timespan happens
            var notifiyTime = context.CurrentUtcDateTime.Add(TimeSpan.FromMinutes(Convert.ToDouble(remData.RemindInMinutes))); // FromMinutes for testing purposes, probably should be FromHours or FromDays
            await context.CreateTimer(notifiyTime, CancellationToken.None);

            // Get the cart instance
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(new Uri(remData.CartUrl)).Result;
                cInstance = await response.Content.ReadAsAsync<CartInstance>();
            }

            // Only notify if the cart is in status "Running" AND it contains > 0 items
            if ((cInstance != null) && (cInstance.input.Count > 0) && (cInstance.runtimeStatus == "Running"))
            {
                // Push notif to Event Grid
                var noti = new Models.NotifData { From = "team@durashop.com", To = "johan.eriksson@stratiteq.com", Body = $"Just a friendly reminder.\r\nYou have {cInstance.input.Count} items in your DuraShop Cart", Subject = "Shopping Cart items at DuraShop" };
                var notif = JsonConvert.SerializeObject(noti);

                DuraShop.EventGrid.PublishCommunication.Push(
                    notif,
                    cInstance.input.FirstOrDefault().CartId,
                    (Conf.Subject)Enum.Parse(typeof(Conf.Subject), remData.NotificationType),
                    Conf.EventType.REMINDERITEMSINCART
                    );
            }
        }
    }
}