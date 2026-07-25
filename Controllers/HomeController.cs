using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using San_Pham_Do_An1.Models;
using System.Diagnostics;

namespace San_Pham_Do_An1.Controllers
{
    public class HomeController : Controller
    {
        private readonly WedQuanAoDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, WedQuanAoDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // 1. Lấy tin tức Blog hiển thị ở trang chủ
            ViewBag.Blog = _context.TbBlogs
                .Where(m => m.IsActive == true)
                .OrderByDescending(m => m.CreatedDate)
                .ToList();

            // 2. Lấy đánh giá của khách hàng hiển thị ở trang chủ
            ViewBag.Reviews = _context.TbProductReviews
                .Include(r => r.Customer)
                .Where(r => r.IsActive == true && r.Customer != null)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();

            return View();
        }

        public IActionResult Preview(int id)
        {
            var product = _context.TbProducts.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
                return NotFound();

            ViewBag.ProductReviewCount = _context.TbProductReviews
                .Count(i => i.ProductId == id && (i.IsActive == true || i.IsActive == null));

            return PartialView("_ProductPreview", product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
