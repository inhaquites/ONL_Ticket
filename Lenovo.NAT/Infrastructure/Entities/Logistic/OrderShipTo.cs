using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class OrderShipTo
    {
        public int Id { get; set; }
        public long IdOrderNotLoaded { get; set; }
        public int IdOrderSoldTo { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string CompanyTaxId { get; set; }
        public string Address { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string SapOrder { get; set; }
        public string SapOrderService { get; set; }
    }
}
