using Lenovo.NAT.Services;
using Lenovo.NAT.Services.Admin;
using Lenovo.NAT.Services.Logistic;
using Lenovo.NAT.ViewModel.Logistic.Picking;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lenovo.NAT.Areas.Logistic.Controllers
{
    [Area("Logistic")]
    public class PickingController : Controller
    {
        private readonly IPickingService _pickingRequestService;
        private readonly IExcelExportService _excelExportService;

        public PickingController(IPickingService pickingRequestService, IExcelExportService excelExportService)
        {
            _pickingRequestService = pickingRequestService;
            _excelExportService = excelExportService;
        }

        private bool UserAllowed()
        {
            return true;
        }

        [HttpGet]
        public async Task<IActionResult> Index(PickingFilter? filterInfo, [FromQuery] int? pageSize, [FromQuery] int? page, string? errorMessage = null)
        {
            if (!UserAllowed())
                return RedirectToAction("Index", "Home", new { Area = "" });

            var domain = "LENOVO\\";
            var userNetworkId = User.Identity?.Name?.Replace(domain, string.Empty) ?? string.Empty;

            var key = userNetworkId;

            filterInfo.NetworkId = userNetworkId;

            var isSubmitedForm = pageSize == null && page == null;

            var atLeastOneFilterWasPassed = filterInfo.Process != null || filterInfo.Client != null || filterInfo.IdPickingStatus != null || filterInfo.IdPickingProcessType != null
                || filterInfo.IdPickingArea != null || filterInfo.CreatedOn != null;

            if (atLeastOneFilterWasPassed || isSubmitedForm)
            {
                // Save
                var str = JsonConvert.SerializeObject(filterInfo);
                HttpContext.Session.SetString(key, str);
                if (filterInfo.PaginationInfo == null)
                {
                    filterInfo.PaginationInfo = new ViewModel.Pagination.PaginationInfo();
                    filterInfo.PaginationInfo.PageSize = pageSize ?? 25;
                    filterInfo.PaginationInfo.Page = page ?? 0;
                }

                HttpContext.Session.SetString(key, JsonConvert.SerializeObject(filterInfo));
            }
            else
            {
                var filterInfoString = HttpContext.Session.GetString(key);

                var filterInfoSession = !string.IsNullOrEmpty(filterInfoString) ? JsonConvert.DeserializeObject<PickingFilter>(filterInfoString) : filterInfo;

                var isDifferentRoute = !(filterInfoSession?.PaginationInfo != null && filterInfoSession.PaginationInfo.PageSize == pageSize && filterInfoSession.PaginationInfo.Page == page);

                if (isDifferentRoute && !isSubmitedForm)
                    filterInfo = filterInfoSession;

            }

            var viewModel = await _pickingRequestService.GetPaginatedPickingViewModule(filterInfo, pageSize ?? filterInfo?.PaginationInfo?.PageSize ?? 25, page ?? filterInfo?.PaginationInfo?.Page ?? 0);

            ViewBag.Alert = errorMessage;

            return View(viewModel);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var domain = "LENOVO\\";
            var userNetworkId = User.Identity?.Name?.Replace(domain, string.Empty) ?? string.Empty;

            var model = await _pickingRequestService.GetPickingDetail(int.Parse(id), userNetworkId);

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var domain = "LENOVO\\";
            var userNetworkId = User.Identity?.Name?.Replace(domain, string.Empty) ?? string.Empty;

            var model = await _pickingRequestService.GetEmptyPicking(userNetworkId);

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNew(PickingDetailsViewModel form)
        {
            var domain = "LENOVO\\";
            var userNetworkId = User.Identity?.Name?.Replace(domain, string.Empty) ?? string.Empty;

            var errorMessage = await _pickingRequestService.CreateNew(form);

            return RedirectToAction("Index", "Picking", new { filterInfo = new PickingFilter(), pageSize = 10, page = 0, errorMessage });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task SaveDetails(PickingDetailsViewModel form)
        {
            var domain = "LENOVO\\";
            var userNetworkId = User.Identity?.Name?.Replace(domain, string.Empty) ?? string.Empty;

            form.NetworkId = userNetworkId;

            await _pickingRequestService.SaveDetails(form);
        }
        
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task UploadAttachment(PickingInvoiceDetailsViewModel form)
        {
            var domain = "LENOVO\\";
            var userNetworkId = User.Identity?.Name?.Replace(domain, string.Empty) ?? string.Empty;

            await _pickingRequestService.UploadAttachment(form);
        }

        public async Task UpdateTotalValue(PickingInvoiceDetailsViewModel form)
        {
            var domain = "LENOVO\\";
            var userNetworkId = User.Identity?.Name?.Replace(domain, string.Empty) ?? string.Empty;

            await _pickingRequestService.UpdateTotalValue(form);
        }

        [Route("Logistic/Picking/DownloadApprovalFile/{id?}")]
        public async Task<IActionResult> DownloadApprovalFile(long id)
        {
            var domain = "LENOVO\\";
            var userNetworkId = User.Identity?.Name?.Replace(domain, string.Empty) ?? string.Empty;

            var invoice = await _pickingRequestService.GetPickingInvoice(id);
            var fileName = invoice.ApprovalFileName;
            
            return File(invoice.ApprovalFile, "model/obj", fileName);
        }

        [Route("Logistic/Picking/DownloadReturnFile/{id?}")]
        public async Task<IActionResult> DownloadReturnFile(long id)
        {
            var domain = "LENOVO\\";
            var userNetworkId = User.Identity?.Name?.Replace(domain, string.Empty) ?? string.Empty;

            var invoice = await _pickingRequestService.GetPickingInvoice(id);
            var fileName = invoice.ReturnFileName;

            return File(invoice.ReturnFile, "model/obj", fileName);
        }

        [Route("Logistic/Picking/DownloadSeriesLetterFile/{id?}")]
        public async Task<IActionResult> DownloadSeriesLetterFile(long id)
        {
            var domain = "LENOVO\\";
            var userNetworkId = User.Identity?.Name?.Replace(domain, string.Empty) ?? string.Empty;

            var invoice = await _pickingRequestService.GetPickingInvoice(id);
            var fileName = invoice.SeriesLetterFileName;

            return File(invoice.SeriesLetterFile, "model/obj", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(PickingFilter filterInfo)
        {
            var domain = "LENOVO\\";
            var userNetworkId = User.Identity?.Name?.Replace(domain, string.Empty) ?? string.Empty;

            var allPicking = await _pickingRequestService.GetAllPicking(filterInfo);

            var columnNames = new List<string>() { "Status", "Area", "Process Type", "Validation Date", "Created By", "Process", "Client", "City", "UF", "Contact", "Email", "Telephone", "Origin Invoice", "Return Order", "Invoice Return", "PN", "MTM", "Quantity", "Item Value", "Brand", "Tax Validation", "Reason", "Carrier Name", "Invoice Return Date", "Carrier Request Date", "Expected Picking Date", "Effective Picking Date", "Expected Return Date", "Effective Return Date", "End Date", "Obs" };

            var workbook = _excelExportService.WriteExcelWithNPOI(allPicking.ToList(), columnNames);

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            MemoryStream tempStream = null;
            MemoryStream stream = null;

            try
            {
                tempStream = new MemoryStream();
                workbook.Write(tempStream);

                var byteArray = tempStream.ToArray();

                stream = new MemoryStream();
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Seek(0, SeekOrigin.Begin);

                return File(fileContents: stream.ToArray(), contentType: contentType, fileDownloadName: "Picking_" + DateTime.UtcNow.AddHours(-3).ToString() + ".xlsx");
            }
            finally
            {
                if (tempStream != null) tempStream.Dispose();
                if (stream != null) stream.Dispose();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AdminExportExcel(PickingFilter filterInfo)
        {
            var domain = "LENOVO\\";
            var userNetworkId = User.Identity?.Name?.Replace(domain, string.Empty) ?? string.Empty;

            var allPicking = await _pickingRequestService.GetAllPickingAdmin(filterInfo);

            var columnNames = new List<string>() { "Area", "Process Type", "Validation Date", "Created By", "Process", "Client", "City", "UF", "Contact", "Email", "Telephone", "Origin Invoice", "Return Order", "Invoice Return", "PN", "MTM", "Quantity", "Item Value", "Tax Validation", "Reason", "Carrier Name", "Invoice Return Date", "Carrier Request Date", "Expected Picking Date", "Expected Return Date" };

            var workbook = _excelExportService.WriteExcelWithNPOI(allPicking.ToList(), columnNames);

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            MemoryStream tempStream = null;
            MemoryStream stream = null;

            try
            {
                tempStream = new MemoryStream();
                workbook.Write(tempStream);

                var byteArray = tempStream.ToArray();

                stream = new MemoryStream();
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Seek(0, SeekOrigin.Begin);

                return File(fileContents: stream.ToArray(), contentType: contentType, fileDownloadName: "Picking_" + DateTime.UtcNow.AddHours(-3).ToString() + ".xlsx");
            }
            finally
            {
                if (tempStream != null) tempStream.Dispose();
                if (stream != null) stream.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(PaginatedPickingViewModel model)
        {
            var domain = "LENOVO\\";
            var userNetworkId = User.Identity?.Name?.Replace(domain, string.Empty) ?? string.Empty;

            model.NetworkId = userNetworkId;
            var message = string.Empty;

            if (model.File == null)
            {
                message = "You must choose a file to be Uploaded.";
                return RedirectToAction("Index", "Picking", new { message });
            }
           
            var errorMessages = await _pickingRequestService.UploadFileV2(model);

            if(errorMessages.Count == 0)
                return RedirectToAction("Index", "Picking", new { message });

            var columnNames = new List<string>() { "Process", "Error Message"};

            var workbook = _excelExportService.WriteExcelWithNPOI(errorMessages.ToList(), columnNames);

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            MemoryStream tempStream = null;
            MemoryStream stream = null;

            try
            {
                tempStream = new MemoryStream();
                workbook.Write(tempStream);

                var byteArray = tempStream.ToArray();

                stream = new MemoryStream();
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Seek(0, SeekOrigin.Begin);

                return File(fileContents: stream.ToArray(), contentType: contentType, fileDownloadName: "TrackingError_" + DateTime.UtcNow.AddHours(-3).ToString() + ".xlsx");
            }
            finally
            {
                if (tempStream != null) tempStream.Dispose();
                if (stream != null) stream.Dispose();
            }
        }
    }
}
