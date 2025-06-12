using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LicentaPharmastock.Models;

namespace LicentaPharmastock.Areas.Identity.Pages.Manager
{
    public class FixExistingAccountsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public string StatusMessage { get; set; }

        public FixExistingAccountsModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var email = "razvan.rugina24@gmail.com"; // Change this to the real existing manager's email
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                StatusMessage = $"No user found with email {email}.";
                return Page();
            }

            if (!await _userManager.IsInRoleAsync(user, "Manager"))
            {
                if (!await _roleManager.RoleExistsAsync("Manager"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Manager"));
                }

                await _userManager.AddToRoleAsync(user, "Manager");
                StatusMessage = "Role 'Manager' has been assigned successfully.";
            }
            else
            {
                StatusMessage = "User already has the 'Manager' role.";
            }

            return Page();
        }
    }
}
