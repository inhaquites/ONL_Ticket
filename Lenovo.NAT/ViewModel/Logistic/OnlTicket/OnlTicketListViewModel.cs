namespace Lenovo.NAT.ViewModel.Logistic.OnlTicket;

public class OnlTicketListViewModel
{
    public int Id { get; set; }
    public string? LogNumber { get; set; }
    public string? Customer { get; set; }
    public string? Segment { get; set; }
    public string? AssignTo { get; set; }
    public string? OrderAging { get; set; }
    public string? AgingBucket { get; set; }
    public string? SAPOrder { get; set; }
    public int? TotalCAs { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? EmailFrom { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Status { get; set; }
}
