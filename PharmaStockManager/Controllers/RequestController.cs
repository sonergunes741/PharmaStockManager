using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models;

[Authorize(Roles = "Admin")]
public class RequestController : Controller
{
    private readonly PharmaContext _context;

    public RequestController(PharmaContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
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

        return View("~/Views/Request/Requests.cshtml", requests);// Bu durumda /Views/Request/Requests.cshtml kullanılacaktır
    }
    [HttpPost]
    public IActionResult Approve(int id, string drugName, int quantity)
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

        // Veritabanını güncelle
        _context.SaveChanges();

        return Json(new { success = true });
    }
    [HttpPost]
    public IActionResult Reject(int id)
    {
        var request = _context.Requests.FirstOrDefault(r => r.Id == id);
        if (request == null || request.IsApproved || request.IsRejected)
        {
            return Json(new { success = false, message = "Request not found or already processed." });
        }

        request.IsRejected = true;

        _context.SaveChanges();

        return Json(new { success = true });
    }


}
