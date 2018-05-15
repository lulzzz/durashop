using DuraShop.EventGridSend;
using durashopcheckout.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
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
            if ((cartInfo.CartId == null) || (cartInfo.CartUrl == null)) { return new HttpResponseMessage(HttpStatusCode.BadRequest); }

            // retryoptions
            var retryOptionsCartContent = new RetryOptions(firstRetryInterval: TimeSpan.FromSeconds(5), maxNumberOfAttempts: 3);
            var retryOptionsPayment = new RetryOptions(firstRetryInterval: TimeSpan.FromSeconds(4), maxNumberOfAttempts: 2);

            // Start the workflow
            try
            {
                var payment = await cartContext.CallActivityAsync<bool>("HandlePayment", 22);
                var cart = await cartContext.CallActivityWithRetryAsync<CartInstance>("GetCartContent", retryOptionsCartContent, cartInfo.CartUrl);
                var notif = await cartContext.CallActivityWithRetryAsync<bool>("SendReceipt", retryOptionsPayment, cart);
                if (notif == false) throw new Exception("Hoppsan");

                var order = await cartContext.CallActivityAsync<bool>("UpdateOrderSystem", cart);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [FunctionName("HandlePayment")]
        public static Task<bool> HandlePayment([ActivityTrigger] double amount, TraceWriter log)
        {
            // Call some PSP blabla
            log.Info($"Crediting total sum of '{amount}'...");
            return Task.FromResult(true);
        }

        [FunctionName("GetCartContent")]
        public static async Task<CartInstance> GetCartContent([ActivityTrigger] string cartUrl, TraceWriter log)
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

        [FunctionName("SendReceipt")]
        public static async Task<bool> SendUserConfirmationAsync([ActivityTrigger] CartInstance cart, TraceWriter log)
        {
            // Push receipt to Event Grid
            await Event.PushNotification(
                new NotifData {
                    From = "order@durashop.com",
                    To = "johan.eriksson@stratiteq.com",
                    Body = $"Your {cart.input.Count} items from DuraShop are about to ship",
                    Subject = "DuraShop Receipt"
                },
                cart.input.FirstOrDefault().CartId,
                "MAIL",
                "durashop.notification.RECEIPT");

            log.Info($"Sending receipt and confirmation to '{cart.input[0].UserEmail}'...");

            return true;
        }

        [FunctionName("UpdateOrderSystem")]
        public static bool UpdateOrderSystem([ActivityTrigger] CartInstance cart, TraceWriter log)
        {
            // Update some backend order system
            log.Info($"Updating Order System with '{cart.input.Count}' products...");
            return true;
        }

        static double TotalSum(CartInstance cart) => cart.input.Sum(item => item.Price);
    }
}