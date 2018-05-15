using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace durashoppingcart.Models
{
    public class StatusResponse
    {
        /// <summary>
        /// Status for the execution
        /// </summary>
        public string RuntimeStatus { get; set; }

        /// <summary>
        /// Input data 
        /// </summary>
        public List<object> Input { get; set; }

        /// <summary>
        /// Output data
        /// </summary>
        public List<object> Output { get; set; }

        /// <summary>
        /// Creating time
        /// </summary>
        public string CreatedTime { get; set; }

        /// <summary>
        /// Last updated time
        /// </summary>
        public string LastUpdatedTime { get; set; }

        public string CustomStatus { get; set; }


    }
}
