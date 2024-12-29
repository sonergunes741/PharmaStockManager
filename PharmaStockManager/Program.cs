using PharmaStockManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmaStockManager.Filters;
using PharmaStockManager.Models.Identity;
using PharmaStockManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<EmailConfirmedFilter>();
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<LogFilter>();

// Add DbContext and Identity to the services
builder.Services.AddDbContext<PharmaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PharmaDatabase")));

// Add Identity configuration
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<PharmaContext>()
.AddDefaultTokenProviders();

// Add RoleManager explicitly
builder.Services.AddScoped<RoleManager<AppRole>>();

var app = builder.Build();

// Create a scope to access services and create roles and users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

    var superAdminRole = "SuperAdmin";
    if (!await roleManager.RoleExistsAsync(superAdminRole))
    {
        await roleManager.CreateAsync(new AppRole(superAdminRole));
    }

    // Create Admin role if it doesn't exist
    var adminRole = "Admin";
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new AppRole(adminRole));
    }

    var employeeRole = "Employee";
    if (!await roleManager.RoleExistsAsync(employeeRole))
    {
        await roleManager.CreateAsync(new AppRole(employeeRole));
    }

    var customerRole = "Customer";
    if (!await roleManager.RoleExistsAsync(customerRole))
    {
        await roleManager.CreateAsync(new AppRole(customerRole));
    }

    // Create default users with RefCode and UserType
    var superAdminEmail = "webwizardssol@example.com";
    var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);
    if (superAdminUser == null)
    {
        superAdminUser = new AppUser
        {
            ActiveUser = true,
            UserName = superAdminEmail,
            Email = superAdminEmail,
            EmailConfirmed = true,
            UserType = "SuperAdmin",
            RefCode = "SUPER001"
        };
        await userManager.CreateAsync(superAdminUser, "Superadmin123!"); // Default password
        await userManager.AddToRoleAsync(superAdminUser, superAdminRole);
    }

    var adminEmail = "Admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new AppUser
        {
            ActiveUser = true,
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            UserType = "Admin",
            RefCode = "ADMIN001"
        };
        await userManager.CreateAsync(adminUser, "Admin123!"); // Default password
        await userManager.AddToRoleAsync(adminUser, adminRole);
    }

    var employeeEmail = "Employee@example.com";
    var employeeUser = await userManager.FindByEmailAsync(employeeEmail);
    if (employeeUser == null)
    {
        employeeUser = new AppUser
        {
            ActiveUser = true,
            UserName = employeeEmail,
            Email = employeeEmail,
            EmailConfirmed = true,
            UserType = "Employee",
            RefCode = "ADMIN001" // Aynı RefCode ile Admin'e bağlı
        };
        await userManager.CreateAsync(employeeUser, "Employee123!"); // Default password
        await userManager.AddToRoleAsync(employeeUser, employeeRole);
    }

    var customerEmail = "Customer@example.com";
    var customerUser = await userManager.FindByEmailAsync(customerEmail);
    if (customerUser == null)
    {
        customerUser = new AppUser
        {
            ActiveUser = true,
            UserName = customerEmail,
            Email = customerEmail,
            EmailConfirmed = true,
            UserType = "Customer",
            RefCode = "ADMIN001" // Aynı RefCode ile Admin'e bağlı
        };
        await userManager.CreateAsync(customerUser, "Customer123!"); // Default password
        await userManager.AddToRoleAsync(customerUser, customerRole);
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