using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace durashopcommunication
{
    public static class CartReminderMail
    {
        [FunctionName("CartReminderMail")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            var data = await req.Content.ReadAsAsync<IEnumerable<dynamic>>();
            foreach (var item in data)
                log.Info($"{item.Id}\r\n{ item.Data.runtimeStatus}");


            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}