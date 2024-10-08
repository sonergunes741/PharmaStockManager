using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models;
using System.Linq;

[Authorize(Roles = "Admin")]
public class AdminDashboardController : Controller
{
    private readonly PharmaContext _context;

    public AdminDashboardController(PharmaContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var model = new
        {
            TotalDrugs = _context.Drugs.Count(),
            CriticalStockDrugs = _context.Drugs.Where(d => d.Quantity < 10).ToList(),
            TotalCategories = _context.Categories.Count(),
            TotalUsers = _context.Users.Count()
        };
        return View(model);
    }
}
