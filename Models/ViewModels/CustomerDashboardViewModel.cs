using System.Collections.Generic;

namespace San_Pham_Do_An1.Models.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public TbCustomer Customer { get; set; } = null!;
        public IEnumerable<TbOrder> Orders { get; set; } = null!;
    }
}
