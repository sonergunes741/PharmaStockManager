using System.ComponentModel.DataAnnotations;

namespace PharmaStockManager.Models.Identity
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword   { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

    }
}
