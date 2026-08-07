using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using San_Pham_Do_An1.Models;

namespace San_Pham_Do_An1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly WedQuanAoDbContext _context;

        public OrdersController(WedQuanAoDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index(string status = "all")
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }

            var query = _context.TbOrders
                .Include(o => o.Customer)
                .Include(o => o.OrderStatus)
                .Include(o => o.TbOrderDetails)
                    .ThenInclude(od => od.Product)
                .AsQueryable();

            // Lọc theo trạng thái
            if (status != "all" && !string.IsNullOrEmpty(status))
            {
                var statusId = GetStatusId(status);
                if (statusId.HasValue)
                {
                    // Lọc chính xác theo OrderStatusId (xử lý cả trường hợp nullable)
                    int filterStatusId = statusId.Value;
                    query = query.Where(o => o.OrderStatusId.HasValue && o.OrderStatusId.Value == filterStatusId);
                }
            }

            ViewBag.Status = status;

            var statusList = new List<object> { new { Value = "all", Text = "Tất cả đơn hàng" } };

            var allowedStatusIds = new[] { 1, 3, 4, 5, 6, 7, 9, 10 };
            var statusDict = new Dictionary<int, string>
            {
                { 1, "Chờ xử lý" },
                { 3, "Đang xử lý" },
                { 4, "Đã gửi hàng" },
                { 5, "Đã giao" },
                { 6, "Đã hủy" },
                { 7, "Đã hoàn tiền" },
                { 9, "Trả hàng" },
                { 10, "Giao dịch thất bại" }
            };

            foreach (var id in allowedStatusIds)
            {
                statusList.Add(new { Value = id.ToString(), Text = statusDict.ContainsKey(id) ? statusDict[id] : $"Trạng thái {id}" });
            }

            ViewBag.StatusDict = statusDict;
            ViewBag.StatusList = new SelectList(statusList, "Value", "Text", status);

            return View(await query.OrderByDescending(o => o.CreatedDate).ToListAsync());
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id, string status = "all")
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.TbOrders
                .Include(o => o.Customer)
                .Include(o => o.OrderStatus)
                .Include(o => o.TbOrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var statuses = await _context.TbOrderStatuses.ToListAsync();
            var statusDict = new Dictionary<int, string>
            {
                { 1, "Chờ xử lý" },
                { 2, "Đang xử lý" },
                { 3, "Đã gửi hàng" },
                { 4, "Đã giao" },
                { 5, "Đã hủy" },
                { 6, "Đã hoàn tiền" },
                { 7, "Trả hàng" },
                { 8, "Giao dịch thất bại" }
            };

            var statusListItems = statuses.Select(s => new SelectListItem
            {
                Value = s.OrderStatusId.ToString(),
                Text = statusDict.ContainsKey(s.OrderStatusId) ? statusDict[s.OrderStatusId] : $"Trạng thái {s.OrderStatusId}"
            }).ToList();

            ViewBag.OrderStatuses = new SelectList(statusListItems, "Value", "Text", order.OrderStatusId?.ToString());
            ViewBag.Status = status; // Lưu status để quay lại
            return View(order);
        }

        // POST: Admin/Orders/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, int orderStatusId, string status = "all")
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            var order = await _context.TbOrders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatusId = orderStatusId;
            order.ModifiedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id, status });
        }

        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id, string status = "all")
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.TbOrders
                .Include(o => o.Customer)
                .Include(o => o.OrderStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.Status = status; // Lưu status để quay lại
            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string status = "all")
        {
            var order = await _context.TbOrders
                .Include(o => o.TbOrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order != null)
            {
                _context.TbOrderDetails.RemoveRange(order.TbOrderDetails);
                _context.TbOrders.Remove(order);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { status });
        }

        private int? GetStatusId(string status)
        {
            if (string.IsNullOrEmpty(status) || status == "all")
                return null;

            if (int.TryParse(status, out int statusId))
            {
                return statusId;
            }

            return null;
        }
    }
}



