using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using PharmaStockManager.Models;
using PharmaStockManager.Models.Identity;
using PharmaStockManager.Models.ViewModels;
using PharmaStockManager.Services;

namespace PharmaStockManager.Controllers
{
    [Authorize(Roles = "Employee")]
    [ServiceFilter(typeof(LogFilter))]
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
            var permissions = await _context.Permissions
                .FirstOrDefaultAsync(p => p.UserID == user.Id);

            ViewData["Permissions"] = permissions ?? new Permissions();
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
            var user = await _userManager.GetUserAsync(User);

            var drugs = await _context.Drugs
                .Where(d => d.RefCode == user.RefCode)
                .ToListAsync();

            var permissions = await _context.Permissions
                .FirstOrDefaultAsync(p => p.UserID == user.Id);

            ViewData["Permissions"] = permissions ?? new Permissions();
            return View(drugs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStock(int id, int quantity)
        {
            if (!await CheckPermission("stockin"))
                return Json(new { success = false, message = "You don't have permission to add stock." });

            var drug = await _context.Drugs.FindAsync(id);
            if (drug == null)
                return Json(new { success = false, message = "İlaç bulunamadı." });

            drug.Quantity += quantity;
            await _context.SaveChangesAsync();

            // Create transaction record
            var transaction = new Transaction
            {
                DrugName = drug.Name,
                TransactionType = "Add",
                Quantity = quantity,
                TransactionDate = DateTime.Now,
                ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                Type = drug.DrugType,
                Price = drug.UnitPrice * drug.Quantity,
                UserName = (await _userManager.GetUserAsync(User)).FullName,
                RefCode = drug.RefCode,
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStock(int id, int quantity)
        {
            if (!await CheckPermission("stockout"))
                return Json(new { success = false, message = "You don't have permission to remove stock." });


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
                DrugName = drug.Name,
                TransactionType = "Remove",
                Quantity = quantity,
                TransactionDate = DateTime.Now,
                ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                Type = drug.DrugType,
                Price = drug.UnitPrice * drug.Quantity,
                UserName = (await _userManager.GetUserAsync(User)).FullName,
                RefCode = drug.RefCode,
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

        private async Task<bool> CheckPermission(string permission)
        {
            var user = await _userManager.GetUserAsync(User);
            var userPermissions = await _context.Permissions
                .FirstOrDefaultAsync(p => p.UserID == user.Id);

            if (userPermissions == null)
                return false;

            switch (permission.ToLower())
            {
                case "editstocks":
                    return userPermissions.EditStocks;
                case "stockin":
                    return userPermissions.StockIn;
                case "stockout":
                    return userPermissions.StockOut;
                case "requestapprove":
                    return userPermissions.RequestApprove;
                default:
                    return false;
            }
        }

        public async Task<IActionResult> Requests(
            string search,
            int? minQuantity,
            int? maxQuantity,
            string status,
            string sortOption,
            int page = 1,
            int pageSize = 5)
        {
            if (!await CheckPermission("requestapprove"))
                return RedirectToAction("Index");

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return NotFound();

            // Base query
            var requestsQuery = _context.Requests
                .Where(r => _context.Drugs.Any(d => d.Id == r.DrugId && d.RefCode == currentUser.RefCode))
                .Select(r => new RequestViewModel
                {
                    Id = r.Id,
                    DrugName = _context.Drugs.FirstOrDefault(d => d.Id == r.DrugId).Name,
                    Quantity = r.Quantity,
                    UserName = r.UserName,
                    RequestDate = r.RequestDate,
                    IsApproved = r.IsApproved,
                    IsRejected = r.IsRejected
                });

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                requestsQuery = requestsQuery.Where(r =>
                    r.DrugName.ToLower().Contains(search) ||
                    r.UserName.ToLower().Contains(search));
            }

            if (minQuantity.HasValue)
                requestsQuery = requestsQuery.Where(r => r.Quantity >= minQuantity.Value);

            if (maxQuantity.HasValue)
                requestsQuery = requestsQuery.Where(r => r.Quantity <= maxQuantity.Value);

            if (!string.IsNullOrEmpty(status))
            {
                requestsQuery = status.ToLower() switch
                {
                    "pending" => requestsQuery.Where(r => !r.IsApproved && !r.IsRejected),
                    "approved" => requestsQuery.Where(r => r.IsApproved),
                    "rejected" => requestsQuery.Where(r => r.IsRejected),
                    _ => requestsQuery
                };
            }

            // Apply sorting
            requestsQuery = sortOption switch
            {
                "name-asc" => requestsQuery.OrderBy(r => r.DrugName),
                "name-desc" => requestsQuery.OrderByDescending(r => r.DrugName),
                "quantity-asc" => requestsQuery.OrderBy(r => r.Quantity),
                "quantity-desc" => requestsQuery.OrderByDescending(r => r.Quantity),
                "date-asc" => requestsQuery.OrderBy(r => r.RequestDate),
                "date-desc" => requestsQuery.OrderByDescending(r => r.RequestDate),
                _ => requestsQuery.OrderByDescending(r => r.RequestDate)
            };

            // Pagination
            int totalItems = await requestsQuery.CountAsync();
            var requests = await requestsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRequest(int id, string drugName, int quantity)
        {
            if (!await CheckPermission("requestapprove"))
                return Json(new { success = false, message = "You don't have permission to approve requests." });

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                    return Json(new { success = false, message = "User not found." });

                var request = await _context.Requests
                    .FirstOrDefaultAsync(r => r.Id == id &&
                        _context.Drugs.Any(d => d.Name == drugName && d.RefCode == currentUser.RefCode));

                if (request == null || request.IsApproved)
                    return Json(new { success = false, message = "Request not found or already processed." });

                var drug = await _context.Drugs
                    .FirstOrDefaultAsync(d => d.Name == drugName && d.RefCode == currentUser.RefCode);

                if (drug == null || drug.Quantity < quantity)
                    return Json(new { success = false, message = "Insufficient stock or drug not found." });

                drug.Quantity -= quantity;
                request.IsApproved = true;

                var transactions = new Transaction
                {
                    DrugName = drug.Name,
                    Quantity = quantity,
                    TransactionType = "Request Approved",
                    TransactionDate = DateTime.Now,
                    ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                    Type = drug.DrugType,
                    Price = drug.UnitPrice * quantity,
                    UserName = currentUser.FullName,
                    RefCode = currentUser.RefCode
                };

                _context.Transactions.Add(transactions);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "An error occurred while processing the request." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(int id)
        {
            if (!await CheckPermission("requestapprove"))
                return Json(new { success = false, message = "You don't have permission to reject requests." });

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Json(new { success = false, message = "User not found." });

            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.Id == id &&
                    _context.Drugs.Any(d => d.Id == r.DrugId && d.RefCode == currentUser.RefCode));

            if (request == null || request.IsApproved || request.IsRejected)
                return Json(new { success = false, message = "Request not found or already processed." });

            try
            {
                var drug = await _context.Drugs
                    .FirstOrDefaultAsync(d => d.Id == request.DrugId && d.RefCode == currentUser.RefCode);

                if (drug != null)
                {
                    var transaction = new Transaction
                    {
                        DrugName = drug.Name,
                        Quantity = request.Quantity,
                        TransactionType = "Request Rejected",
                        TransactionDate = DateTime.Now,
                        ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                        Type = drug.DrugType,
                        Price = drug.UnitPrice * request.Quantity,
                        UserName = currentUser.FullName,
                        RefCode = currentUser.RefCode
                    };

                    _context.Transactions.Add(transaction);
                }

                request.IsRejected = true;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while rejecting the request." });
            }
        }

    }
}