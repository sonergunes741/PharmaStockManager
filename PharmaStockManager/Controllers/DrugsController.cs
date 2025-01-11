using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaStockManager.Models;
using PharmaStockManager.Models.Identity;
using PharmaStockManager.Services;
using System;
using System.Threading.Tasks;
[Authorize(Roles = "Admin")]
[ServiceFilter(typeof(LogFilter))]
public class DrugsController : Controller
{
    private readonly PharmaContext _context;
    private readonly UserManager<AppUser> _userManager;

    public DrugsController(PharmaContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Drugs
    public async Task<IActionResult> Index()
    {
        return View(await _context.Drugs.ToListAsync());
    }

    // GET: Drugs/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return Json(new { success = false, message = "ID bulunamadı." });
        }

        var drug = await _context.Drugs
            .FirstOrDefaultAsync(m => m.Id == id);

        if (drug == null)
        {
            return Json(new { success = false, message = "İlaç bulunamadı." });
        }

        return Json(new { success = true, data = drug });
    }

    // POST: Drugs/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Category,Quantity,UnitPrice,ExpiryDate,DrugType,CriticalStockLevel,MaxRequest")] Drug drug)
    {
        if (ModelState.IsValid)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Add(drug);
                await _context.SaveChangesAsync();

                var transactionRecord = new Transaction
                {
                    DrugName = drug.Name,
                    Quantity = drug.Quantity,
                    TransactionType = "Initial Stock",
                    TransactionDate = DateTime.Now,
                    ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                    Type = drug.DrugType,
                    Price = drug.UnitPrice * drug.Quantity,
                    UserName = (await _userManager.GetUserAsync(User)).FullName
                };

                _context.Transactions.Add(transactionRecord);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "İlaç başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "İlaç eklenirken hata oluştu: " + ex.Message });
            }
        }
        return Json(new { success = false, message = "Geçersiz veri girişi." });
    }

    // POST: Drugs/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([Bind("Id,Name,Category,Quantity,UnitPrice,ExpiryDate,DrugType,CriticalStockLevel,MaxRequest")] Drug drug)
    {
        if (ModelState.IsValid)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var oldDrug = await _context.Drugs.AsNoTracking().FirstOrDefaultAsync(d => d.Id == drug.Id);
                if (oldDrug == null)
                {
                    return Json(new { success = false, message = "İlaç bulunamadı." });
                }
                // Miktar değişikliğini kontrol et
                var quantityDifference = drug.Quantity - oldDrug.Quantity;
                if (quantityDifference != 0)
                {
                    var transactionRecord = new Transaction
                    {
                        DrugName = drug.Name,
                        Quantity = Math.Abs(quantityDifference),
                        TransactionType = quantityDifference > 0 ? "Stock Increase" : "Stock Decrease",
                        TransactionDate = DateTime.Now,
                        ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                        Type = drug.DrugType,
                        Price = drug.UnitPrice * Math.Abs(quantityDifference),
                        UserName = (await _userManager.GetUserAsync(User)).FullName
                    };

                    _context.Transactions.Add(transactionRecord);
                }

                // Güncelleme işlemini yap
                _context.Update(drug);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "İlaç başarıyla güncellendi." });
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                if (!DrugExists(drug.Id))
                {
                    return Json(new { success = false, message = "İlaç bulunamadı." });
                }
                else
                {
                    return Json(new { success = false, message = "Güncelleme sırasında hata oluştu." });
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Güncelleme sırasında hata oluştu: " + ex.Message });
            }
        }
        return Json(new { success = false, message = "Geçersiz veri girişi." });
    }

    // POST: Drugs/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var drug = await _context.Drugs.FindAsync(id);
            if (drug == null)
            {
                return Json(new { success = false, message = "İlaç bulunamadı." });
            }

            var transactionRecord = new Transaction
            {
                DrugName = drug.Name,
                Quantity = drug.Quantity,
                TransactionType = "Drug Deleted",
                TransactionDate = DateTime.Now,
                ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                Type = drug.DrugType,
                Price = drug.UnitPrice * drug.Quantity,
                UserName = (await _userManager.GetUserAsync(User)).FullName
            };

            _context.Transactions.Add(transactionRecord);
            _context.Drugs.Remove(drug);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Json(new { success = true, message = "İlaç başarıyla silindi." });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Json(new { success = false, message = "Silme işlemi sırasında hata oluştu: " + ex.Message });
        }
    }

    // GET: Drugs/TransactionHistory
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> TransactionHistory()
    {
        var transactions = await _context.Transactions
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

        return View(transactions);
    }

    private bool DrugExists(int id)
    {
        return _context.Drugs.Any(e => e.Id == id);
    }

    // Helper action for stock operations
    [HttpPost]
    public async Task<IActionResult> StockOperation(int id, int quantity, string operationType)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var drug = await _context.Drugs.FindAsync(id);
            if (drug == null)
            {
                return Json(new { success = false, message = "İlaç bulunamadı." });
            }

            if (operationType == "decrease" && drug.Quantity < quantity)
            {
                return Json(new { success = false, message = "Yetersiz stok." });
            }

            drug.Quantity += operationType == "increase" ? quantity : -quantity;

            var transactionRecord = new Transaction
            {
                DrugName = drug.Name,
                Quantity = quantity,
                TransactionType = operationType == "increase" ? "Stock In" : "Stock Out",
                TransactionDate = DateTime.Now,
                ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                Type = drug.DrugType,
                Price = drug.UnitPrice * quantity,
                UserName = (await _userManager.GetUserAsync(User)).FullName
            };

            _context.Transactions.Add(transactionRecord);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Json(new { success = true, message = $"Stok {(operationType == "increase" ? "artırma" : "azaltma")} işlemi başarılı." });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Json(new { success = false, message = $"Stok işlemi sırasında hata oluştu: {ex.Message}" });
        }
    }
}