using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace durashoppingcart.Models
{
    public class ExecutionDetails
    {
        public int Iterations { get; set; }
        public int IterationPeriod { get; set; }
        public RequestMethod ReqMethod { get; internal set; }
    }
    public enum RequestMethod
    {
        AddItem,
        DeleteItem,
        StartCart
    }
}
