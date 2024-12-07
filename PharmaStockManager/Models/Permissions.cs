using PharmaStockManager.Models.Identity;
namespace PharmaStockManager.Models
{
    public class Permissions
    {
        public int Id { get; set; }
        public bool EditStocks { get; set; } = false;
        public bool StockIn { get; set; } = false;
        public bool StockOut { get; set; } = false;






        public int UserID { get; set; }
        public AppUser User { get; set; }

    }
}
