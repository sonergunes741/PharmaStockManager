using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
        
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Requests()
        {
            // Talepleri listeleme
            var requests = _context.Requests
                                   .Select(r => new
                                   {
                                       r.Id,
                                       DrugName = _context.Drugs.FirstOrDefault(d => d.Id == r.DrugId).Name,
                                       r.Quantity,
                                       r.UserName,
                                       r.RequestDate,
                                       r.IsApproved,
                                       r.IsRejected
                                   })
                                   .ToList();

            return View("~/Views/Request/Requests.cshtml", requests); // Talepler Request.cshtml adlı bir view'de gösterilir
        }
    }
}
