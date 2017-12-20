using durashopuser.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace durashopuser
{
    public static class HttpStartUser
    {
        [FunctionName("HttpStartUser")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "{functionName}")]HttpRequestMessage req, [OrchestrationClient]DurableOrchestrationClient orchestrationClient, string functionName, TraceWriter log)
        {
            var uData = await req.Content.ReadAsAsync<UserData>();
            var userId = uData.UserId;

            if (userId == null) { return req.CreateResponse(HttpStatusCode.BadRequest, "UserId value missing", new JsonMediaTypeFormatter()); }

            // Check if an instance with the specified ID already exists.
            var existingUser = await orchestrationClient.GetStatusAsync(userId);

            if (existingUser == null)
            {
                // A user with the specified ID doesn't exist, create one.
                userId = await orchestrationClient.StartNewAsync(functionName, userId, uData);
                log.Info($"Started user orchestration with ID = '{userId}'.");
            }
            else
            {
                // A user with the specified ID exists, don't create one. Return BadRequest maybe ?
                log.Info($"An instance with userID {userId}, already exists: Got existing instance (name {existingUser.Name}). status {existingUser.RuntimeStatus})");
            }

            return orchestrationClient.CreateCheckStatusResponse(req, userId);
        }
    }
}