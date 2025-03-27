using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LicentaPharmastock.Models;

namespace LicentaPharmastock.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<LicentaPharmastock.Models.Product> Product { get; set; } = default!;
        public DbSet<LicentaPharmastock.Models.Brand> Brand { get; set; } = default!;
    }
}
