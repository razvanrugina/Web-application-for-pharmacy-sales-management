using Microsoft.AspNetCore.Identity;
namespace LicentaPharmastock.Models

{
    public class ApplicationUser : IdentityUser
    {
        public string DatabaseName { get; set; }

    }
}
