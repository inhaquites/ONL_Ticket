namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class OrderProduct
    {
        public int Id { get; set; }
        public int IdOrderShipTo { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string BID { get; set; }
        public string ContractNumber { get; set; }
        public string PartNumber { get; set; }
        public string MTMDescription { get; set; }
        public int Quantity { get; set; }
        public decimal UnitNetPrice { get; set; }
        public decimal UnitGrossPrice { get; set; }
    }
}
