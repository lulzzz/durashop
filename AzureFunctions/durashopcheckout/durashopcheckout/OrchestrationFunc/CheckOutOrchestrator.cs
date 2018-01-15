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

            var retryOptionsCartContent = new RetryOptions(firstRetryInterval: TimeSpan.FromSeconds(5), maxNumberOfAttempts: 3);
            var retryOptionsPayment = new RetryOptions(firstRetryInterval: TimeSpan.FromSeconds(4), maxNumberOfAttempts: 2);

            try
            {
                var cart = await cartContext.CallActivityWithRetryAsync<CartInstance>("GetCartContent", retryOptionsCartContent, cartInfo.CartUrl);
                var payment = await cartContext.CallActivityWithRetryAsync<Task<bool>>("HandlePayment", retryOptionsPayment, TotalSum(cart));
                var notif = await cartContext.CallActivityAsync<Task<bool>>("SendUserConfirmation", TotalSum(cart));
                var order = await cartContext.CallActivityAsync<Task<bool>>("UpdateOrderSystem", cart);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

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
            // Call some PSP blabla
            log.Info($"Crediting total sum of '{amount}'...");
            return true;
        }

        [FunctionName("SendUserConfirmation")]
        public static bool SendUserConfirmation([ActivityTrigger] List<CartData> cartDataList, TraceWriter log)
        {
            DuraShop.EventGrid.PublishCommunication.Push
            (
                new DuraShop.EventGrid.NotifData { From = "order@durashop.com", To = "johan.eriksson@stratiteq.com", Body = $"Your {cartDataList.Count} items from DuraShop are about to ship", Subject = "DuraShop Order Confirmation" },
                cartDataList.FirstOrDefault().CartId,
                DuraShop.EventGrid.Conf.Subject.MAIL,
                DuraShop.EventGrid.Conf.EventType.ORDERCONFIRMATION
            );

            log.Info($"Sending receipt and confirmation to '{cartDataList[0].UserId}'..."); // Event Grid ?!
            return true;
        }

        [FunctionName("UpdateOrderSystem")]
        public static bool UpdateOrderSystem([ActivityTrigger] List<CartData> cartDataList, TraceWriter log)
        {
            // Update some backend order system
            log.Info($"Updating Order System with '{cartDataList.Count}' products...");
            return true;
        }

        static double TotalSum(CartInstance cart) => cart.input.Sum(item => item.Price);
    }
}