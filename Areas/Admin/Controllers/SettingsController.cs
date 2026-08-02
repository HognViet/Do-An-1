using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using San_Pham_Do_An1.Models;

using Microsoft.AspNetCore.Http;

namespace San_Pham_Do_An1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingsController : Controller
    {
        private readonly WedQuanAoDbContext _context;

        public SettingsController(WedQuanAoDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Settings/StoreInfo
        public IActionResult StoreInfo()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveStoreInfo(string storeName, string storeAddress, string storePhone, string storeEmail, string storeHours)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            TempData["SuccessMessage"] = "Cập nhật thông tin cửa hàng thành công!";
            return RedirectToAction("StoreInfo");
        }

        // GET: Admin/Settings/General
        public IActionResult General()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveGeneral(string defaultLanguage, string currency, bool maintenanceMode)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            TempData["SuccessMessage"] = "Cập nhật cài đặt chung thành công!";
            return RedirectToAction("General");
        }

        // GET: Admin/Settings/Tax
        public IActionResult Tax()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveTax(decimal vatRate, decimal defaultShippingFee)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            TempData["SuccessMessage"] = "Cập nhật Thuế & Phí vận chuyển thành công!";
            return RedirectToAction("Tax");
        }

        // GET: Admin/Settings/ApiIntegration
        public IActionResult ApiIntegration()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveApiIntegration(string vnpayTmnCode, string vnpayHashSecret, string geminiApiKey)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            TempData["SuccessMessage"] = "Cập nhật cấu hình tích hợp API thành công!";
            return RedirectToAction("ApiIntegration");
        }

        // GET: Admin/Settings/Staff
        public async Task<IActionResult> Staff()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            var accounts = await _context.TbAccounts
                .Include(a => a.Role)
                .ToListAsync();
            return View(accounts);
        }
    }
}

