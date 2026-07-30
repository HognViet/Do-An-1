using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace San_Pham_Do_An1.Models.ViewModels
{
    public class CheckoutFormModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }
    }

    public class CheckoutViewModel
    {
        public List<CartItem> Items { get; set; } = new();
        public CheckoutFormModel Form { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }

    public class CheckoutResultViewModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? OrderCode { get; set; }
        public decimal TotalAmount { get; set; }
    }
}

