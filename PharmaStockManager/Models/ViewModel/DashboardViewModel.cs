namespace PharmaStockManager.Models.ViewModel
{
    public class DashboardViewModel
    {
        public int TotalDrugs { get; set; }
        public List<Drug> CriticalStockDrugs { get; set; }
        public List<Drug> ExpiringDrugs { get; set; }
        public int TotalCategories { get; set; }
        public int TotalUsers { get; set; }
        public DateTime CurrentDate { get; set; }
    }
}
