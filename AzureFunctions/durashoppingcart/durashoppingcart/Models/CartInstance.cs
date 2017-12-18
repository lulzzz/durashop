using System;
using System.Collections.Generic;

namespace durashoppingcart.Models
{
    public class CartInstance
    {
        public string runtimeStatus { get; set; }
        public List<Input> input { get; set; }
        public object output { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime lastUpdatedTime { get; set; }
    }

    public class Input
    {
        public string CartId { get; set; }
        public string ItemName { get; set; }
        public string ItemId { get; set; }
        public double Price { get; set; }
        public string UserEmail { get; set; }
    }
}
