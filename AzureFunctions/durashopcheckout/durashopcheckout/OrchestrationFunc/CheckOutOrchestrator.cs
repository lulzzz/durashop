using durashopcheckout.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace durashopcheckout.OrchestrationFunc
{
    public static class CheckOutOrchestrator
    {
        [FunctionName("CheckOutOrchestrator")]
        public static async Task<HttpResponseMessage> Run([OrchestrationTrigger] DurableOrchestrationContext cartContext)
        {
            // Fetch cart contents based on OrchestrationInstanceId in cartContext
            string cartId = cartContext.GetInput<string>()?.Trim();
            if (string.IsNullOrEmpty(cartId)) { return new HttpResponseMessage(HttpStatusCode.BadRequest); }

            // Function chaining
            var cartList = await cartContext.CallActivityAsync<List<CartData>>("GetCartContent", cartId);
            var payment = await cartContext.CallActivityAsync<Task<bool>>("HandlePayment", TotalSum(cartList));
            var notif = await cartContext.CallActivityAsync<Task<bool>>("SendUserConfirmation", TotalSum(cartList));
            var order = await cartContext.CallActivityAsync<Task<bool>>("UpdateOrderSystem", cartList);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [FunctionName("GetCartContent")]
        public static Task<List<CartData>> GetCartContent([ActivityTrigger] string cartId, TraceWriter log)
        {
            log.Info($"Searching for Shopping Cart with OrchestrationInstanceId '{cartId}'...");
            return null;
        }


        [FunctionName("HandlePayment")]
        public static bool HandlePayment([ActivityTrigger] Double amount, TraceWriter log)
        {
            log.Info($"Crediting total sum of '{amount}'...");
            return true;
        }

        [FunctionName("SendUserConfirmation")]
        public static bool SendUserConfirmation([ActivityTrigger] List<CartData> cartDataList, TraceWriter log)
        {
            log.Info($"Sending receipt and confirmation to '{cartDataList[0].UserEmail}'..."); // Event Grid ?!
            return true;
        }

        [FunctionName("UpdateOrderSystem")]
        public static bool UpdateOrderSystem([ActivityTrigger] List<CartData> cartDataList, TraceWriter log)
        {
            log.Info($"Updating Order System with '{cartDataList.Count}' products...");
            return true;
        }

        private static double TotalSum(List<CartData> cartList)
        {
            return cartList.Sum(item => item.Price);
        }
    }
}
