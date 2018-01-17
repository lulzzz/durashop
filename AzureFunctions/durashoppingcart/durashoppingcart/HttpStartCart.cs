using durashoppingcart.Models;
using durashoppingcart.Polling;
using durashoppingcart.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace durashoppingcart
{
    public static class HttpStartCart
    {
        [FunctionName("HttpStartCart")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "{functionName}")]HttpRequestMessage req, [OrchestrationClient]DurableOrchestrationClient orchestrationClient, string functionName, TraceWriter log)
        {
            dynamic eventData = await req.Content.ReadAsAsync<object>();
            string instanceId = await orchestrationClient.StartNewAsync(functionName, eventData);

            log.Info($"Started orchestration with ID = '{instanceId}'.");

            var orchStatus = orchestrationClient.CreateCheckStatusResponse(req, instanceId);
            var clientResponse = JsonConvert.DeserializeObject<OrchestrationClientResponse>(await orchStatus.Content.ReadAsStringAsync());
            var executionDetails = Helper.GetExecutionDetails(RequestMethod.StartCart);
            await ProvideOutput.DoIt(clientResponse, executionDetails, log);

            return orchestrationClient.CreateCheckStatusResponse(req, instanceId);
        }
    }
}