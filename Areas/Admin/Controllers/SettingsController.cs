using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using San_Pham_Do_An1.Models;

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
            return View();
        }

        // GET: Admin/Settings/General
        public IActionResult General()
        {
            return View();
        }

        // GET: Admin/Settings/Tax
        public IActionResult Tax()
        {
            return View();
        }

        // GET: Admin/Settings/ApiIntegration
        public IActionResult ApiIntegration()
        {
            return View();
        }

        // GET: Admin/Settings/Staff
        public async Task<IActionResult> Staff()
        {
            var accounts = await _context.TbAccounts
                .Include(a => a.Role)
                .ToListAsync();
            return View(accounts);
        }
    }
}

