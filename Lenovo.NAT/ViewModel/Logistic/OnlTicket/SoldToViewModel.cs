namespace Lenovo.NAT.ViewModel.Logistic.OnlTicket
{
    public class SoldToViewModel
    {
        public int Id { get; set; }
        public string? CNPJ { get; set; }
        public string? Address { get; set; }
        public string? Neighborhood { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? State { get; set; }
        public List<ShipToViewModel> ShipToAddresses { get; set; } = new();
    }
}
