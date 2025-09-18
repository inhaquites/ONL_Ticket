partial class Program
{
    public Program()
    {
    }

    private static void Main(string[] args)
    {
        DateTime now = DateTime.Now;

        DateTime myDate = now;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string? ToString()
    {
        return base.ToString();
    }
}

namespace Lenovo.NAT.ViewModel.Logistic.Picking
{
    public class PickingDetailsViewModel
    {
        public long Id { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? Process { get; set; }
        public int? IdPickingProcessType { get; set; }
        public string? Requestor { get; set; }
        public int? IdPickingArea { get; set; }
        public int? IdPickingStatus { get; set; }
        public string? Client { get; set; }
        public string? City { get; set; }
        public string? UF { get; set; }
        public string? Contact { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public string? CarrierName { get; set; }
        public string? Reason { get; set; }
        public DateTime? CarrierRequestDate { get; set; }
        public DateTime? ExpectedPickingDate { get; set; }
        public DateTime? EffectivePickingDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public DateTime? EffectiveReturnDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CorrectionObservation { get; set; }
        public string? AdminObservation { get; set; }
        public  string? NetworkId { get; set; }
        public IEnumerable<PickingInvoiceDetailsViewModel> Invoices { get; set; }

        public IEnumerable<PickingItemDetailsViewModel> Items { get; set; }
    }

    public class PickingInvoiceDetailsViewModel
    {
        public long Id { get; set; }
        public string InvoiceReturn { get; set; }
        public DateTime InvoiceReturnDate { get; set; }
        public string? ApprovalFileName { get; set; }
        public IFormFile ApprovalFile { get; set; }
        public string? ReturnFileName { get; set; }
        public IFormFile InvoiceReturnFile { get; set; }
        public string? SeriesLetterFileName { get; set; }
        public IFormFile SeriesLetterFile { get; set; }
        public string? Observation { get; set; }
        public decimal? TotalValue { get; set; }
    }

    public class PickingItemDetailsViewModel
    {
        public long Id { get; set; }
        public string PartNumber { get; set; }
        public string MTM { get; set; }
        public int IdBrand { get; set; }
        public string ReturnOrder { get; set; }
        public string OriginInvoive { get; set; }
        public int Quantity { get; set; }
        public decimal UnitValue { get; set; }
        public decimal ItemValue { get; set; }
        public string InvoiceReturn { get; set; }
    }
}
