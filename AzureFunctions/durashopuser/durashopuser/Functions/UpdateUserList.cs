using durashopuser.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace durashopuser.Functions
{
    public static class UpdateUser
    {
        [FunctionName("UpdateUser")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "User/Update")]HttpRequestMessage req, [OrchestrationClient]DurableOrchestrationClient orchestrationClient, TraceWriter log)
        {
            var eventData = await req.Content.ReadAsAsync<UserData>();

            var eventName = UserEvents.UpdateUser;

            await orchestrationClient.RaiseEventAsync(eventData.UserId, eventName, eventData);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
