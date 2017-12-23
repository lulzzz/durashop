using durashoppingcart.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace durashoppingcart
{
    public class CartOrchestrator
    {
        [FunctionName("CartOrchestrator")]
        public static async Task<List<CartData>> Run([OrchestrationTrigger]DurableOrchestrationContext context, TraceWriter log)
        {
            var cartList = context.GetInput<List<CartData>>() ?? new List<CartData>();

            var addItemTask = context.WaitForExternalEvent<CartData>(CartEvents.AddItem);
            var removeItemTask = context.WaitForExternalEvent<CartData>(CartEvents.RemoveItem);
            var isCompletedTask = context.WaitForExternalEvent<bool>(CartEvents.IsCompleted);
            var setCartReminder = context.WaitForExternalEvent<CartReminderData>(CartEvents.SetReminder);

            // Wait for external events
            var resultingEvent = await Task.WhenAny(addItemTask, removeItemTask, isCompletedTask, setCartReminder);

            // Add item to cart
            if (resultingEvent == addItemTask)
            {
                log.Info($"cartList IsReplaying: {context.IsReplaying}");
                cartList.Add(addItemTask.Result);
                log.Info($"Added {addItemTask.Result.ItemName} to the Shopping Cart.");
            }

            // Remove Item from cart
            else if (resultingEvent == removeItemTask)
            {
                cartList.Remove(cartList.Where(x => x.ItemId == removeItemTask.Result.ItemId).FirstOrDefault());
                log.Info($"Removed {removeItemTask.Result.ItemName} from the Shopping Cart.");
            }

            // Add reminder for cart (used to notify user of waiting cart with items)
            else if (resultingEvent == setCartReminder)
            {
                var provisionTask = context.CallSubOrchestratorAsync("SetCartNotificationTimer", setCartReminder.Result);
            }

            // Complete cart or stay running ?
            if (resultingEvent == isCompletedTask && isCompletedTask.Result)
            {
                log.Info("Completed the Shopping Cart.");
            }
            else
            {
                context.ContinueAsNew(cartList); // the magic line
                log.Info($"cartList Count: {cartList.Count}");
                log.Info($"cartList IsReplaying: {context.IsReplaying}");
            }

            return cartList;
        }
    }
}
