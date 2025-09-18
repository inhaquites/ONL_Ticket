namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class OrderHistory
    {
        public int Id { get; set; }
        public long IdOrderNotLoaded { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int IdOrderStatus { get; set; }
        public string OrderStatus { get; set; }
        public string Comments { get; set; }
        public int IdProblemSubCategory { get; set; }
        public string EmailsCopy { get; set; }
        public string SalesRep { get; set; }
    }
}
