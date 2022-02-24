using BilHealth.Data;
using BilHealth.Model;
using BilHealth.Services.Users;
using BilHealth.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("AppDbContext")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IPasswordHasher<User>, BCryptPasswordHasher>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "bilhealthsess";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    options.SlidingExpiration = true;
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
}); // May want to use a server-side ticket instead: https://mikerussellnz.github.io/.NET-Core-Auth-Ticket-Redis/

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<IAuthenticationService>().CreateRoles().Wait();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. See https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // For production scenarios, it may make more sense to use a reverse-proxy instead of Kestrel-based HTTPS
    // See the comments in file `docker-compose.prod.yml` for details
    // Although, is this needed even in development where the edge server is CRA's live Express server?
    app.UseHttpsRedirection();

    using (var scope = app.Services.CreateScope())
    {
        // Register an admin user for development-only
        var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
        var adminUsername = "0000";

        authService.DeleteUser(adminUsername).Wait();
        authService.Register(new()
        {
            UserName = adminUsername,
            Password = "admin123",
            Email = "tempmail@example.com",
            FirstName = "John",
            LastName = "Smith"
        }).Wait();
        authService.AssignRole(adminUsername, UserRoles.Admin).Wait();
    }
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
