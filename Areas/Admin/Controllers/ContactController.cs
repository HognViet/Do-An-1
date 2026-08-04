using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
=======
using Microsoft.AspNetCore.Http;
>>>>>>> son
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

        // 1. Hiển thị danh sách
        public IActionResult Index()
        {
<<<<<<< HEAD
=======
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
>>>>>>> son
            var items = _context.TbContacts.OrderByDescending(x => x.CreatedDate).ToList();
            return View(items);
        }

        // 2. XEM CHI TIẾT (Fix lỗi không xem được nội dung)
        public IActionResult Detail(int id)
        {
<<<<<<< HEAD
=======
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
>>>>>>> son
            var item = _context.TbContacts.Find(id);
            if (item != null)
            {
                // Đánh dấu đã đọc khi admin bấm vào xem
                item.IsRead = true;
                _context.SaveChanges();
            }
            return View(item);
        }

        // 3. XÓA (Fix lỗi không xóa được)
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