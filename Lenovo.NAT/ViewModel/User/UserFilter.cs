namespace Lenovo.NAT.ViewModel.User
{
    public class UserFilter
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? NetworkId { get; set; }
        public string? Email { get; set; }
        public string? Position { get; set; }
        public string? Department { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CountryID { get; set; }
    }
}
