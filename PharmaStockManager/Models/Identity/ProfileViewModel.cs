namespace PharmaStockManager.Models.Identity
{
    public class ProfileViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string ProfilePicture { get; set; } // URL or file path of the profile picture
    }
}
