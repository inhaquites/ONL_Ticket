using Lenovo.NAT.Infrastructure.Entities.Admin;

namespace Lenovo.NAT.ViewModel.User
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string NetworkId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string RecordStatus { get; set; }
        public Guid GroupID { get; set; }
        public Guid? GroupAdminID { get; set; }
        public Guid CountryID { get; set; }
        public bool isRequestorBR { get; set; } = false;
        public bool isRequestorLAS { get; set; } = false;
        public bool IsApprover { get; set; }
        public bool CanChangeRegion { get; set; } = false;
        public List<Country> Countries { get; set; }
        public List<ModuleViewModel> Modules { get; set; }

    }

    public class ModuleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}