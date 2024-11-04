using System.Collections.Generic;
using System.Threading.Tasks;
using PharmaStockManager.Models;

namespace PharmaStockManager.Services
{
    public static class StockService
    {
        public static async Task<List<Stock>> FetchStockDataAsync()
        {
            // Implement API call to fetch stock data
            return await Task.FromResult(new List<Stock>
            {
                new Stock { Symbol = "AAPL", Price = 150.00m },
                new Stock { Symbol = "MSFT", Price = 250.00m },
                new Stock { Symbol = "GOOG", Price = 2800.00m }
            });
        }

        public static async Task<ChartData> FetchChartDataAsync()
        {
            // Implement API call to fetch stock price history
            return await Task.FromResult(new ChartData
            {
                DataPoints = new List<DataPoint>
                {
                    new DataPoint { Date = "2023-01-01", Price = 140.00m },
                    new DataPoint { Date = "2023-02-01", Price = 145.00m },
                    new DataPoint { Date = "2023-03-01", Price = 150.00m }
                }
            });
        }
    }
}