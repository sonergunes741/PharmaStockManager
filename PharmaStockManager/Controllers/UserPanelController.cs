using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models;
using Microsoft.Extensions.Logging; // Loglama için gerekli
using System.Linq;
using PharmaStockManager.Models.ViewModels;
using PharmaStockManager.Models.ViewModel;
using PharmaStockManager.Services;
using Microsoft.AspNetCore.Identity;
using PharmaStockManager.Models.Identity;

[Authorize(Roles = "Customer")] // This controller can only be accessed by "Customer" role users.
[ServiceFilter(typeof(LogFilter))]
public class UserPanelController : Controller
{
    private readonly PharmaContext _context;
    private readonly ILogger<UserPanelController> _logger; // Logger
    private readonly UserManager<AppUser> _userManager;
    public int MAX_ITEM = 50; // Maximum number of medicines a user can select.

    public UserPanelController(PharmaContext context, ILogger<UserPanelController> logger, UserManager<AppUser> userManager)
    {
        _context = context;
        _logger = logger; // Configure Logger
        _userManager = userManager;
    }

    // GET: UserPanel/Index
    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Json(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        // Fetch drugs and categories based on the user's RefCode
        var model = new
        {
            Drugs = _context.Drugs.Where(d => d.RefCode == currentUser.RefCode).ToList(),
            Categories = _context.Categories.Where(d => d.RefCode == currentUser.RefCode).ToList()
        };

        return View(model);
    }

    // GET: UserPanel/ViewDrugs
    public async Task<IActionResult> ViewDrugs()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Json(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        var drugs = _context.Drugs.Where(d => d.RefCode == currentUser.RefCode).ToList();
        return View(drugs);
    }

    // GET: UserPanel/ViewCategories
    public async Task<IActionResult> ViewCategories()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Json(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        var categories = _context.Categories.Where(c => c.RefCode == currentUser.RefCode).ToList();
        return View(categories);
    }

    // POST: UserPanel/RequestStock
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RequestStock(int drugId, int quantity)
    {
        if (quantity <= 0)
        {
            TempData["ErrorMessage"] = "Girilen Sayı 0 dan büyük olmalı.";
            return RedirectToAction(nameof(Index));
        }

        var currentUser = _userManager.GetUserAsync(User).Result;
        var drug = _context.Drugs.FirstOrDefault(d => d.Id == drugId && d.RefCode == currentUser.RefCode);

        if (drug != null)
        {
            // MaxRequest control
            if (quantity > drug.MaxRequest)
            {
                TempData["ErrorMessage"] = $"En fazla {drug.MaxRequest} adet ilaç talep edilebilir.";
                return RedirectToAction(nameof(Index));
            }

            // Create request
            var request = new Request
            {
                UserName = currentUser.UserName,
                DrugId = drug.Id,
                Quantity = quantity,
                RequestDate = DateTime.Now,
                IsApproved = false,
                RefCode = currentUser.RefCode,
            };

            _context.Requests.Add(request);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $" {quantity} adet {drug.Name} istek gönderildi.";
        }
        else
        {
            TempData["ErrorMessage"] = "Böyle bir ilaç yok.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> FilterMedicines(string search, decimal? minPrice, decimal? maxPrice, string stockStatus, string sortOption)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Json(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        var query = _context.Drugs.Where(m => m.RefCode == currentUser.RefCode).AsQueryable();

        // Search filter
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(m => m.Name.Contains(search) || m.Category.Contains(search));
        }

        // Price filters
        if (minPrice.HasValue)
        {
            query = query.Where(m => m.UnitPrice >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(m => m.UnitPrice <= maxPrice.Value);
        }

        // Stock status
        if (stockStatus == "in-stock")
        {
            query = query.Where(m => m.Quantity > 0);
        }
        else if (stockStatus == "out-of-stock")
        {
            query = query.Where(m => m.Quantity == 0);
        }

        // Sorting
        query = sortOption switch
        {
            "name-asc" => query.OrderBy(m => m.Name),
            "name-desc" => query.OrderByDescending(m => m.Name),
            "price-asc" => query.OrderBy(m => m.UnitPrice),
            "price-desc" => query.OrderByDescending(m => m.UnitPrice),
            "category-asc" => query.OrderBy(m => m.Category),
            "category-desc" => query.OrderByDescending(m => m.Category),
            _ => query
        };

        var filteredMedicines = query.Select(m => new
        {
            m.Id,
            m.Name,
            m.Category,
            m.UnitPrice,
            m.Quantity
        }).ToList();

        return Json(filteredMedicines);
    }

    // GET: UserPanel/ViewOrders
    public async Task<IActionResult> ViewOrders()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Json(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        var userOrders = _context.Requests
            .Where(r => r.UserName == User.Identity.Name)
            .Select(r => new RequestViewModel
            {
                Id = r.Id,
                DrugName = _context.Drugs.FirstOrDefault(d => d.Id == r.DrugId).Name,
                Quantity = r.Quantity,
                RequestDate = r.RequestDate,
                IsApproved = r.IsApproved,
                IsRejected = r.IsRejected
            })
            .ToList();

        return View(userOrders);
    }

    // My Profile
    public async Task<IActionResult> MyProfile()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return NotFound();
        }

        var model = new UserManagementViewModel
        {
            UserName = currentUser.FullName,
            Email = currentUser.Email,
            ActiveUser = currentUser.ActiveUser,
            Role = currentUser.UserType,
            PhoneNumber = currentUser.PhoneNumber,
            RefCode = currentUser.RefCode
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(string fullName, string email, string phoneNumber)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Json(new { success = false, message = "Kullanıcı bulunamadı." });

        user.FullName = fullName;
        user.Email = email;
        user.PhoneNumber = phoneNumber;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
            return Json(new { success = true });

        return Json(new { success = false, message = "Güncelleme başarısız." });
    }
}
