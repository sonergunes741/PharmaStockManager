namespace PharmaStockManager.Models.ViewModel
{
    public class UserManagementViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool ActiveUser { get; set; }
        public string Role { get; set; }
    }
}
