﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace durashoppingcart.Models
{
    public class CartData
    {
        public string CartId { get; set; }

        public string ItemName { get; set; }

        public string ItemId { get; set; }

        public Double Price { get; set; }

        public string UserEmail { get; set; }
    }
}
