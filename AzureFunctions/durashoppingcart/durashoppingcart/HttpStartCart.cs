using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace durashoppingcart
{
    public static class HttpStartCart
    {
        private const string Timeout = "timeout";
        private const string RetryInterval = "retryInterval";

        [FunctionName("HttpStartCart")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "{functionName}")]HttpRequestMessage req, [OrchestrationClient]DurableOrchestrationClient orchestrationClient, string functionName, TraceWriter log)
        {
            dynamic eventData = await req.Content.ReadAsAsync<object>().ConfigureAwait(false);
            string instanceId = await orchestrationClient.StartNewAsync(functionName, eventData).ConfigureAwait(false);

            log.Info($"Started orchestration with ID = '{instanceId}'.");

            TimeSpan timeout = GetTimeSpan(req, Timeout) ?? TimeSpan.FromSeconds(5);
            TimeSpan retryInterval = GetTimeSpan(req, RetryInterval) ?? TimeSpan.FromSeconds(1);

            return await orchestrationClient.WaitForCompletionOrCreateCheckStatusResponseAsync(
                req,
                instanceId,
                timeout,
                retryInterval).ConfigureAwait(false);
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