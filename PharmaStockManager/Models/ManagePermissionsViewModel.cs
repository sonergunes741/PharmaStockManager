namespace PharmaStockManager.Models
{
    public class ManagePermissionsViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<PermissionViewModel> Permissions { get; set; }
    }

    public class PermissionViewModel
    {
        public string Permission { get; set; }
        public bool IsAssigned { get; set; }
    }
}
