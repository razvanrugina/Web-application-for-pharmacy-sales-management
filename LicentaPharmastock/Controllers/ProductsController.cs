using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LicentaPharmastock.Data;
using LicentaPharmastock.Models;
using LicentaPharmastock.Services;

namespace LicentaPharmastock.Controllers
{
    public class ProductsController : Controller
    {
        private readonly PerUserDbContextFactory _contextFactory;

        public ProductsController(PerUserDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }


        // GET: Products
        public async Task<IActionResult> Index()
        {
            await using var context = await _contextFactory.CreateAsync();
            var applicationDbContext = context.Product.Include(p => p.Brand);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await using var context = await _contextFactory.CreateAsync();
            var product = await context.Product
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }


            ViewData["BrandId"] = new SelectList(await context.Brand.ToListAsync(), "Id", "name");
            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            await using var context = await _contextFactory.CreateAsync();
            ViewData["BrandId"] = new SelectList(await context.Brand.ToListAsync(), "Id", "name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Type,BrandId,Prospectus,ImagePath,Locations,ExpirationDate,price,quantity")] Product product)
        {
            await using var context = await _contextFactory.CreateAsync();

            if (ModelState.IsValid)
            {
                context.Add(product);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BrandId"] = new SelectList(await context.Brand.ToListAsync(), "Id", "Id", product.BrandId);
            return View(product);
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await using var context = await _contextFactory.CreateAsync();

            var product = await context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["BrandId"] = new SelectList(await context.Brand.ToListAsync(), "Id", "name", product.BrandId);
            return View(product);
        }


        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Type,BrandId,Prospectus,ImagePath,Locations,ExpirationDate,price,quantity")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            await using var context = await _contextFactory.CreateAsync();

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(product);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductExistsAsync(product.Id))
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

            ViewData["BrandId"] = new SelectList(await context.Brand.ToListAsync(), "Id", "name", product.BrandId);
            return View(product);
        }


        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await using var context = await _contextFactory.CreateAsync();

            var product = await context.Product
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await using var context = await _contextFactory.CreateAsync();

            var product = await context.Product.FindAsync(id);
            if (product != null)
            {
                context.Product.Remove(product);
                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private async Task<bool> ProductExistsAsync(int id)
        {
            await using var context = await _contextFactory.CreateAsync();
            return await context.Product.AnyAsync(e => e.Id == id);
        }
    }
}
