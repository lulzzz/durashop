using durashoppingcart.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace durashoppingcart.Utils
{
    public static class Helper
    {
        public static ExecutionDetails GetExecutionDetails(RequestMethod method)
        {
            int.TryParse(ConfigurationManager.AppSettings["MaxExecutionTime"], out var maxExecutionTime);
            int.TryParse(ConfigurationManager.AppSettings["ExecutionPeriod"], out var executionPeriod);

            return new ExecutionDetails
            {
                IterationPeriod = executionPeriod,
                Iterations = maxExecutionTime / executionPeriod,
                ReqMethod = method
            };
        }
    }
}
