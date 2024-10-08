public class StockRequest
{
    public int Id { get; set; }
    public int DrugId { get; set; }
    public string UserId { get; set; }
    public int QuantityRequested { get; set; }
    public bool IsApproved { get; set; } // Talep onaylandı mı?
    public bool IsRejected { get; set; } // Talep reddedildi mi?

    public DateTime RequestDate { get; set; } // Talep tarihi
}
