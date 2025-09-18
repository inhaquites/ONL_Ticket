using Lenovo.NAT.ViewModel.Pagination;
using System.ComponentModel.DataAnnotations;

namespace Lenovo.NAT.ViewModel.Logistic.Picking
{
    public class PickingFilter
    {
        public PickingFilter() 
        {
            ProcessTypes = new List<PickingProcessTypeViewModel>();
            PickingAreas = new List<PickingAreaViewModel>();
            PickingStatus = new List<PickingStatusViewModel>();
        }  
        public string? Process { get; set; }
        public int? IdPickingProcessType { get; set; }
        public IEnumerable<PickingProcessTypeViewModel> ProcessTypes { get; set; }
        public int? IdPickingStatus { get; set; }
        public IEnumerable<PickingStatusViewModel> PickingStatus { get; set; }
        public int? IdPickingArea { get; set; }
        public IEnumerable<PickingAreaViewModel> PickingAreas { get; set; }
        public string? Client { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreatedOn { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ValidationDate { get; set; }
        public bool IsAdmin { get; set; }
        public string? NetworkId { get; set; }
        public PaginationInfo? PaginationInfo { get; set; }
    }
}
