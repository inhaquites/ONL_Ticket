using Lenovo.NAT.ViewModel.Pagination;

namespace Lenovo.NAT.ViewModel.User
{
    public class PaginatedUserViewModel
    {
        public PaginatedUserViewModel(PaginationInfo paginationInfo, IEnumerable<UserViewModel> users)
        {
            PaginationInfo = paginationInfo;
            Users = users;
        }

        public PaginationInfo PaginationInfo { get; set; }

        public UserFilter FilterInfo { get; set; }

        public IEnumerable<UserViewModel> Users { get; set; }
    }
}
