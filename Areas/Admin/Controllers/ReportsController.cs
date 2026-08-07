using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using San_Pham_Do_An1.Models;

namespace San_Pham_Do_An1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportsController : Controller
    {
        private readonly WedQuanAoDbContext _context;

        public ReportsController(WedQuanAoDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Revenue(DateTime? startDate, DateTime? endDate)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }


            DateTime? start = startDate;
            DateTime? end = endDate;

            var query = _context.TbOrders
                .Include(o => o.OrderStatus)
                .Include(o => o.Customer)
                .AsQueryable();

            if (start.HasValue)
            {
                query = query.Where(o => o.CreatedDate >= start.Value);
            }

            if (end.HasValue)
            {
                query = query.Where(o => o.CreatedDate <= end.Value);
            }

            var orders = await query.ToListAsync();

            ViewBag.StartDate = start;
            ViewBag.EndDate = end;


            var allOrdersWithAmount = orders.Where(o => o.TotalAmount.HasValue && o.TotalAmount > 0).ToList();


            var completedOrders = allOrdersWithAmount.Where(o =>
                o.OrderStatusId == 5 ||
                o.PaymentMethod == "VNPAY" ||
                (o.OrderStatus != null && (
                    (o.OrderStatus.Name != null && (o.OrderStatus.Name.Contains("Đã giao") || o.OrderStatus.Name.Contains("Hoàn thành"))) ||
                    (o.OrderStatus.Description != null && (o.OrderStatus.Description.Contains("Đã giao") || o.OrderStatus.Description.Contains("Hoàn thành")))
                ))
            ).ToList();


            ViewBag.TotalRevenue = allOrdersWithAmount.Sum(o => o.TotalAmount ?? 0);
            ViewBag.TotalOrders = orders.Count;
            ViewBag.CompletedOrders = completedOrders.Count;
            ViewBag.AllOrdersWithAmount = allOrdersWithAmount.Count;

            return View(orders);
        }


        public async Task<IActionResult> Inventory()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }

            var products = await _context.TbProducts
                .Include(p => p.CategoryProduct)
                .Include(p => p.TbProductVariants)
                .ToListAsync();

            ViewBag.LowStock = products.Where(p =>
                (p.TbProductVariants.Any(v => v.IsActive == true)
                    ? p.TbProductVariants.Where(v => v.IsActive == true).Sum(v => v.Quantity ?? 0)
                    : (p.Quantity ?? 0)) < 10
            ).ToList();

            return View(products);
        }
    }
}

