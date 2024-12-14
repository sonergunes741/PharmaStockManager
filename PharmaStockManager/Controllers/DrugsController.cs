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
    [Authorize(Roles = "Admin")]
    public class DrugsController : Controller
    {
        private readonly PharmaContext _context;

        public DrugsController(PharmaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = _context.Categories.Select(c => c.Name).ToList();
            return View(await _context.Drugs.ToListAsync());
        }

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

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Category,Quantity,UnitPrice,ExpiryDate,DrugType")] Drug drug)
        {
            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Add(drug);
                    await _context.SaveChangesAsync();

                    // Yeni ilaç eklendiğinde transaction kaydı oluştur
                    var transactionRecord = new Transaction
                    {
                        DrugId = drug.Id,
                        Quantity = drug.Quantity,
                        TransactionType = "Initial Stock",
                        TransactionDate = DateTime.Now,
                        ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                        Type = drug.DrugType,
                        Price = drug.UnitPrice * drug.Quantity
                    };

                    _context.Transactions.Add(transactionRecord);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Drug created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "Failed to create drug.";
                }
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Name", "Name", drug.Category);
            return View(drug);
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,Quantity,UnitPrice,ExpiryDate,DrugType")] Drug drug)
        {
            if (id != drug.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var oldDrug = await _context.Drugs.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
                    if (oldDrug != null && oldDrug.Quantity != drug.Quantity)
                    {
                        // Stok değişimi varsa transaction kaydı oluştur
                        var quantityDifference = drug.Quantity - oldDrug.Quantity;
                        var transactionRecord = new Transaction
                        {
                            DrugId = drug.Id,
                            Quantity = Math.Abs(quantityDifference),
                            TransactionType = quantityDifference > 0 ? "Stock Adjustment (+" : "Stock Adjustment (-",
                            TransactionDate = DateTime.Now,
                            ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                            Type = drug.DrugType,
                            Price = drug.UnitPrice * Math.Abs(quantityDifference)
                        };

                        _context.Transactions.Add(transactionRecord);
                    }

                    _context.Update(drug);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Drug updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();
                    if (!DrugExists(drug.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewBag.Categories = new SelectList(_context.Categories, "Name", "Name", drug.Category);
            return View(drug);
        }

        [HttpPost]
        public async Task<IActionResult> StockIn(int id, int quantity, DateTime expiryDate, string type, decimal price)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var drug = await _context.Drugs.FindAsync(id);
                if (drug != null)
                {
                    drug.Quantity += quantity;

                    var transactionRecord = new Transaction
                    {
                        DrugId = drug.Id,
                        Quantity = quantity,
                        TransactionType = "Stock In",
                        TransactionDate = DateTime.Now,
                        ExpiryDate = expiryDate,
                        Type = type,
                        Price = price * quantity
                    };

                    _context.Transactions.Add(transactionRecord);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = $"{quantity} units added to {drug.Name}.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to add stock. Drug not found.";
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "An error occurred while processing the stock in.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> StockOut(int id, int quantity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var drug = await _context.Drugs.FindAsync(id);
                if (drug != null && drug.Quantity >= quantity)
                {
                    drug.Quantity -= quantity;

                    var transactionRecord = new Transaction
                    {
                        DrugId = drug.Id,
                        Quantity = quantity,
                        TransactionType = "Stock Out",
                        TransactionDate = DateTime.Now,
                        ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                        Type = drug.DrugType,
                        Price = drug.UnitPrice * quantity
                    };

                    _context.Transactions.Add(transactionRecord);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = $"{quantity} units removed from {drug.Name}.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Insufficient stock or drug not found.";
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "An error occurred while processing the stock out.";
            }
            return RedirectToAction(nameof(Index));
        }

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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var drug = await _context.Drugs.FindAsync(id);
                if (drug != null)
                {
                    var transactionRecord = new Transaction
                    {
                        DrugId = drug.Id,
                        Quantity = drug.Quantity,
                        TransactionType = "Drug Deleted",
                        TransactionDate = DateTime.Now,
                        ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                        Type = drug.DrugType,
                        Price = drug.UnitPrice * drug.Quantity
                    };

                    _context.Transactions.Add(transactionRecord);
                    _context.Drugs.Remove(drug);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Drug deleted successfully.";
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Failed to delete drug.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TransactionHistory()
        {
            var transactions = await _context.Transactions
                .Include(t => t.Drug)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return View(transactions);
        }

        private bool DrugExists(int id)
        {
            return _context.Drugs.Any(e => e.Id == id);
        }
    }
}