using durashopcheckout.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace durashopcheckout.OrchestrationFunc
{
    public static class CheckOutOrchestrator
    {
        [FunctionName("CheckOutOrchestrator")]
        public static async Task<HttpResponseMessage> Run([OrchestrationTrigger] DurableOrchestrationContext cartContext)
        {
            // Get input data
            var cartInfo = cartContext.GetInput<CheckoutData>() ?? new CheckoutData();

            if (cartInfo == null) { return new HttpResponseMessage(HttpStatusCode.BadRequest); }

            // Function chaining or fan-out/fan-in ? Fråga Robin
            var cart = await cartContext.CallActivityAsync<CartInstance>("GetCartContent", cartInfo.CartUrl);
            var payment = await cartContext.CallActivityAsync<Task<bool>>("HandlePayment", TotalSum(cart));
            var notif = await cartContext.CallActivityAsync<Task<bool>>("SendUserConfirmation", TotalSum(cart));
            var order = await cartContext.CallActivityAsync<Task<bool>>("UpdateOrderSystem", cart);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [FunctionName("GetCartContent")]
        public static async Task<CartInstance> GetCartContentAsync([ActivityTrigger] string cartUrl, DurableOrchestrationContext ctx, TraceWriter log)
        {
            CartInstance cInstance;
            log.Info($"Searching for Shopping Cart with URL: '{cartUrl}'...");

            // query the cart
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(new Uri(cartUrl)).Result;
                cInstance = await response.Content.ReadAsAsync<CartInstance>();
            }

            return cInstance;
        }

        [FunctionName("HandlePayment")]
        public static bool HandlePayment([ActivityTrigger] double amount, TraceWriter log)
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

        static double TotalSum(CartInstance cart) => cart.input.Sum(item => item.Price);
    }
}