using durashoppingcart.Models;
using durashoppingcart.Polling;
using durashoppingcart.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace durashoppingcart
{
    public static class HttpStartCart
    {
        private const string Timeout = "timeout";
        private const string RetryInterval = "retryInterval";

        [FunctionName("HttpStartCart")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "{functionName}")]HttpRequestMessage req, [OrchestrationClient]DurableOrchestrationClientBase orchestrationClient, string functionName, TraceWriter log)
        {
            dynamic eventData = await req.Content.ReadAsAsync<object>();
            string instanceId = await orchestrationClient.StartNewAsync(functionName, eventData);

            log.Info($"Started orchestration with ID = '{instanceId}'.");

            TimeSpan timeout = GetTimeSpan(req, Timeout) ?? TimeSpan.FromSeconds(5);
            TimeSpan retryInterval = GetTimeSpan(req, RetryInterval) ?? TimeSpan.FromSeconds(1);

            return await orchestrationClient.WaitForCompletionOrCreateCheckStatusResponseAsync(
                req,
                instanceId,
                timeout,
                retryInterval).ConfigureAwait(false);

            //var orchStatus = orchestrationClient.CreateCheckStatusResponse(req, instanceId);
            //var clientResponse = JsonConvert.DeserializeObject<OrchestrationClientResponse>(await orchStatus.Content.ReadAsStringAsync());
            //var executionDetails = Helper.GetExecutionDetails(RequestMethod.StartCart);
            //await ProvideOutput.DoIt(clientResponse, executionDetails, log);

            //return orchestrationClient.CreateCheckStatusResponse(req, instanceId);


        }

        private static TimeSpan? GetTimeSpan(HttpRequestMessage request, string queryParameterName)
        {
            string queryParameterStringValue = request.RequestUri.ParseQueryString()[queryParameterName];
            if (string.IsNullOrEmpty(queryParameterStringValue))
            {
                return null;
            }

            return TimeSpan.FromSeconds(double.Parse(queryParameterStringValue));
        }
    }
}