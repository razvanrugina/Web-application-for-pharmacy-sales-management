using LicentaPharmastock.Models;
using Microsoft.EntityFrameworkCore;

namespace LicentaPharmastock.Data
{
    public class PerUserDbContext : DbContext
    {
        public PerUserDbContext(DbContextOptions<PerUserDbContext> options) : base(options) { }

        // Adaugă aici entitățile specifice utilizatorului
        public DbSet<Product> Product { get; set; }
        //public DbSet<Sale> Sales { get; set; }
        public DbSet<Brand> Brand { get; set; } = default!;
        public DbSet<Location> Location { get; set; } = default!;
    }
}
