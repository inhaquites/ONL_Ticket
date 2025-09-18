using System.ComponentModel.DataAnnotations;

namespace Lenovo.NAT.ViewModel.Logistic.Picking
{
    public class PickingInvoiceViewModel
    {
        public long Id { get; set; }
        public long IdPicking { get; set; }
        public string InvoiceReturn { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime InvoiceReturnDate { get; set; }
        public string? ApprovalFileName { get; set; }
        public byte[]? ApprovalFile { get; }
        public string? ReturnFileName { get; set; }
        public byte[]? ReturnFile { get; }
        public string? SeriesLetterFileName { get; set; }
        public byte[]? SeriesLetterFile { get; }
        public string? Observation { get; set; }
        public decimal? TotalValue { get; set; }
    }
}
