using PharmaStockManager.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace PharmaStockManager.Models
{
    public class Warehouse
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        [MaxLength(8)] // Adjust length based on your shortened GUID size
        public string RefCode { get; set; } // Unique Warehouse Reference Code

        [Required]
        [MaxLength(100)] // Name of the warehouse
        public string Name { get; set; }

        [MaxLength(100)] // Name of the warehouse
        public string Address { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Timestamp

    }
}
