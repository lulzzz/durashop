using System.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace durashopcommunication.Utils
{
    internal class SMS
    {
        internal static void Send(string to, string from, string text)
        {
            TwilioClient.Init(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);

            var message = MessageResource.Create(
                new PhoneNumber(to),
                from: new PhoneNumber(from),
                body: text
            );
        }
    }
}
