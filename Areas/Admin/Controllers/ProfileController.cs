using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using San_Pham_Do_An1.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace San_Pham_Do_An1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProfileController : Controller
    {
        private readonly WedQuanAoDbContext _context;

        public ProfileController(WedQuanAoDbContext context)
        {
            _context = context;
        }


        private static string ToMD5(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sbHash = new StringBuilder();
            foreach (byte b in bHash) sbHash.Append(string.Format("{0:x2}", b));
            return sbHash.ToString();
        }

        public IActionResult Index()
        {

            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }


            var adminIdStr = HttpContext.Session.GetString("AdminId");
            if (int.TryParse(adminIdStr, out int adminId))
            {

                var account = _context.TbAccounts
                    .Include(a => a.Role)
                    .FirstOrDefault(a => a.AccountId == adminId);

                if (account != null)
                {
                    return View(account);
                }
            }

            return RedirectToAction("Login", "Accounts", new { area = "Admin" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(int accountId, string username, string fullName, string phone, string email, string password, bool? isActive)
        {

            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }


            var adminIdStr = HttpContext.Session.GetString("AdminId");
            if (!int.TryParse(adminIdStr, out int adminId) || adminId != accountId)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }

            var account = await _context.TbAccounts.FindAsync(accountId);
            if (account == null)
            {
                return NotFound();
            }


            if (!string.IsNullOrEmpty(username) && await _context.TbAccounts.AnyAsync(a => a.Username == username && a.AccountId != accountId))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                account = await _context.TbAccounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.AccountId == accountId);
                return View("Index", account);
            }


            if (!string.IsNullOrEmpty(email) && await _context.TbAccounts.AnyAsync(a => a.Email == email && a.AccountId != accountId))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng!");
                account = await _context.TbAccounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.AccountId == accountId);
                return View("Index", account);
            }


            account.Username = username;
            account.FullName = fullName;
            account.Phone = phone;
            account.Email = email;
            account.IsActive = isActive ?? account.IsActive;


            if (!string.IsNullOrEmpty(password))
            {
                account.Password = ToMD5(password);
            }

            try
            {
                _context.Update(account);
                await _context.SaveChangesAsync();


                HttpContext.Session.SetString("AdminName", account.Username ?? account.FullName ?? "");

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TbAccounts.Any(e => e.AccountId == accountId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
