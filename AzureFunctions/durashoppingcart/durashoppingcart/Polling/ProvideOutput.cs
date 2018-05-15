using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using durashoppingcart.Models;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace durashoppingcart.Polling
{
    public static class ProvideOutput
    {
        public static async Task<object> DoIt(OrchestrationClientResponse clientResponse, ExecutionDetails executionDetails, TraceWriter log)
        {
            object result = null;

            using (var httpClient = new HttpClient())
            {
                while (true)
                {
                    Thread.Sleep(executionDetails.IterationPeriod);
                    string statusCheck;
                    try
                    {

                        statusCheck = await httpClient.GetStringAsync(clientResponse.StatusQueryGetUri);
                    }
                    catch (Exception e)
                    {
                        // log the exception
                        log.Error(e.Message);
                        continue;
                    }

                    var status = JsonConvert.DeserializeObject<StatusResponse>(statusCheck);
                    if (status.RuntimeStatus.ToLower() == "continuedasnew") continue;
                    if (status.CustomStatus.ToLower() != "readynow") continue;
                    if (executionDetails.ReqMethod == RequestMethod.AddItem && (status.Input == null || status.Input.Count <= 0)) continue;

                    result = status.Input;
                    break;
                }
            }

            return result;
        }
    }
}
