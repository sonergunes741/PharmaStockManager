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

        public string DrugType { get; set; } // "Commercial" or "Clinical"
        public string ResearchNumber { get; set; } // Only for commercial drugs


    }
}