namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class OrderNotLoaded
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public int? IdOrderStatus { get; set; }
        public string OrderStatus { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string NumberOrder { get; set; }
        public string AssignedTo { get; set; }
        public string EmailResolutionOwner { get; set; }
        public Guid? IdCountry { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string State { get; set; }
        public int? IdBusinessUnit { get; set; }
        public string BusinessUnit { get; set; }
        public int? IdNFType { get; set; }
        public string NFType { get; set; }
        public string PONumber { get; set; }
        public DateTime PODate { get; set; }
        public int? IdCustomer { get; set; }
        public string Customer { get; set; }
        public int? IdSegment { get; set; }
        public string Segment { get; set; }
        public string DMU { get; set; }
        public int? IdOrderType { get; set; }
        public string OrderType { get; set; }
        public string RecolocationType { get; set; }
        public string BillAhead { get; set; }
        public string ISRName { get; set; }
        public int? IdCancelReason { get; set; }
        public string CancelReason { get; set; }
    }
}
