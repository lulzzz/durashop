using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace durashoppingcart.Models
{
    public class CartEvents
    {
        public static string AddItem => "additem";

        public static string RemoveItem => "removeitem";

        public static string IsCompleted => "iscompleted";

        public static string GetItems => "getitems";
    }
}
