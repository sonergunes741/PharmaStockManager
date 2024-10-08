using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmaStockManager.Models
{
    public class StockRequest
    {
        [Key]
        public int Id { get; set; }

        // ForeignKey attribute for specifying the relationship
        [ForeignKey("Drug")]
        public int DrugId { get; set; }

        public Drug Drug { get; set; } // Navigation property

        [Required]
        public string UserId { get; set; } // Kullanıcının talebini belirlemek için UserId

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public int QuantityRequested { get; set; } // Talep edilen ilaç miktarı

        public bool IsApproved { get; set; } // Onaylanmış mı

        public bool IsRejected { get; set; } // Reddedilmiş mi

        [DataType(DataType.DateTime)]
        public DateTime RequestDate { get; set; } // Talep tarihi
    }
}
