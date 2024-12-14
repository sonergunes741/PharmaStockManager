using Microsoft.AspNetCore.Identity;

namespace PharmaStockManager.Models.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public bool ActiveUser { get; set; }

        // Var olan RefCode'u koruyoruz
        public string? RefCode { get; set; }

        public int? ActivationCode { get; set; }

        public string? FullName { get; set; }

        // Kullanıcı tipi için yeni alan
        public string? UserType { get; set; }  // "Admin", "Employee", "Customer"

        // İlişkisel veri - Permissions ile bağlantı
        public virtual ICollection<Permissions> Permissions { get; set; }

        public AppUser()
        {
            Permissions = new HashSet<Permissions>();
            ActiveUser = true; // Varsayılan olarak aktif
        }
    }
}