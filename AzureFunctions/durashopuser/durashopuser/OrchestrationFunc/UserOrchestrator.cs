using durashopuser.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace durashopuser.OrchestrationFunc
{
    public class UserOrchestrator
    {
        [FunctionName("UserOrchestrator")]
        public static async Task<List<UserData>> Run([OrchestrationTrigger]DurableOrchestrationContext context, TraceWriter log)
        {
            var userList = context.GetInput<List<UserData>>() ?? new List<UserData>();

            var addUserTask = context.WaitForExternalEvent<UserData>(UserEvents.AddUser);
            var removeUserTask = context.WaitForExternalEvent<UserData>(UserEvents.DeleteUser);
            var updateUserTask = context.WaitForExternalEvent<UserData>(UserEvents.UpdateUser);

            // Wait for external events
            var resultingEvent = await Task.WhenAny(addUserTask, removeUserTask, updateUserTask);

            // Add User
            if (resultingEvent == addUserTask)
            {
                userList.Add(addUserTask.Result);
                log.Info($"Added {addUserTask.Result.FirstName} to the User list.");
            }

            // Remove User
            else if (resultingEvent == removeUserTask)
            {
                userList.Remove(userList.Where(x => x.UserId == removeUserTask.Result.UserId).FirstOrDefault());
                log.Info($"Removed {removeUserTask.Result.FirstName} from the User list.");
            }

            // Update User
            else if (resultingEvent == updateUserTask)
            {
                // dont know how to do right now
                log.Info($"Updated {updateUserTask.Result.FirstName}.");
            }

            context.ContinueAsNew(userList); // the magic line

            return userList;
        }
    }
}
