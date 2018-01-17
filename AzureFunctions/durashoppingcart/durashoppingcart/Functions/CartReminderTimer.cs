using DuraShop.EventGrid;
using durashoppingcart.Models;
using Microsoft.Azure.WebJobs;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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
                // Just some mockdata
                string toAddress = string.Empty, fromAddress = string.Empty;
                switch (remData.NotificationType)
                {
                    case "SMS":
                        toAddress = "+46765269844";
                        fromAddress = "+46769439183";
                        break;
                    case "MAIL":
                        toAddress = "jed.johan@gmail.com";
                        fromAddress = "noreply@durashop.com";
                        break;
                }

                // Push notif to Event Grid
                await DuraShop.EventGrid.PublishCommunication.Push(
                    new DuraShop.EventGrid.NotifData { From = fromAddress, To = toAddress, Body = $"Just a friendly reminder. \r\nYou have {cInstance.input.Count} items in your DuraShop Cart", Subject = "Shopping Cart items at DuraShop" },
                    cInstance.input.FirstOrDefault().CartId,
                    (Conf.Subject)Enum.Parse(typeof(Conf.Subject), remData.NotificationType),
                    Conf.EventType.REMINDERITEMSINCART
                    );
            }
        }
    }
}