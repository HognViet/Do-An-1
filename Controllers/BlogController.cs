using San_Pham_Do_An1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace San_Pham_Do_An1.Controllers
{
    public class BlogController : Controller
    {
        private readonly WedQuanAoDbContext _context;
        public BlogController(WedQuanAoDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? categoryId)
        {
            // Lấy tất cả blog đang active
            var query = _context.TbBlogs
                .Include(b => b.BlogCategory)
                .Where(b => b.IsActive == true)
                .AsQueryable();

            // Lọc theo danh mục nếu có
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(b => b.BlogCategoryId == categoryId.Value);
            }

            var blog = query
                .OrderByDescending(b => b.CreatedDate)
                .ToList();

            // Lưu thông tin filter để hiển thị
            ViewBag.BlogCategories = _context.TbBlogCategories
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.TotalResults = blog.Count;

            return View(blog);
        }
        [Route("/blog/{alias}-{id}.html")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TbBlogs == null)
            {
                return NotFound();
            }
            var blog = await _context.TbBlogs
                .Include(m => m.BlogCategory)
                .FirstOrDefaultAsync(m => m.BlogId == id);
            if (blog == null)
            {
                return NotFound();
            }
            ViewBag.BlogComment = await _context.TbBlogComments
                .Include(m => m.Customer)
                .Where(i => i.BlogId == id)
                .OrderByDescending(i => i.CreatedDate)
                .ToListAsync();
            ViewBag.Catetory = _context.TbBlogCategories
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

            var relatedBlogsQuery = _context.TbBlogs
                .Where(b => b.BlogId != blog.BlogId && b.IsActive == true);

            if (blog.BlogCategoryId.HasValue)
            {
                relatedBlogsQuery = relatedBlogsQuery
                    .Where(b => b.BlogCategoryId == blog.BlogCategoryId);
            }

            ViewBag.RelatedBlogs = await relatedBlogsQuery
                .OrderByDescending(b => b.CreatedDate)
                .Take(4)
                .ToListAsync();
            return View(blog);
        }
    }
}
