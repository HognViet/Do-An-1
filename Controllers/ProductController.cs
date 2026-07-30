using San_Pham_Do_An1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace San_Pham_Do_An1.Controllers
{
    public class ProductController : Controller
    {
        private readonly WedQuanAoDbContext _context;
        public ProductController(WedQuanAoDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? categoryId, decimal? priceMin, decimal? priceMax, string? sortBy, string? search)
        {
            // Lấy tất cả sản phẩm đang active
            var query = _context.TbProducts
                .Include(p => p.CategoryProduct)
                .Where(p => p.IsActive == true);

            // Lọc theo danh mục
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryProductId == categoryId.Value);
            }

            // Lọc theo khoảng giá (sử dụng PriceSale nếu có, nếu không thì dùng Price)
            if (priceMin.HasValue)
            {
                query = query.Where(p => (p.PriceSale ?? p.Price ?? 0) >= priceMin.Value);
            }
            if (priceMax.HasValue)
            {
                query = query.Where(p => (p.PriceSale ?? p.Price ?? 0) <= priceMax.Value);
            }

            // Tìm kiếm theo tên sản phẩm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Title != null && p.Title.Contains(search));
            }

            // Sắp xếp
            switch (sortBy)
            {
                case "popularity":
                    query = query.OrderByDescending(p => p.IsBestSeller).ThenByDescending(p => p.Star ?? 0);
                    break;
                case "newness":
                    query = query.OrderByDescending(p => p.CreatedDate);
                    break;
                case "rating":
                    query = query.OrderByDescending(p => p.Star ?? 0);
                    break;
                case "price_asc":
                    query = query.OrderBy(p => p.PriceSale ?? p.Price ?? 0);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.PriceSale ?? p.Price ?? 0);
                    break;
                default: // latest
                    query = query.OrderByDescending(p => p.CreatedDate);
                    break;
            }

            var products = query.ToList();

            // Giá đã được lưu trong database từ lúc edit, không cần tính toán lại

            // Lưu các giá trị filter để hiển thị lại trên view
            ViewBag.ProductCategories = _context.TbProductCategories.ToList();
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.PriceMin = priceMin;
            ViewBag.PriceMax = priceMax;
            ViewBag.SortBy = sortBy ?? "latest";
            ViewBag.Search = search;
            ViewBag.TotalResults = products.Count;

            return View(products);
        }
        [Route("/product/{alias}-{id}.html")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbProducts == null)
            {
                return NotFound();
            }
            var product = await _context.TbProducts.Include(i => i.CategoryProduct)
            .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            // Giá đã được lưu trong database từ lúc edit, không cần tính toán lại

            ViewBag.ProductImages = _context.TbProductVariants
                .Where(v => v.ProductId == id && v.IsActive == true)
                .Select(v => v.Image)
                .ToList();
            ViewBag.productSize = _context.TbProductVariants
            .Include(v => v.Size)
            .Where(v => v.ProductId == id && v.IsActive == true && v.Size != null)
            .Select(v => v.Size)
            .Distinct()
            .ToList();
            ViewBag.productColor = _context.TbProductVariants
            .Include(v => v.Color)
            .Where(v => v.ProductId == id && v.IsActive == true)
            .Select(v => v.Color)
            .Distinct()
            .ToList();
            ViewBag.productReview = _context.TbProductReviews.
                Include(i => i.Customer).
            Where(i => i.ProductId == id && (i.IsActive == true || i.IsActive == null)).OrderByDescending(r => r.CreatedDate).ToList();
            return View(product);
        }
        public IActionResult Preview(int id)
        {
            var product = _context.TbProducts.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.productSize = _context.TbProductVariants
            .Include(v => v.Size)
            .Where(v => v.ProductId == id && v.IsActive == true && v.Size != null)
            .Select(v => v.Size.SizeName)
            .Distinct()
            .ToList();
            ViewBag.productColor = _context.TbProductVariants
            .Include(v => v.Color)
            .Where(v => v.ProductId == id && v.IsActive == true)
            .Select(v => v.Color)
            .Distinct()
            .ToList();
            return PartialView("_ProductPreview", product);
        }
    }
}
