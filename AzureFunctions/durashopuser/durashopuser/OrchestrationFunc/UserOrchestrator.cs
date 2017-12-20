using durashopuser.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;

namespace durashopuser.OrchestrationFunc
{
    public class UserOrchestrator
    {
        [FunctionName("UserOrchestrator")]
        public static async Task<UserData> Run([OrchestrationTrigger]DurableOrchestrationContext context, TraceWriter log)
        {
            var user = context.GetInput<UserData>() ?? new UserData();

            var updateUserTask = context.WaitForExternalEvent<UserData>(UserEvents.UpdateUser);

            // Wait for external events
            var resultingEvent = await Task.WhenAny(updateUserTask);

            // Update User
            if (resultingEvent == updateUserTask)
            {
                user = updateUserTask.Result;
                log.Info($"Updated {updateUserTask.Result.FirstName}.");
            }

            context.ContinueAsNew(user); // the magic line
            return user;
        }
    }
}
