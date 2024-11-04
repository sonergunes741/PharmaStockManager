using System.ComponentModel.DataAnnotations;
namespace PharmaStockManager.Models
{
    public class Depos
    {
        public int Id {  get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
