using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaStockManager.Models;
using PharmaStockManager.Models.Identity;

namespace PharmaStockManager.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly PharmaContext _context;

        private readonly UserManager<AppUser> _userManager;

        public EmployeeController(PharmaContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string fullName, string email, string phoneNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not found." });

            user.FullName = fullName;
            user.Email = email;
            user.PhoneNumber = phoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Json(new { success = true });

            return Json(new { success = false, message = "Update failed." });
        }

        public async Task<IActionResult> Index()
        {
            var drugs = await _context.Drugs.ToListAsync();
            return View(drugs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStock(int id, int quantity)
        {
            var drug = await _context.Drugs.FindAsync(id);
            if (drug == null)
                return Json(new { success = false, message = "İlaç bulunamadı." });

            drug.Quantity += quantity;
            await _context.SaveChangesAsync();

            // Create transaction record
            var transaction = new Transaction
            {
                DrugId = drug.Id,
                TransactionType = "Add",
                Quantity = quantity,
                TransactionDate = DateTime.Now,
                ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                Type = drug.DrugType,
                Price = drug.UnitPrice * drug.Quantity
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStock(int id, int quantity)
        {
            var drug = await _context.Drugs.FindAsync(id);
            if (drug == null)
                return Json(new { success = false, message = "İlaç bulunamadı." });

            if (drug.Quantity < quantity)
                return Json(new { success = false, message = "Yetersiz stok." });

            drug.Quantity -= quantity;
            await _context.SaveChangesAsync();

            // Create transaction record
            var transaction = new Transaction
            {
                DrugId = drug.Id,
                TransactionType = "Remove",
                Quantity = quantity,
                TransactionDate = DateTime.Now,
                ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                Type = drug.DrugType,
                Price = drug.UnitPrice * drug.Quantity
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetDrugDetails(int id)
        {
            var drug = await _context.Drugs.FindAsync(id);
            if (drug == null)
                return Json(new { success = false, message = "İlaç bulunamadı." });

            return Json(new { success = true, data = drug });
        }
    }
}