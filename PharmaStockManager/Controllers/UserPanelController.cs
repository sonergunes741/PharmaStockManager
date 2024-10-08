using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models;
using Microsoft.Extensions.Logging; // Loglama için gerekli
using System;
using System.Linq;

[Authorize(Roles = "User")] // Bu controller'a yalnızca "User" rolündeki kullanıcılar erişebilir.
public class UserPanelController : Controller
{
    private readonly PharmaContext _context;
    private readonly ILogger<UserPanelController> _logger; // Logger

    public UserPanelController(PharmaContext context, ILogger<UserPanelController> logger)
    {
        _context = context;
        _logger = logger; // Logger'ı yapılandırıyoruz
    }

    // GET: UserPanel/Index
    public IActionResult Index()
    {
        // Kullanıcıya özel veri işlemleri burada yapılabilir
        var model = new
        {
            Drugs = _context.Drugs.ToList(),
            Categories = _context.Categories.ToList()
        };
        return View(model);
    }

    // GET: UserPanel/ViewDrugs
    public IActionResult ViewDrugs()
    {
        var drugs = _context.Drugs.ToList();
        return View(drugs);
    }

    // GET: UserPanel/ViewCategories
    public IActionResult ViewCategories()
    {
        var categories = _context.Categories.ToList();
        return View(categories);
    }

    // POST: UserPanel/RequestStock
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RequestStock(int drugId, int quantity)
    {
        if (quantity <= 0)
        {
            TempData["ErrorMessage"] = "Quantity must be a positive number.";
            return RedirectToAction(nameof(Index));
        }

        var drug = _context.Drugs.FirstOrDefault(d => d.Id == drugId);
        if (drug != null)
        {
            // Stok kontrolü: Kullanıcının talep ettiği miktarın mevcut stoku aşmamasını kontrol ediyoruz.
            if (quantity > drug.Quantity)
            {
                TempData["ErrorMessage"] = $"Requested quantity exceeds available stock for {drug.Name}.";
            }
            else
            {
                TempData["SuccessMessage"] = $"You have requested {quantity} units of {drug.Name}.";

                // Stok miktarını güncelle:
                drug.Quantity -= quantity;

                // Stok talebini kaydet:
                var stockRequest = new StockRequest
                {
                    DrugId = drugId,
                    UserId = User.Identity.Name,
                    QuantityRequested = quantity,
                    IsApproved = false,
                    IsRejected = false,
                    RequestDate = DateTime.Now
                };
                _context.StockRequests.Add(stockRequest);

                _context.SaveChanges();

                // İşlemi logluyoruz
                _logger.LogInformation("User {User} requested {Quantity} units of {Drug} on {Date}.",
                                        User.Identity.Name, quantity, drug.Name, DateTime.Now);
            }
        }
        else
        {
            TempData["ErrorMessage"] = "Requested drug does not exist.";
        }

        return RedirectToAction(nameof(Index));
    }
}
