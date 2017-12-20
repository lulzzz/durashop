using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using System.Threading.Tasks;

namespace durashopuser
{
    public static class HttpStart
    {
        [FunctionName("HttpStart")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "orchestration/{functionName}/{userId?}")]HttpRequestMessage req, [OrchestrationClient]DurableOrchestrationClient orchestrationClient, string functionName, string userId, TraceWriter log)
        {
            DurableOrchestrationStatus existingUser = null;

            // Check if an instance with the specified ID already exists.
            if (userId != null) { existingUser = await orchestrationClient.GetStatusAsync(userId); };

            if (existingUser == null)
            {
                // A user with the specified ID doesn't exist, create one.
                dynamic eventData = await req.Content.ReadAsAsync<object>();
                string instanceId = await orchestrationClient.StartNewAsync(functionName, eventData);
                log.Info($"Started user orchestration with ID = '{instanceId}'.");
            }
            else
            {
                // A user with the specified ID exists, don't create one.
                log.Info($"An instance with userID {userId}, already exists: Got existing instance (name {existingUser.Name}). status {existingUser.RuntimeStatus})");
            }

            return orchestrationClient.CreateCheckStatusResponse(req, userId);
        }
    }
}