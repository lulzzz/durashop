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
