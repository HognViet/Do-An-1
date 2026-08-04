using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using San_Pham_Do_An1.Models;

<<<<<<< HEAD
=======
using Microsoft.AspNetCore.Http;

>>>>>>> son
namespace San_Pham_Do_An1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SupportController : Controller
    {
        private readonly WedQuanAoDbContext _context;

        public SupportController(WedQuanAoDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Support
        public async Task<IActionResult> Index()
        {
<<<<<<< HEAD
=======
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
>>>>>>> son
            var contacts = await _context.TbContacts
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
            return View(contacts);
        }
    }
}

