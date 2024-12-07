using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaStockManager.Filters;
using PharmaStockManager.Models;
using PharmaStockManager.Models.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")] // Bu controller'a yalnızca Admin rolündeki kullanıcılar erişebilir.
public class AdminController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly PharmaContext _dbContext;

    public AdminController(UserManager<AppUser> userManager, PharmaContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    // GET: Admin/Users
    public IActionResult Users()
    {
        var users = _userManager.Users.ToList(); // Mevcut tüm kullanıcıları listele
        return View(users);
    }

    [PermissionFilter("StockIn", "StockOut")]
    [HttpGet]
    public IActionResult EditPermissions(int? Id)
    {
        if (!Id.HasValue)
        {
            return NotFound("User ID is required");
        }

        Console.WriteLine($"User ID received: {Id}"); // For debugging
        var permissions = _dbContext.Permissions
                                  .FirstOrDefault(p => p.UserID == Id);

        if (permissions == null)
        {
            // Optionally, create a new permissions record if it doesn't exist
            permissions = new Permissions { UserID = Id.Value };
            _dbContext.Permissions.Add(permissions);
            _dbContext.SaveChanges();
        }
        return View(permissions);
    }

    [HttpPost]
    public IActionResult EditPermissions(Permissions model)
    {
        if (model.UserID == 0)
        {
            return BadRequest("User ID is required");
        }

        Console.WriteLine($"Model UserID: {model.UserID}, EditStocks: {model.EditStocks}, StockIn: {model.StockIn}, StockOut: {model.StockOut}");
        var existingPermission = _dbContext.Permissions.FirstOrDefault(p => p.UserID == model.UserID);

        if (existingPermission != null)
        {
            existingPermission.EditStocks = model.EditStocks;
            existingPermission.StockIn = model.StockIn;
            existingPermission.StockOut = model.StockOut;
        }
        else
        {
            // Create a new permissions record if it doesn't exist
            _dbContext.Permissions.Add(model);
        }

        _dbContext.SaveChanges();
        return RedirectToAction("Users");
    }


}
