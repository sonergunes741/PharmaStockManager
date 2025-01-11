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
        [Required]
        [Display(Name = "Drug Type")]
        public string DrugType { get; set; }
        public decimal TotalValue => Quantity * UnitPrice;

        [Required]
        [Range(0, int.MaxValue)]
        public int CriticalStockLevel { get; set; } = 10;
        public int MaxRequest { get; set; } = 50;
    }
}
