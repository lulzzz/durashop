namespace durashoppingcart.Models
{
    public class CartEvents
    {
        public static string AddItem => "additem";

        public static string RemoveItem => "removeitem";

        public static string IsCompleted => "iscompleted";

        public static string ClearCart => "clearcart";
    }
}
