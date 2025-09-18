using Lenovo.NAT.Infrastructure.Context;
using Lenovo.NAT.Infrastructure.Entities.Logistic;
using Lenovo.NAT.ViewModel.Logistic.Picking;
using Microsoft.EntityFrameworkCore;

namespace Lenovo.NAT.Infrastructure.Repositories.Logistic
{
    public interface IPickingRepository
    {
        Task<IEnumerable<Picking>> GetPickingFiltered(PickingFilter? filterInfo, int pageSize, int page);
        Task<PickingInvoice> GetPickingInvoice(long id);
        Task<IEnumerable<PickingInvoice>> GetPickingInvoices(long idPicking);
        Task<IEnumerable<PickingItem>> GetPickingItems(long idPicking);
        Task<IEnumerable<PickingProcessType>> GetAllProcessTypes();
        Task<IEnumerable<PickingArea>> GetAllPickingAreas();
        Task<IEnumerable<PickingReason>> GetAllPickingReasons();
        Task<IEnumerable<PickingCarrier>> GetAllPickingCarriers();
        Task NewPickingCarrier(PickingCarrier carrier);
        Task SavePickingCarrier(PickingCarrier carrier);
        Task Delete(PickingCarrier carrier);
        Task<bool> AtLeastOnePicking(string carrier);
        Task<PickingCarrier> GetPickingCarrier(int id);
        Task<IEnumerable<PickingStatus>> GetPickingStatus();
        Task<Picking> GetPickingById(long id);
        Task<Picking?> GetPickingByProcess(string process);
        Task<IEnumerable<PickingBrand>> GetPickingBrands();
        Task DeleteInvoices(IEnumerable<PickingInvoice> invoices);
        Task DeleteItems(IEnumerable<PickingItem> items);
        Task CreateNewInvoices(IEnumerable<PickingInvoice> invoices);
        Task CreateNewItems(IEnumerable<PickingItem> items);
        Task CreateNewPicking(Picking newPicking);
        bool HasExistingPickingForThisProcess(string process);
        Task<IEnumerable<PickingStatus>> GetAllStatus();
        Task UpdatePickingInvoice(PickingInvoice pickingInvoice);
        Task UpdatePicking(Picking picking);
        Task<long> CountPickingRequests(PickingFilter? filterInfo);
        Task<IEnumerable<PickingExcelTemplateViewModel>> GetAllPicking(PickingFilter? filterInfo);
        Task<IEnumerable<PickingExcelAdminTemplateViewModel>> GetAllPickingAdmin(PickingFilter? filterInfo);
        string GetEmailContactFromCarrier(string carrierName);
        Task<IEnumerable<LogisticInvoice>> GetLogisticInvoice(LogisticInvoiceFilter? filterInfo, int pageSize, int page);
        Task<long> CountLogisticInvoices(LogisticInvoiceFilter? filterInfo);
        Task<IEnumerable<LogisticInvoice>> GetAllLogisticInvoices(LogisticInvoiceFilter? filterInfo);
        Task<IEnumerable<string>> GetConditionCodes();
    }
    public class PickingRepository : IPickingRepository
    {
        private readonly ThinkToolContext _thinkToolContext;
        public PickingRepository(ThinkToolContext thinkToolContext)
        {
            _thinkToolContext = thinkToolContext;
        }

        public async Task<IEnumerable<Picking>> GetPickingFiltered(PickingFilter? filterInfo, int pageSize, int page)
        {
            var pickingList = await _thinkToolContext.Picking
                       .Where(picking => filterInfo == null
                            ||
                            
                                (filterInfo.Client == null || picking.Client.ToLower().Contains(filterInfo.Client))
                                && (filterInfo.Process == null || filterInfo.Process.Equals(picking.Process))
                                && (filterInfo.CreatedOn == null || ((DateTime)filterInfo.CreatedOn).Date.Equals(picking.CreatedOn.Date))
                                && (filterInfo.ValidationDate == null || ((DateTime)filterInfo.ValidationDate).Date.Equals(picking.UpdatedOn.Value.Date))
                                && (filterInfo.IdPickingArea == null || filterInfo.IdPickingArea.Equals(picking.IdPickingArea))
                                && (filterInfo.IdPickingStatus == null || filterInfo.IdPickingStatus.Equals(picking.IdPickingStatus))
                                && (filterInfo.IdPickingProcessType == null || filterInfo.IdPickingProcessType.Equals(picking.IdPickingProcessType))
                            
                        )
                       .OrderByDescending(x => x.CreatedOn)
                       .Skip(pageSize * page)
                       .Take(pageSize)
                       .ToListAsync();

            return pickingList;
        }

        public async Task<long> CountPickingRequests(PickingFilter? filterInfo)
        {
            return await _thinkToolContext.Picking
                       .Where(picking => filterInfo == null
                            ||
                            
                                (filterInfo.Client == null || filterInfo.Client.Equals(picking.Client))
                                && (filterInfo.Process == null || filterInfo.Process.Equals(picking.Process))
                                && (filterInfo.CreatedOn == null || ((DateTime)filterInfo.CreatedOn).Date.Equals(picking.CreatedOn.Date))
                                && (filterInfo.IdPickingArea == null || filterInfo.IdPickingArea.Equals(picking.IdPickingArea))
                                && (filterInfo.IdPickingStatus == null || filterInfo.IdPickingStatus.Equals(picking.IdPickingStatus))
                                && (filterInfo.IdPickingProcessType == null || filterInfo.IdPickingProcessType.Equals(picking.IdPickingProcessType))
                            
                        )
                       .LongCountAsync();
        }

        public async Task<IEnumerable<PickingExcelTemplateViewModel>> GetAllPicking(PickingFilter? filterInfo)
        {
            var allPicking =
                from picking in _thinkToolContext.Picking
                join pickingInvoice in _thinkToolContext.PickingInvoice on picking.Id equals pickingInvoice.IdPicking into pickingInvoiceLeft
                from leftJoinPickingInvoice in pickingInvoiceLeft.DefaultIfEmpty()
                join pickingItem in _thinkToolContext.PickingItem on leftJoinPickingInvoice.Id equals pickingItem.IdPickingInvoice into pickingItemLeft
                from leftJoinPickingItem in pickingItemLeft.DefaultIfEmpty()
                join pickingStatus in _thinkToolContext.PickingStatus on picking.IdPickingStatus equals pickingStatus.Id
                join pickingBrand in _thinkToolContext.PickingBrand on leftJoinPickingItem.IdBrand equals pickingBrand.Id into pickingBrandLeft
                from leftPickingBrand in pickingBrandLeft.DefaultIfEmpty()
                join users in _thinkToolContext.Users on picking.CreatedBy equals users.NetworkId
                where 
                    (filterInfo.Client == null || filterInfo.Client.Equals(picking.Client))
                    && (filterInfo.Process == null || filterInfo.Process.Equals(picking.Process))
                    && (filterInfo.CreatedOn == null || ((DateTime)filterInfo.CreatedOn).Date.Equals(picking.CreatedOn.Date))
                    && (filterInfo.ValidationDate == null || ((DateTime)filterInfo.ValidationDate).Date.Equals(picking.UpdatedOn.Value.Date))
                    && (filterInfo.IdPickingArea == null || filterInfo.IdPickingArea.Equals(picking.IdPickingArea))
                    && (filterInfo.IdPickingStatus == null || filterInfo.IdPickingStatus.Equals(picking.IdPickingStatus))
                    && (filterInfo.IdPickingProcessType == null || filterInfo.IdPickingProcessType.Equals(picking.IdPickingProcessType))
                
                orderby picking.CreatedOn descending
                select new PickingExcelTemplateViewModel()
                {
                    Area = "RVL",
                    ProcessType = "Coleta",
                    UpdatedOn = picking.UpdatedOn.Value.ToShortDateString(),
                    CreatedBy = users.Name ?? picking.CreatedBy,
                    Process = picking.Process,
                    Client = picking.Client,
                    City = picking.City,
                    UF = picking.UF,
                    Contact = picking.Contact,
                    Email = picking.Email,
                    Telephone = picking.Telephone,
                    OriginInvoice = leftJoinPickingItem.OriginInvoive,
                    ReturnOrder = leftJoinPickingItem.ReturnOrder,
                    InvoiceReturn = leftJoinPickingInvoice.InvoiceReturn,
                    PN = leftJoinPickingItem.PartNumber,
                    MTM = leftJoinPickingItem.MTM,
                    Quantity = leftJoinPickingItem.Quantity,
                    ItemValue = leftJoinPickingItem.ItemValue,
                    Brand = leftPickingBrand.Name,
                    TaxValidation = "VALIDADO POR RVL",
                    Reason = picking.Reason ?? string.Empty,
                    CarrierName = picking.CarrierName ?? string.Empty,
                    InvoiceReturnDate = leftJoinPickingInvoice.InvoiceReturnDate.ToShortDateString(),
                    CarrierRequestDate = picking.CarrierRequestDate.Value.ToShortDateString(),
                    ExpectedPickingDate = picking.ExpectedPickingDate.Value.ToShortDateString(),
                    EffectivePickingDate = picking.EffectivePickingDate.Value.ToShortDateString(),
                    ExpectedReturnDate = picking.ExpectedReturnDate.Value.ToShortDateString(),
                    EffectiveReturnDate = picking.EffectiveReturnDate.Value.ToShortDateString(),
                    EndDate = picking.EndDate.Value.ToShortDateString(),
                    Observation = picking.CorrectionObservation,
                    Status = pickingStatus.Name
                };


            return allPicking;
        }

        public async Task<IEnumerable<PickingExcelAdminTemplateViewModel>> GetAllPickingAdmin(PickingFilter? filterInfo)
        {
            var allPicking =
                from picking in _thinkToolContext.Picking
                join pickingInvoice in _thinkToolContext.PickingInvoice on picking.Id equals pickingInvoice.IdPicking into pickingInvoiceLeft
                from leftJoinPickingInvoice in pickingInvoiceLeft.DefaultIfEmpty()
                join pickingItem in _thinkToolContext.PickingItem on leftJoinPickingInvoice.Id equals pickingItem.IdPickingInvoice into pickingItemLeft
                from leftJoinPickingItem in pickingItemLeft.DefaultIfEmpty()
                join pickingStatus in _thinkToolContext.PickingStatus on picking.IdPickingStatus equals pickingStatus.Id
                join pickingBrand in _thinkToolContext.PickingBrand on leftJoinPickingItem.IdBrand equals pickingBrand.Id into pickingBrandLeft
                from leftPickingBrand in pickingBrandLeft.DefaultIfEmpty()
                join users in _thinkToolContext.Users on picking.CreatedBy equals users.NetworkId
                where 
                    (filterInfo.Client == null || filterInfo.Client.Equals(picking.Client))
                    && (filterInfo.Process == null || filterInfo.Process.Equals(picking.Process))
                    && (filterInfo.CreatedOn == null || ((DateTime)filterInfo.CreatedOn).Date.Equals(picking.CreatedOn.Date))
                    && (filterInfo.ValidationDate == null || ((DateTime)filterInfo.ValidationDate).Date.Equals(picking.UpdatedOn.Value.Date))
                    && (filterInfo.IdPickingArea == null || filterInfo.IdPickingArea.Equals(picking.IdPickingArea))
                    && (filterInfo.IdPickingStatus == null || filterInfo.IdPickingStatus.Equals(picking.IdPickingStatus))
                    && (filterInfo.IdPickingProcessType == null || filterInfo.IdPickingProcessType.Equals(picking.IdPickingProcessType))
                
                orderby picking.CreatedOn descending
                select new PickingExcelAdminTemplateViewModel()
                {
                    Area = "RVL",
                    ProcessType = "Coleta",
                    UpdatedOn = picking.UpdatedOn.Value.ToShortDateString(),
                    CreatedBy = users.Name ?? picking.CreatedBy,
                    Process = picking.Process,
                    Client = picking.Client,
                    City = picking.City,
                    UF = picking.UF,
                    Contact = picking.Contact,
                    Email = picking.Email,
                    Telephone = picking.Telephone,
                    OriginInvoice = leftJoinPickingItem.OriginInvoive,
                    ReturnOrder = leftJoinPickingItem.ReturnOrder,
                    InvoiceReturn = leftJoinPickingInvoice.InvoiceReturn,
                    PN = leftJoinPickingItem.PartNumber,
                    MTM = leftJoinPickingItem.MTM,
                    Quantity = leftJoinPickingItem.Quantity,
                    ItemValue = leftJoinPickingItem.ItemValue,
                    TaxValidation = "VALIDADO POR RVL",
                    Reason = picking.Reason ?? string.Empty,
                    CarrierName = picking.CarrierName ?? string.Empty,
                    InvoiceReturnDate = leftJoinPickingInvoice.InvoiceReturnDate.ToShortDateString(),
                    CarrierRequestDate = picking.CarrierRequestDate != null ? picking.CarrierRequestDate.Value.ToShortDateString() : string.Empty,
                    ExpectedPickingDate = picking.ExpectedPickingDate != null ? picking.ExpectedPickingDate.Value.ToShortDateString(): string.Empty,
                    ExpectedReturnDate = picking.ExpectedReturnDate != null ? picking.ExpectedReturnDate.Value.ToShortDateString() : string.Empty
                };


            return allPicking;
        }

        public async Task<PickingInvoice> GetPickingInvoice(long id)
        {
            return await _thinkToolContext.PickingInvoice.FirstOrDefaultAsync(invoice => invoice.Id == id) ?? new PickingInvoice();
        }

        public async Task<IEnumerable<PickingInvoice>> GetPickingInvoices(long idPicking)
        {
            return await _thinkToolContext.PickingInvoice.Where(invoice => invoice.IdPicking == idPicking).ToListAsync();
        }

        public async Task<IEnumerable<PickingItem>> GetPickingItems(long idPicking)
        {
            return await _thinkToolContext.PickingItem.Where(item => item.IdPicking == idPicking).ToListAsync();
        }

        public async Task<IEnumerable<PickingBrand>> GetPickingBrands()
        {
            return await _thinkToolContext.PickingBrand.ToListAsync();
        }

        public async Task<IEnumerable<PickingStatus>> GetPickingStatus()
        {
            return await _thinkToolContext.PickingStatus.ToListAsync();
        }

        public async Task<Picking> GetPickingById(long id)
        {
            return await _thinkToolContext.Picking.FirstOrDefaultAsync(x => x.Id == id) ?? new Picking();
        }

        public async Task<Picking?> GetPickingByProcess(string process)
        {
            return await _thinkToolContext.Picking.FirstOrDefaultAsync(x => x.Process == process);
        }

        public async Task<IEnumerable<PickingProcessType>> GetAllProcessTypes()
        {
            return await _thinkToolContext.PickingProcessType.ToListAsync();
        }

        public async Task<IEnumerable<PickingArea>> GetAllPickingAreas()
        {
            return await _thinkToolContext.PickingArea.ToListAsync();
        }

        public async Task<IEnumerable<PickingReason>> GetAllPickingReasons()
        {
            return await _thinkToolContext.PickingReason.ToListAsync();
        }

        public async Task<IEnumerable<PickingCarrier>> GetAllPickingCarriers()
        {
            return await _thinkToolContext.PickingCarrier.ToListAsync();
        }

        public async Task<PickingCarrier> GetPickingCarrier(int id)
        {
            return await _thinkToolContext.PickingCarrier.FirstOrDefaultAsync(x => x.Id == id) ?? new PickingCarrier();
        }

        public async Task<bool> AtLeastOnePicking(string carrier)
        {
            return await _thinkToolContext.Picking.AnyAsync(x => x.CarrierName == carrier);
        }

        public async Task SavePickingCarrier(PickingCarrier carrier)
        {
            _thinkToolContext.Update(carrier);

            await _thinkToolContext.SaveChangesAsync();
        }

        public async Task Delete(PickingCarrier carrier)
        {
            _thinkToolContext.PickingCarrier.Remove(carrier);

            await _thinkToolContext.SaveChangesAsync();
        }

        public async Task NewPickingCarrier(PickingCarrier carrier)
        {
            await _thinkToolContext.PickingCarrier.AddRangeAsync(carrier);

            await _thinkToolContext.SaveChangesAsync();
        }

        public async Task DeleteInvoices(IEnumerable<PickingInvoice> invoices)
        {
            _thinkToolContext.PickingInvoice.RemoveRange(invoices);

            await _thinkToolContext.SaveChangesAsync();
        }

        public async Task CreateNewInvoices(IEnumerable<PickingInvoice> invoices)
        {
            await _thinkToolContext.PickingInvoice.AddRangeAsync(invoices);

            await _thinkToolContext.SaveChangesAsync();
        }

        public async Task DeleteItems(IEnumerable<PickingItem> items)
        {
            _thinkToolContext.PickingItem.RemoveRange(items);

            await _thinkToolContext.SaveChangesAsync();
        }

        public async Task CreateNewItems(IEnumerable<PickingItem> items)
        {
            await _thinkToolContext.PickingItem.AddRangeAsync(items);

            await _thinkToolContext.SaveChangesAsync();
        }

        public async Task CreateNewPicking(Picking newPicking)
        {
            await _thinkToolContext.Picking.AddAsync(newPicking);

            await _thinkToolContext.SaveChangesAsync();

            if (string.IsNullOrEmpty(newPicking.Process))
            {
                newPicking.Process = newPicking.Id.ToString();

                _thinkToolContext.Update(newPicking);

                await _thinkToolContext.SaveChangesAsync();
            }
        }

        public async Task UpdatePicking(Picking picking)
        {
            _thinkToolContext.Update(picking);

            await _thinkToolContext.SaveChangesAsync();
        }

        public async Task UpdatePickingInvoice(PickingInvoice pickingInvoice)
        {
            _thinkToolContext.Update(pickingInvoice);

            await _thinkToolContext.SaveChangesAsync();
        }

        public bool HasExistingPickingForThisProcess(string process)
        {
            return _thinkToolContext.Picking.Any(x => x.Process == process);
        }

        public async Task<IEnumerable<PickingStatus>> GetAllStatus()
        {
            return await _thinkToolContext.PickingStatus.ToListAsync();
        }

        public string GetEmailContactFromCarrier(string carrierName)
        {
            return _thinkToolContext.PickingCarrier.FirstOrDefault(carrier => carrier.Name == carrierName)?.ContactEmails ?? string.Empty;
        }

        public async Task<IEnumerable<LogisticInvoice>> GetLogisticInvoice(LogisticInvoiceFilter? filterInfo, int pageSize, int page)
        {
            var invoices = await _thinkToolContext.LogisticInvoice
                       .Where(invoice => filterInfo == null
                            ||
                            
                                (filterInfo.AccessKey == null || invoice.AccessKey.ToUpper() == filterInfo.AccessKey.ToUpper())
                                && (filterInfo.FileSentOption == null || filterInfo.FileSentOption.Value == invoice.FileSent)
                                && (filterInfo.ProcessedOn == null || filterInfo.ProcessedOn.Value.Date == invoice.CreatedOn.Date)
                                && (filterInfo.AirwayBillNumber == null || filterInfo.AirwayBillNumber == invoice.AirwayBillNumber)
                                && (filterInfo.ConditionCodeSelected == null || filterInfo.ConditionCodeSelected == invoice.ConditionCode)
                            
                        )
                       .OrderByDescending(x => x.Id)
                       .Skip(pageSize * page)
                       .Take(pageSize)
                       .ToListAsync();

            return invoices;
        }

        public async Task<IEnumerable<string>> GetConditionCodes()
        {
            return await _thinkToolContext.LogisticInvoice.Where(x => !string.IsNullOrEmpty(x.ConditionCode)).Select(x => x.ConditionCode ?? string.Empty).Distinct().ToListAsync();
        }

        public async Task<IEnumerable<LogisticInvoice>> GetAllLogisticInvoices(LogisticInvoiceFilter? filterInfo)
        {
            var invoices = await _thinkToolContext.LogisticInvoice
                       .Where(invoice => filterInfo == null
                            ||
                            
                                (filterInfo.AccessKey == null || invoice.AccessKey.ToUpper() == filterInfo.AccessKey.ToUpper())
                                && (filterInfo.FileSentOption == null || filterInfo.FileSentOption.Value == invoice.FileSent)
                                && (filterInfo.ProcessedOn == null || filterInfo.ProcessedOn.Value.Date == invoice.CreatedOn.Date)
                                && (filterInfo.AirwayBillNumber == null || filterInfo.AirwayBillNumber == invoice.AirwayBillNumber)
                                && (filterInfo.ConditionCodeSelected == null || filterInfo.ConditionCodeSelected == invoice.ConditionCode)
                            
                        )
                       .ToListAsync();

            return invoices;
        }

        public async Task<long> CountLogisticInvoices(LogisticInvoiceFilter? filterInfo)
        {
            return await _thinkToolContext.LogisticInvoice
                       .Where(invoice => filterInfo == null
                            ||
                            
                                (filterInfo.AccessKey == null || invoice.AccessKey.ToUpper() == filterInfo.AccessKey.ToUpper())
                                && (filterInfo.FileSentOption == null || filterInfo.FileSentOption.Value == invoice.FileSent)
                                && (filterInfo.ProcessedOn == null || filterInfo.ProcessedOn.Value.Date == invoice.CreatedOn.Date)
                                && (filterInfo.AirwayBillNumber == null || filterInfo.AirwayBillNumber == invoice.AirwayBillNumber)
                                && (filterInfo.ConditionCodeSelected == null || filterInfo.ConditionCodeSelected == invoice.ConditionCode)
                            
                        )
                       .LongCountAsync();
        }
    }
}
