using Lenovo.NAT.ViewModel.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Lenovo.NAT.ViewModel.Logistic.Picking
{
    public class LogisticInvoiceFilter
    {
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ProcessedOn { get; set; }
        public string? AccessKey { get; set; }
        public List<SelectListItem> FileSent { get; set; }
        public bool? FileSentOption { get; set; }
        public List<SelectListItem> ConditionCodes { get; set; }
        public string? ConditionCodeSelected { get; set; }
        public string? AirwayBillNumber { get; set; }
        public PaginationInfo? PaginationInfo { get; set; }
    }
}
