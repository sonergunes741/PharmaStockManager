using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models;
using System.Linq;

[Authorize(Roles = "Admin")] // Bu controller'a yalnızca "Admin" rolündeki kullanıcılar erişebilir.
public class AdminPanelController : Controller
{
    private readonly PharmaContext _context;

    public AdminPanelController(PharmaContext context)
    {
        _context = context;
    }

    // GET: AdminPanel/ManageRequests
    public IActionResult ManageRequests()
    {
        // Kullanıcıların yaptığı stok taleplerini alıyoruz.
        var stockRequests = _context.StockRequests
                                    .Select(r => new StockRequest
                                    {
                                        Id = r.Id,
                                        DrugId = r.DrugId,
                                        Quantity = r.Quantity,
                                        UserName = r.UserName,
                                        RequestDate = r.RequestDate,
                                        IsApproved = r.IsApproved,
                                        // DrugName yerine doğrudan ilişkili ilaç nesnesini çekiyoruz
                                        Drug = r.Drug
                                    })
                                    .ToList();

        return View(stockRequests);
    }

    // POST: AdminPanel/ApproveRequest
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApproveRequest(int requestId)
    {
        var stockRequest = _context.StockRequests.FirstOrDefault(r => r.Id == requestId);
        if (stockRequest != null)
        {
            var drug = _context.Drugs.FirstOrDefault(d => d.Id == stockRequest.DrugId);
            if (drug != null && drug.Quantity >= stockRequest.Quantity)
            {
                // Talebi onaylayarak stoktan düşüyoruz
                drug.Quantity -= stockRequest.Quantity;
                stockRequest.IsApproved = true; // Onaylandığını işaretle
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Request approved for {stockRequest.Quantity} units of {drug.Name}.";
            }
            else
            {
                TempData["ErrorMessage"] = "Insufficient stock or drug not found.";
            }
        }
        else
        {
            TempData["ErrorMessage"] = "Stock request not found.";
        }

        return RedirectToAction(nameof(ManageRequests));
    }

    // POST: AdminPanel/RejectRequest
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RejectRequest(int requestId)
    {
        var stockRequest = _context.StockRequests.FirstOrDefault(r => r.Id == requestId);
        if (stockRequest != null)
        {
            _context.StockRequests.Remove(stockRequest); // Talep reddedildiği için kaldırılıyor
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Request rejected successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Stock request not found.";
        }

        return RedirectToAction(nameof(ManageRequests));
    }
}