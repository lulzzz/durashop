using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace duranotification
{
    internal class SMS
    {
        internal static void Send(string to, string from, string text)
        {
            if (string.IsNullOrEmpty(from)) { from = Environment.GetEnvironmentVariable("TwilioPhoneNumber"); };
            TwilioClient.Init(Environment.GetEnvironmentVariable("TwilioAccountSid"), Environment.GetEnvironmentVariable("TwilioAuthToken"));

            var message = MessageResource.Create(
                new PhoneNumber(to),
                from: new PhoneNumber(from),
                body: text
            );
        }
    }
}
