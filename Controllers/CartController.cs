using Microsoft.AspNetCore.Mvc;
using San_Pham_Do_An1.Models;
using Microsoft.EntityFrameworkCore;

namespace San_Pham_Do_An1.Controllers
{
    public class CartController : Controller
    {
        private readonly WedQuanAoDbContext _context;
        public CartController(WedQuanAoDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddToCart([FromBody] CartRequest req)
        {
            var product = _context.TbProducts
                .Include(p => p.TbProductVariants)
                    .ThenInclude(v => v.Color)
                .Include(p => p.TbProductVariants)
                    .ThenInclude(v => v.Size)
                .FirstOrDefault(p => p.ProductId == req.Id);

            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Kiểm tra số lượng tồn kho
            int availableQuantity = 0;
            bool hasVariants = product.TbProductVariants != null &&
                              product.TbProductVariants.Any(v => v.IsActive == true);

            if (hasVariants)
            {
                // Sản phẩm có biến thể - kiểm tra số lượng của biến thể cụ thể
                if (string.IsNullOrEmpty(req.Size) || string.IsNullOrEmpty(req.Color))
                {
                    return Json(new { success = false, message = "Vui lòng chọn kích thước và màu sắc" });
                }

                var variant = product.TbProductVariants?
                    .FirstOrDefault(v => v.IsActive == true &&
                        v.Size != null && v.Size.SizeName == req.Size &&
                        v.Color != null && v.Color.ColorName == req.Color);

                if (variant == null)
                {
                    return Json(new { success = false, message = "Biến thể sản phẩm không tồn tại" });
                }

                availableQuantity = variant.Quantity ?? 0;
            }
            else
            {
                // Sản phẩm không có biến thể - kiểm tra số lượng tổng
                availableQuantity = product.Quantity ?? 0;
            }

            // Tìm sản phẩm trùng (cùng ProductId, Size và Color)
            var existing = cart.FirstOrDefault(x =>
                x.ProductId == req.Id &&
                x.Size == req.Size &&
                x.Color == req.Color);

            // Tính số lượng mới sau khi thêm
            int newQuantity = existing != null ? existing.Quantity + req.Quantity : req.Quantity;

            // Kiểm tra số lượng có vượt quá tồn kho không
            if (newQuantity > availableQuantity)
            {
                return Json(new
                {
                    success = false,
                    message = $"Số lượng vượt quá tồn kho. Số lượng còn lại: {availableQuantity}",
                    availableQuantity = availableQuantity
                });
            }

            if (existing != null)
            {
                // Cộng dồn số lượng
                existing.Quantity = newQuantity;
            }
            else
            {
                // Thêm sản phẩm mới
                cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    Title = product.Title ?? "",
                    Image = product.Image ?? "",
                    Price = product.PriceSale ?? product.Price,
                    Quantity = req.Quantity,
                    Size = req.Size ?? "",
                    Color = req.Color ?? "",
                });
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return Json(new { success = true, cartCount = cart.Sum(x => x.Quantity) });
        }

        [HttpGet]
        public IActionResult MiniCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            return PartialView("_MiniCartPartial", cart);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart")
               ?? new List<CartItem>();
            return View(cart);
        }

        [HttpPost]
        public IActionResult UpdateQuantity([FromBody] UpdateQuantityRequest req)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.ProductId == req.Id);
            if (item == null) return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng" });

            // Kiểm tra số lượng tồn kho
            var product = _context.TbProducts
                .Include(p => p.TbProductVariants)
                    .ThenInclude(v => v.Color)
                .Include(p => p.TbProductVariants)
                    .ThenInclude(v => v.Size)
                .FirstOrDefault(p => p.ProductId == req.Id);

            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }

            int availableQuantity = 0;
            bool hasVariants = product.TbProductVariants != null &&
                              product.TbProductVariants.Any(v => v.IsActive == true);

            if (hasVariants)
            {
                // Sản phẩm có biến thể - kiểm tra số lượng của biến thể cụ thể
                if (string.IsNullOrEmpty(item.Size) || string.IsNullOrEmpty(item.Color))
                {
                    return Json(new { success = false, message = "Sản phẩm có biến thể nhưng thiếu thông tin Size hoặc Color" });
                }

                var variant = product.TbProductVariants?
                    .FirstOrDefault(v => v.IsActive == true &&
                        v.Size != null && v.Size.SizeName == item.Size &&
                        v.Color != null && v.Color.ColorName == item.Color);

                if (variant == null)
                {
                    return Json(new { success = false, message = "Biến thể sản phẩm không tồn tại" });
                }

                availableQuantity = variant.Quantity ?? 0;
            }
            else
            {
                // Sản phẩm không có biến thể - kiểm tra số lượng tổng
                availableQuantity = product.Quantity ?? 0;
            }

            // Kiểm tra số lượng yêu cầu có vượt quá tồn kho không
            if (req.Quantity > availableQuantity)
            {
                return Json(new
                {
                    success = false,
                    message = $"Số lượng vượt quá tồn kho. Số lượng còn lại: {availableQuantity}",
                    availableQuantity = availableQuantity
                });
            }

            // Cập nhật số lượng nếu hợp lệ
            item.Quantity = req.Quantity > 0 ? req.Quantity : 1;
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            var total = cart.Sum(x => x.Price.GetValueOrDefault() * x.Quantity);
            return Json(new { success = true, cartCount = cart.Sum(x => x.Quantity), total });
        }

        [HttpPost]
        public IActionResult RemoveFromCart([FromBody] CartRequest req)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.ProductId == req.Id);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            var total = cart.Sum(x => x.Price.GetValueOrDefault() * x.Quantity);
            return Json(new { success = true, cartCount = cart.Sum(x => x.Quantity), total });
        }
        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return Json(new { success = true, cartCount = 0, total = 0 });
        }

        public class UpdateQuantityRequest
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
        }

        public class CartRequest
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
            public string? Image { get; set; }
            public decimal? Price { get; set; }
            public string? Size { get; set; }
            public string? Color { get; set; }
        }
    }
}
