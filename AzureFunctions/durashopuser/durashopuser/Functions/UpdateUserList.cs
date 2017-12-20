using durashopuser.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace durashopuser.Functions
{
    public static class UpdateUserList
    {
        [FunctionName("UpdateUserList")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", "delete", "put", Route = "User/Update")]HttpRequestMessage req, [OrchestrationClient]DurableOrchestrationClient orchestrationClient, TraceWriter log)
        {
            var eventData = await req.Content.ReadAsAsync<UserData>();
            var eventName = UserEvents.AddUser;

            if (req.Method == HttpMethod.Delete) { eventName = UserEvents.DeleteUser; }
            else if (req.Method == HttpMethod.Post) { eventName = UserEvents.AddUser; }
            else if (req.Method == HttpMethod.Put) { eventName = UserEvents.UpdateUser; }

            await orchestrationClient.RaiseEventAsync(eventData.UserId, eventName, eventData);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
