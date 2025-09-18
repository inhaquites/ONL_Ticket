using System.ComponentModel.DataAnnotations;

namespace Lenovo.NAT.ViewModel.Logistic.Picking
{
    public class PickingViewModel
    {
        public PickingViewModel()
        {
            CreatedOn = DateTime.UtcNow.AddHours(-3);
            PickingItem = new PickingItemViewModel();
            PickingItems = new List<PickingItemViewModel>();
            PickingInvoice = new PickingInvoiceViewModel();
            PickingInvoices = new List<PickingInvoiceViewModel>();
            UserCanAddItems = false;
        }
        public long Id { get; set; }
        
        public string CreatedBy { get; set; }
        
        public DateTime CreatedOn { get; set; }

        public string Process { get; set; }
        
        public int IdPickingProcessType { get; set; }
        
        public string ProcessType { get; set; }
        
        public IEnumerable<PickingProcessTypeViewModel> ProcessTypes { get; set; }
        
        public int IdPickingArea { get; set; }
        
        public string Area { get; set; }
        
        public IEnumerable<PickingAreaViewModel> Areas { get; set; }
        
        public string Client { get; set; }
        
        public string City { get; set; }
        
        public string UF { get; set; }
        
        public string Contact { get; set; }
        
        public string Email { get; set; }
        
        public string Telephone { get; set; }
        
        public decimal TotalValue { get; set; }
        
        public int IdPickingStatus { get; set; }
        
        public string PickingStatus { get; set; }
        
        public string? CarrierName { get; set; }
        public IEnumerable<PickingCarrierViewModel> Carriers { get; set; }

        public string? Reason { get; set; }
        public IEnumerable<PickingReasonViewModel> Reasons { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CarrierRequestDate { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? ExpectedPickingDate { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? EffectivePickingDate { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? ExpectedReturnDate { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? EffectiveReturnDate { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        
        public PickingItemViewModel PickingItem { get; set; }
        
        public IEnumerable<PickingItemViewModel> PickingItems { get; set; }
        
        public PickingInvoiceViewModel PickingInvoice { get; set; }
        
        public IEnumerable<PickingInvoiceViewModel> PickingInvoices { get; set; }
        
        public bool UserCanAddItems { get; set; }
        public bool UserCanChangeHeaderInformation { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? CorrectionObservation { get; set; }

        public string? AdminObservation { get; set; }
        public bool UserIsAdmin { get; set; }
    }

    public class ShortPickingViewModel
    {
        public string Process { get; set; }
        public DateTime? EffectivePickingDate { get; set; }
        public DateTime? EffectiveReturnDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public enum PickingStatusEnumViewModel
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
