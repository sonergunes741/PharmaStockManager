using System.Collections.Generic;
using PharmaStockManager.Models;

namespace PharmaStockManager.ViewModels
{
    public class DashboardViewModel
    {
        public IEnumerable<Stock> Stocks { get; set; }
        public ChartData ChartData { get; set; }
        public IEnumerable<Alert> Alerts { get; set; }
    }
}