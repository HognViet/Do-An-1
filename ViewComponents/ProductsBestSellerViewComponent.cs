using San_Pham_Do_An1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace San_Pham_Do_An1.ViewComponents
{
    public class ProductsBestSellerViewComponent : ViewComponent
    {
        private readonly WedQuanAoDbContext _context;
        public ProductsBestSellerViewComponent(WedQuanAoDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = _context.TbProducts.Include(m => m.CategoryProduct)
                .Where(m => m.IsActive).Where(m => m.IsBestSeller == true);
            return await Task.FromResult<IViewComponentResult>
                (View(items.OrderByDescending(m => m.ProductId).ToList()));
        }
    }
}
