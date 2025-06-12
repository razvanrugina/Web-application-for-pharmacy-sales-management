using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using LicentaPharmastock.Data;
using LicentaPharmastock.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LicentaPharmastock.Areas.Identity.Pages.Manager
{
    [Authorize(Roles = "Manager")]
    public class CreatePharmacistModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PerUserDbContext _perUserContext;
        private readonly ApplicationDbContext _identityContext;

        public CreatePharmacistModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            PerUserDbContext perUserContext,
            ApplicationDbContext identityContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _perUserContext = perUserContext;
            _identityContext = identityContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<SelectListItem> LocationList { get; set; }

        public class InputModel
        {
            [Required, Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required, Display(Name = "Surname")]
            public string Surname { get; set; }

            [Required, Display(Name = "Location")]
            public int LocationId { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            LocationList = await _perUserContext.Locations
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = $"{l.name} ({l.address})"
                })
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            // 1) Find the per-user location
            var perLoc = await _perUserContext.Locations.FindAsync(Input.LocationId);
            if (perLoc == null)
            {
                ModelState.AddModelError("", "Location not found");
                await OnGetAsync();
                return Page();
            }

            // 2) Ensure same location exists in Identity DB
            var idLoc = await _identityContext.Locations
                .FirstOrDefaultAsync(l => l.name == perLoc.name && l.address == perLoc.address);
            if (idLoc == null)
            {
                idLoc = new Location
                {
                    name = perLoc.name,
                    address = perLoc.address,
                    Latitude = perLoc.Latitude,
                    Longitude = perLoc.Longitude
                };
                _identityContext.Locations.Add(idLoc);
                await _identityContext.SaveChangesAsync();
            }

            // 3) Build email + password using only the segment before a space or hyphen
            var yy = DateTime.UtcNow.Year.ToString()[2..];
            var fnKey = Input.FirstName
                .Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries)[0]
                .ToLower();
            var snKey = Input.Surname
                .Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries)[0]
                .ToLower();
            var email = $"{snKey}{fnKey}{yy}@pharmastock.ro";
            var password = GenerateSecurePassword(10);
            var manager = await _userManager.GetUserAsync(User);

            // 4) Create the user
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = Input.FirstName,
                Surname = Input.Surname,
                Nickname = Input.FirstName,
                LocationId = idLoc.Id,
                EmailConfirmed = true,
                CreatedByManagerId = manager.Id,
                DatabaseName = manager.DatabaseName,
                PlainPassword = password
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);

                await OnGetAsync();
                return Page();
            }

            if (!await _roleManager.RoleExistsAsync("Pharmacist"))
                await _roleManager.CreateAsync(new IdentityRole("Pharmacist"));
            await _userManager.AddToRoleAsync(user, "Pharmacist");

            TempData["PharmacistCreated"] = $"Created: {email} / {password}";
            // redirect to MVC index where the alert and Show/Hide logic run
            return RedirectToAction("Index", "Pharmacists");
        }

        private string GenerateSecurePassword(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
            var sb = new StringBuilder();
            var rnd = new Random();
            while (sb.Length < length)
                sb.Append(chars[rnd.Next(chars.Length)]);
            return sb.ToString();
        }
    }
}
