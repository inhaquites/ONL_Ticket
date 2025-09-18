using Lenovo.NAT.ViewModel.Pagination;

namespace Lenovo.NAT.ViewModel.Logistic.Picking
{
    public class PaginatedPickingViewModel
    {
        public PaginatedPickingViewModel() { }
        public PaginatedPickingViewModel(PaginationInfo paginationInfo, PickingFilter filterInfo, IEnumerable<PickingViewModel> pickings)
        {
            PaginationInfo = paginationInfo;
            Pickings = pickings;
            FilterInfo = filterInfo;
        }

        public PaginationInfo PaginationInfo { get; set; }

        public PickingFilter FilterInfo { get; set; }

        public IEnumerable<PickingViewModel> Pickings { get; set; }
        public string? NetworkId { get; set; }
        public IFormFile File { get; set; }
    }
}
