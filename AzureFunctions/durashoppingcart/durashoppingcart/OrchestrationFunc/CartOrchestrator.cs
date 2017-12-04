using durashoppingcart.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace durashoppingcart
{
    public class CartOrchestrator
    {
        [FunctionName("CartOrchestrator")]
        public static async Task<List<CartEventData>> Run([OrchestrationTrigger]DurableOrchestrationContext context, TraceWriter log)
        {
            var cartList = context.GetInput<List<CartEventData>>() ?? new List<CartEventData>();

            var addItemTask = context.WaitForExternalEvent<CartEventData>(CartEvents.AddItem);
            var removeItemTask = context.WaitForExternalEvent<CartEventData>(CartEvents.RemoveItem);
            var clearCartTask = context.WaitForExternalEvent<bool>(CartEvents.ClearCart);
            var isCompletedTask = context.WaitForExternalEvent<bool>(CartEvents.IsCompleted);

            var resultingEvent = await Task.WhenAny(addItemTask, removeItemTask, isCompletedTask, clearCartTask);

            if (resultingEvent == addItemTask)
            {
                cartList.Add(addItemTask.Result);
                log.Info($"Added {addItemTask.Result.ItemName} to the Shopping Cart.");
            }
            else if (resultingEvent == removeItemTask)
            {
                cartList.Remove(removeItemTask.Result);
                log.Info($"Removed {removeItemTask.Result.ItemName} from the Shopping Cart.");
            }
            else if (resultingEvent == clearCartTask)
            {
                cartList.Clear();
                log.Info($"Shopping Cart cleared.");
            }

            if (resultingEvent == isCompletedTask && isCompletedTask.Result)
            {
                log.Info("Completed updating the Shopping Cart.");
            }
            else
            {
                context.ContinueAsNew(cartList);
            }

            return cartList;
        }
    }
}
