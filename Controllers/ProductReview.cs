using San_Pham_Do_An1.Models;
using Microsoft.AspNetCore.Mvc;

namespace San_Pham_Do_An1.Controllers
{
    public class ProductReview : Controller
    {
        private readonly WedQuanAoDbContext _context;

        public ProductReview(WedQuanAoDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(int productId, int rating, string detail)
        {
            var CustomerId = int.Parse(HttpContext.Session.GetString("CustomerId"));
            try
            {

                TbProductReview review = new TbProductReview();

                review.ProductId = productId;
                review.CustomerId = CustomerId;
                review.Star = rating;
                review.CreatedDate = DateTime.Now;
                review.Detail = detail;
                review.IsActive = true;
                _context.Add(review);
                _context.SaveChanges();

                var product = _context.TbProducts.FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    var reviews = _context.TbProductReviews
                        .Where(r => r.ProductId == productId && r.IsActive == true)
                        .ToList();

                    double avgStar = reviews.Any()
                ? reviews.Average(r => (double?)r.Star) ?? 0
                : 0;

                    product.Star = avgStar;

                    _context.Update(product);
                    _context.SaveChanges();
                }
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { status = false });
            }
        }
    }
}
