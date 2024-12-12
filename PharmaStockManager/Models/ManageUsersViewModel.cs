using PharmaStockManager.Models.Identity;
namespace PharmaStockManager.Models
{
    public class ManageUsersViewModel
    {
        public IList<AppUser_Warehouse> AdminUsers { get; set; }
        public IList<AppUser_Warehouse> EmployeeUsers { get; set; }
        public IList<AppUser_Warehouse> CustomerUsers { get; set; }
    }

    public class AppUser_Warehouse : AppUser
    {
        public string WarehouseName { get; set; }
    }

}