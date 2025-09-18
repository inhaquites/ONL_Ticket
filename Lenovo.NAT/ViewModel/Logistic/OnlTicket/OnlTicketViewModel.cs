using Lenovo.NAT.ViewModel.Logistic.OnlTicket;

namespace Lenovo.NAT.ViewModel.Logistic.OnlTicket;

public class OnlTicketAttachmentViewModel
{
    public int Id { get; set; }
    public string? CustomerPO { get; set; }
    public string? Descricao { get; set; }
    public string? Comentarios { get; set; }
}

public class CountryViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class OrderItemViewModel
{
    public int Id { get; set; }
    public string? BidContractNumber { get; set; }
    public string? PartNumber { get; set; }
    public string? PartNumberDescription { get; set; }
    public int? Qty { get; set; }
    public decimal? UnityNetPrice { get; set; }
    public decimal? UnitGrossPrice { get; set; }
}

public class AddressViewModel
{
    public string? CNPJ { get; set; }
    public string? Address { get; set; }
    public string? Neighborhood { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? State { get; set; }
}

public class SAPOrderViewModel
{
    public int Id { get; set; }
    public string? SAPOrderNumber { get; set; }
    public string? DeliveryNumber { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? NFNumber { get; set; }
    public DateTime? NFDate { get; set; }
    public decimal? NetAmount { get; set; }
    public decimal? TotalOrderAmount { get; set; }
}

public class HistoricEntryViewModel
{
    public int Id { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? Status { get; set; }
    public string? Comments { get; set; }
}

public class OnlTicketViewModel
{
    // ONL Ticket Information
    public string? LogNumber { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public string? Status { get; set; }
    public string? EmailFrom { get; set; }
    public string? AssignedOperator { get; set; }

    // Order Information - Header
    public string? PONumber { get; set; }
    public string? OrderType { get; set; }
    public string? OrderStatus { get; set; }
    public string? NFType { get; set; }
    public string? CustomerName { get; set; }
    public string? DMU { get; set; }
    public string? Country { get; set; }
    public Guid? CountryId { get; set; }
    public string? BU { get; set; }
    public DateTime? PODate { get; set; }
    public string? Segment { get; set; }
    public int? SegmentId { get; set; }
    public string? BillAhead { get; set; }
    public string? ISRName { get; set; }
    public string? Region { get; set; }
    public string? UF { get; set; }
    public string? ReplacementType { get; set; }

    // Anexos
    public List<OnlTicketAttachmentViewModel> Attachments { get; set; } = new();
    public string? Comentarios { get; set; }

    // Order Information - Item
    public string? ID { get; set; }
    public string? OrderId { get; set; }
    public List<SoldToViewModel> SoldToAddresses { get; set; } = new();

    // Extra Order Information - Audit Section
    public List<SAPOrderViewModel> SAPOrders { get; set; } = new();
    public List<string> Comments { get; set; } = new();

    // Historic
    public List<HistoricEntryViewModel> HistoricEntries { get; set; } = new();

    // Listas para dropdowns
    public List<OnlTicketOrderTypeViewModel> OrderTypes { get; set; } = new();
    public List<OnlTicketOrderStatusViewModel> OrderStatuses { get; set; } = new();
    public List<OnlTicketNFTypeViewModel> NFTypes { get; set; } = new();
    public List<OnlTicketCustomerSegmentViewModel> CustomerSegments { get; set; } = new();
    public List<CountryViewModel> Countries { get; set; } = new();
    public List<string> Segments { get; set; } = new();
    public List<string> CustomerNames { get; set; } = new();
}