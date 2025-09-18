using Lenovo.NAT.ViewModel.Pagination;

namespace Lenovo.NAT.ViewModel.Logistic.OnlTicket;

public class OnlTicketFilter : PaginationInfo
{
    public string? LogNumber { get; set; }
    public string? Customer { get; set; }
    public string? Segment { get; set; }
    public int? SegmentId { get; set; }
    public string? OrderType { get; set; }
    public string? AssignTo { get; set; }
    public string? AgingBucket { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? Status { get; set; }
    public string? SAPOrder { get; set; }
    public bool IsAdmin { get; set; }

    // Listas para dropdowns
    public List<string> Statuses { get; set; } = new();
    public List<string> Customers { get; set; } = new();
    public List<string> Segments { get; set; } = new();
    public List<OnlTicketCustomerSegmentViewModel> CustomerSegments { get; set; } = new();
    public List<OnlTicketOrderTypeViewModel> OrderTypes { get; set; } = new();
    public List<string> AssignToUsers { get; set; } = new();
    public List<string> AgingBuckets { get; set; } = new();
    public List<string> CreatedByUsers { get; set; } = new();
}
