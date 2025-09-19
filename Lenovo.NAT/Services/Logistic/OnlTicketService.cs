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
        
        // New methods for real data
        Task<IEnumerable<OnlTicket>> GetAllOnlTicketsAsync();
        Task<OnlTicket?> GetOnlTicketByIdAsync(int id);
        Task<OrderAttachment?> GetAttachmentByIdAsync(int id);
    }

    public class OnlTicketService : IOnlTicketService
    {
        private readonly IOnlTicketRepository _onlTicketRepository;
        private readonly IOrderNotLoadedRepository _orderNotLoadedRepository;
        private readonly IOrderSoldTORepository _orderSoldTORepository;
        private readonly IOrderShipToRepository _orderShipToRepository;
        private readonly IOrderProductRepository _orderProductRepository;
        private readonly IOrderAttachmentRepository _orderAttachmentRepository;
        private readonly IMapper _mapper;

        public OnlTicketService(
            IOnlTicketRepository onlTicketRepository,
            IOrderNotLoadedRepository orderNotLoadedRepository,
            IOrderSoldTORepository orderSoldTORepository,
            IOrderShipToRepository orderShipToRepository,
            IOrderProductRepository orderProductRepository,
            IOrderAttachmentRepository orderAttachmentRepository,
            IMapper mapper)
        {
            _onlTicketRepository = onlTicketRepository;
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
                Console.WriteLine($"[DEBUG] GetOnlTicketDetail - Iniciando busca para ID: {id}");
                
                // MIGRADO: Buscar dados das novas tabelas Order
                var orderNotLoaded = await _orderNotLoadedRepository.GetOrderNotLoadedByIdAsync(id);
                if (orderNotLoaded == null)
                {
                    Console.WriteLine($"[DEBUG] Order com ID {id} não encontrado");
                    throw new InvalidOperationException($"Order com ID {id} não encontrado");
                }

                Console.WriteLine($"[DEBUG] Order encontrado: {orderNotLoaded.NumberOrder}");

                var model = await MapOrderToViewModel(orderNotLoaded);
                
                Console.WriteLine($"[DEBUG] Modelo mapeado, iniciando população de dropdowns");
                
                // CORRIGIDO: Popular dropdowns para exibir na tela
                await PopulateDropdowns(model);
                
                Console.WriteLine($"[DEBUG] Dropdowns populadas:");
                Console.WriteLine($"  - OrderTypes: {model.OrderTypes?.Count ?? 0} itens");
                Console.WriteLine($"  - OrderStatuses: {model.OrderStatuses?.Count ?? 0} itens");
                Console.WriteLine($"  - NFTypes: {model.NFTypes?.Count ?? 0} itens");
                Console.WriteLine($"  - CustomerSegments: {model.CustomerSegments?.Count ?? 0} itens");
                Console.WriteLine($"  - Countries: {model.Countries?.Count ?? 0} itens");
                
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Erro em GetOnlTicketDetail: {ex.Message}");
                Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");
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
                return "ONL Ticket cadastrado com sucesso!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro em SaveOnlTicket: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
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

                Console.WriteLine($"[DEBUG] UpdateOnlTicket - Iniciando atualização para ID: {model.ID}");

                // MIGRADO: Verificar se existe nas tabelas Order
                var orderId = long.Parse(model.ID);
                var existingOrder = await _orderNotLoadedRepository.GetOrderNotLoadedByIdAsync(orderId);
                if (existingOrder == null)
                    throw new ArgumentException("ONL Ticket não encontrado nas tabelas Order");

                Console.WriteLine($"[DEBUG] Registro encontrado: {existingOrder.NumberOrder}");

                // CORRIGIDO: Usar método específico para atualização
                await UpdateOrderEntities(model, orderId);
                return "ONL Ticket atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Erro em UpdateOnlTicket: {ex.Message}");
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
                Descricao = a.Description,
                Comentarios = a.Comments,
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
                Console.WriteLine($"[DEBUG] UpdateOrderEntities - Iniciando atualização para Order ID: {orderId}");

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

                Console.WriteLine($"IDs selecionados: OrderType={selectedOrderTypeId}, NFType={selectedNFTypeId}, Segment={selectedSegmentId}");
                
                // Buscar textos correspondentes aos IDs selecionados
                var selectedOrderTypeText = validOrderTypes.FirstOrDefault(x => x.Id == selectedOrderTypeId)?.Name ?? model.OrderType ?? "";
                var selectedNFTypeText = validNFTypes.FirstOrDefault(x => x.Id == selectedNFTypeId)?.Name ?? model.NFType ?? "";
                var selectedSegmentText = validCustomerSegments.FirstOrDefault(x => x.Id == selectedSegmentId)?.Name ?? model.Segment ?? "";
                var selectedCountryText = validCountries.FirstOrDefault(x => x.Id == model.CountryId)?.Name ?? model.Country ?? "";
                
                Console.WriteLine($"Textos encontrados: OrderType='{selectedOrderTypeText}', NFType='{selectedNFTypeText}', Segment='{selectedSegmentText}', Country='{selectedCountryText}'");

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
                Console.WriteLine($"OrderNotLoaded atualizado com ID: {updatedOrder.Id}");

                // TODO: Implementar atualização de entidades relacionadas (SoldTo, ShipTo, Products, Attachments)
                // Por enquanto, vamos manter as entidades relacionadas como estão
                // Em uma versão futura, implementar a lógica completa de atualização

                Console.WriteLine("UpdateOrderEntities concluído com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro em UpdateOrderEntities: {ex.Message}");
                Console.WriteLine($"InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
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

                Console.WriteLine($"IDs selecionados: OrderType={selectedOrderTypeId}, NFType={selectedNFTypeId}, Segment={selectedSegmentId}");
                
                // Buscar textos correspondentes aos IDs selecionados
                var selectedOrderTypeText = validOrderTypes.FirstOrDefault(x => x.Id == selectedOrderTypeId)?.Name ?? model.OrderType ?? "";
                var selectedNFTypeText = validNFTypes.FirstOrDefault(x => x.Id == selectedNFTypeId)?.Name ?? model.NFType ?? "";
                var selectedSegmentText = validCustomerSegments.FirstOrDefault(x => x.Id == selectedSegmentId)?.Name ?? model.Segment ?? "";
                var selectedCountryText = validCountries.FirstOrDefault(x => x.Id == model.CountryId)?.Name ?? model.Country ?? "";
                
                Console.WriteLine($"Textos encontrados: OrderType='{selectedOrderTypeText}', NFType='{selectedNFTypeText}', Segment='{selectedSegmentText}', Country='{selectedCountryText}'");

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
                Console.WriteLine($"OrderNotLoaded criado com ID: {createdOrder.Id}");

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
                Console.WriteLine($"Salvando {model.Attachments?.Count ?? 0} anexos...");
                foreach (var attachmentVm in model.Attachments ?? new List<OnlTicketAttachmentViewModel>())
                {
                    // Verificar se o anexo tem dados válidos (metadados ou arquivo)
                    if (!string.IsNullOrEmpty(attachmentVm.CustomerPO) || 
                        !string.IsNullOrEmpty(attachmentVm.Descricao) || 
                        !string.IsNullOrEmpty(attachmentVm.Comentarios) ||
                        !string.IsNullOrEmpty(attachmentVm.FileData))
                    {
                        Console.WriteLine($"[DEBUG] Dados do anexo recebidos:");
                        Console.WriteLine($"  - FileName: '{attachmentVm.FileName}'");
                        Console.WriteLine($"  - FileExtension: '{attachmentVm.FileExtension}'");
                        Console.WriteLine($"  - ContentType: '{attachmentVm.ContentType}'");
                        Console.WriteLine($"  - FileSize: {attachmentVm.FileSize}");
                        Console.WriteLine($"  - CustomerPO: '{attachmentVm.CustomerPO}'");
                        Console.WriteLine($"  - Descricao: '{attachmentVm.Descricao}'");
                        Console.WriteLine($"  - FileData: {(string.IsNullOrEmpty(attachmentVm.FileData) ? "VAZIO" : $"{attachmentVm.FileData.Length} caracteres")}");
                        
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
                                Console.WriteLine($"Arquivo convertido: {fileBytes.Length} bytes");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Erro ao converter Base64: {ex.Message}");
                            }
                        }
                        
                        var orderAttachment = new OrderAttachment
                        {
                            IdOrderNotLoaded = createdOrder.Id,
                            CreatedOn = DateTime.UtcNow.AddHours(-3),
                            CreatedBy = model.CreatedBy ?? "System",
                            PONumber = attachmentVm.CustomerPO ?? "",
                            Description = attachmentVm.Descricao ?? "",
                            Comments = attachmentVm.Comentarios ?? "",
                            AttachemntFileName = attachmentVm.FileName ?? "unknown_file",
                            Attachment = fileBytes,
                            FileExtension = attachmentVm.FileExtension ?? ".txt"
                        };

                        var createdAttachment = await _orderAttachmentRepository.CreateAsync(orderAttachment);
                        Console.WriteLine($"Anexo salvo com ID: {createdAttachment.Id}, Nome: {attachmentVm.FileName}");
                    }
                }

                Console.WriteLine("SaveToOrderEntities concluído com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro em SaveToOrderEntities: {ex.Message}");
                Console.WriteLine($"InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
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
                Console.WriteLine("[DEBUG] PopulateDropdowns - Iniciando população de dropdowns");
                
                model.OrderTypes = (await GetOrderTypes()).ToList();
                Console.WriteLine($"[DEBUG] OrderTypes carregados: {model.OrderTypes.Count}");
                
                model.OrderStatuses = (await GetOrderStatuses()).ToList();
                Console.WriteLine($"[DEBUG] OrderStatuses carregados: {model.OrderStatuses.Count}");
                
                model.NFTypes = (await GetNFTypes()).ToList();
                Console.WriteLine($"[DEBUG] NFTypes carregados: {model.NFTypes.Count}");
                
                model.CustomerSegments = (await GetCustomerSegments()).ToList();
                Console.WriteLine($"[DEBUG] CustomerSegments carregados: {model.CustomerSegments.Count}");
                
                model.Countries = (await GetCountries()).ToList();
                Console.WriteLine($"[DEBUG] Countries carregados: {model.Countries.Count}");
                
                model.Segments = (await GetSegments()).ToList();
                Console.WriteLine($"[DEBUG] Segments carregados: {model.Segments.Count}");
                
                model.CustomerNames = (await GetCustomerNames()).ToList();
                Console.WriteLine($"[DEBUG] CustomerNames carregados: {model.CustomerNames.Count}");
                
                Console.WriteLine("[DEBUG] PopulateDropdowns - Finalizado com sucesso");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Erro em PopulateDropdowns: {ex.Message}");
                Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        // New methods for real data - MIGRADO PARA TABELAS ORDER
        public async Task<IEnumerable<OnlTicket>> GetAllOnlTicketsAsync()
        {
            // Buscar dados das novas tabelas Order e mapear para OnlTicket (para compatibilidade com a listagem)
            var orderNotLoadedList = await _orderNotLoadedRepository.GetAllOrderNotLoadedAsync();
            
            var onlTicketList = new List<OnlTicket>();
            
            foreach (var order in orderNotLoadedList)
            {
                // Buscar dados relacionados
                var soldTos = await _orderSoldTORepository.GetByOrderIdAsync(order.Id);
                var shipTos = await _orderShipToRepository.GetByOrderIdAsync(order.Id);
                var products = await _orderProductRepository.GetByOrderIdAsync(order.Id);
                
                var onlTicket = new OnlTicket
                {
                    Id = (int)order.Id,
                    LogNumber = order.NumberOrder,
                    CreatedDate = order.CreatedOn,
                    CreatedBy = order.CreatedBy,
                    UpdatedDate = order.UpdatedOn,
                    UpdatedBy = order.UpdatedBy,
                    Status = order.OrderStatus,
                    EmailFrom = order.From,
                    AssignedOperator = order.AssignedTo,
                    PONumber = order.PONumber,
                    OrderType = order.OrderType,
                    OrderStatus = order.OrderStatus,
                    NFType = order.NFType,
                    CustomerName = order.Customer,
                    DMU = order.DMU,
                    Country = order.Country,
                    CountryId = order.IdCountry,
                    BU = order.BusinessUnit,
                    PODate = order.PODate,
                    Segment = order.Segment,
                    SegmentId = order.IdSegment,
                    BillAhead = order.BillAhead,
                    ISRName = order.ISRName,
                    Region = order.Region,
                    UF = order.State,
                    ReplacementType = order.RecolocationType,
                    SoldToAddresses = new List<OnlTicketSoldTo>(),
                    Attachments = new List<OnlTicketAttachment>(),
                    SAPOrders = new List<OnlTicketSAPOrder>(),
                    Comments = new List<OnlTicketComment>()
                };
                
                onlTicketList.Add(onlTicket);
            }
            
            return onlTicketList;
        }

        public async Task<OnlTicket?> GetOnlTicketByIdAsync(int id)
        {
            // Buscar nas novas tabelas Order
            var order = await _orderNotLoadedRepository.GetOrderNotLoadedByIdAsync(id);
            if (order == null) return null;

            // Mapear para OnlTicket (compatibilidade)
            var onlTicket = new OnlTicket
            {
                Id = (int)order.Id,
                LogNumber = order.NumberOrder,
                CreatedDate = order.CreatedOn,
                CreatedBy = order.CreatedBy,
                UpdatedDate = order.UpdatedOn,
                UpdatedBy = order.UpdatedBy,
                Status = order.OrderStatus,
                EmailFrom = order.From,
                AssignedOperator = order.AssignedTo,
                PONumber = order.PONumber,
                OrderType = order.OrderType,
                OrderStatus = order.OrderStatus,
                NFType = order.NFType,
                CustomerName = order.Customer,
                DMU = order.DMU,
                Country = order.Country,
                CountryId = order.IdCountry,
                BU = order.BusinessUnit,
                PODate = order.PODate,
                Segment = order.Segment,
                SegmentId = order.IdSegment,
                BillAhead = order.BillAhead,
                ISRName = order.ISRName,
                Region = order.Region,
                UF = order.State,
                ReplacementType = order.RecolocationType,
                SoldToAddresses = new List<OnlTicketSoldTo>(),
                Attachments = new List<OnlTicketAttachment>(),
                SAPOrders = new List<OnlTicketSAPOrder>(),
                Comments = new List<OnlTicketComment>()
            };

            return onlTicket;
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
