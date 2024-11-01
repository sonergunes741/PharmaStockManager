using System.ComponentModel.DataAnnotations;

namespace PharmaStockManager.Models.Identity
{
    public class ChangePasswordViewModel
    {
        [Required]
        public required string OldPassword;

        [Required]
        public required string NewPassword;

        [Required]
        public required string ConfirmPassword;

    }
}
