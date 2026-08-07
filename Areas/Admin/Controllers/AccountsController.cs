using Microsoft.AspNetCore.Mvc;
using San_Pham_Do_An1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Threading.Tasks;

namespace San_Pham_Do_An1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : Controller
    {
        private readonly WedQuanAoDbContext _context;

        public AccountsController(WedQuanAoDbContext context)
        {
            _context = context;
        }


        public static string ToMD5(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sbHash = new StringBuilder();
            foreach (byte b in bHash) sbHash.Append(String.Format("{0:x2}", b));
            return sbHash.ToString();
        }


        [HttpGet]
        public IActionResult Login()
        {

            if (HttpContext.Session.GetString("AdminId") != null)
            {
                var roleId = HttpContext.Session.GetString("RoleId");
                if (roleId == "2")
                {
                    return RedirectToAction("Index", "Orders", new { area = "Employees" });
                }
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            return View();
        }


        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                string passHash = ToMD5(password);



                var account = _context.TbAccounts.FirstOrDefault(x => (x.Email == email || x.Username == email) && x.Password == passHash);

                if (account != null)
                {
                    if (account.IsActive == false)
                    {
                        ViewBag.Error = "Tài khoản quản trị này đã bị khóa!";
                        return View();
                    }


                    HttpContext.Session.SetString("AdminId", account.AccountId.ToString());
                    HttpContext.Session.SetString("AdminName", account.Username ?? account.FullName);
                    HttpContext.Session.SetString("RoleId", account.RoleId.ToString() ?? "0");


                    account.LastLogin = DateTime.Now;
                    _context.SaveChanges();


                    if (account.RoleId == 2)
                    {
                        return RedirectToAction("Index", "Orders", new { area = "Employees" });
                    }
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng!";
                }
            }
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {

            if (HttpContext.Session.GetString("AdminId") != null)
            {
                var roleId = HttpContext.Session.GetString("RoleId");
                if (roleId == "2")
                {
                    return RedirectToAction("Index", "Orders", new { area = "Employees" });
                }
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            return View();
        }


        [HttpPost]
        public IActionResult Register(string username, string email, string password, string confirmPassword, string fullName, string phone, int roleId)
        {
            if (ModelState.IsValid)
            {

                if (password != confirmPassword)
                {
                    ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                    return View();
                }


                var existingUsername = _context.TbAccounts.FirstOrDefault(x => x.Username == username);
                if (existingUsername != null)
                {
                    ViewBag.Error = "Tên đăng nhập này đã tồn tại!";
                    return View();
                }


                if (!string.IsNullOrEmpty(email))
                {
                    var existingEmail = _context.TbAccounts.FirstOrDefault(x => x.Email == email);
                    if (existingEmail != null)
                    {
                        ViewBag.Error = "Email này đã được sử dụng!";
                        return View();
                    }
                }


                var newAccount = new TbAccount
                {
                    Username = username,
                    Email = email,
                    Password = ToMD5(password),
                    FullName = fullName,
                    Phone = phone,
                    IsActive = true,
                    RoleId = (roleId == 1 || roleId == 2) ? roleId : 1
                };

                _context.TbAccounts.Add(newAccount);
                _context.SaveChanges();

                ViewBag.Success = "Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.";
                return View();
            }

            return View();
        }


        public async Task<IActionResult> Index()
        {
            var accounts = await _context.TbAccounts
                .Include(a => a.Role)
                .OrderByDescending(a => a.AccountId)
                .ToListAsync();
            return View(accounts);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.TbAccounts
                .Include(a => a.Role)
                .Include(a => a.TbBlogs)
                .Include(a => a.TbChatMessages)
                .FirstOrDefaultAsync(m => m.AccountId == id);

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }


        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.TbRoles, "RoleId", "RoleName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Password,FullName,Phone,Email,RoleId,IsActive")] TbAccount account, string password)
        {
            if (ModelState.IsValid)
            {

                if (await _context.TbAccounts.AnyAsync(a => a.Username == account.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                    ViewData["RoleId"] = new SelectList(_context.TbRoles, "RoleId", "RoleName", account.RoleId);
                    return View(account);
                }


                if (!string.IsNullOrEmpty(account.Email) && await _context.TbAccounts.AnyAsync(a => a.Email == account.Email))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng!");
                    ViewData["RoleId"] = new SelectList(_context.TbRoles, "RoleId", "RoleName", account.RoleId);
                    return View(account);
                }


                if (!string.IsNullOrEmpty(password))
                {
                    account.Password = ToMD5(password);
                }

                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.TbRoles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.TbAccounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.TbRoles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,Username,Password,FullName,Phone,Email,RoleId,IsActive")] TbAccount account, string newPassword)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (await _context.TbAccounts.AnyAsync(a => a.Username == account.Username && a.AccountId != account.AccountId))
                    {
                        ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                        ViewData["RoleId"] = new SelectList(_context.TbRoles, "RoleId", "RoleName", account.RoleId);
                        return View(account);
                    }


                    if (!string.IsNullOrEmpty(account.Email) && await _context.TbAccounts.AnyAsync(a => a.Email == account.Email && a.AccountId != account.AccountId))
                    {
                        ModelState.AddModelError("Email", "Email này đã được sử dụng!");
                        ViewData["RoleId"] = new SelectList(_context.TbRoles, "RoleId", "RoleName", account.RoleId);
                        return View(account);
                    }


                    var existingAccount = await _context.TbAccounts.AsNoTracking().FirstOrDefaultAsync(a => a.AccountId == id);
                    if (existingAccount != null)
                    {

                        if (!string.IsNullOrEmpty(newPassword))
                        {
                            account.Password = ToMD5(newPassword);
                        }
                        else
                        {
                            account.Password = existingAccount.Password;
                        }
                    }

                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.TbRoles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.TbAccounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.TbAccounts.FindAsync(id);
            if (account != null)
            {
                _context.TbAccounts.Remove(account);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.TbAccounts.Any(e => e.AccountId == id);
        }


        public IActionResult Logout()
        {

            HttpContext.Session.Remove("AdminId");
            HttpContext.Session.Remove("AdminName");
            HttpContext.Session.Remove("RoleId");

            return RedirectToAction("Login", "Accounts", new { area = "Admin" });
        }
    }
}