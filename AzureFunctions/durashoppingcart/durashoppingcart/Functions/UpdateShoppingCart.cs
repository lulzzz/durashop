using durashoppingcart.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace durashoppingcart.Functions
{
    public static class UpdateShoppingCart
    {
        [FunctionName("UpdateShoppingCart")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", "delete", Route = "Cart/Update")]HttpRequestMessage req, [OrchestrationClient]DurableOrchestrationClient orchestrationClient, TraceWriter log)
        {
            var eventData = await req.Content.ReadAsAsync<CartData>();

            string eventName = req.Method == HttpMethod.Delete ? CartEvents.RemoveItem : CartEvents.AddItem;

            await orchestrationClient.RaiseEventAsync(eventData.CartId, eventName, eventData);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}


