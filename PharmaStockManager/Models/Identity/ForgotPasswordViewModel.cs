using System.ComponentModel.DataAnnotations;

namespace PharmaStockManager.Models.Identity
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
