using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

[Authorize(Roles = "Admin")]
public class AdminPanelController : Controller
{
    private readonly PharmaContext _context;

    public AdminPanelController(PharmaContext context)
    {
        _context = context;
    }

    // Tüm stok taleplerini listelemek için
    public IActionResult StockRequests()
    {
        // Stok talepleri ile ilgili ilaç bilgilerini de dahil etmek için Include kullanıyoruz
        var requests = _context.StockRequests
                               .Include(sr => sr.Drug) // Drug ile ilişkiyi ekliyoruz
                               .ToList();
        return View(requests);
    }

    // Stok talebini onaylamak için
    [HttpPost]
    public IActionResult ApproveRequest(int requestId)
    {
        var request = _context.StockRequests.FirstOrDefault(r => r.Id == requestId);
        if (request != null)
        {
            request.IsApproved = true;
            request.IsRejected = false; // Onaylanan talepler için reddetmeyi iptal ediyoruz
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(StockRequests));
    }

    // Stok talebini reddetmek için
    [HttpPost]
    public IActionResult RejectRequest(int requestId)
    {
        var request = _context.StockRequests.FirstOrDefault(r => r.Id == requestId);
        if (request != null)
        {
            request.IsRejected = true;
            request.IsApproved = false; // Reddedilen talepler için onaylamayı iptal ediyoruz
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(StockRequests));
    }
}
