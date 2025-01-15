using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models;
using PharmaStockManager.Services;
using PharmaStockManager.Models.Identity;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "SuperAdmin")]
[ServiceFilter(typeof(LogFilter))]
public class SuperAdminController : Controller
{
    private readonly PharmaContext _dbContext;
    private readonly WarehouseService _warehouseService;
    private readonly UserManager<AppUser> _userManager;

    public SuperAdminController(PharmaContext dbContext, WarehouseService warehouseService, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _warehouseService = warehouseService;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> ManageUsers()
    {
        var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
        var employeeUsers = await _userManager.GetUsersInRoleAsync("Employee");
        var customerUsers = await _userManager.GetUsersInRoleAsync("Customer");

        var warehouses = _dbContext.Warehouses.AsNoTracking().ToList();

        var adminUsersWithWarehouses = adminUsers.Select(user => new AppUser_Warehouse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            ActiveUser = user.ActiveUser,
            WarehouseName = warehouses.FirstOrDefault(w => w.RefCode == user.RefCode)?.Name
        }).ToList();

        var employeeUsersWithWarehouses = employeeUsers.Select(user => new AppUser_Warehouse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            ActiveUser = user.ActiveUser,
            WarehouseName = warehouses.FirstOrDefault(w => w.RefCode == user.RefCode)?.Name
        }).ToList();

        var customerUsersWithWarehouses = customerUsers.Select(user => new AppUser_Warehouse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            ActiveUser = user.ActiveUser,
            WarehouseName = warehouses.FirstOrDefault(w => w.RefCode == user.RefCode)?.Name
        }).ToList();

        var model = new ManageUsersViewModel
        {
            AdminUsers = adminUsersWithWarehouses,
            EmployeeUsers = employeeUsersWithWarehouses,
            CustomerUsers = customerUsersWithWarehouses
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        if (id == 0) {
            return RedirectToAction("ManageUsers", "SuperAdmin");
        }
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user != null) { 
            user.ActiveUser = false;
            await _dbContext.SaveChangesAsync();
        }
        return RedirectToAction("ManageUsers", "SuperAdmin");
    }

    [HttpGet]
    public async Task<IActionResult> ActivateUser(int id)
    {
        if (id == 0)
        {
            return RedirectToAction("ManageUsers", "SuperAdmin");
        }
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user != null)
        {
            user.ActiveUser = true;
            await _dbContext.SaveChangesAsync();
        }
        return RedirectToAction("ManageUsers", "SuperAdmin");
    }

    [HttpGet]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (id == 0)
        {
            return RedirectToAction("ManageUsers", "SuperAdmin");
        }
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user != null)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, roles);
            }
            user.ActiveUser = false;
            user.RefCode = null;
            user.UserName = null;
            await _dbContext.SaveChangesAsync();
        }
        return RedirectToAction("ManageUsers", "SuperAdmin");
    }

    public IActionResult ManageWarehouses()
    {
        var warehouses = _dbContext.Warehouses.ToList();
        return View(warehouses);
    }

    [HttpGet]
    public IActionResult GetWarehouse(int id)
    {
        var warehouse = _dbContext.Warehouses.Find(id);
        if (warehouse == null)
        {
            return NotFound();
        }
        return Json(warehouse);
    }

    [HttpPost]
    public async Task<IActionResult> EditWarehouse(int id, AddWarehouseModel updatedWarehouse)
    {
        var warehouse = await _dbContext.Warehouses.FindAsync(id);
        if (warehouse != null)
        {
            warehouse.Name = updatedWarehouse.Name;
            warehouse.Address = updatedWarehouse.Address;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("ManageWarehouses","SuperAdmin");
        }
        return NotFound();
    }


    [HttpPost]
    public async Task<IActionResult> SaveWarehouse(AddWarehouseModel warehouse)
    {
        if (ModelState.IsValid)
        {
            await _warehouseService.CreateWarehouseAsync(warehouse.Name, warehouse.Address);
        }
        Console.WriteLine(warehouse.Name + " " + warehouse.Address + " " + " 2 ");
        return RedirectToAction(nameof(ManageWarehouses));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteWarehouse(int id)
    {
        var warehouse = _dbContext.Warehouses.Find(id);
        if (warehouse != null)
        {
            _dbContext.Warehouses.Remove(warehouse);
            _dbContext.SaveChanges();
            return Ok();
        }
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> AddAdminUser(RegisterViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var warehouse = await _dbContext.Warehouses.FirstOrDefaultAsync(u => u.RefCode == viewModel.RefCode);
        if (warehouse == null)
        {
            ModelState.AddModelError("RefCode", "Invalid warehouse reference code.");
            return BadRequest(ModelState);
        }

        AppUser appUser = new AppUser()
        {
            UserName = viewModel.Email,
            Email = viewModel.Email,
            RefCode = viewModel.RefCode,
            ActivationCode = null,
            FullName = viewModel.FullName,
            ActiveUser = true
        };

        var result = await _userManager.CreateAsync(appUser, viewModel.Password);
        if (!result.Succeeded)
        {
            foreach (var item in result.Errors)
            {
                Console.WriteLine(item.Description);
                ModelState.AddModelError("", item.Description);
            }
            return BadRequest(ModelState);
        }

        Permissions permissions = new Permissions()
        {
            EditStocks = true,
            StockIn = true,
            StockOut = true,
            UserID = appUser.Id
        };

        _dbContext.Permissions.Add(permissions);
        await _userManager.AddToRoleAsync(appUser, "Admin");
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("ManageWarehouses", "SuperAdmin");
    }
}