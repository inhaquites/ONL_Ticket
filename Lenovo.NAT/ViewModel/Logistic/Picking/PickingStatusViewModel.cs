namespace Lenovo.NAT.ViewModel.Logistic.Picking
{
    public class PickingStatusViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PickingReasonViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PickingCarrierViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ContactEmails { get; set; }
    }
}
