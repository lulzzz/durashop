using durashoppingcart.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace durashoppingcart.Functions
{
    public class SetCartReminder
    {
        [FunctionName("SetCartReminder")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Cart/CartReminder")]HttpRequestMessage req, [OrchestrationClient]DurableOrchestrationClient orchestrationClient, TraceWriter log)
        {
            var eventData = await req.Content.ReadAsAsync<CartReminderData>();

            await orchestrationClient.RaiseEventAsync(eventData.CartId, CartEvents.SetReminder, eventData);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}