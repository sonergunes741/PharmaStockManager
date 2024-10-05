using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PharmaStockManager.Models;

namespace PharmaStockManager.Controllers
{
    [Authorize(Roles = "Admin")] // Only allow access to users with the "Admin" role
    public class DrugsController : Controller
    {
        private readonly PharmaContext _context;

        public DrugsController(PharmaContext context)
        {
            _context = context;
        }

        // GET: Drugs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Drugs.ToListAsync());
        }

        // GET: Drugs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drug = await _context.Drugs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drug == null)
            {
                return NotFound();
            }

            return View(drug);
        }

        // GET: Drugs/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Name", "Name");
            return View();
        }

        // POST: Drugs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Category,Quantity,UnitPrice")] Drug drug)
        {
            if (ModelState.IsValid)
            {
                _context.Add(drug);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Drug created successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Failed to create drug.";
            ViewBag.Categories = new SelectList(_context.Categories, "Name", "Name", drug.Category);
            return View(drug);
        }

        // GET: Drugs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drug = await _context.Drugs.FindAsync(id);
            if (drug == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Name", "Name", drug.Category);
            return View(drug);
        }

        // POST: Drugs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,Quantity,UnitPrice")] Drug drug)
        {
            if (id != drug.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(drug);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Drug updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DrugExists(drug.Id))
                    {
                        TempData["ErrorMessage"] = "Drug not found.";
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to update drug.";
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Invalid input. Failed to update drug.";
            ViewBag.Categories = new SelectList(_context.Categories, "Name", "Name", drug.Category);
            return View(drug);
        }

        // POST: StockIn
        [HttpPost]
        public async Task<IActionResult> StockIn(int id, int quantity)
        {
            var drug = await _context.Drugs.FindAsync(id);
            if (drug != null)
            {
                drug.Quantity += quantity;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"{quantity} units added to {drug.Name}.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add stock. Drug not found.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: StockOut
        [HttpPost]
        public async Task<IActionResult> StockOut(int id, int quantity)
        {
            var drug = await _context.Drugs.FindAsync(id);
            if (drug != null && drug.Quantity >= quantity)
            {
                drug.Quantity -= quantity;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"{quantity} units removed from {drug.Name}.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Cannot remove {quantity} units from {drug?.Name}. Not enough stock or drug not found.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Drugs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drug = await _context.Drugs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drug == null)
            {
                return NotFound();
            }

            return View(drug);
        }

        // POST: Drugs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drug = await _context.Drugs.FindAsync(id);
            if (drug != null)
            {
                _context.Drugs.Remove(drug);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Drug deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete drug. Drug not found.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DrugExists(int id)
        {
            return _context.Drugs.Any(e => e.Id == id);
        }
    }
}
