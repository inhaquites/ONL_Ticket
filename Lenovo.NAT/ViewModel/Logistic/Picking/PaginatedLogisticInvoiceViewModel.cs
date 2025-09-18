using Lenovo.NAT.ViewModel.Pagination;

namespace Lenovo.NAT.ViewModel.Logistic.Picking
{
    public class PaginatedLogisticInvoiceViewModel
    {
        public PaginatedLogisticInvoiceViewModel() { }
        public PaginatedLogisticInvoiceViewModel(PaginationInfo paginationInfo, LogisticInvoiceFilter filterInfo, IEnumerable<LogisticInvoiceViewModel> invoices)
        {
            PaginationInfo = paginationInfo;
            LogisticInvoices = invoices;
            FilterInfo = filterInfo;
        }

        public PaginationInfo PaginationInfo { get; set; }

        public LogisticInvoiceFilter FilterInfo { get; set; }

        public IEnumerable<LogisticInvoiceViewModel> LogisticInvoices { get; set; }
        public string? NetworkId { get; set; }
    }
}
