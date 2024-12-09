using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models.Identity;
using PharmaStockManager.Models;

namespace PharmaStockManager.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IWebHostEnvironment _environment;
        private readonly PharmaContext _dbContext;
        private readonly IConfiguration _configuration;

        public SuperAdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            IWebHostEnvironment environment, PharmaContext dbContext, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
            _dbContext = dbContext;
            _configuration = configuration;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageUsers()
        {
            return View();
        }

        public IActionResult ManageWarehouses() {
            return View();
        }

        public IActionResult SystemSettings()
        {
            return View();
        }

    }
}
