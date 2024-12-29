using PharmaStockManager.Models.Identity;
using System;

namespace PharmaStockManager.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string? DrugName { get; set; } // Foreign key to Drug
        public int Quantity { get; set; }
        public string TransactionType { get; set; } // "StockIn" or "StockOut"
        public DateTime TransactionDate { get; set; }
        public DateTime ExpiryDate { get; set; } // Optional, if necessary
        public string Type { get; set; } // Drug type, optional
        public decimal Price { get; set; }

        public string? UserName { get; set; }
    }
}
