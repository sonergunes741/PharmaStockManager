using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models;
using System.Linq;

[Authorize(Roles = "User")] // Bu controller'a yalnızca "User" rolündeki kullanıcılar erişebilir.
public class UserPanelController : Controller
{
    private readonly PharmaContext _context;

    public UserPanelController(PharmaContext context)
    {
        _context = context;
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
        var drug = _context.Drugs.FirstOrDefault(d => d.Id == drugId);
        if (drug != null)
        {
            TempData["SuccessMessage"] = $"You have requested {quantity} units of {drug.Name}.";
        }
        else
        {
            TempData["ErrorMessage"] = "Requested drug does not exist.";
        }

        return RedirectToAction(nameof(Index));
    }
}
