using AutoMapper;
using Lenovo.NAT.Infrastructure.Context;
using Lenovo.NAT.Infrastructure.Entities.Logistic;
using Lenovo.NAT.Infrastructure.Repositories.Logistic;
using Lenovo.NAT.ViewModel.Logistic.OnlTicket;
using Lenovo.NAT.ViewModel.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Lenovo.NAT.Services.Logistic
{
    public interface IOnlTicketService
    {
        Task<PaginatedOnlTicketViewModel> GetPaginatedOnlTicketViewModel(OnlTicketFilter filterInfo, int pageSize, int page);
        Task<OnlTicketViewModel> GetOnlTicketDetail(int id);
        Task<OnlTicketViewModel> GetEmptyOnlTicket();
        Task<string> SaveOnlTicket(OnlTicketViewModel model);
        Task<string> UpdateOnlTicket(OnlTicketViewModel model);
        Task<string> DeleteOnlTicket(int id);
        Task<IEnumerable<OnlTicketOrderTypeViewModel>> GetOrderTypes();
        Task<IEnumerable<OnlTicketOrderStatusViewModel>> GetOrderStatuses();
        Task<IEnumerable<OnlTicketNFTypeViewModel>> GetNFTypes();
        Task<IEnumerable<OnlTicketCustomerSegmentViewModel>> GetCustomerSegments();
        Task<IEnumerable<CountryViewModel>> GetCountries();
        Task<IEnumerable<string>> GetSegments();
        Task<IEnumerable<string>> GetCustomerNames();
        Task<IEnumerable<string>> GetStatuses();
        Task<IEnumerable<string>> GetAssignToUsers();
        Task<IEnumerable<string>> GetAgingBuckets();
        Task<IEnumerable<string>> GetCreatedByUsers();
        
        // Methods for Order entities (migrated from OnlTicket)
        Task<IEnumerable<OrderNotLoaded>> GetAllOrderNotLoadedAsync();
        Task<OrderNotLoaded?> GetOrderNotLoadedByIdAsync(long id);
        Task<OrderAttachment?> GetAttachmentByIdAsync(int id);
    }

    public class OnlTicketService : IOnlTicketService
    {
        private readonly IOrderNotLoadedRepository _orderNotLoadedRepository;
        private readonly IOrderSoldTORepository _orderSoldTORepository;
        private readonly IOrderShipToRepository _orderShipToRepository;
        private readonly IOrderProductRepository _orderProductRepository;
        private readonly IOrderAttachmentRepository _orderAttachmentRepository;
        private readonly IMapper _mapper;

        public OnlTicketService(
            IOrderNotLoadedRepository orderNotLoadedRepository,
            IOrderSoldTORepository orderSoldTORepository,
            IOrderShipToRepository orderShipToRepository,
            IOrderProductRepository orderProductRepository,
            IOrderAttachmentRepository orderAttachmentRepository,
            IMapper mapper)
        {
            _orderNotLoadedRepository = orderNotLoadedRepository;
            _orderSoldTORepository = orderSoldTORepository;
            _orderShipToRepository = orderShipToRepository;
            _orderProductRepository = orderProductRepository;
            _orderAttachmentRepository = orderAttachmentRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedOnlTicketViewModel> GetPaginatedOnlTicketViewModel(OnlTicketFilter filterInfo, int pageSize, int page)
        {
            // TODO: Implementar busca real no banco de dados
            // Por enquanto, retornar dados de exemplo
            var onlTickets = new List<OnlTicketListViewModel>
            {
                new OnlTicketListViewModel
                {
                    Id = 1,
                    LogNumber = "LOG001",
                    Customer = "Cliente Teste",
                    Segment = "Enterprise",
                    AssignTo = "Operador1",
                    OrderAging = "5 dias",
                    AgingBucket = "0-7 days",
                    SAPOrder = "SAP001",
                    TotalCAs = 2,
                    CreatedAt = DateTime.Now.AddDays(-5),
                    CreatedBy = "admin",
                    EmailFrom = "test@lenovo.com",
                    UpdatedAt = DateTime.Now.AddDays(-1),
                    Status = "Active"
                }
            };

            var paginationInfo = new PaginationInfo
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = onlTickets.Count,
                TotalPages = (int)Math.Ceiling((double)onlTickets.Count / pageSize)
            };

            return new PaginatedOnlTicketViewModel
            {
                OnlTickets = onlTickets,
                PaginationInfo = paginationInfo,
                FilterInfo = filterInfo
            };
        }

        public async Task<OnlTicketViewModel> GetOnlTicketDetail(int id)
        {
            try
            {
                // MIGRADO: Buscar dados das novas tabelas Order
                var orderNotLoaded = await _orderNotLoadedRepository.GetOrderNotLoadedByIdAsync(id);
                if (orderNotLoaded == null)
                {
                    throw new InvalidOperationException($"Order com ID {id} não encontrado");
                }

                var model = await MapOrderToViewModel(orderNotLoaded);
                
                // CORRIGIDO: Popular dropdowns para exibir na tela
                await PopulateDropdowns(model);
                
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OnlTicketViewModel> GetEmptyOnlTicket()
        {
            return new OnlTicketViewModel
            {
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System",
                Status = "Draft",
                SoldToAddresses = new List<SoldToViewModel>(),
                Attachments = new List<OnlTicketAttachmentViewModel>(),
                HistoricEntries = new List<HistoricEntryViewModel>()
            };
        }

        public async Task<string> SaveOnlTicket(OnlTicketViewModel model)
        {
            try
            {
                // MIGRADO: Salvar nas novas tabelas Order
                await SaveToOrderEntities(model);
                return "ONL Ticket created successfully!";
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "Nenhuma exceção interna";
                throw new Exception($"Erro ao salvar ONL Ticket: {ex.Message}. Detalhes: {innerMessage}", ex);
            }
        }

        public async Task<string> UpdateOnlTicket(OnlTicketViewModel model)
        {
            try
            {
                // Validar ID obrigatório
                if (string.IsNullOrEmpty(model.ID))
                    throw new ArgumentException("ID é obrigatório para atualização");


                // MIGRADO: Verificar se existe nas tabelas Order
                var orderId = long.Parse(model.ID);
                var existingOrder = await _orderNotLoadedRepository.GetOrderNotLoadedByIdAsync(orderId);
                if (existingOrder == null)
                    throw new ArgumentException("ONL Ticket não encontrado nas tabelas Order");


                // CORRIGIDO: Usar método específico para atualização
                await UpdateOrderEntities(model, orderId);
                return "ONL Ticket updated successfully!";
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar ONL Ticket: {ex.Message}", ex);
            }
        }

        public async Task<string> DeleteOnlTicket(int id)
        {
            // TODO: Implementar exclusão no banco de dados
            return "ONL Ticket excluído com sucesso!";
        }

        private async Task<OnlTicketViewModel> MapOrderToViewModel(OrderNotLoaded orderNotLoaded)
        {
            // Buscar entidades relacionadas
            var soldTos = await _orderSoldTORepository.GetByOrderIdAsync(orderNotLoaded.Id);
            var shipTos = await _orderShipToRepository.GetByOrderIdAsync(orderNotLoaded.Id);
            var products = await _orderProductRepository.GetByOrderIdAsync(orderNotLoaded.Id);
            var attachments = await _orderAttachmentRepository.GetByOrderIdAsync(orderNotLoaded.Id);

            var model = new OnlTicketViewModel
            {
                ID = orderNotLoaded.Id.ToString(),
                LogNumber = orderNotLoaded.NumberOrder,
                CreatedDate = orderNotLoaded.CreatedOn,
                CreatedBy = orderNotLoaded.CreatedBy,
                UpdatedDate = orderNotLoaded.UpdatedOn,
                UpdatedBy = orderNotLoaded.UpdatedBy,
                Status = orderNotLoaded.OrderStatus,
                EmailFrom = orderNotLoaded.From,
                AssignedOperator = orderNotLoaded.AssignedTo,
                PONumber = orderNotLoaded.PONumber,
                // CORRIGIDO: Usar IDs para os dropdowns selecionarem corretamente
                OrderType = orderNotLoaded.IdOrderType?.ToString() ?? "1",
                OrderStatus = orderNotLoaded.IdOrderStatus?.ToString() ?? "1", 
                NFType = orderNotLoaded.IdNFType?.ToString() ?? "1",
                Segment = orderNotLoaded.IdSegment?.ToString() ?? "1",
                CustomerName = orderNotLoaded.Customer,
                DMU = orderNotLoaded.DMU,
                Country = orderNotLoaded.Country,
                CountryId = orderNotLoaded.IdCountry,
                BU = orderNotLoaded.BusinessUnit,
                PODate = orderNotLoaded.PODate,
                SegmentId = orderNotLoaded.IdSegment,
                BillAhead = orderNotLoaded.BillAhead,
                ISRName = orderNotLoaded.ISRName,
                Region = orderNotLoaded.Region,
                UF = orderNotLoaded.State,
                ReplacementType = orderNotLoaded.RecolocationType
            };

            // Mapear SoldTo
            model.SoldToAddresses = soldTos.Select(soldTo => new SoldToViewModel
            {
                CNPJ = soldTo.CompanyTaxId,
                Address = soldTo.Address,
                Neighborhood = soldTo.Neighborhood,
                City = soldTo.City,
                PostalCode = soldTo.PostalCode,
                State = soldTo.State,
                ShipToAddresses = shipTos.Where(st => st.IdOrderSoldTo == soldTo.Id).Select(shipTo => new ShipToViewModel
                {
                    CNPJ = shipTo.CompanyTaxId,
                    Address = shipTo.Address,
                    Neighborhood = shipTo.Neighborhood,
                    City = shipTo.City,
                    PostalCode = shipTo.PostalCode,
                    State = shipTo.State,
                    SAPOrderNumber = shipTo.SapOrder,
                    OrderItems = products.Where(p => p.IdOrderShipTo == shipTo.Id).Select(product => new OrderItemViewModel
                    {
                        BidContractNumber = product.BID,
                        PartNumber = product.PartNumber,
                        PartNumberDescription = product.MTMDescription,
                        Qty = product.Quantity,
                        UnityNetPrice = product.UnitNetPrice,
                        UnitGrossPrice = product.UnitGrossPrice
                    }).ToList()
                }).ToList()
            }).ToList();

            // Mapear Attachments
            model.Attachments = attachments.Select(a => new OnlTicketAttachmentViewModel
            {
                Id = a.Id,
                CustomerPO = a.PONumber,
                Description = a.Description,
                Comments = a.Comments,
                FileName = a.AttachemntFileName, // CORRIGIDO: Mapear nome do arquivo
                FileExtension = a.FileExtension, // CORRIGIDO: Mapear extensão
                ContentType = GetContentTypeFromExtension(a.FileExtension), // CORRIGIDO: Mapear content type
                FileSize = a.Attachment?.Length ?? 0 // CORRIGIDO: Mapear tamanho do arquivo
            }).ToList();

            return model;
        }

        private async Task UpdateOrderEntities(OnlTicketViewModel model, long orderId)
        {
            try
            {
                // Buscar dados válidos para os dropdowns
                var validOrderTypes = await GetOrderTypes();
                var validNFTypes = await GetNFTypes();
                var validCustomerSegments = await GetCustomerSegments();
                var validCountries = await GetCountries();
                
                var defaultOrderTypeId = validOrderTypes.FirstOrDefault()?.Id ?? 1;
                var defaultNFTypeId = validNFTypes.FirstOrDefault()?.Id ?? 1;
                
                // Buscar IDs específicos dos dropdowns selecionados
                var selectedOrderTypeId = TryParseInt(model.OrderType) ?? defaultOrderTypeId;
                var selectedNFTypeId = TryParseInt(model.NFType) ?? defaultNFTypeId;
                var selectedSegmentId = model.SegmentId ?? TryParseInt(model.Segment);

                
                // Buscar textos correspondentes aos IDs selecionados
                var selectedOrderTypeText = validOrderTypes.FirstOrDefault(x => x.Id == selectedOrderTypeId)?.Name ?? model.OrderType ?? "";
                var selectedNFTypeText = validNFTypes.FirstOrDefault(x => x.Id == selectedNFTypeId)?.Name ?? model.NFType ?? "";
                var selectedSegmentText = validCustomerSegments.FirstOrDefault(x => x.Id == selectedSegmentId)?.Name ?? model.Segment ?? "";
                var selectedCountryText = validCountries.FirstOrDefault(x => x.Id == model.CountryId)?.Name ?? model.Country ?? "";
                

                // BUSCAR O REGISTRO EXISTENTE PARA ATUALIZAR
                var existingOrder = await _orderNotLoadedRepository.GetOrderNotLoadedByIdAsync(orderId);
                if (existingOrder == null)
                    throw new ArgumentException($"Order com ID {orderId} não encontrado para atualização");

                // ATUALIZAR OS CAMPOS DO REGISTRO EXISTENTE
                existingOrder.UpdatedOn = DateTime.UtcNow.AddHours(-3);
                existingOrder.UpdatedBy = model.UpdatedBy ?? "System";
                existingOrder.OrderStatus = model.Status ?? existingOrder.OrderStatus;
                existingOrder.From = model.EmailFrom ?? existingOrder.From;
                existingOrder.NumberOrder = model.LogNumber ?? existingOrder.NumberOrder;
                existingOrder.AssignedTo = model.AssignedOperator ?? existingOrder.AssignedTo;
                existingOrder.PONumber = model.PONumber ?? existingOrder.PONumber;
                existingOrder.OrderType = selectedOrderTypeText;
                existingOrder.NFType = selectedNFTypeText;
                existingOrder.Customer = model.CustomerName ?? existingOrder.Customer;
                existingOrder.DMU = model.DMU ?? existingOrder.DMU;
                existingOrder.Country = selectedCountryText;
                existingOrder.IdCountry = model.CountryId;
                existingOrder.BusinessUnit = model.BU ?? existingOrder.BusinessUnit;
                existingOrder.PODate = model.PODate ?? existingOrder.PODate;
                existingOrder.Segment = selectedSegmentText;
                existingOrder.IdSegment = selectedSegmentId;
                existingOrder.Region = model.Region ?? existingOrder.Region;
                existingOrder.State = model.UF ?? existingOrder.State;
                existingOrder.RecolocationType = model.ReplacementType ?? existingOrder.RecolocationType;
                existingOrder.BillAhead = model.BillAhead ?? existingOrder.BillAhead;
                existingOrder.ISRName = model.ISRName ?? existingOrder.ISRName;
                existingOrder.IdOrderType = selectedOrderTypeId;
                existingOrder.IdNFType = selectedNFTypeId;

                // ATUALIZAR NO BANCO DE DADOS
                var updatedOrder = await _orderNotLoadedRepository.UpdateOrderNotLoadedAsync(existingOrder);

                // IMPLEMENTAÇÃO COMPLETA: Gerenciamento inteligente de anexos
                
                // 1. PROCESSAR REMOÇÕES EXPLÍCITAS
                if (!string.IsNullOrEmpty(model.RemovedAttachmentIds))
                {
                    var idsToRemove = model.RemovedAttachmentIds.Split(',')
                        .Where(id => int.TryParse(id.Trim(), out _))
                        .Select(id => int.Parse(id.Trim()))
                        .ToList();
                    
                    
                    foreach (var attachmentId in idsToRemove)
                    {
                        await _orderAttachmentRepository.DeleteAsync(attachmentId);
                    }
                }
                
                // 2. ADICIONAR APENAS ANEXOS NOVOS (com dados de arquivo)
                var newAttachments = model.Attachments?.Where(a => 
                    a.Id <= 0 && // Não tem ID (é novo)
                    !string.IsNullOrEmpty(a.FileData) // Tem dados do arquivo
                ) ?? new List<OnlTicketAttachmentViewModel>();
                
                
                foreach (var attachmentVm in newAttachments)
                {
                    if (!string.IsNullOrEmpty(attachmentVm.CustomerPO) || 
                        !string.IsNullOrEmpty(attachmentVm.Description) || 
                        !string.IsNullOrEmpty(attachmentVm.Comments) ||
                        !string.IsNullOrEmpty(attachmentVm.FileData))
                    {
                        
                        // Converter Base64 para bytes
                        byte[] fileBytes = new byte[] { 0x00 };
                        if (!string.IsNullOrEmpty(attachmentVm.FileData))
                        {
                            try
                            {
                                var base64Data = attachmentVm.FileData;
                                if (base64Data.Contains(","))
                                {
                                    base64Data = base64Data.Split(',')[1];
                                }
                                fileBytes = Convert.FromBase64String(base64Data);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        
                        var orderAttachment = new OrderAttachment
                        {
                            IdOrderNotLoaded = updatedOrder.Id,
                            CreatedOn = DateTime.UtcNow.AddHours(-3),
                            CreatedBy = model.UpdatedBy ?? "System",
                            PONumber = attachmentVm.CustomerPO ?? "",
                            Description = attachmentVm.Description ?? "",
                            Comments = attachmentVm.Comments ?? "",
                            AttachemntFileName = attachmentVm.FileName ?? "unknown_file",
                            Attachment = fileBytes,
                            FileExtension = attachmentVm.FileExtension ?? ".txt"
                        };

                        var createdAttachment = await _orderAttachmentRepository.CreateAsync(orderAttachment);
                    }
                }
                

                // 3. ATUALIZAR SOLD TO, SHIP TO E ORDER ITEMS
                
                // CORREÇÃO: Remover na ordem correta das dependências (FK constraints)
                // 1º: OrderProduct (grandchild - sem dependências)
                await _orderProductRepository.DeleteByOrderIdAsync(orderId);
                // 2º: OrderShipTo (child - depende de OrderSoldTO)
                await _orderShipToRepository.DeleteByOrderIdAsync(orderId);
                // 3º: OrderSoldTO (parent - é referenciado por OrderShipTo)
                await _orderSoldTORepository.DeleteByOrderIdAsync(orderId);
                
                // Recriar todos os registros com os dados atualizados
                foreach (var soldToVm in model.SoldToAddresses ?? new List<SoldToViewModel>())
                {
                    if (!string.IsNullOrEmpty(soldToVm.CNPJ) || !string.IsNullOrEmpty(soldToVm.Address))
                    {
                        var orderSoldTo = new OrderSoldTO
                        {
                            IdOrderNotLoaded = orderId,
                            CreatedOn = DateTime.UtcNow.AddHours(-3),
                            CreatedBy = model.UpdatedBy ?? "System",
                            CompanyTaxId = soldToVm.CNPJ ?? "",
                            Address = soldToVm.Address ?? "",
                            Neighborhood = soldToVm.Neighborhood ?? "",
                            City = soldToVm.City ?? "",
                            PostalCode = soldToVm.PostalCode ?? "",
                            State = soldToVm.State ?? ""
                        };

                        var createdSoldTo = await _orderSoldTORepository.CreateAsync(orderSoldTo);

                        // Salvar ShipTo addresses
                        foreach (var shipToVm in soldToVm.ShipToAddresses ?? new List<ShipToViewModel>())
                        {
                            if (!string.IsNullOrEmpty(shipToVm.CNPJ) || !string.IsNullOrEmpty(shipToVm.Address))
                            {
                                var orderShipTo = new OrderShipTo
                                {
                                    IdOrderNotLoaded = orderId,
                                    IdOrderSoldTo = createdSoldTo.Id,
                                    CreatedOn = DateTime.UtcNow.AddHours(-3),
                                    CreatedBy = model.UpdatedBy ?? "System",
                                    CompanyTaxId = shipToVm.CNPJ ?? "",
                                    Address = shipToVm.Address ?? "",
                                    Neighborhood = shipToVm.Neighborhood ?? "",
                                    City = shipToVm.City ?? "",
                                    PostalCode = shipToVm.PostalCode ?? "",
                                    State = shipToVm.State ?? "",
                                    SapOrder = shipToVm.SAPOrderNumber ?? "",
                                    SapOrderService = ""
                                };

                                var createdShipTo = await _orderShipToRepository.CreateAsync(orderShipTo);

                                // Salvar Order Items
                                foreach (var itemVm in shipToVm.OrderItems ?? new List<OrderItemViewModel>())
                                {
                                    if (!string.IsNullOrEmpty(itemVm.PartNumber) || itemVm.Qty > 0)
                                    {
                                        var orderProduct = new OrderProduct
                                        {
                                            IdOrderShipTo = createdShipTo.Id,
                                            CreatedOn = DateTime.UtcNow.AddHours(-3),
                                            CreatedBy = model.UpdatedBy ?? "System",
                                            BID = itemVm.BidContractNumber ?? "",
                                            ContractNumber = itemVm.BidContractNumber ?? "",
                                            PartNumber = itemVm.PartNumber ?? "",
                                            MTMDescription = itemVm.PartNumberDescription ?? "",
                                            Quantity = itemVm.Qty ?? 0,
                                            UnitNetPrice = itemVm.UnityNetPrice ?? 0,
                                            UnitGrossPrice = itemVm.UnitGrossPrice ?? 0
                                        };

                                        var createdProduct = await _orderProductRepository.CreateAsync(orderProduct);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                
                var detailedMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Erro ao atualizar nas entidades Order: {detailedMessage}", ex);
            }
        }

        private async Task SaveToOrderEntities(OnlTicketViewModel model)
        {
            try
            {
                // Buscar dados válidos para os dropdowns
                var validOrderTypes = await GetOrderTypes();
                var validNFTypes = await GetNFTypes();
                var validCustomerSegments = await GetCustomerSegments();
                var validCountries = await GetCountries();
                
                var defaultOrderTypeId = validOrderTypes.FirstOrDefault()?.Id ?? 1;
                var defaultNFTypeId = validNFTypes.FirstOrDefault()?.Id ?? 1;
                
                // Buscar IDs específicos dos dropdowns selecionados
                var selectedOrderTypeId = TryParseInt(model.OrderType) ?? defaultOrderTypeId;
                var selectedNFTypeId = TryParseInt(model.NFType) ?? defaultNFTypeId;
                var selectedSegmentId = model.SegmentId ?? TryParseInt(model.Segment);

                
                // Buscar textos correspondentes aos IDs selecionados
                var selectedOrderTypeText = validOrderTypes.FirstOrDefault(x => x.Id == selectedOrderTypeId)?.Name ?? model.OrderType ?? "";
                var selectedNFTypeText = validNFTypes.FirstOrDefault(x => x.Id == selectedNFTypeId)?.Name ?? model.NFType ?? "";
                var selectedSegmentText = validCustomerSegments.FirstOrDefault(x => x.Id == selectedSegmentId)?.Name ?? model.Segment ?? "";
                var selectedCountryText = validCountries.FirstOrDefault(x => x.Id == model.CountryId)?.Name ?? model.Country ?? "";
                

                // Criar OrderNotLoaded
                var orderNotLoaded = new OrderNotLoaded
                {
                    CreatedOn = DateTime.UtcNow.AddHours(-3),
                    CreatedBy = model.CreatedBy ?? "System",
                    UpdatedOn = DateTime.UtcNow.AddHours(-3),
                    UpdatedBy = model.UpdatedBy ?? "System",
                    OrderStatus = model.Status ?? "",
                    From = model.EmailFrom ?? "",
                    Subject = "",
                    NumberOrder = model.LogNumber ?? "",
                    AssignedTo = model.AssignedOperator ?? "",
                    EmailResolutionOwner = "",
                    PONumber = model.PONumber ?? "",
                    OrderType = selectedOrderTypeText,
                    NFType = selectedNFTypeText,
                    Customer = model.CustomerName ?? "",
                    DMU = model.DMU ?? "",
                    Country = selectedCountryText,
                    IdCountry = model.CountryId,
                    BusinessUnit = model.BU ?? "",
                    PODate = model.PODate ?? DateTime.UtcNow,
                    Segment = selectedSegmentText,
                    IdSegment = selectedSegmentId,
                    Region = model.Region ?? "",
                    State = model.UF ?? "",
                    RecolocationType = model.ReplacementType ?? "",
                    BillAhead = model.BillAhead ?? "",
                    ISRName = model.ISRName ?? "",
                    CancelReason = "",
                    // Usar IDs corretos
                    IdNFType = selectedNFTypeId,
                    IdOrderType = selectedOrderTypeId,
                    // FKs nullable para evitar constraint errors
                    IdOrderStatus = null,
                    IdBusinessUnit = null,
                    IdCustomer = null,
                    IdCancelReason = null
                };

                var createdOrder = await _orderNotLoadedRepository.CreateOrderNotLoadedAsync(orderNotLoaded);

                // Salvar SoldTo, ShipTo e Products
                foreach (var soldToVm in model.SoldToAddresses ?? new List<SoldToViewModel>())
                {
                    if (!string.IsNullOrEmpty(soldToVm.CNPJ) || !string.IsNullOrEmpty(soldToVm.Address))
                    {
                        var orderSoldTo = new OrderSoldTO
                        {
                            IdOrderNotLoaded = createdOrder.Id,
                            CompanyTaxId = soldToVm.CNPJ ?? "",
                            Address = soldToVm.Address ?? "",
                            Neighborhood = soldToVm.Neighborhood ?? "",
                            City = soldToVm.City ?? "",
                            PostalCode = soldToVm.PostalCode ?? "",
                            State = soldToVm.State ?? "",
                            CreatedBy = model.CreatedBy ?? "System"
                        };

                        var createdSoldTo = await _orderSoldTORepository.CreateAsync(orderSoldTo);

                        foreach (var shipToVm in soldToVm.ShipToAddresses ?? new List<ShipToViewModel>())
                        {
                            if (!string.IsNullOrEmpty(shipToVm.CNPJ) || !string.IsNullOrEmpty(shipToVm.Address))
                            {
                                var orderShipTo = new OrderShipTo
                                {
                                    IdOrderNotLoaded = createdOrder.Id,
                                    IdOrderSoldTo = createdSoldTo.Id,
                                    CompanyTaxId = shipToVm.CNPJ ?? "",
                                    Address = shipToVm.Address ?? "",
                                    Neighborhood = shipToVm.Neighborhood ?? "",
                                    City = shipToVm.City ?? "",
                                    PostalCode = shipToVm.PostalCode ?? "",
                                    State = shipToVm.State ?? "",
                                    SapOrder = shipToVm.SAPOrderNumber ?? "",
                                    SapOrderService = "",
                                    CreatedBy = model.CreatedBy ?? "System"
                                };

                                var createdShipTo = await _orderShipToRepository.CreateAsync(orderShipTo);

                                foreach (var itemVm in shipToVm.OrderItems ?? new List<OrderItemViewModel>())
                                {
                                    if (!string.IsNullOrEmpty(itemVm.PartNumber))
                                    {
                                        var orderProduct = new OrderProduct
                                        {
                                            IdOrderShipTo = createdShipTo.Id,
                                            CreatedOn = DateTime.Now,
                                            CreatedBy = model.CreatedBy ?? "System",
                                            BID = itemVm.BidContractNumber ?? "",
                                            ContractNumber = itemVm.BidContractNumber ?? "",
                                            PartNumber = itemVm.PartNumber ?? "",
                                            MTMDescription = itemVm.PartNumberDescription ?? "",
                                            Quantity = itemVm.Qty ?? 0,
                                            UnitNetPrice = itemVm.UnityNetPrice ?? 0,
                                            UnitGrossPrice = itemVm.UnitGrossPrice ?? 0
                                        };

                                        await _orderProductRepository.CreateAsync(orderProduct);
                                    }
                                }
                            }
                        }
                    }
                }

                // CORRIGIDO: Salvar Attachments que estavam sendo ignorados
                foreach (var attachmentVm in model.Attachments ?? new List<OnlTicketAttachmentViewModel>())
                {
                    // Verificar se o anexo tem dados válidos (metadados ou arquivo)
                    if (!string.IsNullOrEmpty(attachmentVm.CustomerPO) || 
                        !string.IsNullOrEmpty(attachmentVm.Description) || 
                        !string.IsNullOrEmpty(attachmentVm.Comments) ||
                        !string.IsNullOrEmpty(attachmentVm.FileData))
                    {
                        
                        // Converter Base64 para bytes se houver dados do arquivo
                        byte[] fileBytes = new byte[] { 0x00 }; // Default vazio
                        if (!string.IsNullOrEmpty(attachmentVm.FileData))
                        {
                            try
                            {
                                // Remover prefixo data:type/subtype;base64, se existir
                                var base64Data = attachmentVm.FileData;
                                if (base64Data.Contains(","))
                                {
                                    base64Data = base64Data.Split(',')[1];
                                }
                                fileBytes = Convert.FromBase64String(base64Data);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        
                        var orderAttachment = new OrderAttachment
                        {
                            IdOrderNotLoaded = createdOrder.Id,
                            CreatedOn = DateTime.UtcNow.AddHours(-3),
                            CreatedBy = model.CreatedBy ?? "System",
                            PONumber = attachmentVm.CustomerPO ?? "",
                            Description = attachmentVm.Description ?? "",
                            Comments = attachmentVm.Comments ?? "",
                            AttachemntFileName = attachmentVm.FileName ?? "unknown_file",
                            Attachment = fileBytes,
                            FileExtension = attachmentVm.FileExtension ?? ".txt"
                        };

                        var createdAttachment = await _orderAttachmentRepository.CreateAsync(orderAttachment);
                    }
                }

            }
            catch (Exception ex)
            {
                
                var detailedMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Erro ao salvar nas entidades Order: {detailedMessage}", ex);
            }
        }

        public async Task<OrderAttachment?> GetAttachmentByIdAsync(int id)
        {
            return await _orderAttachmentRepository.GetByIdAsync(id);
        }

        // Dropdown methods
        public async Task<IEnumerable<OnlTicketOrderTypeViewModel>> GetOrderTypes() => _mapper.Map<IEnumerable<OnlTicketOrderTypeViewModel>>(await _orderNotLoadedRepository.GetAllOrderTypes());

        public async Task<IEnumerable<OnlTicketOrderStatusViewModel>> GetOrderStatuses() => _mapper.Map<IEnumerable<OnlTicketOrderStatusViewModel>>(await _orderNotLoadedRepository.GetAllOrderStatuses());

        public async Task<IEnumerable<OnlTicketNFTypeViewModel>> GetNFTypes() => _mapper.Map<IEnumerable<OnlTicketNFTypeViewModel>>(await _orderNotLoadedRepository.GetAllNFTypes());

        public async Task<IEnumerable<OnlTicketCustomerSegmentViewModel>> GetCustomerSegments() => _mapper.Map<IEnumerable<OnlTicketCustomerSegmentViewModel>>(await _orderNotLoadedRepository.GetAllCustomerSegments());
             
        public async Task<IEnumerable<CountryViewModel>> GetCountries() => _mapper.Map<IEnumerable<CountryViewModel>>(await _orderNotLoadedRepository.GetAllCountries());

        public async Task<IEnumerable<string>> GetSegments()
        {
            // TODO: Buscar do banco de dados quando a tabela estiver criada
            return new List<string>
            {
                "Enterprise",
                "SMB",
                "Consumer",
                "Government"
            };
        }

        public async Task<IEnumerable<string>> GetCustomerNames()
        {
            // TODO: Buscar do banco de dados quando a tabela estiver criada
            return new List<string>
            {
                "Cliente A",
                "Cliente B", 
                "Cliente C"
            };
        }

        public async Task<IEnumerable<string>> GetStatuses()
        {
            // TODO: Buscar do banco de dados quando a tabela estiver criada
            return new List<string>
            {
                "Draft",
                "Active",
                "Completed",
                "Cancelled"
            };
        }

        public async Task<IEnumerable<string>> GetAssignToUsers()
        {
            // TODO: Buscar do banco de dados quando a tabela estiver criada
            return new List<string>
            {
                "Operador1",
                "Operador2",
                "Operador3"
            };
        }

        public async Task<IEnumerable<string>> GetAgingBuckets()
        {
            // TODO: Buscar do banco de dados quando a tabela estiver criada
            return new List<string>
            {
                "0-7 days",
                "8-15 days",
                "16-30 days",
                ">30 days"
            };
        }

        public async Task<IEnumerable<string>> GetCreatedByUsers()
        {
            // TODO: Buscar do banco de dados quando a tabela estiver criada
            return new List<string>
            {
                "admin",
                "user1",
                "user2"
            };
        }

        private async Task PopulateDropdowns(OnlTicketViewModel model)
        {
            try
            {
                
                model.OrderTypes = (await GetOrderTypes()).ToList();
                
                model.OrderStatuses = (await GetOrderStatuses()).ToList();
                
                model.NFTypes = (await GetNFTypes()).ToList();
                
                model.CustomerSegments = (await GetCustomerSegments()).ToList();
                
                model.Countries = (await GetCountries()).ToList();
                
                model.Segments = (await GetSegments()).ToList();
                
                model.CustomerNames = (await GetCustomerNames()).ToList();
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Methods for Order entities - MIGRAÇÃO COMPLETA
        public async Task<IEnumerable<OrderNotLoaded>> GetAllOrderNotLoadedAsync()
        {
            return await _orderNotLoadedRepository.GetAllOrderNotLoadedAsync();
        }

        public async Task<OrderNotLoaded?> GetOrderNotLoadedByIdAsync(long id)
        {
            return await _orderNotLoadedRepository.GetOrderNotLoadedByIdAsync(id);
        }

        private int? TryParseInt(string? value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            if (int.TryParse(value, out int result)) return result;
            return null;
        }

        private string GetContentTypeFromExtension(string? fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension)) return "application/octet-stream";
            
            return fileExtension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}
