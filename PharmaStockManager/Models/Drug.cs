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
        public int CategoryId { get; set; } // Foreign key for Category

        [ForeignKey("CategoryId")]
        public Category Category { get; set; } // Navigation property for the Category

        [Required]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")] // Precision and scale specified here
        public decimal UnitPrice { get; set; }

        public decimal TotalValue => Quantity * UnitPrice;
    }
}
