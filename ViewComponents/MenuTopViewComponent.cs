using San_Pham_Do_An1.Models;
using Microsoft.AspNetCore.Mvc;

namespace San_Pham_Do_An1.ViewComponents
{
    public class MenuTopViewComponent : ViewComponent
    {
        private readonly WedQuanAoDbContext _context;
        public MenuTopViewComponent(WedQuanAoDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = _context.TbMenus.Where(m => m.IsActive).
            OrderBy(m => m.Position).ToList();
            return await Task.FromResult<IViewComponentResult>(View(items));
        }
    }
}
