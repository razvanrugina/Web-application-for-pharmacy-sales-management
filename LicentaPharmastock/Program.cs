using LicentaPharmastock.Data;
using LicentaPharmastock.Models;
using LicentaPharmastock.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Get the default connection string from appsettings.json
var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register ApplicationDbContext for Identity and shared data
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(defaultConnectionString));

// Register IHttpContextAccessor early — required for PerUserDbContext factory
builder.Services.AddHttpContextAccessor();

// Register PerUserDbContext (scoped per request)
builder.Services.AddScoped<PerUserDbContext>(provider =>
{
    var httpContext = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

    if (httpContext?.User?.Identity?.IsAuthenticated == true)
    {
        var userId = userManager.GetUserId(httpContext.User);
        var user = userManager.Users.FirstOrDefault(u => u.Id == userId);

        var userDbName = user?.DatabaseName;

        if (!string.IsNullOrWhiteSpace(userDbName))
        {
            var connStr = BuildConnectionStringFor(userDbName);

            var optionsBuilder = new DbContextOptionsBuilder<PerUserDbContext>();
            optionsBuilder.UseSqlServer(connStr);
            return new PerUserDbContext(optionsBuilder.Options);
        }
    }

    throw new Exception("User not authenticated or missing database info");
});

// Helper method to build connection string for a user's database
static string BuildConnectionStringFor(string dbName)
{
    return $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True;MultipleActiveResultSets=true";
}

// Add Identity with roles and Entity Framework stores
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add MVC and Razor Pages support
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Your custom services
builder.Services.AddScoped<PerUserDbContextFactory>();
builder.Services.AddScoped<UserDatabaseService>();

var app = builder.Build();



// Middleware pipeline configuration
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.Use(async (context, next) =>
{
    if (!context.User.Identity.IsAuthenticated &&
        !context.Request.Path.StartsWithSegments("/Identity/Account/Login") &&
        !context.Request.Path.StartsWithSegments("/Identity/Account/Register"))
    {
        context.Response.Redirect("/Identity/Account/Login");
        return;
    }
    await next();
});
app.UseAuthorization();

// Redirect unauthenticated users to login page (except for login/register pages)


// Configure routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
