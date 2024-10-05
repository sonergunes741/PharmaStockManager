using System;
using System.ComponentModel.DataAnnotations;

namespace PharmaStockManager.Models
{
    public class StockRequest
    {
        public int Id { get; set; }

        // İlişkilendirilen Drug ID'si
        public int DrugId { get; set; }

        // İlişkilendirilmiş Drug nesnesi
        public Drug Drug { get; set; }

        // Kullanıcıların görmesi için Drug Name
        public string DrugName => Drug != null ? Drug.Name : string.Empty;

        [Required]
        public int Quantity { get; set; }

        // Talep eden kullanıcının ID'si
        public string UserId { get; set; }

        // Talep eden kullanıcının adı
        public string UserName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime RequestDate { get; set; }

        public bool IsApproved { get; set; }
    }
}
