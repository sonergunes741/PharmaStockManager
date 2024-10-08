using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

[Authorize(Roles = "Admin")] // Bu controller'a yalnızca Admin rolündeki kullanıcılar erişebilir.
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: Admin/Users
    public async Task<IActionResult> Users()
    {
        var users = _userManager.Users.ToList();
        var userRoles = new Dictionary<IdentityUser, IList<string>>();

        // Kullanıcıların rollerini alıp sözlük olarak oluşturuyoruz
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add(user, roles);
        }

        return View(userRoles); // Dictionary modelini view'e gönder
    }

    // GET: Admin/CreateUser
    public IActionResult CreateUser()
    {
        return View();
    }

    // POST: Admin/CreateUser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(string email, string password)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Users));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return View();
    }

    // GET: Admin/EditUser
    public async Task<IActionResult> EditUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: Admin/EditUser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(string id, string email, string password)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            user.Email = email;
            user.UserName = email;

            var result = await _userManager.UpdateAsync(user);
            if (!string.IsNullOrEmpty(password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, password);
                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Users));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(user);
    }

    // POST: Admin/DeleteUser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Users));
            }
            else
            {
                ModelState.AddModelError("", "Error occurred while deleting the user.");
            }
        }
        return RedirectToAction(nameof(Users));
    }

    // GET: Admin/ManageRoles
    public IActionResult ManageRoles()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    // POST: Admin/CreateRole
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        if (!string.IsNullOrEmpty(roleName))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ManageRoles));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return View(nameof(ManageRoles));
    }

    // POST: Admin/DeleteRole
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role != null)
        {
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ManageRoles));
            }
        }

        return RedirectToAction(nameof(ManageRoles));
    }
}
