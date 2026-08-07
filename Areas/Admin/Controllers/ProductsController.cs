using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using San_Pham_Do_An1.Models;
using San_Pham_Do_An1.Models.ViewModels;

namespace San_Pham_Do_An1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly WedQuanAoDbContext _context;

        public ProductsController(WedQuanAoDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Accounts", new { area = "Admin" });
            }
            var WedQuanAoDbContext = _context.TbProducts
                .Include(t => t.CategoryProduct)
                .Include(t => t.TbProductVariants)
                    .ThenInclude(v => v.Size)
                .Include(t => t.TbProductVariants)
                    .ThenInclude(v => v.Color)
                .ToListAsync();

            return View(await WedQuanAoDbContext);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbProduct = await _context.TbProducts
                .Include(t => t.CategoryProduct)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (tbProduct == null)
            {
                return NotFound();
            }

            return View(tbProduct);
        }


        public IActionResult Create()
        {
            ViewData["CategoryProductId"] = new SelectList(_context.TbProductCategories, "CategoryProductId", "Title");
            ViewData["Colors"] = new SelectList(_context.TbColors.Where(c => c.IsActive == true), "ColorId", "ColorName");
            ViewData["Sizes"] = new SelectList(_context.TbSizes.Where(s => s.IsActive == true), "SizeId", "SizeName");
            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = new TbProduct
                {
                    Title = model.Title,
                    Alias = model.Alias,
                    CategoryProductId = model.CategoryProductId,
                    Description = model.Description,
                    Detail = model.Detail,
                    Image = model.Image,
                    Price = model.Price,
                    PriceSale = model.PriceSale,
                    Quantity = model.Quantity,
                    IsNew = model.IsNew,
                    IsBestSeller = model.IsBestSeller,
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now,
                };
                _context.Add(product);
                await _context.SaveChangesAsync();

                if (model.Variants != null && model.Variants.Count > 0)
                {
                    foreach (var v in model.Variants)
                    {
                        var variant = new TbProductVariant
                        {
                            ProductId = product.ProductId,
                            ColorId = v.ColorId,
                            SizeId = v.SizeId,
                            Image = v.Image,
                            Sku = v.Sku,
                            Price = v.Price,
                            PriceSale = v.PriceSale,
                            Quantity = v.Quantity,
                            IsActive = v.IsActive
                        };
                        _context.TbProductVariants.Add(variant);
                    }
                    await _context.SaveChangesAsync();


                    var allVariants = await _context.TbProductVariants
                        .Where(v => v.ProductId == product.ProductId && v.IsActive == true)
                        .ToListAsync();

                    if (allVariants.Any())
                    {

                        decimal? minPrice = null;
                        decimal? minPriceSale = null;
                        decimal? minFinalPrice = null;


                        int totalQuantity = allVariants
                            .Where(v => v.Quantity.HasValue)
                            .Sum(v => v.Quantity.Value);

                        foreach (var variant in allVariants)
                        {
                            decimal? variantFinalPrice = variant.PriceSale ?? variant.Price;
                            if (variantFinalPrice.HasValue)
                            {
                                if (!minFinalPrice.HasValue || variantFinalPrice.Value < minFinalPrice.Value)
                                {
                                    minPrice = variant.Price;
                                    minPriceSale = variant.PriceSale;
                                    minFinalPrice = variantFinalPrice;
                                }
                            }
                        }


                        product.Price = minPrice;
                        product.PriceSale = minPriceSale;
                        product.Quantity = totalQuantity;
                    }



                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryProductId"] = new SelectList(_context.TbProductCategories, "CategoryProductId", "CategoryProductId", model.CategoryProductId);
            ViewData["Colors"] = new SelectList(_context.TbColors.Where(c => c.IsActive == true), "ColorId", "ColorName");
            ViewData["Sizes"] = new SelectList(_context.TbSizes.Where(s => s.IsActive == true), "SizeId", "SizeName");
            return View(model);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbProduct = await _context.TbProducts
                .Include(p => p.TbProductVariants)
                    .ThenInclude(v => v.Color)
                .Include(p => p.TbProductVariants)
                    .ThenInclude(v => v.Size)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (tbProduct == null)
            {
                return NotFound();
            }
            ViewData["CategoryProductId"] = new SelectList(_context.TbProductCategories, "CategoryProductId", "CategoryProductId", tbProduct.CategoryProductId);
            ViewData["Colors"] = new SelectList(_context.TbColors.Where(c => c.IsActive == true), "ColorId", "ColorName");
            ViewData["Sizes"] = new SelectList(_context.TbSizes.Where(s => s.IsActive == true), "SizeId", "SizeName");
            return View(tbProduct);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Title,Alias,CategoryProductId,Description,Detail,Image,Price,PriceSale,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy,IsNew,IsBestSeller,IsActive,Quantity,Star")] TbProduct tbProduct, IFormCollection form)
        {
            if (id != tbProduct.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(tbProduct);


                    var deletedVariantIds = new List<int>();
                    var deletedVariants = form["DeletedVariants"].ToList();
                    foreach (var deletedId in deletedVariants)
                    {
                        if (int.TryParse(deletedId, out int deletedVariantId))
                        {
                            deletedVariantIds.Add(deletedVariantId);
                        }
                    }


                    if (deletedVariantIds.Any())
                    {
                        var variantsToDelete = await _context.TbProductVariants
                            .Where(v => deletedVariantIds.Contains(v.VariantId))
                            .ToListAsync();
                        _context.TbProductVariants.RemoveRange(variantsToDelete);
                    }


                    var variantKeys = form.Keys.Where(k => k.StartsWith("Variants[") && k.Contains("].VariantId")).ToList();

                    foreach (var key in variantKeys)
                    {

                        var match = Regex.Match(key, @"Variants\[(\d+)\]");
                        if (!match.Success) continue;

                        var index = match.Groups[1].Value;
                        var variantIdStr = form[$"Variants[{index}].VariantId"].ToString();
                        int variantId = 0;
                        int.TryParse(variantIdStr, out variantId);

                        var colorIdStr = form[$"Variants[{index}].ColorId"].ToString();
                        var sizeIdStr = form[$"Variants[{index}].SizeId"].ToString();


                        if (string.IsNullOrEmpty(colorIdStr) || string.IsNullOrEmpty(sizeIdStr))
                            continue;

                        int colorId = int.Parse(colorIdStr);
                        int sizeId = int.Parse(sizeIdStr);


                        var skuValue = form[$"Variants[{index}].Sku"];
                        var priceValue = form[$"Variants[{index}].Price"];
                        var priceSaleValue = form[$"Variants[{index}].PriceSale"];
                        var quantityValue = form[$"Variants[{index}].Quantity"];
                        var imageValue = form[$"Variants[{index}].Image"];
                        var isActiveValue = form[$"Variants[{index}].IsActive"];

                        var sku = skuValue.ToString().Trim();
                        var priceStr = priceValue.ToString().Trim();
                        var priceSaleStr = priceSaleValue.ToString().Trim();
                        var quantityStr = quantityValue.ToString().Trim();
                        var imageStr = imageValue.ToString().Trim();
                        var isActive = isActiveValue.ToString() == "true";

                        if (variantId > 0)
                        {

                            var existingVariant = await _context.TbProductVariants
                                .FirstOrDefaultAsync(v => v.VariantId == variantId);

                            if (existingVariant != null && !deletedVariantIds.Contains(variantId))
                            {
                                existingVariant.ColorId = colorId;
                                existingVariant.SizeId = sizeId;
                                existingVariant.Sku = !string.IsNullOrWhiteSpace(sku) ? sku : null;
                                existingVariant.Price = !string.IsNullOrWhiteSpace(priceStr) ? decimal.Parse(priceStr) : null;
                                existingVariant.PriceSale = !string.IsNullOrWhiteSpace(priceSaleStr) ? decimal.Parse(priceSaleStr) : null;
                                existingVariant.Quantity = !string.IsNullOrWhiteSpace(quantityStr) ? int.Parse(quantityStr) : null;


                                var newImageValue = !string.IsNullOrWhiteSpace(imageStr) ? imageStr : null;
                                existingVariant.Image = newImageValue;

                                existingVariant.IsActive = isActive;


                                _context.Entry(existingVariant).State = EntityState.Modified;
                            }
                        }
                        else
                        {

                            var newVariant = new TbProductVariant
                            {
                                ProductId = tbProduct.ProductId,
                                ColorId = colorId,
                                SizeId = sizeId,
                                Sku = !string.IsNullOrWhiteSpace(sku) ? sku : null,
                                Price = !string.IsNullOrWhiteSpace(priceStr) ? decimal.Parse(priceStr) : null,
                                PriceSale = !string.IsNullOrWhiteSpace(priceSaleStr) ? decimal.Parse(priceSaleStr) : null,
                                Quantity = !string.IsNullOrWhiteSpace(quantityStr) ? int.Parse(quantityStr) : null,
                                Image = !string.IsNullOrWhiteSpace(imageStr) ? imageStr : null,
                                IsActive = isActive
                            };
                            _context.TbProductVariants.Add(newVariant);
                        }
                    }

                    await _context.SaveChangesAsync();


                    var allVariants = await _context.TbProductVariants
                        .Where(v => v.ProductId == tbProduct.ProductId && v.IsActive == true)
                        .ToListAsync();

                    var productToUpdate = await _context.TbProducts
                        .FirstOrDefaultAsync(p => p.ProductId == tbProduct.ProductId);

                    if (productToUpdate != null)
                    {
                        if (allVariants.Any())
                        {

                            decimal? minPrice = null;
                            decimal? minPriceSale = null;
                            decimal? minFinalPrice = null;


                            int totalQuantity = allVariants
                                .Where(v => v.Quantity.HasValue)
                                .Sum(v => v.Quantity.Value);

                            foreach (var variant in allVariants)
                            {
                                decimal? variantFinalPrice = variant.PriceSale ?? variant.Price;
                                if (variantFinalPrice.HasValue)
                                {
                                    if (!minFinalPrice.HasValue || variantFinalPrice.Value < minFinalPrice.Value)
                                    {
                                        minPrice = variant.Price;
                                        minPriceSale = variant.PriceSale;
                                        minFinalPrice = variantFinalPrice;
                                    }
                                }
                            }


                            productToUpdate.Price = minPrice;
                            productToUpdate.PriceSale = minPriceSale;
                            productToUpdate.Quantity = totalQuantity;
                        }
                        else
                        {


                        }


                        _context.Update(productToUpdate);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbProductExists(tbProduct.ProductId))
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


            var tbProductWithVariants = await _context.TbProducts
                .Include(p => p.TbProductVariants)
                    .ThenInclude(v => v.Color)
                .Include(p => p.TbProductVariants)
                    .ThenInclude(v => v.Size)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            ViewData["CategoryProductId"] = new SelectList(_context.TbProductCategories, "CategoryProductId", "CategoryProductId", tbProduct.CategoryProductId);
            ViewData["Colors"] = new SelectList(_context.TbColors.Where(c => c.IsActive == true), "ColorId", "ColorName");
            ViewData["Sizes"] = new SelectList(_context.TbSizes.Where(s => s.IsActive == true), "SizeId", "SizeName");
            return View(tbProductWithVariants ?? tbProduct);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbProduct = await _context.TbProducts
                .Include(t => t.CategoryProduct)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (tbProduct == null)
            {
                return NotFound();
            }

            return View(tbProduct);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbProduct = await _context.TbProducts
                .Include(p => p.TbProductVariants)
                .Include(p => p.TbProductReviews)
                .Include(p => p.TbOrderDetails)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (tbProduct != null)
            {

                if (tbProduct.TbOrderDetails.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa sản phẩm này vì đã có trong đơn hàng. Vui lòng xóa các đơn hàng liên quan trước.";
                    return RedirectToAction(nameof(Index));
                }


                if (tbProduct.TbProductVariants.Any())
                {
                    _context.TbProductVariants.RemoveRange(tbProduct.TbProductVariants);
                }


                if (tbProduct.TbProductReviews.Any())
                {
                    _context.TbProductReviews.RemoveRange(tbProduct.TbProductReviews);
                }


                _context.TbProducts.Remove(tbProduct);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TbProductExists(int id)
        {
            return _context.TbProducts.Any(e => e.ProductId == id);
        }
    }
}
