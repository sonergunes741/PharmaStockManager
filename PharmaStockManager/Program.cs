using PharmaStockManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmaStockManager.Filters;
using PharmaStockManager.Models.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<EmailConfirmedFilter>();

// Add DbContext and Identity to the services
builder.Services.AddDbContext<PharmaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PharmaDatabase")));

builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<PharmaContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Create a scope to access services and create roles and users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

    // Create Admin role if it doesn't exist
    var adminRole = "Admin";
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new AppRole(adminRole));
    }

    // Create User role if it doesn't exist
    var userRole = "User";
    if (!await roleManager.RoleExistsAsync(userRole))
    {
        await roleManager.CreateAsync(new AppRole(userRole));
    }

    // Create a default admin user if it doesn't exist
    var adminEmail = "webwizardssol@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new AppUser
        {
            ActiveUser = true,
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, "Admin123!"); // Default password
        await userManager.AddToRoleAsync(adminUser, adminRole);
    }

    // Create a default user if it doesn't exist
    var userEmail = "user@example.com";
    var defaultUser = await userManager.FindByEmailAsync(userEmail);
    if (defaultUser == null)
    {
        defaultUser = new AppUser
        {
            ActiveUser = true,
            UserName = userEmail,
            Email = userEmail
        };
        await userManager.CreateAsync(defaultUser, "User123!"); // Default password
        await userManager.AddToRoleAsync(defaultUser, userRole);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Add authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
