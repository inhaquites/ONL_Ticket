namespace Lenovo.NAT.ViewModel.Logistic.Picking
{
    public class PickingItemViewModel
    {
        public PickingItemViewModel()
        {
            Brands = new List<PickingBrandViewModel>();
            Processes = new List<string>();
        }
        public long Id { get; set; }
        public long IdPicking { get; set; }
        public string PartNumber { get; set; }
        public string MTM { get; set; }
        public int IdBrand { get; set; }
        public string Brand { get; set; }
        public string ReturnOrder { get; set; }
        public string OriginInvoive { get; set; }
        public int Quantity { get; set; }
        public decimal UnitValue { get; set; }
        public decimal ItemValue { get; set; }
        public IEnumerable<PickingBrandViewModel> Brands { get; set; }
        public IEnumerable<string> Processes { get; set; }
        public string? Process { get; set; }
        public string InvoiceReturn { get; set; }
        public long IdPickingInvoice { get; set; }

    }
}
