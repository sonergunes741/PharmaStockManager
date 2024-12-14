using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaStockManager.Models;
using PharmaStockManager.Models.Identity;
using PharmaStockManager.Filters;
using PharmaStockManager.Models.ViewModels;
using PharmaStockManager.Models.ViewModel;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly PharmaContext _context;

    public AdminController(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        PharmaContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IActionResult> Users()
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Admin user not found";
                return View(new List<UserManagementViewModel>());
            }

            var users = await _userManager.Users
                .Where(u => u.RefCode == currentUser.RefCode)
                .ToListAsync();

            var userViewModels = new List<UserManagementViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "Pending";

                if(role != "SuperAdmin" && role != "Admin")
                {
                    userViewModels.Add(new UserManagementViewModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        ActiveUser = user.ActiveUser,
                        Role = role
                    });
                }
                
            }

            return View(userViewModels);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred while fetching users.";
            return View(new List<UserManagementViewModel>());
        }
    }

    [HttpPost]
    public async Task<JsonResult> AcceptUser(int userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            // Mevcut admin kullanıcısını al ve yetki kontrolü yap
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || user.RefCode != currentUser.RefCode)
            {
                return Json(new { success = false, message = "Not authorized" });
            }

            user.ActiveUser = true;
            var result = await _userManager.UpdateAsync(user);

            return Json(new { success = result.Succeeded });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "An error occurred" });
        }
    }

    [HttpPost]
    public async Task<JsonResult> RejectUser(int userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            // Mevcut admin kullanıcısını al ve yetki kontrolü yap
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || user.RefCode != currentUser.RefCode)
            {
                return Json(new { success = false, message = "Not authorized" });
            }

            var result = await _userManager.DeleteAsync(user);
            return Json(new { success = result.Succeeded });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "An error occurred" });
        }
    }
    [HttpPost]
    public async Task<JsonResult> MakeEmployee([FromQuery] int userId) // FromQuery attribute'unu ekledik
    {
        try
        {
            // Debug için log ekleyelim
            Console.WriteLine($"Received userId: {userId}");

            // Kullanıcıyı bul
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                Console.WriteLine("User not found");
                return Json(new { success = false, message = "User not found" });
            }

            // Mevcut admin'i al ve yetki kontrolü yap
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || user.RefCode != currentUser.RefCode)
            {
                return Json(new { success = false, message = "Not authorized" });
            }

            // Önce tüm rolleri temizle
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // Employee rolünü ekle
            var result = await _userManager.AddToRoleAsync(user, "Employee");

            if (!result.Succeeded)
            {
                return Json(new { success = false, message = "Failed to assign Employee role" });
            }

            // UserRoles tablosunda rol ID'sini güncelle
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId);

            if (userRole != null)
            {
                userRole.RoleId = 3; // Employee rol ID'si
            }
            else
            {
                _context.UserRoles.Add(new IdentityUserRole<int>
                {
                    UserId = userId,
                    RoleId = 3
                });
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("Role updated successfully");
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<JsonResult> MakeCustomer([FromQuery] int userId)  // FromQuery attribute'unu ekledik
    {
        try
        {
            // Debug için log ekleyelim
            Console.WriteLine($"Received userId: {userId}");

            // Kullanıcıyı bul
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                Console.WriteLine("User not found");
                return Json(new { success = false, message = "User not found" });
            }

            // Mevcut admin'i al ve yetki kontrolü yap
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || user.RefCode != currentUser.RefCode)
            {
                return Json(new { success = false, message = "Not authorized" });
            }

            // Önce tüm rolleri temizle
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // Customer rolünü ekle
            var result = await _userManager.AddToRoleAsync(user, "Customer");

            if (!result.Succeeded)
            {
                return Json(new { success = false, message = "Failed to assign Customer role" });
            }

            // UserRoles tablosunda rol ID'sini güncelle
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId);

            if (userRole != null)
            {
                userRole.RoleId = 4; // Customer rol ID'si
            }
            else
            {
                _context.UserRoles.Add(new IdentityUserRole<int>
                {
                    UserId = userId,
                    RoleId = 4
                });
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("Role updated successfully");
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return Json(new { success = false, message = ex.Message });
        }
    }
    [HttpGet]
    public async Task<IActionResult> EditPermissions(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return NotFound("Admin user not found");
        }

        var user = await _userManager.Users
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound("User not found");
        }

        if (user.RefCode != currentUser.RefCode)
        {
            return Forbid("You don't have permission to edit this user's permissions");
        }

        var permissions = user.Permissions.FirstOrDefault() ?? new Permissions { UserID = id };
        return View(permissions);
    }

    [HttpPost]
    public async Task<IActionResult> EditPermissions(Permissions model)
    {
        if (model.UserID == 0)
        {
            return BadRequest("User ID is required");
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return NotFound("Admin user not found");
        }

        var user = await _userManager.FindByIdAsync(model.UserID.ToString());
        if (user == null)
        {
            return NotFound("User not found");
        }

        if (user.RefCode != currentUser.RefCode)
        {
            return Forbid("You don't have permission to edit this user's permissions");
        }

        var existingPermission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.UserID == model.UserID);

        if (existingPermission != null)
        {
            existingPermission.EditStocks = model.EditStocks;
            existingPermission.StockIn = model.StockIn;
            existingPermission.StockOut = model.StockOut;
            _context.Permissions.Update(existingPermission);
        }
        else
        {
            await _context.Permissions.AddAsync(model);
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Permissions updated successfully";
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    public async Task<JsonResult> DeleteUser(int id)
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "Admin user not found" });
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            if (user.RefCode != currentUser.RefCode)
            {
                return Json(new { success = false, message = "You don't have permission to delete this user" });
            }

            var permissions = await _context.Permissions.FirstOrDefaultAsync(p => p.UserID == id);
            if (permissions != null)
            {
                _context.Permissions.Remove(permissions);
                await _context.SaveChangesAsync();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Failed to delete user" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "An error occurred while deleting the user" });
        }
    }
}