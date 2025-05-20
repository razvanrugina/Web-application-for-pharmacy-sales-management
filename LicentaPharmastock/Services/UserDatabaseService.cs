using LicentaPharmastock.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace LicentaPharmastock.Services
{
    public class UserDatabaseService
    {
        private readonly IConfiguration _configuration;

        public UserDatabaseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> CreateDatabaseAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be null or empty.");

            string dbName = $"PharmaDb_{userId.Replace("-", "")}";
            string connectionString = _configuration.GetConnectionString("PerUserDbTemplate")
                .Replace("{DBNAME}", dbName);

            var optionsBuilder = new DbContextOptionsBuilder<PerUserDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using var context = new PerUserDbContext(optionsBuilder.Options);

            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                // Log or rethrow
                throw new InvalidOperationException($"Database creation failed for user {userId}", ex);
            }
            return dbName;
        }
    }
}
