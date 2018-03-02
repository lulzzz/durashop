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
    public static class Notification
    {
        [FunctionName("SendMailSMS")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger processed a (notification) request from EventGrid.");
            var eventData = await req.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<NotificationGridEvent[]>(eventData);

            foreach (var notif in events)
            {
                log.Info(message: $"{notif.Id}\r\n{notif.EventType}\r\n{notif.Subject}\r\n{notif.Data.To}");

                if (notif.Subject == "MAIL")
                {
                    Mail.Send(notif.Data.To, notif.Data.From, notif.Data.Subject, notif.Data.Body, log);
                }

                if (notif.Subject == "SMS")
                {
                    SMS.Send(notif.Data.To, notif.Data.From, notif.Data.Body);
                }
            }

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}