namespace PharmaStockManager.Models
{
    public class Request
    {
        public int Id { get; set; }
        public string UserName { get; set; } // Talepte bulunan kullanıcı
        public int DrugId { get; set; } // İlaç ID'si
        public int Quantity { get; set; } // Talep edilen miktar
        public DateTime RequestDate { get; set; } // Talep tarihi
        public bool IsApproved { get; set; } // Onay durumu (Default: False)
        public bool IsRejected { get; set; }
    }


}
