using System.ComponentModel.DataAnnotations;

namespace PharmaStockManager.Models.Identity
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }
        public string Token { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [StrongPassword]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        [StrongPassword]
        public string ConfirmPassword { get; set; }
    }
}
