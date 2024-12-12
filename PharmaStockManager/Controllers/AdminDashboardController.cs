// FILE: Controllers/AdminDashboardController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PharmaStockManager.Models;
using PharmaStockManager.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace PharmaStockManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly PharmaContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AdminDashboardController(PharmaContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var criticalStockDrugs = await _context.Drugs
                .Where(d => d.Quantity <= 10)
                .ToListAsync();

            var viewModel = new
            {
                TotalDrugs = await _context.Drugs.CountAsync(),
                CriticalStockDrugs = criticalStockDrugs,
                TotalCategories = await _context.Categories.CountAsync(),
                TotalUsers = await _userManager.Users.CountAsync()
            };

            return View(viewModel);
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult StockManagement()
        {
            return View("stock_management");
        }
    }
}