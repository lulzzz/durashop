﻿using durashoppingcart.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace durashoppingcart
{
    public class CartOrchestrator
    {
        [FunctionName("CartOrchestrator")]
        public static async Task<List<CartData>> Run([OrchestrationTrigger]DurableOrchestrationContext context, TraceWriter log)
        {
            var cartList = context.GetInput<List<CartData>>() ?? new List<CartData>();
            //var cts = new CancellationTokenSource();
            DateTime deadline = context.CurrentUtcDateTime.Add(TimeSpan.FromMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["notifiertimeout-min"])));


            var addItemTask = context.WaitForExternalEvent<CartData>(CartEvents.AddItem);
            var removeItemTask = context.WaitForExternalEvent<CartData>(CartEvents.RemoveItem);
            //var getCartItemsTask = context.WaitForExternalEvent<CartData>(CartEvents.GetItems);
            var isCompletedTask = context.WaitForExternalEvent<bool>(CartEvents.IsCompleted);
            var notifyTask = context.CreateTimer(deadline, new CancellationToken());

            var resultingEvent = await Task.WhenAny(addItemTask, removeItemTask, isCompletedTask);

            if (resultingEvent == addItemTask)
            {
                cartList.Add(addItemTask.Result);
                log.Info($"Added {addItemTask.Result.ItemName} to the Shopping Cart.");
            }

            else if (resultingEvent == removeItemTask)
            {
                cartList.Remove(cartList.Where(x => x.ItemId == removeItemTask.Result.ItemId).FirstOrDefault());
                log.Info($"Removed {removeItemTask.Result.ItemName} from the Shopping Cart.");
            }

            if (resultingEvent == notifyTask)
            {
                await context.CallActivityAsync("NotifyUser", $"You have {cartList.Count} items in your cart");
                log.Info($"Shopping Cart Notification sent.");
            }

            if (resultingEvent == isCompletedTask && isCompletedTask.Result)
            {
                //if (notifyTask.Status == TaskStatus.Running)
                //{
                //    cts.Cancel();
                //}
                log.Info("Completed the Shopping Cart.");
            }
            else
            {
                context.ContinueAsNew(cartList);
            }

            return cartList;
        }

        [FunctionName("NotifyUser")]
        public static string NotifyUser([ActivityTrigger] string name)
        {
            // push to eventgrid
            return $"Hello {name}!";
        }
    }
}
