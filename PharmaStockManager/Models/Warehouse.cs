using PharmaStockManager.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace PharmaStockManager.Models
{
    public class Warehouse
    {
        [Key]
        public int Id { get; set; }

        public bool isActive { get; set; }

        [Required]
        [MaxLength(8)]
        public string RefCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Address { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
