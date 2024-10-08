using Microsoft.AspNetCore.Identity;

namespace PharmaStockManager.ViewModels
{
    public class ManageRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<string> UserRoles { get; set; } = new List<string>(); // Kullanıcının sahip olduğu roller
        public List<IdentityRole> AllRoles { get; set; } = new List<IdentityRole>(); // Tüm roller

        // Eğer 'Roles' özelliğini de kullanıyorsan, bu şekilde ekleyebilirsin:
        public List<string> Roles { get; set; } = new List<string>(); // Diğer roller
    }
}
