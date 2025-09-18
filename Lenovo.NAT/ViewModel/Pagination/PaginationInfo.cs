namespace Lenovo.NAT.ViewModel.Pagination
{
    public class PaginationInfo
    {
        public PaginationInfo() 
        { 
            Page= 0;
            PageSize = 10;
            Previous= string.Empty;
            Next= string.Empty;
        }
        public long TotalItems { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public string Previous { get; set; }
        public string Next { get; set; }
        public long HeaderTotalOrders { get; set; }
        public long HeaderTotalPrintedTags { get; set; }
        public long HeaderTotalNotPrintedTags { get; set; }
        public long HeaderTotalWithoutXml { get; set; }
        public long HeaderTotalOrdersCanceled { get; set; }
        public long HeaderOtherCarriers { get; set; }
    }
}
