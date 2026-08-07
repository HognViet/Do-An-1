using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using San_Pham_Do_An1.Models;
using Microsoft.AspNetCore.Http;

namespace San_Pham_Do_An1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenusController : Controller
    {
        private readonly WedQuanAoDbContext _context;

        public MenusController(WedQuanAoDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }

            var menus = await _context.TbMenus
                .OrderBy(m => m.Position)
                .ThenBy(m => m.CreatedDate)
                .ToListAsync();

            return View(menus);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbMenu = await _context.TbMenus
                .FirstOrDefaultAsync(m => m.MenuId == id);
            if (tbMenu == null)
            {
                return NotFound();
            }


            if (tbMenu.ParentId.HasValue)
            {
                var parent = await _context.TbMenus.FindAsync(tbMenu.ParentId.Value);
                ViewBag.ParentName = parent?.Title ?? "N/A";
            }

            return View(tbMenu);
        }


        public IActionResult Create()
        {

            var menus = _context.TbMenus
                .Where(m => m.IsActive == true)
                .OrderBy(m => m.Position)
                .ToList();

            ViewBag.ParentId = new SelectList(menus, "MenuId", "Title");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Alias,Description,ParentId,Position,IsActive")] TbMenu tbMenu)
        {
            if (ModelState.IsValid)
            {

                if (string.IsNullOrEmpty(tbMenu.Alias) && !string.IsNullOrEmpty(tbMenu.Title))
                {
                    tbMenu.Alias = San_Pham_Do_An1.Utilities.Function.TitleSlugGenerationAlias(tbMenu.Title);
                }


                var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";

                tbMenu.CreatedDate = DateTime.Now;
                tbMenu.CreatedBy = adminName;


                if (tbMenu.ParentId.HasValue)
                {
                    var parent = await _context.TbMenus.FindAsync(tbMenu.ParentId.Value);
                    if (parent != null)
                    {
                        tbMenu.Levels = (parent.Levels ?? 0) + 1;
                    }
                }
                else
                {
                    tbMenu.Levels = 1;
                }

                _context.Add(tbMenu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            var menus = _context.TbMenus
                .Where(m => m.IsActive == true)
                .OrderBy(m => m.Position)
                .ToList();

            ViewBag.ParentId = new SelectList(menus, "MenuId", "Title", tbMenu.ParentId);
            return View(tbMenu);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbMenu = await _context.TbMenus.FindAsync(id);
            if (tbMenu == null)
            {
                return NotFound();
            }


            var menus = _context.TbMenus
                .Where(m => m.IsActive == true && m.MenuId != id)
                .OrderBy(m => m.Position)
                .ToList();

            ViewBag.ParentId = new SelectList(menus, "MenuId", "Title", tbMenu.ParentId);
            return View(tbMenu);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MenuId,Title,Alias,Description,Levels,ParentId,Position,CreatedDate,CreatedBy,IsActive")] TbMenu tbMenu)
        {
            if (id != tbMenu.MenuId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var adminName = HttpContext.Session.GetString("AdminName") ?? "Admin";

                    tbMenu.ModifiedDate = DateTime.Now;
                    tbMenu.ModifiedBy = adminName;


                    if (tbMenu.ParentId.HasValue)
                    {
                        var parent = await _context.TbMenus.FindAsync(tbMenu.ParentId.Value);
                        if (parent != null)
                        {
                            tbMenu.Levels = (parent.Levels ?? 0) + 1;
                        }
                    }
                    else
                    {
                        tbMenu.Levels = 1;
                    }

                    _context.Update(tbMenu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbMenuExists(tbMenu.MenuId))
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


            var menus = _context.TbMenus
                .Where(m => m.IsActive == true && m.MenuId != id)
                .OrderBy(m => m.Position)
                .ToList();

            ViewBag.ParentId = new SelectList(menus, "MenuId", "Title", tbMenu.ParentId);
            return View(tbMenu);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbMenu = await _context.TbMenus
                .FirstOrDefaultAsync(m => m.MenuId == id);
            if (tbMenu == null)
            {
                return NotFound();
            }


            if (tbMenu.ParentId.HasValue)
            {
                var parent = await _context.TbMenus.FindAsync(tbMenu.ParentId.Value);
                ViewBag.ParentName = parent?.Title ?? "N/A";
            }

            return View(tbMenu);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbMenu = await _context.TbMenus.FindAsync(id);
            if (tbMenu != null)
            {

                var hasChildren = await _context.TbMenus.AnyAsync(m => m.ParentId == id);
                if (hasChildren)
                {
                    TempData["ErrorMessage"] = "Không thể xóa menu này vì còn menu con. Vui lòng xóa menu con trước.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.TbMenus.Remove(tbMenu);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TbMenuExists(int id)
        {
            return _context.TbMenus.Any(e => e.MenuId == id);
        }
    }
}

