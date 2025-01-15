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
    private readonly int _pageSize = 10;

    public DrugsController(PharmaContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Drugs
    public async Task<IActionResult> Index(int? page)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Json(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        var pageNumber = page ?? 1; // Eğer sayfa numarası belirtilmemişse 1. sayfa

        // Toplam ilaç sayısını al
        var totalDrugs = await _context.Drugs
            .Where(d => d.RefCode == currentUser.RefCode)
            .CountAsync();

        // İlaçları sayfalı şekilde getir
        var drugs = await _context.Drugs
            .Where(d => d.RefCode == currentUser.RefCode)
            .OrderBy(d => d.Id) // İsteğe bağlı sıralama
            .Skip((pageNumber - 1) * _pageSize)
            .Take(_pageSize)
            .ToListAsync();

        // Toplam sayfa sayısını hesapla
        var totalPages = (int)Math.Ceiling(totalDrugs / (double)_pageSize);

        // ViewBag'e sayfalama bilgilerini ekle
        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = totalPages;
        ViewBag.HasPreviousPage = pageNumber > 1;
        ViewBag.HasNextPage = pageNumber < totalPages;
        ViewBag.Categories = _context.Categories.Where(d => d.RefCode == currentUser.RefCode).ToList();

        return View(drugs);
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
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });
                }

                // Adminin RefCode'unu ilaca ekle
                drug.RefCode = currentUser.RefCode;

                _context.Add(drug);
                await _context.SaveChangesAsync();

                var transactionRecord = new Transaction
                {
                    DrugName = drug.Name,
                    Quantity = drug.Quantity,
                    TransactionType = "Stock In",
                    TransactionDate = DateTime.Now,
                    ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                    Type = drug.DrugType,
                    Price = drug.UnitPrice * drug.Quantity,
                    UserName = currentUser.FullName,
                    RefCode = currentUser.RefCode
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
                        TransactionType = quantityDifference > 0 ? "Stock Adjustment" : "Stock Decrease",
                        TransactionDate = DateTime.Now,
                        ExpiryDate = drug.ExpiryDate ?? DateTime.Now.AddYears(1),
                        Type = drug.DrugType,
                        Price = drug.UnitPrice * Math.Abs(quantityDifference),
                        UserName = (await _userManager.GetUserAsync(User)).FullName,
                        RefCode = drug.RefCode,
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
                UserName = (await _userManager.GetUserAsync(User)).FullName,
                RefCode = drug.RefCode,
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
    public async Task<IActionResult> TransactionHistory(int? page)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Json(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        var pageNumber = page ?? 1;
        var pageSize = 10; // Her sayfada gösterilecek kayıt sayısı

        var query = _context.Transactions
            .Where(t => t.RefCode == currentUser.RefCode)
            .OrderByDescending(t => t.TransactionDate);

        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        var transactions = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = totalPages;
        ViewBag.HasPreviousPage = pageNumber > 1;
        ViewBag.HasNextPage = pageNumber < totalPages;

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
                UserName = (await _userManager.GetUserAsync(User)).FullName,
                RefCode = drug.RefCode,
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

    // POST: Drugs/UpdateStock
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStock([Bind("Id,Quantity")] Drug drug)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var existingDrug = await _context.Drugs.AsNoTracking().FirstOrDefaultAsync(d => d.Id == drug.Id);
            if (existingDrug == null)
            {
                return Json(new { success = false, message = "İlaç bulunamadı." });
            }

            // Miktar değişikliğini kontrol et
            var quantityDifference = drug.Quantity - existingDrug.Quantity;
            if (quantityDifference != 0)
            {
                var transactionRecord = new Transaction
                {
                    DrugName = existingDrug.Name,
                    Quantity = Math.Abs(quantityDifference),
                    TransactionType = quantityDifference > 0 ? "Stock Increase" : "Stock Decrease",
                    TransactionDate = DateTime.Now,
                    ExpiryDate = existingDrug.ExpiryDate ?? DateTime.Now.AddYears(1),
                    Type = existingDrug.DrugType,
                    Price = existingDrug.UnitPrice * Math.Abs(quantityDifference),
                    UserName = (await _userManager.GetUserAsync(User)).FullName,
                    RefCode = existingDrug.RefCode,
                };

                _context.Transactions.Add(transactionRecord);

                // Sadece quantity'yi güncelle
                _context.Database.ExecuteSqlRaw(
                    "UPDATE Drugs SET Quantity = {0} WHERE Id = {1}",
                    drug.Quantity, drug.Id);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Stok başarıyla güncellendi." });
            }

            return Json(new { success = true, message = "Stok miktarı değişmedi." });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Json(new { success = false, message = "Güncelleme sırasında hata oluştu: " + ex.Message });
        }
    }
}