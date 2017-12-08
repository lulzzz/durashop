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

namespace durashoppingcart
{
    public class ShoppingCartInfo
    {
        [FunctionName("ShoppingCartInfo")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Cart/Info")]HttpRequestMessage req, [OrchestrationClient]DurableOrchestrationClient orchestrationClient, TraceWriter log)
        {
            var eventData = await req.Content.ReadAsAsync<GetCartEventData>();

            //await orchestrationClient.RaiseEventAsync(eventData.CartId, CartEvents.GetItems, eventData);

            return orchestrationClient.CreateCheckStatusResponse(req, eventData.CartId);

            //return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
