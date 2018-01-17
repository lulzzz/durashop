using durashoppingcart.Models;
using durashoppingcart.Polling;
using durashoppingcart.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
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
            var eventData = await req.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<CartData>(eventData);

            var eventName = req.Method == HttpMethod.Delete ? CartEvents.RemoveItem : CartEvents.AddItem;

            await orchestrationClient.RaiseEventAsync(data.CartId, eventName, data);

            //Polling
            var executionContext = Helper.GetExecutionDetails(req.Method == HttpMethod.Delete ? RequestMethod.DeleteItem : RequestMethod.AddItem);
            var responseOrchestration = orchestrationClient.CreateCheckStatusResponse(req, data.CartId);
            var clientResponse = JsonConvert.DeserializeObject<OrchestrationClientResponse>(await responseOrchestration.Content.ReadAsStringAsync());
            var cartItems = await ProvideOutput.DoIt(clientResponse, executionContext, log);

            return req.CreateResponse(HttpStatusCode.OK, cartItems);
        }
    }
}


