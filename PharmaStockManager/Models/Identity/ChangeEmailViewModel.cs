using System.ComponentModel.DataAnnotations;

namespace PharmaStockManager.Models.Identity
{
    public class ChangeEmailViewModel
    {
        [Required]
        [EmailAddress]
        public required string NewEmail { get; set; }

        [Required]
        public required string Password { get; set; }
    }

}
