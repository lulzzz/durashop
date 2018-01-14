using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace durashopcommunication
{
    public static class MFASMS
    {
        public static class SendSMS
        {
            [FunctionName("SendSMS")]
            public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
            {
                log.Info("C# HTTP trigger SendSMS processed a request from EventGrid.");
                var eventData = await req.Content.ReadAsStringAsync();
                var events = JsonConvert.DeserializeObject<GridEvent[]>(eventData);

                foreach (var item in events)
                    log.Info($"{item.Id}\r\n{item.EventType}\r\n{item.Subject}");

                return req.CreateResponse(HttpStatusCode.OK);
            }
        }
    }
}