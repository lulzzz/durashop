using durashoppingcart.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace durashoppingcart.Functions
{
    public static class UpdateShoppingCart
    {
        [FunctionName("UpdateShoppingCart")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", "delete", Route = "Cart/Update")]HttpRequestMessage req, [OrchestrationClient]DurableOrchestrationClient orchestrationClient, TraceWriter log)
        {
            var eventData = await req.Content.ReadAsAsync<CartData>();

            var eventName = req.Method == HttpMethod.Delete ? CartEvents.RemoveItem : CartEvents.AddItem;

            await orchestrationClient.RaiseEventAsync(eventData.OrchestrationInstanceId, eventName, eventData);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}


