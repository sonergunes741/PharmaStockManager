using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaStockManager.Models;

namespace PharmaStockManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DeposController:Controller
    {
        private readonly PharmaContext _context;
        public DeposController(PharmaContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Depos.ToListAsync());
        }
    }

}
