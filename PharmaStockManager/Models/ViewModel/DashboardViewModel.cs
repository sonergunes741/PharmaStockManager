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

        // Add method to check stock status
        public string GetStockStatus(Drug drug)
        {
            if (drug.Quantity <= drug.CriticalStockLevel / 2) return "very-low";
            if (drug.Quantity <= drug.CriticalStockLevel) return "low";
            return "normal";
        }
    }
}