using Microsoft.AspNetCore.Mvc;
using San_Pham_Do_An1.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace San_Pham_Do_An1.Controllers
{

    public class CustomerController : Controller
    {
        private readonly WedQuanAoDbContext _context;

        public CustomerController(WedQuanAoDbContext context)
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
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("CustomerId") != null) return RedirectToAction("Dashboard");

            ViewBag.ActiveTab = "login";
            return View();
        }


        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                string passHash = ToMD5(password);
                var customer = _context.TbCustomers.FirstOrDefault(x => (x.Email == email || x.Username == email) && x.Password == passHash);

                if (customer != null)
                {
                    if (customer.IsActive == false)
                    {
                        ViewBag.Error = "Tài khoản đang bị khóa!";
                        ViewBag.ActiveTab = "login";
                        return View("Index");
                    }

                    customer.LastLogin = DateTime.Now;
                    _context.SaveChanges();

                    HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
                    HttpContext.Session.SetString("UserName", customer.Username ?? "Khách hàng");
                    HttpContext.Session.SetString("Avatar", customer.Avatar ?? "");

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Email hoặc Mật khẩu không đúng!";
            ViewBag.ActiveTab = "login";
            return View("Index");
        }


        [HttpPost]
        public IActionResult Register(string username, string name, string email, string password, string confirmPassword)
        {
            try
            {

                if (password != confirmPassword)
                {
                    ViewBag.RegisterError = "Mật khẩu xác nhận không khớp!";
                    ViewBag.ActiveTab = "register";
                    return View("Index");
                }


                var checkEmail = _context.TbCustomers.FirstOrDefault(x => x.Email == email);
                if (checkEmail != null)
                {
                    ViewBag.RegisterError = "Email này đã được sử dụng!";
                    ViewBag.ActiveTab = "register";
                    return View("Index");
                }


                var checkUsername = _context.TbCustomers.FirstOrDefault(x => x.Username == username);
                if (checkUsername != null)
                {
                    ViewBag.RegisterError = "Tên đăng nhập này đã có người dùng!";
                    ViewBag.ActiveTab = "register";
                    return View("Index");
                }


                TbCustomer user = new TbCustomer();
                user.Name = name;
                user.Username = username;
                user.Email = email;
                user.Password = ToMD5(password);
                user.IsActive = true;
                user.LastLogin = DateTime.Now;


                _context.Add(user);
                _context.SaveChanges();

                ViewBag.Success = "Đăng ký thành công! Hãy đăng nhập.";
                ViewBag.ActiveTab = "login";
                return View("Index");
            }
            catch
            {
                ViewBag.RegisterError = "Đăng ký thất bại. Vui lòng thử lại.";
                ViewBag.ActiveTab = "register";
                return View("Index");
            }
        }


        public IActionResult Dashboard()
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Index");
            }

            var customer = _context.TbCustomers.Find(int.Parse(customerId));
            if (customer == null) return RedirectToAction("Index");

            var orders = _context.TbOrders
                .Include(x => x.OrderStatus)
                .Where(x => x.CustomerId == customer.CustomerId)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
            var vm = new Models.ViewModels.CustomerDashboardViewModel
            {
                Customer = customer,
                Orders = orders
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult UpdateProfile(string Name, string Phone, string Birthday, string Location, IFormFile AvatarFile)
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerId))
                return RedirectToAction("Index");
            var customer = _context.TbCustomers.Find(int.Parse(customerId));
            if (customer == null) return RedirectToAction("Index");

            customer.Name = Name;
            customer.Phone = Phone;
            if (DateTime.TryParse(Birthday, out var birth))
            {
                customer.Birthday = birth;
            }
            customer.Location = Location;

            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var fileName = $"avatar_{customer.CustomerId}_{DateTime.Now.Ticks}{System.IO.Path.GetExtension(AvatarFile.FileName)}";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "img", "avatar");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    AvatarFile.CopyTo(stream);
                }
                customer.Avatar = $"/assets/img/avatar/{fileName}";
            }

            _context.Update(customer);
            _context.SaveChanges();


            HttpContext.Session.SetString("UserName", customer.Name ?? customer.Username ?? "");
            HttpContext.Session.SetString("Avatar", customer.Avatar ?? "");

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public IActionResult UpdatePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerId))
                return RedirectToAction("Index");

            var customer = _context.TbCustomers.Find(int.Parse(customerId));
            if (customer == null)
            {
                TempData["PasswordErrorMessage"] = "Không tìm thấy thông tin khách hàng.";
                return RedirectToAction("Dashboard");
            }


            string currentPasswordHash = ToMD5(CurrentPassword);
            if (customer.Password != currentPasswordHash)
            {
                TempData["PasswordErrorMessage"] = "Mật khẩu hiện tại không đúng.";
                return RedirectToAction("Dashboard");
            }


            if (string.IsNullOrEmpty(NewPassword) || NewPassword.Length < 6)
            {
                TempData["PasswordErrorMessage"] = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return RedirectToAction("Dashboard");
            }

            if (NewPassword != ConfirmPassword)
            {
                TempData["PasswordErrorMessage"] = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
                return RedirectToAction("Dashboard");
            }


            string newPasswordHash = ToMD5(NewPassword);
            if (customer.Password == newPasswordHash)
            {
                TempData["PasswordErrorMessage"] = "Mật khẩu mới phải khác với mật khẩu hiện tại.";
                return RedirectToAction("Dashboard");
            }


            customer.Password = newPasswordHash;
            _context.Update(customer);
            _context.SaveChanges();

            TempData["PasswordSuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Dashboard");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
