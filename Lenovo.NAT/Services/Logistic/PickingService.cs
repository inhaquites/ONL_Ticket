using AutoMapper;
using Lenovo.NAT.Infrastructure.Entities.Logistic;
using Lenovo.NAT.Infrastructure.Repositories.Logistic;
using Lenovo.NAT.ViewModel.Email;
using Lenovo.NAT.ViewModel.Logistic.Picking;
using Lenovo.NAT.ViewModel.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System.Data;

namespace Lenovo.NAT.Services.Logistic
{
    public interface IPickingService
    {
        Task<PaginatedPickingViewModel> GetPaginatedPickingViewModule(PickingFilter filterInfo, int pageSize, int page);
        Task<PickingViewModel> GetPickingDetail(int id, string networkId);
        Task<PickingViewModel> GetEmptyPicking(string user);
        Task<PickingInvoice> GetPickingInvoice(long id);
        Task<PickingCarrierViewModel> GetPickingCarrierViewModel(int id);
        Task<string> SavePickingCarrier(PickingCarrierViewModel carrier);
        Task<string> CreatePickingCarrier(string name, string contactEmail);
        Task UploadAttachment(PickingInvoiceDetailsViewModel form);
        Task UpdateTotalValue(PickingInvoiceDetailsViewModel form);
        Task<string> Delete(int id);
        Task<IEnumerable<PickingCarrierViewModel>> GetAllPickingCarrierViewModel();
        Task SaveDetails(PickingDetailsViewModel form);
        Task<string> CreateNew(PickingDetailsViewModel form);
        Task<IEnumerable<PickingExcelTemplateViewModel>> GetAllPicking(PickingFilter filterInfo);
        Task<IEnumerable<PickingExcelAdminTemplateViewModel>> GetAllPickingAdmin(PickingFilter filterInfo);
        Task<Dictionary<string, string>> UploadFile(PaginatedPickingViewModel paginatedPickingViewModel);
        Task<Dictionary<string, string>> UploadFileV2(PaginatedPickingViewModel paginatedPickingViewModel);
        Task<PaginatedLogisticInvoiceViewModel> GetPaginatedLogisticInvoiceViewModule(LogisticInvoiceFilter filterInfo, int pageSize, int page);
        Task<IEnumerable<LogisticInvoiceViewModel>> GetAllInvoicesViewModel(LogisticInvoiceFilter filterInfo);
    }

    public class PickingService : IPickingService
    {
        private readonly IPickingRepository _pickingRepository;
        private readonly IExcelExportService _excelExportService;
        private readonly IMapper _mapper;

        public PickingService(IPickingRepository pickingRepository, IMapper mapper, IExcelExportService excelExportService)
        {
            _pickingRepository = pickingRepository;
            _mapper = mapper;
            _excelExportService = excelExportService;
        }

        public async Task<PaginatedPickingViewModel> GetPaginatedPickingViewModule(PickingFilter filterInfo, int pageSize, int page)
        {
            var pickingList = await _pickingRepository.GetPickingFiltered(filterInfo, pageSize, page);

            var pickingViewModelList = _mapper.Map<IEnumerable<PickingViewModel>>(pickingList);
            var processTypes = _mapper.Map<IEnumerable<PickingProcessTypeViewModel>>(await _pickingRepository.GetAllProcessTypes());
            var pickingAreas = _mapper.Map<IEnumerable<PickingAreaViewModel>>(await _pickingRepository.GetAllPickingAreas());
            var pickingStatus = _mapper.Map<IEnumerable<PickingStatusViewModel>>(await _pickingRepository.GetPickingStatus());

            foreach (var picking in pickingViewModelList)
            {
                picking.ProcessType = processTypes.First(x => x.Id == picking.IdPickingProcessType).Name;
                picking.Area = pickingAreas.First(x => x.Id == picking.IdPickingArea).Name;
                picking.PickingStatus = GetPickingStatus(picking.IdPickingStatus);
                picking.UserCanAddItems = picking.IdPickingStatus == (int)PickingStatusEnum.Pending || picking.IdPickingStatus == (int)PickingStatusEnum.Correction;
            }

            var paginationInfo = new PaginationInfo();
            paginationInfo.TotalItems = await _pickingRepository.CountPickingRequests(filterInfo);
            paginationInfo.Page = page;
            paginationInfo.TotalPages = (int)(paginationInfo.TotalItems / pageSize) + 1;
            paginationInfo.PageSize = pageSize;

            filterInfo.ProcessTypes = processTypes;
            filterInfo.PickingAreas = pickingAreas;
            filterInfo.PickingStatus = pickingStatus;
            filterInfo.PaginationInfo = paginationInfo;
            filterInfo.IsAdmin = true; //await _userService.IsAllowed(filterInfo.NetworkId ?? string.Empty, "Admin");

            return new PaginatedPickingViewModel(paginationInfo, filterInfo, pickingViewModelList);
        }

        public async Task<PickingViewModel> GetPickingDetail(int id, string networkId)
        {
            var picking = await _pickingRepository.GetPickingById(id);
            var pickingInvoices = await _pickingRepository.GetPickingInvoices(id);
            var pickingItems = await _pickingRepository.GetPickingItems(id);
            var pickingBrands = await _pickingRepository.GetPickingBrands();

            var pickingViewModel = _mapper.Map<PickingViewModel>(picking);
            var processTypes = _mapper.Map<IEnumerable<PickingProcessTypeViewModel>>(await _pickingRepository.GetAllProcessTypes());
            var pickingAreas = _mapper.Map<IEnumerable<PickingAreaViewModel>>(await _pickingRepository.GetAllPickingAreas());
            var reasons = _mapper.Map<IEnumerable<PickingReasonViewModel>>(await _pickingRepository.GetAllPickingReasons());

            pickingViewModel.PickingItem.Processes = pickingInvoices.Select(invoice => invoice.InvoiceReturn).Distinct();
            pickingViewModel.PickingInvoices = _mapper.Map<IEnumerable<PickingInvoiceViewModel>>(pickingInvoices);
            pickingViewModel.PickingItems = _mapper.Map<IEnumerable<PickingItemViewModel>>(pickingItems);
            pickingViewModel.ProcessTypes = processTypes;
            pickingViewModel.ProcessType = processTypes.First(x => x.Id == pickingViewModel.IdPickingProcessType).Name;
            pickingViewModel.Areas = pickingAreas;
            pickingViewModel.Area = pickingAreas.First(x => x.Id == pickingViewModel.IdPickingArea).Name;
            pickingViewModel.PickingItem.Brands = _mapper.Map<IEnumerable<PickingBrandViewModel>>(pickingBrands);
            pickingViewModel.Carriers = _mapper.Map<IEnumerable<PickingCarrierViewModel>>(await _pickingRepository.GetAllPickingCarriers());
            pickingViewModel.PickingStatus = GetPickingStatus(pickingViewModel.IdPickingStatus);
            pickingViewModel.UserCanAddItems = (picking.IdPickingStatus == (int)PickingStatusEnum.Pending || picking.IdPickingStatus == (int)PickingStatusEnum.Correction) && picking.CreatedBy == networkId;
            pickingViewModel.UserCanChangeHeaderInformation = picking.IdPickingStatus == (int)PickingStatusEnum.Correction && picking.CreatedBy == networkId;
            pickingViewModel.UserIsAdmin = true;//await _userService.IsAllowed(networkId, "Admin");
            pickingViewModel.Reasons = reasons;

            foreach (var item in pickingViewModel.PickingItems)
            {
                item.Brand = pickingBrands.FirstOrDefault(brand => brand.Id == item.IdBrand)?.Name ?? string.Empty;
            }
            return pickingViewModel;
        }

        public async Task<PickingViewModel> GetEmptyPicking(string user)
        {
            var pickingViewModel = new PickingViewModel();

            var pickingBrands = await _pickingRepository.GetPickingBrands();

            var processTypes = _mapper.Map<IEnumerable<PickingProcessTypeViewModel>>(await _pickingRepository.GetAllProcessTypes());
            var pickingAreas = _mapper.Map<IEnumerable<PickingAreaViewModel>>(await _pickingRepository.GetAllPickingAreas());

            pickingViewModel.CreatedBy = user;
            pickingViewModel.ProcessTypes = processTypes;
            pickingViewModel.Areas = pickingAreas;
            pickingViewModel.PickingItem.Brands = _mapper.Map<IEnumerable<PickingBrandViewModel>>(pickingBrands);
            pickingViewModel.Reasons = _mapper.Map<IEnumerable<PickingReasonViewModel>>(await _pickingRepository.GetAllPickingReasons());
            pickingViewModel.PickingStatus = "Pending";

            return pickingViewModel;
        }

        public async Task<PickingInvoice> GetPickingInvoice(long id)
        {
            return await _pickingRepository.GetPickingInvoice(id);
        }

        public async Task<string> CreateNew(PickingDetailsViewModel form)
        {
            var errorMessage = string.Empty;

            try
            {
                var newPicking = new Picking();

                form.Process = form?.Process?.Replace(" ", "") ?? string.Empty;

                if (_pickingRepository.HasExistingPickingForThisProcess(form.Process))
                    return $"Já existe uma coleta para o Processo {form.Process}!";

                var currentDate = DateTime.UtcNow.AddHours(-3);

                newPicking.CreatedBy = form.CreatedBy ?? string.Empty;
                newPicking.CreatedOn = form.CreatedOn ?? currentDate;
                newPicking.UpdatedOn = form.CreatedOn ?? currentDate;
                newPicking.Process = form.Process ?? string.Empty;
                newPicking.IdPickingArea = form.IdPickingArea ?? int.MinValue;
                newPicking.IdPickingProcessType = form.IdPickingProcessType ?? int.MinValue;
                newPicking.Client = form.Client ?? string.Empty;
                newPicking.City = form.City ?? string.Empty;
                newPicking.UF = form.UF ?? string.Empty;
                newPicking.Contact = form.Contact ?? string.Empty;
                newPicking.Email = form.Email ?? string.Empty;
                newPicking.Telephone = form.Telephone ?? string.Empty;
                newPicking.Reason = form.Reason ?? string.Empty;

                await _pickingRepository.CreateNewPicking(newPicking);

                var idPicking = newPicking.Id;

                var newInvoices = GetNewInvoices(form, idPicking);

                await _pickingRepository.CreateNewInvoices(newInvoices);

                var invoiceDic = new Dictionary<string, long>();

                foreach (var invoice in newInvoices)
                {
                    if (invoiceDic.ContainsKey(invoice.InvoiceReturn))
                        continue;

                    invoiceDic.Add(invoice.InvoiceReturn, invoice.Id);
                }

                var newItems = GetNewItems(form, idPicking, invoiceDic);

                await _pickingRepository.CreateNewItems(newItems);
            }
            catch (Exception ex)
            {

                return "Erro to save the Picking request";
            }

            return errorMessage;
        }

        public async Task SaveDetails(PickingDetailsViewModel form)
        {
            try
            {
                var idPicking = form.Id;

                var picking = await _pickingRepository.GetPickingById(idPicking);

                if (form.IdPickingStatus == (int)PickingStatusEnum.Canceled)
                {
                    picking.IdPickingStatus = (int)PickingStatusEnum.Canceled;
                    picking.UpdatedOn = DateTime.UtcNow.AddHours(-3);
                    picking.UpdatedBy = form.NetworkId;

                    if (!string.IsNullOrEmpty(form.AdminObservation))
                    {
                        picking.CorrectionObservation = string.IsNullOrEmpty(picking.CorrectionObservation)
                        ? $"{DateTime.UtcNow.AddHours(-3).ToShortDateString()} - {form.AdminObservation}"
                        : $"{picking.CorrectionObservation} | {DateTime.UtcNow.AddHours(-3).ToShortDateString()} - {form.AdminObservation}";
                    }

                    await _pickingRepository.UpdatePicking(picking);

                    return;
                }

                if (form.IdPickingStatus == null && picking.IdPickingStatus == (int)PickingStatusEnum.Correction)
                {
                    picking.IdPickingStatus = (int)PickingStatusEnum.Pending;
                    picking.IdPickingArea = form.IdPickingArea ?? int.MinValue;
                    picking.IdPickingProcessType = form.IdPickingProcessType ?? int.MinValue;
                    picking.Client = form.Client ?? string.Empty;
                    picking.City = form.City ?? string.Empty;
                    picking.UF = form.UF ?? string.Empty;
                    picking.Contact = form.Contact ?? string.Empty;
                    picking.Email = form.Email ?? string.Empty;
                    picking.Telephone = form.Telephone ?? string.Empty;
                    picking.Reason = form.Reason ?? string.Empty;

                    picking.UpdatedBy = form.NetworkId;
                    picking.UpdatedOn = DateTime.UtcNow.AddHours(-3);

                    var newObservation = $"{DateTime.UtcNow.AddHours(-3).ToShortDateString()}({form.NetworkId}) - {form.CorrectionObservation}";
                    picking.CorrectionObservation = string.IsNullOrEmpty(picking.CorrectionObservation) ? newObservation : $"{picking.CorrectionObservation} | {newObservation}";

                    await _pickingRepository.UpdatePicking(picking);
                }

                else if (form.IdPickingStatus == (int)PickingStatusEnum.Correction)
                {
                    var newObservation = $"{DateTime.UtcNow.AddHours(-3).ToShortDateString()}({form.NetworkId}) - {form.CorrectionObservation}";

                    picking.UpdatedBy = form.NetworkId;
                    picking.UpdatedOn = DateTime.UtcNow.AddHours(-3);
                    picking.IdPickingStatus = (int)PickingStatusEnum.Correction;
                    picking.CorrectionObservation = string.IsNullOrEmpty(picking.CorrectionObservation) ? newObservation : $"{ picking.CorrectionObservation } | { newObservation }";

                    await _pickingRepository.UpdatePicking(picking);
                }
                else if (form.IdPickingStatus == (int)PickingStatusEnum.PickingProcessing && !string.IsNullOrEmpty(form.CarrierName))
                {
                    picking.IdPickingStatus = (int)PickingStatusEnum.PickingProcessing;
                    picking.CarrierName = form.CarrierName;
                    picking.CarrierRequestDate = DateTime.UtcNow.AddHours(-3).Date;
                    picking.ExpectedPickingDate = form.ExpectedPickingDate;
                    picking.ExpectedReturnDate = form.ExpectedReturnDate;

                    if (!string.IsNullOrEmpty(form.AdminObservation))
                    {
                        var newAdminObservation = $"{DateTime.UtcNow.AddHours(-3).ToShortDateString()}({form.NetworkId}) - {form.AdminObservation}";

                        picking.CorrectionObservation = string.IsNullOrEmpty(picking.CorrectionObservation) ? newAdminObservation : $"{ picking.CorrectionObservation } | { newAdminObservation }";
                    }

                    await _pickingRepository.UpdatePicking(picking);

                    var emailContact = _pickingRepository.GetEmailContactFromCarrier(form.CarrierName);

                    var newFilterInfo = new PickingFilter() { Process = picking.Process };
                    var pickingToSendViaEmail = await GetAllPickingAdmin(newFilterInfo);

                    var columnNames = new List<string>() { "Area", "Process Type", "Created On", "Created By", "Process", "Client", "City", "UF", "Contact", "Email", "Telephone", "Origin Invoice", "Return Order", "Invoice Return", "PN", "MTM", "Quantity", "Item Value", "Tax Validation", "Reason", "Carrier Name", "Invoice Return Date", "Carrier Request Date", "Expected Picking Date", "Expected Return Date" };

                    var workbook = _excelExportService.WriteExcelWithNPOI(pickingToSendViaEmail.ToList(), columnNames);

                    var attachedFile = GetFileByteArray(workbook);

                    var tableTemplate = CreateTableTemplate(pickingToSendViaEmail);

                    var emailHistory = GetEmailViewModel(emailContact, tableTemplate, picking, attachedFile);

                    //await _emailService.CreateEmail(emailHistory);
                }
                else if (form.IdPickingStatus == (int)PickingStatusEnum.Transit && form.EffectivePickingDate != null)
                {
                    picking.IdPickingStatus = (int)PickingStatusEnum.Transit;
                    picking.EffectivePickingDate = form.EffectivePickingDate;
                    picking.ExpectedReturnDate = form.ExpectedReturnDate;

                    if (!string.IsNullOrEmpty(form.AdminObservation))
                    {
                        var newAdminObservation = $"{DateTime.UtcNow.AddHours(-3).ToShortDateString()}({form.NetworkId}) - {form.AdminObservation}";

                        picking.CorrectionObservation = string.IsNullOrEmpty(picking.CorrectionObservation) ? newAdminObservation : $"{picking.CorrectionObservation} | {newAdminObservation}";
                    }

                    await _pickingRepository.UpdatePicking(picking);
                }
                else if (form.IdPickingStatus == (int)PickingStatusEnum.ReceivingProcessing &&  form.EffectiveReturnDate != null)
                {
                    picking.EffectiveReturnDate = form.EffectiveReturnDate;
                    picking.ExpectedReturnDate = form.ExpectedReturnDate;

                    if (!string.IsNullOrEmpty(form.AdminObservation))
                    {
                        var newAdminObservation = $"{DateTime.UtcNow.AddHours(-3).ToShortDateString()}({form.NetworkId}) - {form.AdminObservation}";

                        picking.CorrectionObservation = string.IsNullOrEmpty(picking.CorrectionObservation) ? newAdminObservation : $"{picking.CorrectionObservation} | {newAdminObservation}";
                    }

                    await _pickingRepository.UpdatePicking(picking);

                    if(picking.ExpectedReturnDate != null && picking.EffectiveReturnDate != null)
                    {
                        picking.IdPickingStatus = (int)PickingStatusEnum.ReceivingProcessing;
                        await _pickingRepository.UpdatePicking(picking);
                    }
                }
                else if (form.IdPickingStatus == (int)PickingStatusEnum.ProcessFinished && form.EndDate != null)
                {
                    picking.IdPickingStatus = (int)PickingStatusEnum.ProcessFinished;
                    picking.EndDate = form.EndDate;

                    if (!string.IsNullOrEmpty(form.AdminObservation))
                    {
                        var newAdminObservation = $"{DateTime.UtcNow.AddHours(-3).ToShortDateString()}({form.NetworkId}) - {form.AdminObservation}";

                        picking.CorrectionObservation = string.IsNullOrEmpty(picking.CorrectionObservation) ? newAdminObservation : $"{picking.CorrectionObservation} | {newAdminObservation}";
                    }

                    await _pickingRepository.UpdatePicking(picking);
                }

                var pickingInvoices = await _pickingRepository.GetPickingInvoices(idPicking);
                var pickingItems = await _pickingRepository.GetPickingItems(idPicking);

                var invoiceIdsFromForm = form.Invoices != null && form.Invoices.Any() ? form.Invoices.Where(x => x.Id > 0).Select(x => x.Id) : new List<long>();
                var itemIdsFromForm = form.Items != null && form.Items.Any() ? form.Items.Where(x => x.Id > 0).Select(x => x.Id) : new List<long>();

                var invoicesToBeDeleted = pickingInvoices.Where(invoice => !invoiceIdsFromForm.Contains(invoice.Id));
                var itemsToBeDeleted = pickingItems.Where(item => !itemIdsFromForm.Contains(item.Id));

                await _pickingRepository.DeleteItems(itemsToBeDeleted);
                await _pickingRepository.DeleteInvoices(invoicesToBeDeleted);

                var newInvoices = GetNewInvoices(form, idPicking);

                await _pickingRepository.CreateNewInvoices(newInvoices);

                var invoiceDic = new Dictionary<string, long>();

                foreach (var invoice in newInvoices)
                {
                    if (invoiceDic.ContainsKey(invoice.InvoiceReturn))
                        continue;

                    invoiceDic.Add(invoice.InvoiceReturn, invoice.Id);
                }

                var newItems = GetNewItems(form, idPicking, invoiceDic);

                await _pickingRepository.CreateNewItems(newItems);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UploadAttachment(PickingInvoiceDetailsViewModel form)
        {
            try
            {
                var pickingInvoiceDB = await _pickingRepository.GetPickingInvoice(form.Id);

                if (!string.IsNullOrEmpty(form.ApprovalFileName))
                {
                    pickingInvoiceDB.ApprovalFileName = form.ApprovalFileName;
                    pickingInvoiceDB.ApprovalFile = GetByteArrayFile(form.ApprovalFile);

                    await _pickingRepository.UpdatePickingInvoice(pickingInvoiceDB);
                }
                else if (!string.IsNullOrEmpty(form.SeriesLetterFileName))
                {
                    pickingInvoiceDB.SeriesLetterFileName = form.SeriesLetterFileName;
                    pickingInvoiceDB.SeriesLetterFile = GetByteArrayFile(form.SeriesLetterFile);

                    await _pickingRepository.UpdatePickingInvoice(pickingInvoiceDB);
                }
                else if (!string.IsNullOrEmpty(form.ReturnFileName))
                {
                    pickingInvoiceDB.ReturnFileName = form.ReturnFileName;
                    pickingInvoiceDB.ReturnFile = GetByteArrayFile(form.InvoiceReturnFile);

                    await _pickingRepository.UpdatePickingInvoice(pickingInvoiceDB);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task UpdateTotalValue(PickingInvoiceDetailsViewModel form)
        {
            try
            {
                var pickingInvoiceDB = await _pickingRepository.GetPickingInvoice(form.Id);
                pickingInvoiceDB.TotalValue = form.TotalValue;

                await _pickingRepository.UpdatePickingInvoice(pickingInvoiceDB);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PickingExcelTemplateViewModel>> GetAllPicking(PickingFilter filterInfo)
        {
            return await _pickingRepository.GetAllPicking(filterInfo);
        }

        public async Task<IEnumerable<PickingExcelAdminTemplateViewModel>> GetAllPickingAdmin(PickingFilter filterInfo)
        {
            return await _pickingRepository.GetAllPickingAdmin(filterInfo);
        }

        public async Task<Dictionary<string, string>> UploadFile(PaginatedPickingViewModel paginatedPickingViewModel)
        {
            var errorMessage = new Dictionary<string, string>();

            try
            {
                var files = GetByteArray(paginatedPickingViewModel.File);

                var dataTable = GetDataTableFromFile(files);

                var pickings = GetFromDataTable(dataTable);

                if (!pickings.Any())
                    return errorMessage;

                var updateTime = DateTime.UtcNow.AddHours(-3);

                foreach(var picking in pickings)
                {
                    var dbPicking = await _pickingRepository.GetPickingByProcess(picking.Process);

                    if (dbPicking == null)
                    {
                        if (errorMessage.ContainsKey(picking.Process))
                            errorMessage.Add(picking.Process, "Process included more than one time.");
                        else
                            errorMessage.Add(picking.Process, "Process not found on database.");

                        continue;
                    }

                    dbPicking.UpdatedBy = paginatedPickingViewModel.NetworkId;
                    dbPicking.UpdatedOn = updateTime;

                    if (picking.EffectivePickingDate != null && dbPicking.EffectivePickingDate == null)
                    {
                        if (picking.EffectivePickingDate.Value.Date < dbPicking.CreatedOn.Date)
                        {
                            if (errorMessage.ContainsKey(picking.Process))
                                errorMessage.Add(picking.Process, "Process included more than one time.");
                            else
                                errorMessage.Add(picking.Process, "Effective Picking Date must be greater than CreatedOn.");

                            continue;
                        }

                        if (DateIsGreatedThanToday(picking.EffectivePickingDate))
                        {
                            if (errorMessage.ContainsKey(picking.Process))
                                errorMessage.Add(picking.Process, "Process included more than one time.");
                            else
                                errorMessage.Add(picking.Process, "Effective Picking Date must be lower than today.");

                            continue;
                        }

                        dbPicking.IdPickingStatus = (int)PickingStatusEnum.Transit;
                        dbPicking.EffectivePickingDate = picking.EffectivePickingDate;

                        await _pickingRepository.UpdatePicking(dbPicking);
                    }

                    if (picking.EffectiveReturnDate != null && dbPicking.EffectiveReturnDate == null)
                    {
                        if (picking.EffectiveReturnDate.Value.Date < dbPicking.CreatedOn.Date)
                        {
                            if (errorMessage.ContainsKey(picking.Process))
                                errorMessage.Add(picking.Process, "Process included more than one time.");
                            else
                                errorMessage.Add(picking.Process, "Effective Return Date must be greater than CreatedOn.");

                            continue;
                        }

                        if (DateIsGreatedThanToday(picking.EffectiveReturnDate))
                        {
                            if (errorMessage.ContainsKey(picking.Process))
                                errorMessage.Add(picking.Process, "Process included more than one time.");
                            else
                                errorMessage.Add(picking.Process, "Effective Return Date must be lower than today.");

                            continue;
                        }

                        dbPicking.EffectiveReturnDate = picking.EffectiveReturnDate;

                        if (dbPicking.ExpectedReturnDate != null && dbPicking.EffectivePickingDate != null)
                        {
                            dbPicking.IdPickingStatus = (int)PickingStatusEnum.ReceivingProcessing;
                        }

                        await _pickingRepository.UpdatePicking(dbPicking);
                    }

                    if(picking.EndDate != null && dbPicking.EndDate == null)
                    {
                        if (picking.EndDate.Value.Date < dbPicking.CreatedOn.Date)
                        {
                            if (errorMessage.ContainsKey(picking.Process))
                                errorMessage.Add(picking.Process, "Process included more than one time.");
                            else
                                errorMessage.Add(picking.Process, "End Date must be greater than CreatedOn.");

                            continue;
                        }

                        if (DateIsGreatedThanToday(picking.EndDate))
                        {
                            if (errorMessage.ContainsKey(picking.Process))
                                errorMessage.Add(picking.Process, "Process included more than one time.");
                            else
                                errorMessage.Add(picking.Process, "End Date must be lower than today.");

                            continue;
                        }

                        dbPicking.EndDate = picking.EndDate;

                        if (dbPicking.EffectivePickingDate != null && dbPicking.EffectiveReturnDate != null)
                        {
                            dbPicking.IdPickingStatus = (int)PickingStatusEnum.ProcessFinished;
                        }

                        await _pickingRepository.UpdatePicking(dbPicking);
                    }
                }
            }
            catch { }

            return errorMessage;
        }

        public async Task<Dictionary<string, string>> UploadFileV2(PaginatedPickingViewModel paginatedPickingViewModel)
        {
            var errorMessage = new Dictionary<string, string>();

            try
            {
                var files = GetByteArray(paginatedPickingViewModel.File);

                var dataTable = GetDataTableFromFile(files);

                var pickings = GetFromDataTable(dataTable);

                if (!pickings.Any())
                    return errorMessage;

                foreach (var picking in pickings)
                {
                    var dbPicking = await _pickingRepository.GetPickingByProcess(picking.Process);

                    if (dbPicking == null)
                    {
                        if (errorMessage.ContainsKey(picking.Process))
                            errorMessage.Add(picking.Process, "Process included more than one time.");
                        else
                            errorMessage.Add(picking.Process, "Process not found on database.");

                        continue;
                    }

                    dbPicking.UpdatedBy = paginatedPickingViewModel.NetworkId;
                    //dbPicking.UpdatedOn = updateTime;

                    if (picking.EffectivePickingDate != null)
                    {
                        if (picking.EffectivePickingDate.Value.Date < dbPicking.CreatedOn.Date)
                        {
                            if (errorMessage.ContainsKey(picking.Process))
                                errorMessage.Add(picking.Process, "Process included more than one time.");
                            else
                                errorMessage.Add(picking.Process, "Effective Picking Date must be greater than CreatedOn.");

                            continue;
                        }

                        if (DateIsGreatedThanToday(picking.EffectivePickingDate))
                        {
                            if (errorMessage.ContainsKey(picking.Process))
                                errorMessage.Add(picking.Process, "Process included more than one time.");
                            else
                                errorMessage.Add(picking.Process, "Effective Picking Date must be lower than today.");

                            continue;
                        }

                        dbPicking.IdPickingStatus = (int)PickingStatusEnum.Transit;
                        dbPicking.EffectivePickingDate = picking.EffectivePickingDate;

                        await _pickingRepository.UpdatePicking(dbPicking);
                    }
                    else
                    {
                        dbPicking.EffectivePickingDate = null;

                        await _pickingRepository.UpdatePicking(dbPicking);
                    }

                    if (picking.EffectiveReturnDate != null)
                    {
                        if (picking.EffectiveReturnDate.Value.Date < dbPicking.CreatedOn.Date)
                        {
                            if (errorMessage.ContainsKey(picking.Process))
                                errorMessage.Add(picking.Process, "Process included more than one time.");
                            else
                                errorMessage.Add(picking.Process, "Effective Return Date must be greater than CreatedOn.");

                            continue;
                        }

                        if (DateIsGreatedThanToday(picking.EffectiveReturnDate))
                        {
                            if (errorMessage.ContainsKey(picking.Process))
                                errorMessage.Add(picking.Process, "Process included more than one time.");
                            else
                                errorMessage.Add(picking.Process, "Effective Return Date must be lower than today.");

                            continue;
                        }

                        dbPicking.EffectiveReturnDate = picking.EffectiveReturnDate;

                        if (dbPicking.ExpectedReturnDate != null && dbPicking.EffectivePickingDate != null)
                        {
                            dbPicking.IdPickingStatus = (int)PickingStatusEnum.ReceivingProcessing;
                        }

                        await _pickingRepository.UpdatePicking(dbPicking);
                    }
                    else
                    {
                        dbPicking.EffectiveReturnDate = null;

                        await _pickingRepository.UpdatePicking(dbPicking);
                    }

                    if (picking.EndDate != null)
                    {
                        if (picking.EndDate.Value.Date < dbPicking.CreatedOn.Date)
                        {
                            if (errorMessage.ContainsKey(picking.Process))
                                errorMessage.Add(picking.Process, "Process included more than one time.");
                            else
                                errorMessage.Add(picking.Process, "End Date must be greater than CreatedOn.");

                            continue;
                        }

                        if (DateIsGreatedThanToday(picking.EndDate))
                        {
                            if (errorMessage.ContainsKey(picking.Process))
                                errorMessage.Add(picking.Process, "Process included more than one time.");
                            else
                                errorMessage.Add(picking.Process, "End Date must be lower than today.");

                            continue;
                        }

                        dbPicking.EndDate = picking.EndDate;

                        if (dbPicking.EffectivePickingDate != null && dbPicking.EffectiveReturnDate != null)
                        {
                            dbPicking.IdPickingStatus = (int)PickingStatusEnum.ProcessFinished;
                        }

                        await _pickingRepository.UpdatePicking(dbPicking);
                    }
                    else
                    {
                        dbPicking.EndDate = null;

                        await _pickingRepository.UpdatePicking(dbPicking);
                    }
                }
            }
            catch { }

            return errorMessage;
        }

        public async Task<IEnumerable<PickingCarrierViewModel>> GetAllPickingCarrierViewModel()
        {
            return _mapper.Map<IEnumerable<PickingCarrierViewModel>>(await _pickingRepository.GetAllPickingCarriers());
        }

        public async Task<PickingCarrierViewModel> GetPickingCarrierViewModel(int id)
        {
            return _mapper.Map<PickingCarrierViewModel>(await _pickingRepository.GetPickingCarrier(id));
        }

        public async Task<string> SavePickingCarrier(PickingCarrierViewModel carrier)
        {
            try
            {
                var pickingCarrierDB = await _pickingRepository.GetPickingCarrier(carrier.Id);

                pickingCarrierDB.ContactEmails = carrier.ContactEmails;

                await _pickingRepository.SavePickingCarrier(pickingCarrierDB);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }
        
        public async Task<string> Delete(int id)
        {
            try
            {
                var pickingCarrierDB = await _pickingRepository.GetPickingCarrier(id);

                if (await _pickingRepository.AtLeastOnePicking(pickingCarrierDB.Name))
                    return "Is not possible to delete a Carrier if there is at least one Picking Request";

                await _pickingRepository.Delete(pickingCarrierDB);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public async Task<string> CreatePickingCarrier(string name, string contactEmail)
        {
            try
            {
                var newPickingCarrier = new PickingCarrier();
                newPickingCarrier.Name = name.ToUpper();
                newPickingCarrier.ContactEmails = contactEmail;

                await _pickingRepository.NewPickingCarrier(newPickingCarrier);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public async Task<PaginatedLogisticInvoiceViewModel> GetPaginatedLogisticInvoiceViewModule(LogisticInvoiceFilter filterInfo, int pageSize, int page)
        {
            var logisticInvoices = await _pickingRepository.GetLogisticInvoice(filterInfo, pageSize, page);
            var conditionCodes = await _pickingRepository.GetConditionCodes();

            var pickingViewModelList = _mapper.Map<IEnumerable<LogisticInvoiceViewModel>>(logisticInvoices);

            var paginationInfo = new PaginationInfo();
            paginationInfo.TotalItems = await _pickingRepository.CountLogisticInvoices(filterInfo);
            paginationInfo.Page = page;
            paginationInfo.TotalPages = (int)(paginationInfo.TotalItems / pageSize) + 1;
            paginationInfo.PageSize = pageSize;

            filterInfo.FileSent = new List<SelectListItem>()
            {
                new SelectListItem(){  Text = "Yes", Value = "true" },
                new SelectListItem(){  Text = "No", Value = "false" }
            };

            filterInfo.ConditionCodes = conditionCodes.Select(x => new SelectListItem() { Text = x, Value = x }).ToList();

            filterInfo.PaginationInfo = paginationInfo;

            return new PaginatedLogisticInvoiceViewModel(paginationInfo, filterInfo, pickingViewModelList);
        }

        public async Task<IEnumerable<LogisticInvoiceViewModel>> GetAllInvoicesViewModel(LogisticInvoiceFilter filterInfo)
        {
            var logisticInvoices = await _pickingRepository.GetAllLogisticInvoices(filterInfo);

            return _mapper.Map<IEnumerable<LogisticInvoiceViewModel>>(logisticInvoices);
        }

        private bool DateIsGreatedThanToday(DateTime? date)
        {
            return date != null && date.Value.Date > DateTime.UtcNow.AddHours(-3).Date;
        }

        private List<ShortPickingViewModel> GetFromDataTable(DataTable? dataTable)
        {
            var pickings = new List<ShortPickingViewModel>();

            if (dataTable?.Rows == null)
                return pickings;

            foreach (DataRow row in dataTable.Rows)
            {
                var picking = new ShortPickingViewModel();

                try
                {
                    picking.Process = row?.ItemArray[0]?.ToString() ?? string.Empty;
                }
                catch { }

                try
                {
                    picking.EffectivePickingDate = Convert.ToDateTime(row?.ItemArray[1]?.ToString());
                }
                catch { }

                try
                {
                    picking.EffectiveReturnDate = Convert.ToDateTime(row?.ItemArray[2]?.ToString());
                }
                catch { }

                try
                {

                    picking.EndDate = Convert.ToDateTime(row?.ItemArray[3]?.ToString());
                }
                catch { }
                pickings.Add(picking);
            }

            return pickings;
        }

        private DataTable? GetDataTableFromFile(byte[] files)
        {
            var hasHeader = true;

            using (var excelPack = new ExcelPackage())
            {
                using (var stream = new MemoryStream(files))
                {
                    excelPack.Load(stream);
                }

                var ws = excelPack.Workbook.Worksheets[0];

                var excelasTable = new DataTable();

                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    if (string.IsNullOrEmpty(firstRowCell.Text))
                        continue;

                    var firstColumn = string.Format("Column {0}", firstRowCell.Start.Column);
                    excelasTable.Columns.Add(hasHeader ? firstRowCell.Text : firstColumn);
                }
                var startRow = hasHeader ? 2 : 1;

                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, excelasTable.Columns.Count];
                    var row = excelasTable.Rows.Add();

                    foreach (var cell in wsRow)
                        row[cell.Start.Column - 1] = cell.Value;
                }

                return excelasTable;
            }
        }

        private byte[] GetByteArray(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }

        private IEnumerable<PickingInvoice> GetNewInvoices(PickingDetailsViewModel form, long idPicking)
        {
            var newInvoices = new List<PickingInvoice>();

            if (form?.Invoices == null || !form.Invoices.Where(x => x.Id == 0).Any())
                return newInvoices;

            foreach (var invoice in form.Invoices.Where(x => x.Id == 0))
            {
                var newInvoice = new PickingInvoice()
                {
                    IdPicking = idPicking,
                    CreatedBy = string.Empty,
                    CreatedOn = DateTime.UtcNow.AddHours(-3),
                    InvoiceReturn = invoice.InvoiceReturn,
                    InvoiceReturnDate = invoice.InvoiceReturnDate,
                    ApprovalFile = GetByteArrayFile(invoice.ApprovalFile),
                    ApprovalFileName = invoice.ApprovalFileName,
                    ReturnFile = GetByteArrayFile(invoice.InvoiceReturnFile),
                    ReturnFileName = invoice.ReturnFileName,
                    SeriesLetterFile = GetByteArrayFile(invoice.SeriesLetterFile),
                    SeriesLetterFileName = invoice.SeriesLetterFileName,
                    Observation = invoice.Observation,
                    TotalValue = invoice.TotalValue
                };

                newInvoices.Add(newInvoice);
            }

            return newInvoices;
        }

        private IEnumerable<PickingItem> GetNewItems(PickingDetailsViewModel form, long idPicking, Dictionary<string, long> invoices)
        {
            var newItems = new List<PickingItem>();

            if (form?.Items == null || !form.Items.Where(x => x.Id == 0).Any())
                return newItems;

            foreach (var item in form.Items.Where(x => x.Id == 0))
            {
                long idInvoice;

                invoices.TryGetValue(item.InvoiceReturn, out idInvoice);

                var newItem = new PickingItem()
                {
                    IdPicking = idPicking,
                    CreatedBy = string.Empty,
                    CreatedOn = DateTime.UtcNow.AddHours(-3),
                    PartNumber = item.PartNumber,
                    MTM = item.MTM,
                    IdBrand = item.IdBrand,
                    ReturnOrder = item.ReturnOrder,
                    OriginInvoive = item.OriginInvoive,
                    Quantity = item.Quantity,
                    InvoiceReturn = item.InvoiceReturn,
                    IdPickingInvoice = idInvoice
                };

                newItems.Add(newItem);
            }

            return newItems;
        }

        private string GetPickingStatus(int idPickingStatus)
        {
            var status = "Pending";

            switch (idPickingStatus)
            {

                case (int)PickingStatusEnumViewModel.Pending:
                    status = "Pending";
                    break;
                case (int)PickingStatusEnumViewModel.Correction:
                    status = "Correction";
                    break;
                case (int)PickingStatusEnumViewModel.PickingProcessing:
                    status = "In Picking Processing";
                    break;
                case (int)PickingStatusEnumViewModel.Transit:
                    status = "In Transit";
                    break;
                case (int)PickingStatusEnumViewModel.ReceivingProcessing:
                    status = "In Receiving Processing";
                    break;
                case (int)PickingStatusEnumViewModel.ProcessFinished:
                    status = "Process Finished";
                    break;
                case (int)PickingStatusEnumViewModel.Canceled:
                    status = "Canceled";
                    break;
                default:
                    // code block
                    break;
            }

            return status;
        }

        private byte[] GetByteArrayFile(IFormFile file)
        {
            if (file == null)
                return null;

            byte[] fileBytes;

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            return fileBytes;
        }

        private byte[]? GetFileByteArray(IWorkbook workbook)
        {
            MemoryStream tempStream = null;
            MemoryStream stream = null;

            byte[]? attachedFile;

            try
            {
                tempStream = new MemoryStream();
                workbook.Write(tempStream);
                var byteArray = tempStream.ToArray();

                stream = new MemoryStream();
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Seek(0, SeekOrigin.Begin);

                attachedFile = stream.ToArray();
            }
            catch
            {
                attachedFile = null;
            }
            finally
            {
                if (tempStream != null) tempStream.Dispose();
                if (stream != null) stream.Dispose();
            }

            return attachedFile;
        }

        private EmailViewModel GetEmailViewModel(string emailContact, string tableTemplate, Picking picking, byte[]? attachedFile)
        {
            var opening = DateTime.UtcNow.AddHours(-3).Hour <= 11 ? "Bom dia" : "Boa tarde";
            var expectedPickingDateFormated = picking.ExpectedPickingDate.Value.ToString("dd/MM/yyyy");

            var emailViewModel = new EmailViewModel()
            {
                RoleId = "992f1344-dd6b-4478-b2a4-e1ce710c3c7b",
                From = "apaganuchi@lenovo.com",
                To = emailContact,
                Cc = "apaganuchi@lenovo.com",
                Subject = $"Solicitação de Coleta | {picking.Client} | {picking.City}/{picking.UF} | {picking.Process}",
                Body = $"" +
                $"  <html>" +
                $"      <head></head>" +
                $"      <body>" +
                $"          <p>{opening}!</p>" +
                $"          <p></p>" +
                $"          <p>Por favor, solicitar a coleta abaixo:</p>" +
                $"          <ul>" +
                $"              <li> Endereço de coleta: O mesmo endereço da NF/Declaração de devolução em anexo.</li>" +
                $"              <li> Reforço que precisamos seguir com a <em><strong>coleta até o dia {expectedPickingDateFormated}</strong></em>, conforme prazo de coleta <em><strong>(5 DIAS CORRIDOS)</em></strong>.</li>" +
                $"          </ul>" +
                $"          <p></p>" +
                $"          <p>{tableTemplate}</p>" +
                $"          <p></p>" +
                $"          <strong>ISO LAS Business Transformation<strong>" +
                $"      </body>" +
                $"  </html>",
                Attachments = new List<EmailAttachmentsViewModel>()
            };

            emailViewModel.Attachments.Add(new EmailAttachmentsViewModel() { AttachedFile = attachedFile, AttachedFileName = $"Picking-{picking.Process}.xlsx" });

            var pickingInvoices = _pickingRepository.GetPickingInvoices(picking.Id).Result;

            foreach(var invoice in pickingInvoices)
            {
                if(invoice.ApprovalFile != null && invoice.ApprovalFileName != null)
                {
                    emailViewModel.Attachments.Add(new EmailAttachmentsViewModel() { AttachedFile = invoice.ApprovalFile, AttachedFileName = invoice.ApprovalFileName });
                }

                if (invoice.SeriesLetterFile != null && invoice.SeriesLetterFileName != null)
                {
                    emailViewModel.Attachments.Add(new EmailAttachmentsViewModel() { AttachedFile = invoice.SeriesLetterFile, AttachedFileName = invoice.SeriesLetterFileName });
                }

                if (invoice.ReturnFile != null && invoice.ReturnFileName != null)
                {
                    emailViewModel.Attachments.Add(new EmailAttachmentsViewModel() { AttachedFile = invoice.ReturnFile, AttachedFileName = invoice.ReturnFileName });
                }
            }

            return emailViewModel;
        }

        private string CreateTableTemplate(IEnumerable<PickingExcelAdminTemplateViewModel> pickings)
        {
            var picking = pickings.FirstOrDefault();

            var content =
            $@"
                <tr>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.Area}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.ProcessType}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.UpdatedOn}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.CreatedBy}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.Process}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.Client}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.City}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.UF}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.Contact}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.Email}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.Telephone}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.OriginInvoice}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.ReturnOrder}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.InvoiceReturn}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.PN}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.MTM}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.Quantity}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.ItemValue}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.TaxValidation}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.Reason}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.CarrierName}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.InvoiceReturnDate}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.CarrierRequestDate}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.ExpectedPickingDate}</td>
                    <td style='border: 1px solid black; padding: 8px;'>{picking.ExpectedReturnDate}</td>
                </tr>
            ";

            var table = 
            $@"
                <table border='1' style='border-collapse:collapse; width:100%;'>
                    <tr>
                        <th style='border: 1px solid black; padding: 8px;'>Area</th>
                        <th style='border: 1px solid black; padding: 8px;'>Process Type</th>
                        <th style='border: 1px solid black; padding: 8px;'>Validation Date</th>
                        <th style='border: 1px solid black; padding: 8px;'>Created By</th>
                        <th style='border: 1px solid black; padding: 8px;'>Process</th>
                        <th style='border: 1px solid black; padding: 8px;'>Client</th>
                        <th style='border: 1px solid black; padding: 8px;'>City</th>
                        <th style='border: 1px solid black; padding: 8px;'>UF</th>
                        <th style='border: 1px solid black; padding: 8px;'>Contact</th>
                        <th style='border: 1px solid black; padding: 8px;'>Email</th>
                        <th style='border: 1px solid black; padding: 8px;'>Telephone</th>
                        <th style='border: 1px solid black; padding: 8px;'>Origin Invoice</th>
                        <th style='border: 1px solid black; padding: 8px;'>Return Order</th>
                        <th style='border: 1px solid black; padding: 8px;'>Invoice Return</th>
                        <th style='border: 1px solid black; padding: 8px;'>PN</th>
                        <th style='border: 1px solid black; padding: 8px;'>MTM</th>
                        <th style='border: 1px solid black; padding: 8px;'>Quantity</th>
                        <th style='border: 1px solid black; padding: 8px;'>Item Value</th>
                        <th style='border: 1px solid black; padding: 8px;'>Tax Validation</th>
                        <th style='border: 1px solid black; padding: 8px;'>Reason</th>
                        <th style='border: 1px solid black; padding: 8px;'>Carrier Name</th>
                        <th style='border: 1px solid black; padding: 8px;'>Invoice Return Date</th>
                        <th style='border: 1px solid black; padding: 8px;'>Carrier Request Date</th>
                        <th style='border: 1px solid black; padding: 8px;'>Expected Picking Date</th>
                        <th style='border: 1px solid black; padding: 8px;'>Expected Return Date</th>
                    </tr>
                    {content}
                </table>
            ";

            return table;

        }
    }
}
