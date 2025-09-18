using Microsoft.AspNetCore.Identity;

namespace Lenovo.NAT.Infrastructure.Entities
{
    public class User: IdentityUser
    {
        public string? Name { get; set; }
        public string NetworkId { get; set; }
        public string? Position { get; set; }
        public string? Department { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public Guid? CountryId { get; set; }
    }
}
