using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LicentaPharmastock.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Nickname { get; set; }
        public string DatabaseName { get; set; }

        // New properties for better name management
        public string FirstName { get; set; }
        public string Surname { get; set; } 

        // Optional link to location (for pharmacists)
        public int? LocationId { get; set; }
        public Location Location { get; set; }
        public string CreatedByManagerId { get; set; }
        public string PlainPassword { get; set; }


    }
}
