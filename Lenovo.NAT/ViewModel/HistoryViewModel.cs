namespace Lenovo.NAT.ViewModel
{
    public class HistoryViewModel
    {
        public int Id { get; set; }
        public int IdEntity { get; set; }
        public string UpdatedTable { get; set; }
        public string Field { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
