namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class PickingInvoice
    {
        public PickingInvoice()
        {
            ApprovalFile = new byte[0];
            ReturnFile = new byte[0];
            SeriesLetterFile = new byte[0];
        }
        public long Id { get; set; }
        public long IdPicking { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string InvoiceReturn { get; set; }
        public DateTime InvoiceReturnDate { get; set; }
        public  string? ApprovalFileName { get; set; }
        public byte[]? ApprovalFile { get; set; }
        public string? ReturnFileName { get; set; }
        public byte[]? ReturnFile { get; set; }
        public string? SeriesLetterFileName { get; set; }
        public byte[]? SeriesLetterFile { get; set; }
        public string? Observation { get; set; }
        public decimal? TotalValue { get; set; }
    }
}