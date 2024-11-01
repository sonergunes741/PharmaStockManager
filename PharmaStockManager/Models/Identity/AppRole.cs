using Microsoft.AspNetCore.Identity;

namespace PharmaStockManager.Models.Identity
{
    public class AppRole : IdentityRole<int>

    {
        public AppRole()
        {
        }

        public AppRole(string roleName) : this()
        {
            Name = roleName;
        }
    }
}
