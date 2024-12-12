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

    // Create User role if it doesn't exist
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

    // Create a default admin user if it doesn't exist
    var superAdminEmail = "webwizardssol@example.com";
    var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);
    if (superAdminUser == null)
    {
        superAdminUser = new AppUser
        {
            ActiveUser = true,
            UserName = superAdminEmail,
            Email = superAdminEmail,
            EmailConfirmed = true
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
            EmailConfirmed = true
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
            EmailConfirmed = true
        };
        await userManager.CreateAsync(employeeUser, "Employee123!"); // Default password
        await userManager.AddToRoleAsync(employeeUser, employeeRole);
    }

    // Create a default user if it doesn't exist
    var customerEmail = "Customer@example.com";
    var customerUser = await userManager.FindByEmailAsync(customerEmail);
    if (customerUser == null)
    {
        customerUser = new AppUser
        {
            ActiveUser = true,
            UserName = customerEmail,
            Email = customerEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(customerUser, "Customer123!"); // Default password
        await userManager.AddToRoleAsync(customerUser, customerRole);
    }


    // Create a default employee if it doesn't exist
    var employeeEmail = "employee@example.com";
    var defaultEmployee = await userManager.FindByEmailAsync(employeeEmail);
    if (defaultEmployee == null)
    {
        defaultEmployee = new AppUser
        {
            ActiveUser = true,
            UserName = employeeEmail,
            Email = employeeEmail
        };
        await userManager.CreateAsync(defaultEmployee, "Employee123!"); // Default password
        await userManager.AddToRoleAsync(defaultEmployee, employeeRole);
    }

    var customerEmail = "customer@example.com";
    var defaultCustomer = await userManager.FindByEmailAsync(customerEmail);
    if (defaultCustomer == null)
    {
        defaultCustomer = new AppUser
        {
            ActiveUser = true,
            UserName = customerEmail,
            Email = customerEmail
        };
        await userManager.CreateAsync(defaultCustomer, "Customer123!"); // Default password
        await userManager.AddToRoleAsync(defaultCustomer, customerRole);
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