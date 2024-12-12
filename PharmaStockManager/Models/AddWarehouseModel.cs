using System.ComponentModel.DataAnnotations;

namespace PharmaStockManager.Models
{
    public class AddWarehouseModel
    {
        [Required]
        [MaxLength(100)] // Name of the warehouse
        public string Name { get; set; }

        [MaxLength(100)] // Name of the warehouse
        public string Address { get; set; }

    }
}
