using durashoppingcart.Models;
using Microsoft.Azure.WebJobs;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace durashoppingcart.Functions
{
    public class CartReminderTimer
    {
        [FunctionName("SetCartReminderTimer")]
        public static async Task SetCartReminderTimer([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            CartInstance cInstance;
            var remData = context.GetInput<CartReminderData>();

            // Set the timer. This means that this function will "sleep" until timespan happens
            var notifiyTime = context.CurrentUtcDateTime.Add(TimeSpan.FromMinutes(Convert.ToDouble(remData.RemindInMinutes)));
            await context.CreateTimer(notifiyTime, CancellationToken.None);

            // Need to query the cart
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(new Uri(remData.CartUrl)).Result;
                cInstance = await response.Content.ReadAsAsync<CartInstance>();
            }

            // Only act if the cart is in status "Running" AND it contains > 0 items
            if ((cInstance.input.Count > 1) && (cInstance.runtimeStatus == "Running"))
            {
                var ett = 0;
                ett++;
                // Push notif to Event Grid (mail/SMS or whatever)
            }
        }
    }
}
