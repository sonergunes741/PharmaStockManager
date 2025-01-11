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

[Authorize(Roles = "Customer")] // Bu controller'a yalnızca "Customer" rolündeki kullanıcılar erişebilir.
[ServiceFilter(typeof(LogFilter))]
public class UserPanelController : Controller
{
    private readonly PharmaContext _context;
    private readonly ILogger<UserPanelController> _logger; // Logger
    private readonly UserManager<AppUser> _userManager;
    public int MAX_ITEM = 50;// Maksimum ne kadar ilaç seçebilir?
    public UserPanelController(PharmaContext context, ILogger<UserPanelController> logger, UserManager<AppUser> userManager)
    {
        _context = context;
        _logger = logger; // Logger'ı yapılandırıyoruz
        _userManager = userManager;
    }

    // GET: UserPanel/Index
    public IActionResult Index()
    {
        // Kullanıcıya özel veri işlemleri burada yapılabilir
        var model = new
        {
            Drugs = _context.Drugs.ToList(),
            Categories = _context.Categories.ToList()
        };
        return View(model);
    }

    // GET: UserPanel/ViewDrugs
    public IActionResult ViewDrugs()
    {
        var drugs = _context.Drugs.ToList();
        return View(drugs);
    }

    // GET: UserPanel/ViewCategories
    public IActionResult ViewCategories()
    {
        var categories = _context.Categories.ToList();
        return View(categories);
    }

    // POST: UserPanel/RequestStock
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RequestStock(int drugId, int quantity)
    {
        if (quantity <= 0)
        {
            TempData["ErrorMessage"] = "Quantity must be a positive number.";
            return RedirectToAction(nameof(Index));
        }
        if (quantity >= 50)
        {
            TempData["ErrorMessage"] = $"You can select a maximum of {MAX_ITEM} items.";
            return RedirectToAction(nameof(Index));
        }
        var drug = _context.Drugs.FirstOrDefault(d => d.Id == drugId);
        if (drug != null)
        {
            var request = new Request
            {
                UserName = User.Identity.Name,
                DrugId = drug.Id,
                Quantity = quantity,
                RequestDate = DateTime.Now,
                IsApproved = false
            };

            _context.Requests.Add(request);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Your request for {quantity} units of {drug.Name} has been submitted.";
        }
        else
        {
            TempData["ErrorMessage"] = "Requested drug does not exist.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult FilterMedicines(string search, decimal? minPrice, decimal? maxPrice, string stockStatus, string sortOption)
    {
        var query = _context.Drugs.AsQueryable();

        // Arama filtresi
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(m => m.Name.Contains(search) || m.Category.Contains(search));
        }

        // Fiyat filtresi
        if (minPrice.HasValue)
        {
            query = query.Where(m => m.UnitPrice >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(m => m.UnitPrice <= maxPrice.Value);
        }

        // Stok durumu medici
        if (stockStatus == "in-stock")
        {
            query = query.Where(m => m.Quantity > 0);
        }
        else if (stockStatus == "out-of-stock")
        {
            query = query.Where(m => m.Quantity == 0);
        }

        // Sıralama
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
    public IActionResult ViewOrders()
    {
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

    public IActionResult MyProfile()
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
        if (user == null)
        {
            return NotFound();
        }

        var model = new UserManagementViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            ActiveUser = user.ActiveUser,
            Role = user.UserType,
            PhoneNumber = user.PhoneNumber,
            RefCode = user.RefCode
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(string fullName, string email, string phoneNumber)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Json(new { success = false, message = "User not found." });

        user.FullName = fullName;
        user.Email = email;
        user.PhoneNumber = phoneNumber;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
            return Json(new { success = true });

        return Json(new { success = false, message = "Update failed." });
    }

}
