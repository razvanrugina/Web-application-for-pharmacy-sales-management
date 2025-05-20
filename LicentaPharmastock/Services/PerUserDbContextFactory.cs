using LicentaPharmastock.Data;
using LicentaPharmastock.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LicentaPharmastock.Services
{
    public class PerUserDbContextFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public PerUserDbContextFactory(
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<PerUserDbContext> CreateAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("No HttpContext available");

            var user = await _userManager.GetUserAsync(httpContext.User);
            if (user == null || string.IsNullOrEmpty(user.DatabaseName))
                throw new InvalidOperationException("User not authenticated or database not set");

            var template = _configuration.GetConnectionString("PerUserDbTemplate");
            if (string.IsNullOrEmpty(template))
                throw new InvalidOperationException("Connection string 'PerUserDbTemplate' not found.");

            var connectionString = template.Replace("{DBNAME}", user.DatabaseName);

            var optionsBuilder = new DbContextOptionsBuilder<PerUserDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new PerUserDbContext(optionsBuilder.Options);
        }

    }

}
