using durashopcommunication.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace durashopcommunication
{
    public static class EventSubscriber
    {
        [FunctionName("Send")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger processed a request from EventGrid.");
            var eventData = await req.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<GridEvent[]>(eventData);

            foreach (var item in events)
            {
                log.Info($"{item.Id}\r\n{item.EventType}\r\n{item.Subject}");
                if (item.Subject == "MAIL")
                {
                    Mail.Send(item.Data["To"], item.Data["From"], item.Data["Subject"], item.Data["Body"]);
                }
                if (item.Subject == "SMS")
                {
                    Mail.Send(item.Data["To"], item.Data["From"], item.Data["Subject"], item.Data["Body"]);
                }

            }




            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}