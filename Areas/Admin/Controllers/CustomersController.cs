using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using San_Pham_Do_An1.Models;

namespace San_Pham_Do_An1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomersController : Controller
    {
        private readonly WedQuanAoDbContext _context;

        public CustomersController(WedQuanAoDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }

            return View(await _context.TbCustomers
                .OrderByDescending(c => c.CustomerId)
                .ToListAsync());
        }


        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Phone,Email,Username,Password,Birthday,Location,IsActive")] TbCustomer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }

            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.TbCustomers
                .Include(c => c.TbOrders)
                    .ThenInclude(o => o.OrderStatus)
                .Include(c => c.TbOrders)
                    .ThenInclude(o => o.TbOrderDetails)
                        .ThenInclude(od => od.Product)
                .Include(c => c.TbProductReviews)
                    .ThenInclude(r => r.Product)
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }

            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.TbCustomers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,Name,Phone,Email,Username,Password,Birthday,Avatar,Location,IsActive")] TbCustomer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }


        public async Task<IActionResult> Reviews()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }

            var reviews = await _context.TbProductReviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
            return View(reviews);
        }

        private bool CustomerExists(int id)
        {
            return _context.TbCustomers.Any(e => e.CustomerId == id);
        }
    }
}

