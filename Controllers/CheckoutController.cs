using System;
using System.Collections.Generic;
using System.Linq;
using San_Pham_Do_An1.Models;
using San_Pham_Do_An1.Models.ViewModels;
using San_Pham_Do_An1.Services;
using Microsoft.AspNetCore.Mvc;

namespace San_Pham_Do_An1.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly WedQuanAoDbContext _context;
        private readonly IVnPayService _vnPayService;
        private const string CartSessionKey = "Cart";
        private const string CheckoutSessionKey = "CheckoutInfo";
        private const string BuyNowSessionKey = "BuyNowItem";

        public CheckoutController(WedQuanAoDbContext context, IVnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var buyNowItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>(BuyNowSessionKey) ?? new List<CartItem>();
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
            var useBuyNow = buyNowItems.Any();
            var items = useBuyNow ? buyNowItems : cart;

            if (!items.Any())
            {
                TempData["CheckoutMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Cart");
            }

            var checkoutSession = HttpContext.Session.GetObjectFromJson<CheckoutSessionData>(CheckoutSessionKey);
            CheckoutFormModel autoForm = null;
            if (checkoutSession == null && HttpContext.Session.GetString("CustomerId") != null)
            {
                if (int.TryParse(HttpContext.Session.GetString("CustomerId"), out int customerId))
                {
                    var customer = _context.TbCustomers.FirstOrDefault(c => c.CustomerId == customerId);
                    if (customer != null)
                    {
                        autoForm = new CheckoutFormModel
                        {
                            FullName = customer.Name ?? string.Empty,
                            Address = customer.Location ?? string.Empty,
                            PhoneNumber = customer.Phone ?? string.Empty,
                            Note = string.Empty
                        };
                    }
                }
            }
            var subTotal = items.Sum(item => (item.Price ?? 0) * item.Quantity);
            var discountCode = HttpContext.Session.GetString("DiscountCode");
            var discountPercent = HttpContext.Session.GetInt32("DiscountPercent") ?? 0;
            var discountAmount = subTotal * discountPercent / 100;

            var model = new CheckoutViewModel
            {
                Items = items,
                Form = checkoutSession?.Form ?? autoForm ?? new CheckoutFormModel(),
                TotalAmount = subTotal,
                DiscountCode = discountCode,
                DiscountAmount = discountAmount
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CreatePayment(CheckoutFormModel form, string PaymentMethod)
        {
            Console.WriteLine("PaymentMethod");
            var buyNowItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>(BuyNowSessionKey) ?? new List<CartItem>();
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
            var useBuyNow = buyNowItems.Any();
            var items = useBuyNow ? buyNowItems : cart;
            if (!items.Any())
            {
                TempData["CheckoutMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Cart");
            }
            if (!ModelState.IsValid)
            {
                var subTotalForInvalid = items.Sum(item => (item.Price ?? 0) * item.Quantity);
                var invalidModel = new CheckoutViewModel
                {
                    Items = items,
                    Form = form,
                    TotalAmount = subTotalForInvalid,
                    DiscountCode = HttpContext.Session.GetString("DiscountCode"),
                    DiscountAmount = subTotalForInvalid * (HttpContext.Session.GetInt32("DiscountPercent") ?? 0) / 100
                };
                return View("Index", invalidModel);
            }
            var orderCode = DateTime.UtcNow.Ticks.ToString();
            var subTotal = items.Sum(item => (item.Price ?? 0) * item.Quantity);
            var discountPercent = HttpContext.Session.GetInt32("DiscountPercent") ?? 0;
            var discountAmount = subTotal * discountPercent / 100;
            var totalAmount = subTotal - discountAmount;
            if (PaymentMethod == "VNPAY")
            {
                var request = new VnPayRequest
                {
                    OrderId = orderCode,
                    Amount = totalAmount,
                    OrderDescription = $"Thanh toan don hang {orderCode}"
                };
                var checkoutSession = new CheckoutSessionData
                {
                    TransactionRef = orderCode,
                    TotalAmount = totalAmount,
                    Form = form
                };
                HttpContext.Session.SetObjectAsJson(CheckoutSessionKey, checkoutSession);
                var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, request);
                return Redirect(paymentUrl);
            }
            else
            {

                int? customerId = null;
                if (HttpContext.Session.GetString("CustomerId") != null)
                {
                    if (int.TryParse(HttpContext.Session.GetString("CustomerId"), out int parsedCustomerId))
                    {
                        customerId = parsedCustomerId;
                    }
                }

                var discountCode = HttpContext.Session.GetString("DiscountCode");
                var orderNote = form.Note;
                if (!string.IsNullOrEmpty(discountCode))
                {
                    orderNote = string.IsNullOrEmpty(orderNote)
                        ? $"Áp dụng mã giảm giá: {discountCode}"
                        : $"{orderNote} (Áp dụng mã giảm giá: {discountCode})";
                }

                var order = new TbOrder
                {
                    Code = orderCode,
                    CustomerId = customerId,
                    ShippingAddress = $"{form.FullName} - {form.PhoneNumber} - {form.Address}",
                    TotalAmount = totalAmount,
                    OrderStatusId = 1,
                    CreatedDate = DateTime.Now,
                    PaymentMethod = "cod",
                    Note = orderNote
                };
                _context.TbOrders.Add(order);
                _context.SaveChanges();
                foreach (var item in items)
                {
                    var detail = new TbOrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Price = item.Price ?? 0,
                        Quantity = item.Quantity
                    };
                    _context.TbOrderDetails.Add(detail);
                }
                _context.SaveChanges();
                HttpContext.Session.Remove(CartSessionKey);
                HttpContext.Session.Remove(CheckoutSessionKey);
                HttpContext.Session.Remove(BuyNowSessionKey);

                return RedirectToAction("Result", new CheckoutResultViewModel
                {
                    IsSuccess = true,
                    Message = "Đặt hàng thành công. Bạn sẽ thanh toán khi nhận hàng!",
                    OrderCode = order.Code,
                    TotalAmount = order.TotalAmount ?? 0
                });
            }
        }

        [HttpGet]
        public IActionResult PaymentCallback()
        {
            var query = Request.Query;
            if (!_vnPayService.ValidateSignature(query))
            {
                return View("Result", new CheckoutResultViewModel
                {
                    IsSuccess = false,
                    Message = "Không thể xác thực phản hồi từ VNPAY. Vui lòng thử lại."
                });
            }

            var responseCode = query["vnp_ResponseCode"].ToString();
            var txnRef = query["vnp_TxnRef"].ToString();

            var checkoutSession = HttpContext.Session.GetObjectFromJson<CheckoutSessionData>(CheckoutSessionKey);
            var buyNowItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>(BuyNowSessionKey) ?? new List<CartItem>();
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

            var useBuyNow = buyNowItems.Any();
            var items = useBuyNow ? buyNowItems : cart;

            if (checkoutSession == null || items.Count == 0 || checkoutSession.TransactionRef != txnRef)
            {
                return View("Result", new CheckoutResultViewModel
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy thông tin đơn hàng. Vui lòng liên hệ hỗ trợ."
                });
            }

            if (!string.Equals(responseCode, "00", StringComparison.OrdinalIgnoreCase))
            {
                return View("Result", new CheckoutResultViewModel
                {
                    IsSuccess = false,
                    Message = "Thanh toán không thành công. Vui lòng thử lại.",
                    OrderCode = checkoutSession.TransactionRef,
                    TotalAmount = checkoutSession.TotalAmount
                });
            }


            int? customerId = null;
            if (HttpContext.Session.GetString("CustomerId") != null)
            {
                if (int.TryParse(HttpContext.Session.GetString("CustomerId"), out int parsedCustomerId))
                {
                    customerId = parsedCustomerId;
                }
            }

            var discountCode = HttpContext.Session.GetString("DiscountCode");
            var orderNote = checkoutSession.Form.Note;
            if (!string.IsNullOrEmpty(discountCode))
            {
                orderNote = string.IsNullOrEmpty(orderNote)
                    ? $"Áp dụng mã giảm giá: {discountCode}"
                    : $"{orderNote} (Áp dụng mã giảm giá: {discountCode})";
            }

            var order = new TbOrder
            {
                Code = checkoutSession.TransactionRef,
                CustomerId = customerId,
                ShippingAddress = $"{checkoutSession.Form.FullName} - {checkoutSession.Form.PhoneNumber} - {checkoutSession.Form.Address}",
                TotalAmount = checkoutSession.TotalAmount,
                OrderStatusId = 2,
                CreatedDate = DateTime.Now,
                PaymentMethod = "VNPAY",
                Note = orderNote
            };

            _context.TbOrders.Add(order);
            _context.SaveChanges();

            foreach (var item in items)
            {
                var detail = new TbOrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Price = item.Price ?? 0,
                    Quantity = item.Quantity
                };
                _context.TbOrderDetails.Add(detail);
            }

            _context.SaveChanges();

            HttpContext.Session.Remove(CartSessionKey);
            HttpContext.Session.Remove(CheckoutSessionKey);
            HttpContext.Session.Remove(BuyNowSessionKey);

            return View("Result", new CheckoutResultViewModel
            {
                IsSuccess = true,
                Message = "Thanh toán thành công. Cảm ơn bạn đã mua sắm!",
                OrderCode = order.Code,
                TotalAmount = order.TotalAmount ?? 0
            });
        }

        [HttpPost]
        public IActionResult ApplyDiscount(string discountCode)
        {
            if (string.IsNullOrWhiteSpace(discountCode))
            {
                TempData["DiscountMessage"] = "Vui lòng nhập mã giảm giá.";
                return RedirectToAction("Index");
            }

            discountCode = discountCode.Trim().ToUpper();
            if (discountCode == "M10TSV")
            {
                HttpContext.Session.SetString("DiscountCode", discountCode);
                HttpContext.Session.SetInt32("DiscountPercent", 10);
                TempData["DiscountSuccess"] = "Áp dụng mã giảm giá 10% thành công!";
            }
            else if (discountCode == "M20TSV")
            {
                HttpContext.Session.SetString("DiscountCode", discountCode);
                HttpContext.Session.SetInt32("DiscountPercent", 20);
                TempData["DiscountSuccess"] = "Áp dụng mã giảm giá 20% thành công!";
            }
            else
            {
                HttpContext.Session.Remove("DiscountCode");
                HttpContext.Session.Remove("DiscountPercent");
                TempData["DiscountMessage"] = "Mã giảm giá không hợp lệ.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Result(CheckoutResultViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult BuyNow([FromBody] BuyNowRequest request)
        {
            if (request == null || request.ProductId <= 0 || request.Quantity <= 0)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            var product = _context.TbProducts.FirstOrDefault(p => p.ProductId == request.ProductId);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
            }

            var item = new CartItem
            {
                ProductId = product.ProductId,
                Title = product.Title,
                Image = product.Image,
                Price = product.PriceSale ?? product.Price,
                Quantity = request.Quantity,
                Size = request.Size,
                Color = request.Color
            };

            HttpContext.Session.SetObjectAsJson(BuyNowSessionKey, new List<CartItem> { item });
            TempData["CheckoutMessage"] = null;
            return Json(new { success = true, redirect = Url.Action("Index", "Checkout") });
        }
    }
}
