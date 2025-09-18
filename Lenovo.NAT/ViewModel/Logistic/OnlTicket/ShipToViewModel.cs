namespace Lenovo.NAT.ViewModel.Logistic.OnlTicket
{
    public class ShipToViewModel
    {
        public int Id { get; set; }
        public string? CNPJ { get; set; }
        public string? Address { get; set; }
        public string? Neighborhood { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? State { get; set; }
        public string? SAPOrderNumber { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; } = new();
    }
}
