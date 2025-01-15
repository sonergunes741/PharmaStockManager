using System.ComponentModel.DataAnnotations;

namespace PharmaStockManager.Models.Identity
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "İsim gereklidir.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Eposta adresi gereklidir")]
        [EmailAddress(ErrorMessage = "Geçersiz Eposta Adresi")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre Gereklidir")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Şifre en az 8 karakterli olmalıdır.")]
        [StrongPassword]
        public string Password { get; set; }

        //[Required(ErrorMessage = "Confirm Password is required")]
        //[Compare("Password", ErrorMessage = "Passwords do not match.")]
        //public string ConfirmPassword { get; set; }

        [StringLength(8, ErrorMessage = "Referans kodu en fazla 8 karakterli olabilir.")]
        [Required(ErrorMessage = "Referans kodu gerekldiir.")]
        // Add validation for specific code format and uniqueness if necessary
        public string RefCode { get; set; }
    }
}