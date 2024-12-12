using Microsoft.AspNetCore.Identity;

namespace PharmaStockManager.Models.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public bool ActiveUser { get; set; }
        public string? RefCode { get; set; }
        public int? ActivationCode { get; set; }
        public string? FullName { get; set; }
        public ICollection<Permissions> Permissions { get; set; }
    }
}
