using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaStockManager.Controllers
{
    [Authorize(Roles = "Admin")] // Bu controller'a yalnızca "Admin" rolündeki kullanıcılar erişebilir
    public class AdminPanelController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminPanelController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Admin panelinin ana sayfası, tüm kullanıcıları listeler
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // Kullanıcıların rollerini yönetmek için bir sayfa
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            var model = new ManageRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserRoles = userRoles.ToList(),
                AllRoles = allRoles
            };

            return View(model);
        }

        // POST: Kullanıcıya yeni bir rol ekler
        [HttpPost]
        public async Task<IActionResult> UpdateRoles(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
            return RedirectToAction("ManageRoles", new { userId });
        }

        // POST: Kullanıcıdan rolü kaldırır
        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }
            return RedirectToAction("ManageRoles", new { userId });
        }
    }
}
