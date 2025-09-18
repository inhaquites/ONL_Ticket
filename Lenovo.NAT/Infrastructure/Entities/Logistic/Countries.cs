namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class Countries
    {
        public Guid Id { get; set; }
        public Guid GroupID { get; set; }
        public Guid GroupAdminID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public byte RecordStatus { get; set; }
        public string Abbreviation { get; set; }
        public int CountryId { get; set; }
    }
}
