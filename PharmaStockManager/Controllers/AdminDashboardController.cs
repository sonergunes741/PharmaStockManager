using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PharmaStockManager.Models;
using PharmaStockManager.Models.Identity;
using Microsoft.EntityFrameworkCore;
using PharmaStockManager.Models.ViewModel;
using PharmaStockManager.Services;

namespace PharmaStockManager.Controllers
{
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(LogFilter))]
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
            var model = new DashboardViewModel
            {
                TotalDrugs = await _context.Drugs.CountAsync(),
                CriticalStockDrugs = await _context.Drugs
                    .Where(d => d.Quantity <= d.CriticalStockLevel)
                    .OrderBy(d => (double)d.Quantity / d.CriticalStockLevel)
                    .ToListAsync(),
                ExpiringDrugs = await _context.Drugs
                    .Where(d => d.ExpiryDate.HasValue &&
                           d.ExpiryDate.Value <= DateTime.Now.AddDays(30))
                    .OrderBy(d => d.ExpiryDate)
                    .ToListAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                CurrentDate = DateTime.Now
            };

            return View(model);
        }
    }

}