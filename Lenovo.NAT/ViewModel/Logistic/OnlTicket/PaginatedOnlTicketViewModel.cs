using Lenovo.NAT.ViewModel.Pagination;

namespace Lenovo.NAT.ViewModel.Logistic.OnlTicket;

public class PaginatedOnlTicketViewModel
{
    public PaginatedOnlTicketViewModel() { }
    public PaginatedOnlTicketViewModel(PaginationInfo paginationInfo, OnlTicketFilter filterInfo, IEnumerable<OnlTicketListViewModel> onlTickets)
    {
        PaginationInfo = paginationInfo;
        OnlTickets = onlTickets;
        FilterInfo = filterInfo;
    }

    public PaginationInfo PaginationInfo { get; set; }
    public OnlTicketFilter FilterInfo { get; set; }
    public IEnumerable<OnlTicketListViewModel> OnlTickets { get; set; }
    public string? NetworkId { get; set; }
    public IFormFile File { get; set; }

    // Contadores para o dashboard
    public int NewCount { get; set; }
    public int ApprovedCount { get; set; }
    public int CanceledNotPOCount { get; set; }
    public int RejectCount { get; set; }
    public int WithoutAssignCount { get; set; }
    public int WithoutSAPOrderCount { get; set; }
}
