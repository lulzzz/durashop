using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Twilio;

namespace durashopmfa
{
    public static class PhoneVerification
    {
        [FunctionName("SMSPhoneVerification")]
        public static async Task<bool> Run([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            string phoneNumber = context.GetInput<string>();
            if (string.IsNullOrEmpty(phoneNumber)) { throw new ArgumentNullException(nameof(phoneNumber), "A phone number input is required.");}

            int challengeCode = await context.CallActivityAsync<int>("SendSMSChallenge",phoneNumber);

            using (var timeoutCts = new CancellationTokenSource())
            {
                // Give the user 90 seconds to respond
                DateTime expiration = context.CurrentUtcDateTime.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["smschallengetimeout-sec"])); 
                Task timeoutTask = context.CreateTimer(expiration, timeoutCts.Token);

                bool authorized = false;
                for (int retryCount = 0; retryCount <= 3; retryCount++)
                {
                    Task<int> challengeResponseTask = context.WaitForExternalEvent<int>("SmsChallengeResponse");

                    Task winner = await Task.WhenAny(challengeResponseTask, timeoutTask);
                    if (winner == challengeResponseTask)
                    {
                        // A response. Compare with the challenge code.
                        if (challengeResponseTask.Result == challengeCode)
                        {
                            authorized = true;
                            break;
                        }
                    }
                    else
                    {
                        // Timeout expired
                        break;
                    }
                }

                if (!timeoutTask.IsCompleted)
                {
                    // All pending timers must be complete or canceled before the function exits.
                    timeoutCts.Cancel();
                }

                return authorized;
            }
        }

        [FunctionName("SendSMSChallenge")]
        public static int SendSMSChallenge([ActivityTrigger] string phoneNumber, TraceWriter log, [TwilioSms(AccountSidSetting = "TwilioAccountSid", AuthTokenSetting = "TwilioAuthToken", From = "%TwilioPhoneNumber%")]
            #if NETSTANDARD2_0
                out CreateMessageOptions message)
            #else
                out SMSMessage message)
            #endif
        {
            var rand = new Random(Guid.NewGuid().GetHashCode());
            int challengeCode = rand.Next(10000);

            log.Info($"Sending verification code {challengeCode} to {phoneNumber}.");

            #if NETSTANDARD2_0
                message = new CreateMessageOptions(new PhoneNumber(phoneNumber));
            #else
                message = new  SMSMessage { To = phoneNumber };
            #endif
            message.Body = $"Your verification code is {challengeCode:0000}";

            return challengeCode;
        }
    }
}
