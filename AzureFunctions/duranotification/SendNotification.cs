// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGridExtensionConfig?functionName={functionname}

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Newtonsoft.Json;

namespace duranotification
{
    public static class SendNotification
    {
        [FunctionName("SendNotification")]
        public static void Run([EventGridTrigger]EventGridEvent eventGridEvent, TraceWriter log)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<NotificationGridEventData>(JsonConvert.SerializeObject(eventGridEvent.Data));
                log.Info(message: $"{eventGridEvent.Id}\r\n{eventGridEvent.EventType}\r\n{eventGridEvent.Subject}\r\n{data.To}");

                if (eventGridEvent.Subject == "MAIL")
                {
                    Mail.Send(data.To, data.From, data.Subject, data.Body, log);
                }

                if (eventGridEvent.Subject == "SMS")
                {
                    SMS.Send(data.To, data.From, data.Body);
                }
            }
            catch (System.Exception e)
            {
                log.Info($"Error : {e.Message}");
            }
        }
    }
}