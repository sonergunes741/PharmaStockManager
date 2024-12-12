using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models;
using PharmaStockManager.Services;

[Authorize(Roles = "SuperAdmin")]
public class SuperAdminController : Controller
{
    private readonly PharmaContext _dbContext;
    private readonly WarehouseService _warehouseService;

    public SuperAdminController(PharmaContext dbContext,WarehouseService warehouseService)
    {
        _dbContext = dbContext;
        _warehouseService = warehouseService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ManageUsers()
    {
        return View();
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
        Console.WriteLine(warehouse.Name + " " + warehouse.Address + " "  + " 2 ");
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