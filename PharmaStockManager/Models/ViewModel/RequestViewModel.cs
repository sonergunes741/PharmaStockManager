// Models/ViewModels/RequestViewModel.cs
namespace PharmaStockManager.Models.ViewModels
{
    public class RequestViewModel
    {
        public int Id { get; set; }
        public string DrugName { get; set; }
        public int Quantity { get; set; }
        public string UserName { get; set; }
        public DateTime RequestDate { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
    }
}