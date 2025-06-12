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

        if (string.IsNullOrEmpty(userId))
        {
            throw new Exception("User ID not found in context.");
        }

        var user = userManager.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            throw new Exception($"No user found with ID: {userId}");
        }

        var userDbName = user?.DatabaseName;

        if (!string.IsNullOrWhiteSpace(userDbName))
        {
            var connStr = BuildConnectionStringFor(userDbName);

            var optionsBuilder = new DbContextOptionsBuilder<PerUserDbContext>();
            optionsBuilder.UseSqlServer(connStr);
            return new PerUserDbContext(optionsBuilder.Options);
        }
    }

    //throw new Exception("User not authenticated or missing database info");
    var defaultOptions = new DbContextOptionsBuilder<PerUserDbContext>().Options;
    return new PerUserDbContext(defaultOptions);
});

// Helper method to build connection string for a user's database
static string BuildConnectionStringFor(string dbName)
{
    return $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True;MultipleActiveResultSets=true";
}

// Configure Identity cookie options to use session cookies (expire on browser close)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14); // or whatever duration you want for "Remember me"
    options.SlidingExpiration = true;
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";

    // This is important:
    // If RememberMe is false, cookie will be session cookie (expires on browser close)
    // If RememberMe is true, cookie will persist for ExpireTimeSpan
});

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

app.Use(async (context, next) =>
{
    if (context.User.Identity.IsAuthenticated && context.Request.Path == "/")
    {
        // Resolve UserManager<ApplicationUser> from DI
        var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.GetUserAsync(context.User);

        if (user != null)
        {
            var roles = await userManager.GetRolesAsync(user);

            if (roles.Contains("Manager"))
            {
                context.Response.Redirect("/Manager/Index");
                return; // Important: stop middleware pipeline after redirect
            }
            else if (roles.Contains("Pharmacist"))
            {
                context.Response.Redirect("/Products/Index");
                return;
            }
            else
            {
                // For any other roles or no roles, redirect somewhere safe, e.g. Products
                context.Response.Redirect("/Products/Index");
                return;
            }
        }
    }

    await next();
});


app.UseAuthentication();

// Custom middleware to redirect unauthenticated users to login page (except login/register pages)
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

// Custom middleware to redirect authenticated users at “/” to their role-specific dashboard
app.Use(async (context, next) =>
{
    // Only intercept exact root requests for signed-in users
    if (context.User.Identity.IsAuthenticated && context.Request.Path == "/")
    {
        if (context.User.IsInRole("Manager"))
        {
            context.Response.Redirect("/Identity/Manager");
            return;
        }
        else if (context.User.IsInRole("Pharmacist"))
        {
            context.Response.Redirect("/Products/Index");
            return;
        }
    }
    await next();
});


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
