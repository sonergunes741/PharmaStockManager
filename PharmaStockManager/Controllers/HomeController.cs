using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models;
using System.Diagnostics;
using System.Linq;

namespace PharmaStockManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly PharmaContext _context;

        public HomeController(PharmaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new
            {
                Drugs = _context.Drugs.ToList(),
                Categories = _context.Categories.ToList()
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
