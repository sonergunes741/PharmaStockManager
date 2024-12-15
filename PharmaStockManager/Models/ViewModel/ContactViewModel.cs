using System.ComponentModel.DataAnnotations;

namespace PharmaStockManager.Models.ViewModel
{
    public class ContactViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Message cannot be empty.")]
        public string Content { get; set; }
    }
}
