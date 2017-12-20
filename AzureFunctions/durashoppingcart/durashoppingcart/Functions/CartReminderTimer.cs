using durashoppingcart.Models;
using durashoppingcart.Utils;
using Microsoft.Azure.WebJobs;
using System;
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
            var notifiyTime = context.CurrentUtcDateTime.Add(TimeSpan.FromMinutes(Convert.ToDouble(remData.RemindInMinutes)));
            await context.CreateTimer(notifiyTime, CancellationToken.None);

            // Query the cart
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(new Uri(remData.CartUrl)).Result;
                cInstance = await response.Content.ReadAsAsync<CartInstance>();
            }

            // Only notify if the cart is in status "Running" AND it contains > 0 items
            if ((cInstance.input.Count > 0) && (cInstance.runtimeStatus == "Running"))
            {
                // Push notif to Event Grid (mail/SMS or whatever)
                EventGridReminder.Add(cInstance);
            }
        }
    }
}