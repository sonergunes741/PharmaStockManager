using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PharmaStockManager.Models;
using PharmaStockManager.Models.Identity;
using Microsoft.EntityFrameworkCore;
using PharmaStockManager.Models.ViewModel;

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
            var today = DateTime.Today;
            var thirtyDaysFromNow = today.AddDays(30);

            // Get drugs that are expiring soon or already expired
            var expiringDrugs = await _context.Drugs
                .Where(d => d.ExpiryDate != null && d.ExpiryDate <= thirtyDaysFromNow)
                .OrderBy(d => d.ExpiryDate)
                .ToListAsync();

            // Get drugs with critical stock levels
            var criticalStockDrugs = await _context.Drugs
                .Where(d => d.Quantity <= 10)
                .OrderBy(d => d.Quantity)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                TotalDrugs = await _context.Drugs.CountAsync(),
                CriticalStockDrugs = criticalStockDrugs,
                TotalCategories = await _context.Categories.CountAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                ExpiringDrugs = expiringDrugs,
                CurrentDate = today
            };

            return View(viewModel);
        }
    }

}