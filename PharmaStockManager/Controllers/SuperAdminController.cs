using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models;
using PharmaStockManager.Services;
using PharmaStockManager.Models.Identity;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "SuperAdmin")]
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
            WarehouseName = warehouses.FirstOrDefault(w => w.RefCode == user.RefCode)?.Name
        }).ToList();

        var employeeUsersWithWarehouses = employeeUsers.Select(user => new AppUser_Warehouse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            WarehouseName = warehouses.FirstOrDefault(w => w.RefCode == user.RefCode)?.Name
        }).ToList();

        var customerUsersWithWarehouses = customerUsers.Select(user => new AppUser_Warehouse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
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



    public IActionResult SystemSettings()
    {
        return View();
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
    public IActionResult DeleteWarehouse(int id)
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
}