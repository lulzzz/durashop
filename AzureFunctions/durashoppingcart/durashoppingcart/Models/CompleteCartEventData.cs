using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace durashoppingcart.Models
{
    public class CompleteCartEventData
    {
        public string OrchestrationInstanceId { get; set; }

        public string ItemName { get; set; }
    }
}
