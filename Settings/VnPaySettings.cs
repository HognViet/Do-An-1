namespace San_Pham_Do_An1.Settings
{
    public class VnPaySettings
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        /// <summary>
        /// Optional fixed bank code, e.g. "NCB". If empty, VNPAY will show the bank selection screen.
        /// </summary>
        public string BankCode { get; set; } = string.Empty;
    }
}

