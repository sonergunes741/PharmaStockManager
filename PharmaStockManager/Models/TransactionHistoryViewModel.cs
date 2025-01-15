using X.PagedList;

namespace PharmaStockManager.Models
{
    public class TransactionHistoryViewModel
    {
        public string DrugName { get; set; }
        public string TransactionType { get; set; }
        public string Date { get; set; }
        public IPagedList<Transaction> Transactions { get; set; }
    }
}
