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
        public string SendLocation { get; set; }
        public string NameSurname { get; set; }
        public string TelephoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }


    }
}