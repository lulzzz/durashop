using DuraShop.EventGrid;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace durashopmfa
{
    public static class PhoneVerification
    {
        [FunctionName("SMSPhoneVerification")]
        public static async Task<bool> Run([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var phoneNumber = context.GetInput<string>();
            if (string.IsNullOrEmpty(phoneNumber)) { throw new ArgumentNullException(nameof(phoneNumber), "A phone number input is required."); }

            // Send SMS with challengecode
            var challengeCode = await context.CallActivityAsync<int>("SendSMSChallenge", phoneNumber).ConfigureAwait(false);

            using (var timeoutCts = new CancellationTokenSource())
            {
                // Give the user 90 seconds to respond
                var expiration = context.CurrentUtcDateTime.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["smschallengetimeout-sec"]));
                var timeoutTask = context.CreateTimer(expiration, timeoutCts.Token);

                var authorized = false;
                for (var retryCount = 0; retryCount <= 3; retryCount++)
                {
                    Task<int> challengeResponseTask = await context.WaitForExternalEvent<Task<int>>("SmsChallengeResponse");

                    var winner = await Task.WhenAny(challengeResponseTask, timeoutTask).ConfigureAwait(false);
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
        public static int SendSMSChallenge([ActivityTrigger] string phoneNumber, string instanceId, TraceWriter log)
        {
            var rand = new Random(Guid.NewGuid().GetHashCode());
            var challengeCode = rand.Next(10000);

            log.Info($"Sending verification code {challengeCode} to {phoneNumber}.");

            // Push notif to Event Grid
            var response = PublishCommunication.Push(
                new NotifData { From = "", To = phoneNumber, Body = $"DuraShop verification code is {challengeCode:0000}", Subject = "" },
                challengeCode.ToString(),
                Conf.Subject.SMS,
                Conf.EventType.MFAVERIFICATION
                );

            return challengeCode;
        }
    }
}
