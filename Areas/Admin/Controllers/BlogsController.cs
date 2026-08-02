using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using San_Pham_Do_An1.Models;

namespace San_Pham_Do_An1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogsController : Controller
    {
        private readonly WedQuanAoDbContext _context;

        public BlogsController(WedQuanAoDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Blogs
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            var WedQuanAoDbContext = _context.TbBlogs.Include(t => t.Account).Include(t => t.BlogCategory);
            return View(await WedQuanAoDbContext.ToListAsync());
        }

        // GET: Admin/Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbBlog = await _context.TbBlogs
                .Include(t => t.Account)
                .Include(t => t.BlogCategory)
                .FirstOrDefaultAsync(m => m.BlogId == id);
            if (tbBlog == null)
            {
                return NotFound();
            }

            return View(tbBlog);
        }

        // GET: Admin/Blogs/Create
        public IActionResult Create()
        {
            var accounts = _context.TbAccounts
                .Where(a => a.IsActive == true)
                .Select(a => new
                {
                    AccountId = a.AccountId,
                    DisplayName = a.FullName ?? a.Username ?? $"Account {a.AccountId}"
                })
                .ToList();
            
            ViewData["AccountId"] = new SelectList(accounts, "AccountId", "DisplayName");
            ViewData["BlogCategoryId"] = new SelectList(_context.TbBlogCategories, "BlogCategoryId", "Title");
            return View();
        }

        // POST: Admin/Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Alias,BlogCategoryId,Description,Detail,Image,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy,AccountId,IsActive")] TbBlog tbBlog)
        {
            if (ModelState.IsValid)
            {
                tbBlog.Alias = San_Pham_Do_An1.Utilities.Function.TitleSlugGenerationAlias(tbBlog.Title);
                
                // Tự động lấy tên account nếu có AccountId
                if (tbBlog.AccountId.HasValue && tbBlog.AccountId.Value > 0)
                {
                    var account = await _context.TbAccounts.FindAsync(tbBlog.AccountId.Value);
                    if (account != null)
                    {
                        tbBlog.CreatedBy = account.FullName ?? account.Username ?? $"Account {account.AccountId}";
                    }
                }
                
                // Đảm bảo CreatedDate có giá trị nếu null
                if (!tbBlog.CreatedDate.HasValue)
                {
                    tbBlog.CreatedDate = DateTime.Now;
                }
                
                _context.Add(tbBlog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var accounts = _context.TbAccounts
                .Where(a => a.IsActive == true)
                .Select(a => new
                {
                    AccountId = a.AccountId,
                    DisplayName = a.FullName ?? a.Username ?? $"Account {a.AccountId}"
                })
                .ToList();
            
            ViewData["AccountId"] = new SelectList(accounts, "AccountId", "DisplayName", tbBlog.AccountId);
            ViewData["BlogCategoryId"] = new SelectList(_context.TbBlogCategories, "BlogCategoryId", "Title", tbBlog.BlogCategoryId);
            return View(tbBlog);
        }

        // GET: Admin/Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbBlog = await _context.TbBlogs.FindAsync(id);
            if (tbBlog == null)
            {
                return NotFound();
            }
            
            var accounts = _context.TbAccounts
                .Where(a => a.IsActive == true)
                .Select(a => new
                {
                    AccountId = a.AccountId,
                    DisplayName = a.FullName ?? a.Username ?? $"Account {a.AccountId}"
                })
                .ToList();
            
            ViewData["AccountId"] = new SelectList(accounts, "AccountId", "DisplayName", tbBlog.AccountId);
            ViewData["BlogCategoryId"] = new SelectList(_context.TbBlogCategories, "BlogCategoryId", "Title", tbBlog.BlogCategoryId);
            return View(tbBlog);
        }

        // POST: Admin/Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BlogId,Title,Alias,BlogCategoryId,Description,Detail,Image,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy,AccountId,IsActive")] TbBlog tbBlog)
        {
            if (id != tbBlog.BlogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tự động lấy tên account nếu có AccountId
                    if (tbBlog.AccountId.HasValue)
                    {
                        var account = await _context.TbAccounts.FindAsync(tbBlog.AccountId.Value);
                        if (account != null)
                        {
                            tbBlog.CreatedBy = account.FullName ?? account.Username ?? $"Account {account.AccountId}";
                        }
                    }
                    _context.Update(tbBlog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbBlogExists(tbBlog.BlogId))
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
            var accounts = _context.TbAccounts
                .Where(a => a.IsActive == true)
                .Select(a => new
                {
                    AccountId = a.AccountId,
                    DisplayName = a.FullName ?? a.Username ?? $"Account {a.AccountId}"
                })
                .ToList();
            
            ViewData["AccountId"] = new SelectList(accounts, "AccountId", "DisplayName", tbBlog.AccountId);
            ViewData["BlogCategoryId"] = new SelectList(_context.TbBlogCategories, "BlogCategoryId", "Title", tbBlog.BlogCategoryId);
            return View(tbBlog);
        }

        // GET: Admin/Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbBlog = await _context.TbBlogs
                .Include(t => t.Account)
                .Include(t => t.BlogCategory)
                .FirstOrDefaultAsync(m => m.BlogId == id);
            if (tbBlog == null)
            {
                return NotFound();
            }

            return View(tbBlog);
        }

        // POST: Admin/Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbBlog = await _context.TbBlogs
                .Include(b => b.TbBlogComments)
                .FirstOrDefaultAsync(m => m.BlogId == id);
            
            if (tbBlog != null)
            {
                // Xóa tất cả các comment liên quan trước
                if (tbBlog.TbBlogComments != null && tbBlog.TbBlogComments.Any())
                {
                    _context.TbBlogComments.RemoveRange(tbBlog.TbBlogComments);
                }
                
                // Sau đó mới xóa blog
                _context.TbBlogs.Remove(tbBlog);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TbBlogExists(int id)
        {
            return _context.TbBlogs.Any(e => e.BlogId == id);
        }
    }
}
