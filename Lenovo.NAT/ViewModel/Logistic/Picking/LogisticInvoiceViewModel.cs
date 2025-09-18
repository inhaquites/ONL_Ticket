namespace Lenovo.NAT.ViewModel.Logistic.Picking
{
    public class LogisticInvoiceViewModel
    {
        public DateTime CreatedOn { get; set; }
        public string AccessKey { get; set; }
        public string XML { get; set; }
        public bool? FileSent { get; set; }
        public string? ResponseMessage { get; set; }
        public string? AirwayBillNumber { get; set; }
        public DateTime? ResponseGetOn { get; set; }
        public string? Status { get; set; }
        public int? Retries { get; set; }
        public string? ConditionCode { get; set; }
        public string? ConditionData { get; set; }
    }
}
