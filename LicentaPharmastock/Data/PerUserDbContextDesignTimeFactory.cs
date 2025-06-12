using LicentaPharmastock.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LicentaPharmastock.Services
{
    // 👇 Used only at design time (e.g., Add-Migration, Update-Database)
    public class PerUserDbContextDesignTimeFactory : IDesignTimeDbContextFactory<PerUserDbContext>
    {
        public PerUserDbContext CreateDbContext(string[] args)
        {
            // Load configuration manually
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .Build();

            // Use the static connection string for design-time
            var connectionString = configuration.GetConnectionString("DesignTimeConnection");

            var optionsBuilder = new DbContextOptionsBuilder<PerUserDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new PerUserDbContext(optionsBuilder.Options);
        }
    }
}
