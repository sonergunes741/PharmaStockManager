using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models;
using Microsoft.Extensions.Logging; // Loglama için gerekli
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
            var request = new Request
            {
                UserName = User.Identity.Name,
                DrugId = drug.Id,
                Quantity = quantity,
                RequestDate = DateTime.Now,
                IsApproved = false
            };

            _context.Requests.Add(request);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Your request for {quantity} units of {drug.Name} has been submitted.";
        }
        else
        {
            TempData["ErrorMessage"] = "Requested drug does not exist.";
        }

        return RedirectToAction(nameof(Index));
    }
}
