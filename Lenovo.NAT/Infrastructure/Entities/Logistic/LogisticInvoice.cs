namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class LogisticInvoice
    {
        public LogisticInvoice()
        {
            CreatedOn = DateTime.UtcNow.AddHours(-3);
        }
        public long Id { get; set; }
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
