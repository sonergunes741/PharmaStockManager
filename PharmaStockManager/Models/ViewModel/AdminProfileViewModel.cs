namespace PharmaStockManager.Models.ViewModel
{
    public class AdminProfileViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        // Diğer profil bilgileri eklenebilir
    }
}
