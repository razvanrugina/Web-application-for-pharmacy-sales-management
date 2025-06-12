using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LicentaPharmastock.Data;
using LicentaPharmastock.Models;
using LicentaPharmastock.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LicentaPharmastock.Controllers
{
    [Authorize(Roles = "Manager,Pharmacist")]
    public class ProductsController : Controller
    {
        private readonly PerUserDbContextFactory _contextFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(
            PerUserDbContextFactory contextFactory,
            UserManager<ApplicationUser> userManager)
        {
            _contextFactory = contextFactory;
            _userManager = userManager;
        }

        // GET: Products
        [Authorize(Roles = "Manager,Pharmacist")]
        public async Task<IActionResult> Index()
        {
            // 1) Load all products (with Brand + Locations) into a List
            await using var context = await _contextFactory.CreateAsync();
            var allProducts = await context.Product
                .Include(p => p.Brand)
                .Include(p => p.Locations)
                .ToListAsync();

            // 2) If Pharmacist: filter by their location claim in-memory
            IEnumerable<Product> productsToShow;
            if (User.IsInRole("Pharmacist"))
            {
                var locClaim = User.FindFirst("PharmacyLocation")?.Value;
                if (string.IsNullOrEmpty(locClaim))
                {
                    productsToShow = new List<Product>();
                }
                else
                {
                    productsToShow = allProducts
                        .Where(p => p.Locations.Any(l => l.name == locClaim))
                        .ToList();
                }
            }
            else
            {
                // Manager sees all
                productsToShow = allProducts;
            }

            // 3) If Manager: compute inventory by location & total in-memory
            if (User.IsInRole("Manager"))
            {
                var invByLocation = productsToShow
                    .SelectMany(p => p.Locations, (p, loc) => new { p, loc })
                    .GroupBy(x => x.loc.name)
                    .Select(g => new {
                        LocationName = g.Key,
                        TotalValue = g.Sum(x =>
                            x.p.PackageCount * x.p.PackagePrice
                            + x.p.LooseUnitCount * (x.p.PackagePrice / x.p.UnitsPerPackage)
                        )
                    })
                    .ToList();

                ViewBag.InventoryByLocation = invByLocation;
                ViewBag.InventoryTotal = invByLocation.Sum(x => x.TotalValue);
            }

            // 4) Return the in-memory list to the view
            return View(productsToShow);
        }



        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            await using var context = await _contextFactory.CreateAsync();
            var product = await context.Product
                .Include(p => p.Brand)
                .Include(p => p.Locations)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();
            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            await using var context = await _contextFactory.CreateAsync();
            ViewData["BrandId"] = new SelectList(await context.Brand.ToListAsync(), "Id", "name");
            ViewData["LocationIds"] = new MultiSelectList(await context.Locations.ToListAsync(), "Id", "name");
            return View();
        }

        // POST: Products/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                await using var ctx = await _contextFactory.CreateAsync();
                ViewData["BrandId"] = new SelectList(await ctx.Brand.ToListAsync(), "Id", "name", product.BrandId);
                ViewData["LocationIds"] = new MultiSelectList(await ctx.Locations.ToListAsync(), "Id", "name");
                return View(product);
            }

            await using var context = await _contextFactory.CreateAsync();
            var locationClaim = User.FindFirst("PharmacyLocation")?.Value;
            if (!string.IsNullOrEmpty(locationClaim))
            {
                var matchedLocation = await context.Locations
                    .FirstOrDefaultAsync(loc => loc.name == locationClaim);
                if (matchedLocation != null)
                {
                    product.Locations.Add(matchedLocation);
                }
            }

            context.Add(product);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            await using var context = await _contextFactory.CreateAsync();
            var product = await context.Product
                .Include(p => p.Locations)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            ViewData["BrandId"] = new SelectList(await context.Brand.ToListAsync(), "Id", "name", product.BrandId);
            ViewData["LocationIds"] = new MultiSelectList(
                await context.Locations.ToListAsync(),
                "Id", "name",
                product.Locations.Select(l => l.Id));
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Type,BrandId,UnitsPerPackage,PackageCount,LooseUnitCount,ExpirationDate,PackagePrice")] Product product)
        {
            if (id != product.Id) return NotFound();

            await using var context = await _contextFactory.CreateAsync();
            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await context.Product
                        .Include(p => p.Locations)
                        .FirstOrDefaultAsync(p => p.Id == id);
                    if (existing == null) return NotFound();

                    context.Entry(existing).CurrentValues.SetValues(product);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductExistsAsync(id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["BrandId"] = new SelectList(await context.Brand.ToListAsync(), "Id", "name", product.BrandId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            await using var context = await _contextFactory.CreateAsync();
            var product = await context.Product
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
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

        // GET: Products/GetProductAvailability
        [HttpGet]
        public async Task<IActionResult> GetProductAvailability(string name, int brandId, int currentProductId)
        {
            await using var context = await _contextFactory.CreateAsync();
            var locationClaim = User.FindFirst("PharmacyLocation")?.Value;

            var productMatches = await context.Product
                .Include(p => p.Locations)
                .Include(p => p.Brand)
                .Where(p => p.Name == name &&
                            p.BrandId == brandId &&
                            p.Id != currentProductId)
                .ToListAsync();

            var result = productMatches
                .SelectMany(p => p.Locations)
                .Where(loc => loc.name != locationClaim)
                .GroupBy(loc => loc.name)
                .Select(g => new {
                    LocationName = g.Key,
                    Quantity = productMatches
                        .Where(p => p.Locations.Any(l => l.name == g.Key))
                        .Sum(p => p.Quantity)
                })
                .ToList();

            return Json(result);
        }
    }
}
