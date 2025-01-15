using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaStockManager.Models;
using PharmaStockManager.Models.Identity;
using PharmaStockManager.Models.ViewModels;
using PharmaStockManager.Services;

[Authorize(Roles = "Admin")]
[ServiceFilter(typeof(LogFilter))]
public class RequestController : Controller
{
    private readonly PharmaContext _context;
    private readonly UserManager<AppUser> _userManager;

    public RequestController(PharmaContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Request/Index
    public async Task<IActionResult> Index(
    string search,
    int? minQuantity,
    int? maxQuantity,
    string status,
    string sortOption,
    int page = 1,
    int pageSize = 1)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Json(new { success = false, message = "User not found." });
        }

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

        // Apply filters (same as before)
        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            requestsQuery = requestsQuery.Where(r => r.DrugName.ToLower().Contains(search) || r.UserName.ToLower().Contains(search));
        }
        if (minQuantity.HasValue) requestsQuery = requestsQuery.Where(r => r.Quantity >= minQuantity.Value);
        if (maxQuantity.HasValue) requestsQuery = requestsQuery.Where(r => r.Quantity <= maxQuantity.Value);
        if (!string.IsNullOrEmpty(status))
        {
            status = status.ToLower();
            requestsQuery = status switch
            {
                "pending" => requestsQuery.Where(r => !r.IsApproved && !r.IsRejected),
                "in-stock" => requestsQuery.Where(r => r.IsApproved),
                "out-of-stock" => requestsQuery.Where(r => r.IsRejected),
                _ => requestsQuery
            };
        }
        requestsQuery = sortOption switch
        {
            "name-asc" => requestsQuery.OrderBy(r => r.DrugName),
            "name-desc" => requestsQuery.OrderByDescending(r => r.DrugName),
            "quantity-asc" => requestsQuery.OrderBy(r => r.Quantity),
            "quantity-dsc" => requestsQuery.OrderByDescending(r => r.Quantity),
            "request-asc" => requestsQuery.OrderBy(r => r.RequestDate),
            "request-dsc" => requestsQuery.OrderByDescending(r => r.RequestDate),
            _ => requestsQuery
        };

        // Paging logic
        int totalItems = await requestsQuery.CountAsync();
        var requests = await requestsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return View("~/Views/Request/Requests.cshtml", requests);
    }



    // POST: Request/Approve
    [HttpPost]
    public async Task<IActionResult> Approve(int id, string drugName, int quantity)
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Find the request and ensure the drug belongs to the logged-in user's RefCode
            var request = _context.Requests.FirstOrDefault(r => r.Id == id && _context.Drugs.Any(d => d.Name == drugName && d.RefCode == currentUser.RefCode));
            if (request == null || request.IsApproved)
            {
                return Json(new { success = false, message = "Request not found or already approved." });
            }

            var drug = _context.Drugs.FirstOrDefault(d => d.Name == drugName && d.RefCode == currentUser.RefCode);
            if (drug == null || drug.Quantity < quantity)
            {
                return Json(new { success = false, message = "Insufficient stock or drug not found." });
            }

            // Reduce stock
            drug.Quantity -= quantity;
            request.IsApproved = true;

            // Create transaction record
            var transactionRecord = new Transaction
            {
                DrugName = drug.Name,
                Quantity = quantity,
                TransactionType = "Request Approved",
                TransactionDate = DateTime.Now,
                ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                Type = drug.DrugType,
                Price = drug.UnitPrice * quantity,
                UserName = (await _userManager.GetUserAsync(User)).FullName
            };

            _context.Transactions.Add(transactionRecord);
            _context.SaveChanges();
            transaction.Commit();

            return Json(new { success = true });
        }
        catch (Exception)
        {
            transaction.Rollback();
            return Json(new { success = false, message = "An error occurred while processing the request." });
        }
    }

    // POST: Request/Reject
    [HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Json(new { success = false, message = "User not found." });
        }

        // Find the request and ensure the drug belongs to the logged-in user's RefCode
        var request = _context.Requests.FirstOrDefault(r => r.Id == id && _context.Drugs.Any(d => d.Id == r.DrugId && d.RefCode == currentUser.RefCode));
        if (request == null || request.IsApproved || request.IsRejected)
        {
            return Json(new { success = false, message = "Request not found or already processed." });
        }

        try
        {
            var drug = _context.Drugs.FirstOrDefault(d => d.Id == request.DrugId && d.RefCode == currentUser.RefCode);
            if (drug != null)
            {
                // Create transaction record for rejected request
                var transactionRecord = new Transaction
                {
                    DrugName = drug.Name,
                    Quantity = request.Quantity,
                    TransactionType = "Request Rejected",
                    TransactionDate = DateTime.Now,
                    ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                    Type = drug.DrugType,
                    Price = drug.UnitPrice * request.Quantity,
                    UserName = (await _userManager.GetUserAsync(User)).FullName
                };

                _context.Transactions.Add(transactionRecord);
            }

            request.IsRejected = true;
            _context.SaveChanges();
            return Json(new { success = true });
        }
        catch (Exception)
        {
            return Json(new { success = false, message = "An error occurred while rejecting the request." });
        }
    }
}
