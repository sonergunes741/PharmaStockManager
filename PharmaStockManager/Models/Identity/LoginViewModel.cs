using System.ComponentModel.DataAnnotations;

namespace PharmaStockManager.Models.Identity
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Eposta gerekli")]
        [EmailAddress(ErrorMessage = "Geçersiz Eposta Adresi")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre Gerekli")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
