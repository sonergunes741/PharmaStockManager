using System.ComponentModel.DataAnnotations;

namespace PharmaStockManager.Models.Identity
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [StrongPassword]
        public string Password { get; set; }

        //[Required(ErrorMessage = "Confirm Password is required")]
        //[Compare("Password", ErrorMessage = "Passwords do not match.")]
        //public string ConfirmPassword { get; set; }

        [StringLength(50, ErrorMessage = "Referral code cannot exceed 50 characters.")]
        // Add validation for specific code format and uniqueness if necessary
        public string RefCode { get; set; }
    }
}