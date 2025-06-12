using LicentaPharmastock.Models;
using Microsoft.EntityFrameworkCore;

namespace LicentaPharmastock.Data
{
    public class PerUserDbContext : DbContext
    {
        public PerUserDbContext(DbContextOptions<PerUserDbContext> options) : base(options) { }

        public DbSet<Product> Product { get; set; }
        public DbSet<Brand> Brand { get; set; } = default!;
        public DbSet<Location> Locations { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-many configuration for Product <-> Location
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Locations)
                .WithMany() // No inverse nav
                .UsingEntity(j => j.ToTable("ProductLocations"));

            modelBuilder.Entity<Brand>()
                .HasIndex(b => b.name)
                .IsUnique();
        }
    }
}
