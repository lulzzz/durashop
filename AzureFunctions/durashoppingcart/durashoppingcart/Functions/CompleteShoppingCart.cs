using durashoppingcart.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace durashoppingcart.Functions
{
    public class CompleteShoppingCart
    {
        [FunctionName("CompleteShoppingCart")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Cart/Complete")]HttpRequestMessage req, [OrchestrationClient]DurableOrchestrationClient orchestrationClient, TraceWriter log)
        {
            var eventData = await req.Content.ReadAsAsync<CompleteCartEventData>();

            await orchestrationClient.RaiseEventAsync(eventData.OrchestrationInstanceId, CartEvents.IsCompleted, CartEvents.IsCompleted);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
