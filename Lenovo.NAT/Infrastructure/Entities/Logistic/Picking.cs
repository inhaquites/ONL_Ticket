using NPOI.SS.Formula.Functions;
using System.Diagnostics.Metrics;

namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class Picking
    {
        public Picking() 
        {
            IdPickingStatus = (int)PickingStatusEnum.Pending;
        }
        public long Id { get; set; }
        public string  CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? IdPickingStatus { get; set; }
        public string Process { get; set; }
        public int IdPickingProcessType { get; set; }
        public int IdPickingArea { get; set; }
        public string Client { get; set; }
        public string City { get; set; }
        public string UF { get; set; }  
        public string Contact { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public decimal? TotalValue { get; set; }
        public string? CarrierName { get; set; }
        public string? Reason { get; set; }
        public DateTime? CarrierRequestDate { get; set; }
        public DateTime? ExpectedPickingDate { get; set; }
        public DateTime? EffectivePickingDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public DateTime? EffectiveReturnDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? CorrectionObservation { get; set; }
    }

    public enum PickingStatusEnum
    {
        Pending = 1,
        Correction = 2,
        PickingProcessing = 3,
        Transit = 4,
        ReceivingProcessing = 5,
        ProcessFinished = 6,
        Canceled = 7
    }
}