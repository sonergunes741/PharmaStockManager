using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaStockManager.Models
{
    public class Drug
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    
        public string DrugType { get; set; } // "Commercial" or "Clinical"
        public string ResearchNumber { get; set; } // Only for commercial drugs

    

        [Required]
        public string Category { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")] // Precision and scale specified here
        public decimal UnitPrice { get; set; }

        // Yeni eklenen ExpiryDate alanı
        [DataType(DataType.Date)] // Expiry date input formatı için
        public DateTime? ExpiryDate { get; set; } // Nullable çünkü bazı ilaçların son kullanma tarihi olmayabilir.

        public decimal TotalValue => Quantity * UnitPrice;


      

        
    }
}
