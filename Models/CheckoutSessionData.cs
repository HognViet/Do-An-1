using San_Pham_Do_An1.Models.ViewModels;

namespace San_Pham_Do_An1.Models
{
    public class CheckoutSessionData
    {
        public string TransactionRef { get; set; } = string.Empty;
        public CheckoutFormModel Form { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }
}

