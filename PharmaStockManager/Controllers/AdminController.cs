using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")] // Bu controller'a yalnızca Admin rolündeki kullanıcılar erişebilir.
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: Admin/Users
    public IActionResult Users()
    {
        var users = _userManager.Users.ToList(); // Mevcut tüm kullanıcıları listele
        return View(users);
    }
}
