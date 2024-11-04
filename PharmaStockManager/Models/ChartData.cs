using System.Collections.Generic;

namespace PharmaStockManager.Models
{
    public class ChartData
    {
        public List<DataPoint> DataPoints { get; set; }
    }

    public class DataPoint
    {
        public string Date { get; set; }
        public decimal Price { get; set; }
    }
}