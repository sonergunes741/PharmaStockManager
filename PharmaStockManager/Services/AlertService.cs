using System.Collections.Generic;
using PharmaStockManager.Models;

namespace PharmaStockManager.Services
{
    public static class AlertService
    {
        public static List<Alert> GenerateAlerts(IEnumerable<Stock> stocks)
        {
            var alerts = new List<Alert>();

            foreach (var stock in stocks)
            {
                if (stock.Price > 200)
                {
                    alerts.Add(new Alert { Message = $"{stock.Symbol} price is above $200." });
                }
            }

            return alerts;
        }
    }
}