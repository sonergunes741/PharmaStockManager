using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    public IActionResult Index()
    {
        var requests = _context.Requests
            .Select(r => new RequestViewModel
            {
                Id = r.Id,
                DrugName = _context.Drugs.FirstOrDefault(d => d.Id == r.DrugId).Name,
                Quantity = r.Quantity,
                UserName = r.UserName,
                RequestDate = r.RequestDate,
                IsApproved = r.IsApproved,
                IsRejected = r.IsRejected
            })
            .ToList();
        return View("~/Views/Request/Requests.cshtml", requests);
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id, string drugName, int quantity)
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            // İsteği bul
            var request = _context.Requests.FirstOrDefault(r => r.Id == id);
            if (request == null || request.IsApproved)
            {
                return Json(new { success = false, message = "Request not found or already approved." });
            }

            // İlgili ilaç stoğunu bul ve güncelle
            var drug = _context.Drugs.FirstOrDefault(d => d.Name == drugName);
            if (drug == null || drug.Quantity < quantity)
            {
                return Json(new { success = false, message = "Insufficient stock or drug not found." });
            }

            // Stoktan düş
            drug.Quantity -= quantity;
            request.IsApproved = true;

            // Transaction kaydı oluştur
            // Controllers/RequestController.cs
            var transactionRecord = new Transaction
            {
                DrugName = drug.Name,
                Quantity = quantity,
                TransactionType = "Request Approved",
                TransactionDate = DateTime.Now,
                ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1), // Null ise 1 yıl sonrasını kullan
                Type = drug.DrugType,
                Price = drug.UnitPrice * quantity,
                UserName = (await _userManager.GetUserAsync(User)).FullName
            };

            // Transaction'ı kaydet
            _context.Transactions.Add(transactionRecord);

            // Veritabanını güncelle
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

    [HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        var request = _context.Requests.FirstOrDefault(r => r.Id == id);
        if (request == null || request.IsApproved || request.IsRejected)
        {
            return Json(new { success = false, message = "Request not found or already processed." });
        }

        try
        {
            // İlgili ilaç bilgisini al
            var drug = _context.Drugs.FirstOrDefault(d => d.Id == request.DrugId);
            if (drug != null)
            {
                // Transaction kaydı oluştur
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