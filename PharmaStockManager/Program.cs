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

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // Optional: Configure Identity options
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<PharmaContext>()
.AddDefaultTokenProviders();

var app = builder.Build();

// Create a scope to access services and create roles and users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    var dbContext = services.GetRequiredService<PharmaContext>();

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

        var createResult = await userManager.CreateAsync(adminUser, "Admin123!");
        if (createResult.Succeeded)
        {
            var permissions = new Permissions
            {
                EditStocks = true,
                StockIn = true,
                StockOut = true,
                UserID = adminUser.Id
            };

            dbContext.Permissions.Add(permissions);
            await dbContext.SaveChangesAsync();

            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
        else
        {
            // Log or handle user creation errors
            foreach (var error in createResult.Errors)
            {
                Console.WriteLine($"User creation error: {error.Description}");
            }
        }
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
            Email = userEmail,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(defaultUser, "User123!");
        if (createResult.Succeeded)
        {
            var permissions = new Permissions
            {
                EditStocks = false,
                StockIn = false,
                StockOut = false,
                UserID = defaultUser.Id
            };

            dbContext.Permissions.Add(permissions);
            await dbContext.SaveChangesAsync();

            await userManager.AddToRoleAsync(defaultUser, userRole);
        }
        else
        {
            // Log or handle user creation errors
            foreach (var error in createResult.Errors)
            {
                Console.WriteLine($"User creation error: {error.Description}");
            }
        }
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();