using Lenovo.NAT.Services.Logistic;
using Lenovo.NAT.ViewModel.Logistic.OnlTicket;
using Lenovo.NAT.ViewModel.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace Lenovo.NAT.Areas.Logistic.Controllers;

[Area("Logistic")]
public class OnlTicketController : Controller
{
    private readonly IOnlTicketService _onlTicketService;

    public OnlTicketController(IOnlTicketService onlTicketService)
    {
        _onlTicketService = onlTicketService;
    }
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 25, string? logNumber = null, string? customer = null, 
        string? segment = null, string? orderType = null, string? assignTo = null, string? agingBucket = null, 
        DateTime? createdAt = null, string? createdBy = null, string? status = null, string? sapOrder = null)
    {
        // Criar o filtro
        var filter = new OnlTicketFilter
        {
            Page = page,
            PageSize = pageSize,
            LogNumber = logNumber,
            Customer = customer,
            Segment = segment,
            OrderType = orderType,
            AssignTo = assignTo,
            AgingBucket = agingBucket,
            CreatedAt = createdAt,
            CreatedBy = createdBy,
            Status = status,
            SAPOrder = sapOrder,
            IsAdmin = User.IsInRole("Admin") // Ajustar conforme sua lógica de permissões
        };

        // Popular listas para dropdowns usando o service
        filter.Statuses = (await _onlTicketService.GetStatuses()).ToList();
        filter.Customers = (await _onlTicketService.GetCustomerNames()).ToList();
        filter.Segments = (await _onlTicketService.GetSegments()).ToList();
        filter.CustomerSegments = (await _onlTicketService.GetCustomerSegments()).ToList();
        filter.OrderTypes = (await _onlTicketService.GetOrderTypes()).ToList();
        filter.AssignToUsers = (await _onlTicketService.GetAssignToUsers()).ToList();
        filter.AgingBuckets = (await _onlTicketService.GetAgingBuckets()).ToList();
        filter.CreatedByUsers = (await _onlTicketService.GetCreatedByUsers()).ToList();

        // Buscar dados reais do banco - MIGRADO PARA ORDER
        var orderNotLoadedList = await _onlTicketService.GetAllOrderNotLoadedAsync();
        
        // Mapear para ViewModel de listagem
        var onlTicketList = orderNotLoadedList.Select(x => new OnlTicketListViewModel
        {
            Id = (int)x.Id,
            LogNumber = x.NumberOrder ?? "",
            Customer = x.Customer ?? "",
            Segment = x.Segment ?? "",
            AssignTo = x.AssignedTo ?? "",
            OrderAging = x.PODate.ToString("dd/MM/yyyy"),
            AgingBucket = "0-7 days", // TODO: Calcular baseado na data
            SAPOrder = "", // TODO: Implementar quando necessário
            TotalCAs = 0, // TODO: Implementar quando necessário
            CreatedAt = x.CreatedOn,
            CreatedBy = x.CreatedBy ?? "",
            EmailFrom = x.From ?? "",
            UpdatedAt = x.UpdatedOn,
            Status = x.OrderStatus ?? ""
        }).ToList();

        // Criar modelo paginado
        var model = new PaginatedOnlTicketViewModel(
            new PaginationInfo 
            { 
                Page = page, 
                PageSize = pageSize, 
                TotalItems = onlTicketList.Count,
                TotalPages = (int)Math.Ceiling((double)onlTicketList.Count / pageSize)
            }, 
            filter, 
            onlTicketList
        );

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            // CORRIGIDO: GetOnlTicketDetail já popula as dropdowns internamente
            var model = await _onlTicketService.GetOnlTicketDetail(id);
            
            if (model == null)
            {
                TempData["Error"] = "ONL Ticket não encontrado!";
                return RedirectToAction("Index");
            }

            // VERIFICAÇÃO: Garantir que as dropdowns não estejam nulas
            if (model.OrderTypes == null || !model.OrderTypes.Any())
            {
                await PopulateDropdowns(model);
            }


            // CORREÇÃO: Limpar TempData de sucesso para evitar toast indevido no Details
            TempData.Remove("Success");
            
            return View("Create", model);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading ONL Ticket: {ex.Message}";
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public IActionResult Duplicate(int id)
    {
        // TODO: Duplicar ticket existente
        return RedirectToAction("Create");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        // Redirecionar para Details que já carrega os dados
        return RedirectToAction("Details", new { id = id });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(OnlTicketViewModel model)
    {
        // Inicializar lista de anexos se for null (não adicionar item vazio)
        if (model.Attachments == null)
        {
            model.Attachments = new List<OnlTicketAttachmentViewModel>();
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(model);
            return View("Create", model);
        }

        try
        {
            // Atualizar o ticket no banco de dados
            await _onlTicketService.UpdateOnlTicket(model);
            
            TempData["Success"] = "ONL Ticket updated successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Log do erro (implementar logging adequado)
            ModelState.AddModelError("", $"Error updating ONL Ticket: {ex.Message}");
            await PopulateDropdowns(model);
            return View("Create", model);
        }
    }

    [HttpPost]
    public IActionResult ExportExcel(OnlTicketFilter filter)
    {
        // TODO: Implementar exportação para Excel
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult AdminExportExcel(OnlTicketFilter filter)
    {
        // TODO: Implementar exportação administrativa para Excel
        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = await _onlTicketService.GetEmptyOnlTicket();

        // Popular dropdowns
        model.OrderTypes = (await _onlTicketService.GetOrderTypes()).ToList();
        model.OrderStatuses = (await _onlTicketService.GetOrderStatuses()).ToList();
        model.NFTypes = (await _onlTicketService.GetNFTypes()).ToList();
        model.CustomerSegments = (await _onlTicketService.GetCustomerSegments()).ToList();
        model.Countries = (await _onlTicketService.GetCountries()).ToList();
        model.Segments = (await _onlTicketService.GetSegments()).ToList();
        model.CustomerNames = (await _onlTicketService.GetCustomerNames()).ToList();

        // Inicializar lista de anexos vazia (não adicionar item vazio)
        if (model.Attachments == null)
        {
            model.Attachments = new List<OnlTicketAttachmentViewModel>();
        }

        // Inicializar lista de SoldToAddresses se estiver vazia
        if (model.SoldToAddresses == null || model.SoldToAddresses.Count == 0)
        {
            model.SoldToAddresses = new List<SoldToViewModel>();
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(OnlTicketViewModel model)
    {
        // Inicializar lista de anexos se for null (não adicionar item vazio)
        if (model.Attachments == null)
        {
            model.Attachments = new List<OnlTicketAttachmentViewModel>();
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(model);
            return View(model);
        }

        try
        {
            // Salvar o ticket no banco de dados
            await _onlTicketService.SaveOnlTicket(model);
            
            TempData["Success"] = "ONL Ticket created successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Log do erro (implementar logging adequado)
            ModelState.AddModelError("", $"Error saving ONL Ticket: {ex.Message}");
            await PopulateDropdowns(model);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> DownloadAttachment(int id)
    {
        try
        {
            // Buscar o anexo na tabela OrderAttachment
            var attachment = await _onlTicketService.GetAttachmentByIdAsync(id);
            
            if (attachment == null)
            {
                TempData["Error"] = "Arquivo não encontrado!";
                return RedirectToAction("Index");
            }

            if (attachment.Attachment == null || attachment.Attachment.Length == 0)
            {
                TempData["Error"] = "Arquivo não possui dados!";
                return RedirectToAction("Index");
            }

            // Determinar o content type baseado na extensão
            var contentType = GetContentType(attachment.FileExtension ?? ".txt");
            var fileName = attachment.AttachemntFileName ?? "download";

            // Verificar se dados são válidos
            if (attachment.Attachment.Length == 1 && attachment.Attachment[0] == 0x00)
            {
                TempData["Error"] = "Arquivo está vazio ou corrompido!";
                return RedirectToAction("Index");
            }

            var result = File(attachment.Attachment, contentType, fileName);
            return result;
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Erro ao baixar arquivo: {ex.Message}";
            return RedirectToAction("Index");
        }
    }

    private string GetContentType(string fileExtension)
    {
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

    private async Task PopulateDropdowns(OnlTicketViewModel model)
    {
        model.OrderTypes = (await _onlTicketService.GetOrderTypes()).ToList();
        model.OrderStatuses = (await _onlTicketService.GetOrderStatuses()).ToList();
        model.NFTypes = (await _onlTicketService.GetNFTypes()).ToList();
        model.CustomerSegments = (await _onlTicketService.GetCustomerSegments()).ToList();
        model.Countries = (await _onlTicketService.GetCountries()).ToList();
        model.Segments = (await _onlTicketService.GetSegments()).ToList();
        model.CustomerNames = (await _onlTicketService.GetCustomerNames()).ToList();
    }
}