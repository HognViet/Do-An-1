using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using San_Pham_Do_An1.Models;

using System.Linq;

namespace San_Pham_Do_An1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactController : Controller
    {
        private readonly WedQuanAoDbContext _context;

        public ContactController(WedQuanAoDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            var items = _context.TbContacts.OrderByDescending(x => x.CreatedDate).ToList();
            return View(items);
        }


        public IActionResult Detail(int id)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            var item = _context.TbContacts.Find(id);
            if (item != null)
            {

                item.IsRead = true;
                _context.SaveChanges();
            }
            return View(item);
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _context.TbContacts.Find(id);
            if (item != null)
            {
                _context.TbContacts.Remove(item);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
