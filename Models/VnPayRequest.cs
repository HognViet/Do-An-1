namespace San_Pham_Do_An1.Models
{
    public class VnPayRequest
    {
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string OrderDescription { get; set; } = string.Empty;
    }
}

