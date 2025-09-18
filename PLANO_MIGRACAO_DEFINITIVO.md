# PLANO DE MIGRAÇÃO DEFINITIVO - ONL TICKET PARA ORDER

## 🎯 ESTRATÉGIA CONFIRMADA PELO CLIENTE

### **REGRAS FUNDAMENTAIS:**
- 🔒 **NUNCA REMOVER** campos das entidades Order existentes
- ➕ **PODE ADICIONAR** campos nas entidades Order se necessário
- 🖥️ **MANTER TELAS** intactas - apenas backend muda
- ⏸️ **NÃO MEXER AGORA** em OnlTicketComment e OnlTicketSAPOrder

---

## 📊 MIGRAÇÕES CONFIRMADAS

### **1. OnlTicket → OrderNotLoaded**
### **2. OnlTicketAttachment → OrderAttachment**  
### **3. OnlTicketOrderItem → OrderProduct**
### **4. OnlTicketShipTo → OrderShipTo**
### **5. OnlTicketSoldTo → OrderSoldTO**

---

## 🔍 ANÁLISE DETALHADA DE CAMPOS NECESSÁRIOS

### **1. OnlTicket → OrderNotLoaded**

#### **Campos que EXISTEM em OnlTicket mas FALTAM em OrderNotLoaded:**
```csharp
// ✅ JÁ EXISTEM em OrderNotLoaded:
// - DMU (já existe)
// - BillAhead (já existe) 
// - ISRName (já existe)
// - IdCountry/Country (já existem como Guid IdCountry)
// - IdSegment/Segment (já existem como int IdSegment)

// ❌ NÃO ADICIONAR:
// - Comentarios (cliente não quer)
// - OrderId (não é usado na tela - apenas no JavaScript interno)

// ✅ NENHUM CAMPO PRECISA SER ADICIONADO!
// OrderNotLoaded já tem todos os campos necessários.
```

#### **Mapeamento de Campos:**
```csharp
// OnlTicket → OrderNotLoaded
LogNumber → NumberOrder
CreatedDate → CreatedOn  
CreatedBy → CreatedBy
UpdatedDate → UpdatedOn
UpdatedBy → UpdatedBy
Status → OrderStatus
EmailFrom → From
AssignedOperator → AssignedTo
PONumber → PONumber
OrderType → OrderType
NFType → NFType
CustomerName → Customer
Country → Country
BU → BusinessUnit
PODate → PODate
Segment → Segment
Region → Region
UF → State
ReplacementType → RecolocationType
DMU → DMU (já existe)
BillAhead → BillAhead (já existe)
ISRName → ISRName (já existe)
CountryId → IdCountry (já existe como Guid)
SegmentId → IdSegment (já existe como int)
```

---

### **2. OnlTicketAttachment → OrderAttachment**

#### **Campos que EXISTEM em OnlTicketAttachment mas FALTAM em OrderAttachment:**
```csharp
// ✅ JÁ EXISTEM em OrderAttachment:
// - Id ✅
// - IdOrderNotLoaded ✅
// - CreatedOn ✅ (CreatedDate)
// - CreatedBy ✅
// - PONumber ✅ (CustomerPO)
// - AttachemntFileName ✅ (FileName)
// - Comments ✅ (Comentarios)
// - Description ✅ (Descricao)
// - Attachment ✅ (arquivo em byte[])

// ❌ FALTAM apenas 3 campos:
public string? FileExtension { get; set; }          // Extensão
public DateTime? UpdatedDate { get; set; }          // Data atualização
public string? UpdatedBy { get; set; }              // Usuário atualização
```

#### **Mapeamento de Campos:**
```csharp
// OnlTicketAttachment → OrderAttachment
Id → Id (manter)
OnlTicketId → IdOrderNotLoaded
CustomerPO → PONumber
Descricao → Description
Comentarios → Comments
FileName → AttachemntFileName
CreatedDate → CreatedOn
CreatedBy → CreatedBy
// + campos adicionados acima
```

---

### **3. OnlTicketOrderItem → OrderProduct**

#### **Campos que EXISTEM em OnlTicketOrderItem mas FALTAM em OrderProduct:**
```csharp
// ✅ JÁ EXISTEM TODOS OS CAMPOS em OrderProduct:
// - Id ✅ (chave primária)
// - IdOrderShipTo ✅ (OnlTicketShipToId)
// - CreatedOn ✅ (CreatedDate)
// - CreatedBy ✅
// - BID ✅ + ContractNumber ✅ (BidContractNumber separado)
// - PartNumber ✅
// - MTMDescription ✅ (PartNumberDescription)
// - Quantity ✅ (Qty)
// - UnitNetPrice ✅ (UnityNetPrice)
// - UnitGrossPrice ✅

// ✅ NENHUM CAMPO PRECISA SER ADICIONADO!
```

#### **Mapeamento de Campos:**
```csharp
// OnlTicketOrderItem → OrderProduct
Id → Id (já existe)
OnlTicketShipToId → IdOrderShipTo
BidContractNumber → BID + ContractNumber (split)
PartNumber → PartNumber
PartNumberDescription → MTMDescription
Qty → Quantity
UnityNetPrice → UnitNetPrice
UnitGrossPrice → UnitGrossPrice
CreatedDate → CreatedOn (já existe)
CreatedBy → CreatedBy (já existe)
```

---

### **4. OnlTicketShipTo → OrderShipTo**

#### **Campos que EXISTEM em OnlTicketShipTo mas FALTAM em OrderShipTo:**
```csharp
// ✅ JÁ EXISTEM TODOS OS CAMPOS em OrderShipTo:
// - Id ✅ (chave primária)
// - IdOrderNotLoaded ✅ (referência direta ao pedido)
// - IdOrderSoldTo ✅ (OnlTicketSoldToId)
// - CreatedOn ✅ (CreatedDate)
// - CreatedBy ✅
// - CompanyTaxId ✅ (CNPJ)
// - Address ✅ (Endereco)
// - Neighborhood ✅ (Bairro)
// - City ✅ (Municipio)
// - PostalCode ✅ (CEP)
// - State ✅ (UF)
// - SapOrder ✅ (SAPOrderNumber)
// - SapOrderService ✅ (novo campo)

// ✅ NENHUM CAMPO PRECISA SER ADICIONADO!
```

#### **Mapeamento de Campos:**
```csharp
// OnlTicketShipTo → OrderShipTo
Id → Id (já existe)
OnlTicketSoldToId → IdOrderSoldTo
CNPJ → CompanyTaxId
Endereco → Address
Bairro → Neighborhood
Municipio → City
CEP → PostalCode
UF → State
SAPOrderNumber → SapOrder
CreatedDate → CreatedOn (já existe)
CreatedBy → CreatedBy (já existe)
```

---

### **5. OnlTicketSoldTo → OrderSoldTO**

#### **Campos que EXISTEM em OnlTicketSoldTo mas FALTAM em OrderSoldTO:**
```csharp
// ✅ JÁ EXISTEM TODOS OS CAMPOS em OrderSoldTO:
// - Id ✅ (chave primária)
// - IdOrderNotLoaded ✅ (OnlTicketId)
// - CreatedOn ✅ (CreatedDate)
// - CreatedBy ✅
// - CompanyTaxId ✅ (CNPJ)
// - Address ✅ (Endereco)
// - Neighborhood ✅ (Bairro)
// - City ✅ (Municipio)
// - PostalCode ✅ (CEP)
// - State ✅ (UF)

// ✅ NENHUM CAMPO PRECISA SER ADICIONADO!
```

#### **Mapeamento de Campos:**
```csharp
// OnlTicketSoldTo → OrderSoldTO
Id → Id (já existe)
OnlTicketId → IdOrderNotLoaded
CNPJ → CompanyTaxId
Endereco → Address
Bairro → Neighborhood
Municipio → City
CEP → PostalCode
UF → State
CreatedDate → CreatedOn (já existe)
CreatedBy → CreatedBy (já existe)
```

---

## 🛠️ SCRIPTS DE EXTENSÃO DAS ENTIDADES

### **Script 1: OrderNotLoaded (NENHUMA ALTERAÇÃO NECESSÁRIA)**
```csharp
// ✅ OrderNotLoaded JÁ POSSUI TODOS OS CAMPOS NECESSÁRIOS!
// 
// Campos existentes que atendem às necessidades:
// - DMU (string) ✅
// - BillAhead (string) ✅
// - ISRName (string) ✅
// - IdCountry (Guid) ✅ -> mapeia para CountryId
// - IdSegment (int) ✅ -> mapeia para SegmentId
// - Country (string) ✅
// - Segment (string) ✅
//
// Campos não necessários:
// - Comentarios (cliente não quer) ❌
// - OrderId (não usado na tela) ❌
//
// CONCLUSÃO: Nenhuma extensão necessária para OrderNotLoaded!
```

### **Script 2: Estender OrderAttachment**
```csharp
// Arquivo: OrderAttachment.cs (adicionar campos)
public partial class OrderAttachment
{
    [StringLength(10)]
    public string? FileExtension { get; set; }
    
    public DateTime? UpdatedDate { get; set; }
    
    [StringLength(100)]
    public string? UpdatedBy { get; set; }
}
```

### **Script 3: OrderProduct (NENHUMA ALTERAÇÃO NECESSÁRIA)**
```csharp
// ✅ OrderProduct JÁ POSSUI TODOS OS CAMPOS NECESSÁRIOS!
//
// Campos existentes:
// - Id ✅ (chave primária)
// - IdOrderShipTo ✅
// - CreatedOn ✅ (DateTime)
// - CreatedBy ✅ (string)
// - BID ✅ + ContractNumber ✅
// - PartNumber ✅
// - MTMDescription ✅
// - Quantity ✅
// - UnitNetPrice ✅
// - UnitGrossPrice ✅
//
// CONCLUSÃO: Nenhuma extensão necessária para OrderProduct!
```

### **Script 4: OrderShipTo (NENHUMA ALTERAÇÃO NECESSÁRIA)**
```csharp
// ✅ OrderShipTo JÁ POSSUI TODOS OS CAMPOS NECESSÁRIOS!
//
// Campos existentes:
// - Id ✅ (chave primária)
// - IdOrderNotLoaded ✅
// - IdOrderSoldTo ✅
// - CreatedOn ✅ (DateTime)
// - CreatedBy ✅ (string)
// - CompanyTaxId ✅
// - Address ✅
// - Neighborhood ✅
// - City ✅
// - PostalCode ✅
// - State ✅
// - SapOrder ✅
// - SapOrderService ✅
//
// CONCLUSÃO: Nenhuma extensão necessária para OrderShipTo!
```

### **Script 5: OrderSoldTO (NENHUMA ALTERAÇÃO NECESSÁRIA)**
```csharp
// ✅ OrderSoldTO JÁ POSSUI TODOS OS CAMPOS NECESSÁRIOS!
//
// Campos existentes:
// - Id ✅ (chave primária)
// - IdOrderNotLoaded ✅
// - CreatedOn ✅ (DateTime)
// - CreatedBy ✅ (string)
// - CompanyTaxId ✅
// - Address ✅
// - Neighborhood ✅
// - City ✅
// - PostalCode ✅
// - State ✅
//
// Navigation Properties já podem ser adicionadas via configuração:
// public virtual ICollection<OrderShipTo> ShipToAddresses { get; set; } = new List<OrderShipTo>();
//
// CONCLUSÃO: Nenhuma extensão necessária para OrderSoldTO!
```

---

## 📦 MIGRATION SCRIPTS (Entity Framework)

### **Migration para adicionar campos**
```csharp
public partial class AddMissingFieldsToOrderEntities : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // OrderNotLoaded - NENHUMA ALTERAÇÃO NECESSÁRIA
        // Todos os campos necessários já existem na tabela!

        // OrderAttachment
        migrationBuilder.AddColumn<string>(
            name: "FileExtension",
            table: "OrderAttachment",
            type: "nvarchar(10)",
            maxLength: 10,
            nullable: true);
            
        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedDate",
            table: "OrderAttachment",
            type: "datetime2",
            nullable: true);
            
        migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            table: "OrderAttachment",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: true);

        // OrderProduct - NENHUMA ALTERAÇÃO NECESSÁRIA
        // Todos os campos necessários já existem na tabela!

        // OrderShipTo - NENHUMA ALTERAÇÃO NECESSÁRIA
        // Todos os campos necessários já existem na tabela!

        // OrderSoldTO - NENHUMA ALTERAÇÃO NECESSÁRIA  
        // Todos os campos necessários já existem na tabela!
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Reverter mudanças...
    }
}
```

---

## 🔄 ATUALIZAÇÃO DOS SERVICES E REPOSITORIES

### **1. Criar novos Repositories**
```csharp
// IOrderNotLoadedRepository.cs
public interface IOrderNotLoadedRepository
{
    Task<OrderNotLoaded> GetByIdAsync(long id);
    Task<OrderNotLoaded> CreateAsync(OrderNotLoaded order);
    Task<OrderNotLoaded> UpdateAsync(OrderNotLoaded order);
    Task<bool> DeleteAsync(long id);
    Task<List<OrderNotLoaded>> GetAllAsync();
}

// OrderNotLoadedRepository.cs
public class OrderNotLoadedRepository : IOrderNotLoadedRepository
{
    private readonly ThinkToolContext _context;
    
    public OrderNotLoadedRepository(ThinkToolContext context)
    {
        _context = context;
    }
    
    public async Task<OrderNotLoaded> GetByIdAsync(long id)
    {
        return await _context.OrderNotLoaded
            .Include(o => o.CountryNavigation)
            .Include(o => o.SegmentNavigation)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
    
    // ... outros métodos
}
```

### **2. Atualizar OnlTicketService**
```csharp
public class OnlTicketService
{
    // Substituir repositórios antigos pelos novos
    private readonly IOrderNotLoadedRepository _orderRepository;
    private readonly IOrderAttachmentRepository _attachmentRepository;
    private readonly IOrderSoldTORepository _soldToRepository;
    private readonly IOrderShipToRepository _shipToRepository;
    private readonly IOrderProductRepository _productRepository;
    
    // Manter mesmas assinaturas de métodos públicos
    public async Task<OnlTicketViewModel> GetByIdAsync(int id)
    {
        // Buscar nas novas entidades
        var order = await _orderRepository.GetByIdAsync(id);
        var soldTos = await _soldToRepository.GetByOrderIdAsync(id);
        var attachments = await _attachmentRepository.GetByOrderIdAsync(id);
        
        // Mapear para ViewModel existente (telas não mudam!)
        return MapToViewModel(order, soldTos, attachments);
    }
    
    public async Task<int> CreateAsync(OnlTicketViewModel model)
    {
        // Converter ViewModel para novas entidades
        var order = MapToOrderNotLoaded(model);
        var createdOrder = await _orderRepository.CreateAsync(order);
        
        // Salvar entidades relacionadas
        await SaveRelatedEntities(model, createdOrder.Id);
        
        return (int)createdOrder.Id;
    }
    
    // Métodos privados de mapeamento
    private OnlTicketViewModel MapToViewModel(OrderNotLoaded order, List<OrderSoldTO> soldTos, List<OrderAttachment> attachments)
    {
        return new OnlTicketViewModel
        {
            ID = order.Id.ToString(),
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
            NFType = order.NFType,
            CustomerName = order.Customer,
            Country = order.Country,
            BU = order.BusinessUnit,
            PODate = order.PODate,
            Segment = order.Segment,
            Region = order.Region,
            UF = order.State,
            ReplacementType = order.RecolocationType,
            DMU = order.DMU,
            BillAhead = order.BillAhead,
            ISRName = order.ISRName,
            CountryId = order.IdCountry,
            SegmentId = order.IdSegment,
            // Mapear entidades relacionadas...
        };
    }
    
    private OrderNotLoaded MapToOrderNotLoaded(OnlTicketViewModel model)
    {
        return new OrderNotLoaded
        {
            NumberOrder = model.LogNumber,
            CreatedOn = model.CreatedDate ?? DateTime.UtcNow,
            CreatedBy = model.CreatedBy ?? "System",
            UpdatedOn = model.UpdatedDate ?? DateTime.UtcNow,
            UpdatedBy = model.UpdatedBy ?? "System",
            OrderStatus = model.Status ?? "Pending",
            From = model.EmailFrom,
            AssignedTo = model.AssignedOperator,
            PONumber = model.PONumber,
            OrderType = model.OrderType,
            NFType = model.NFType,
            Customer = model.CustomerName,
            Country = model.Country,
            BusinessUnit = model.BU,
            PODate = model.PODate,
            Segment = model.Segment,
            Region = model.Region,
            State = model.UF,
            RecolocationType = model.ReplacementType,
            DMU = model.DMU,
            BillAhead = model.BillAhead,
            ISRName = model.ISRName,
            IdCountry = model.CountryId ?? Guid.Empty,
            IdSegment = model.SegmentId ?? 0
        };
    }
}
```

---

## 📅 CRONOGRAMA DE IMPLEMENTAÇÃO

| **Fase** | **Atividade** | **Tempo** | **Prioridade** |
|----------|---------------|-----------|----------------|
| 1 | Estender entidades Order com campos faltantes | 2 dias | 🔴 Alta |
| 2 | Criar/executar migrations no banco | 1 dia | 🔴 Alta |
| 3 | Criar novos repositories para entidades Order | 3 dias | 🔴 Alta |
| 4 | Atualizar OnlTicketService com mapeamentos | 4 days | 🔴 Alta |
| 5 | Testes unitários e integração | 3 dias | 🟡 Média |
| 6 | Testes em ambiente de homologação | 2 dias | 🟡 Média |
| 7 | Deploy em produção | 1 dia | 🔴 Alta |
| **TOTAL** | | **16 dias** | |

---

## ✅ CHECKLIST DE IMPLEMENTAÇÃO

### **Preparação:**
- [ ] Backup completo do banco de dados
- [ ] Backup do código fonte atual
- [ ] Ambiente de testes preparado

### **Desenvolvimento:**
- [ ] Estender OrderNotLoaded com campos faltantes
- [ ] Estender OrderAttachment com campos faltantes  
- [ ] Estender OrderProduct com campos faltantes
- [ ] Estender OrderShipTo com campos faltantes
- [ ] Estender OrderSoldTO com campos faltantes (⚠️ CRÍTICO: Id)
- [ ] Criar migrations para adição de campos
- [ ] Executar migrations em ambiente de desenvolvimento
- [ ] Criar repositories para entidades Order
- [ ] Atualizar OnlTicketService com mapeamentos
- [ ] Atualizar dependency injection no Program.cs

### **Testes:**
- [ ] Testes unitários dos repositories
- [ ] Testes unitários do service
- [ ] Testes de integração
- [ ] Teste de criação de OnlTicket
- [ ] Teste de edição de OnlTicket
- [ ] Teste de listagem de OnlTickets
- [ ] Teste de anexos
- [ ] Teste de relacionamentos SoldTo/ShipTo/OrderItems

### **Deploy:**
- [ ] Executar migrations em homologação
- [ ] Testes de aceitação em homologação
- [ ] Executar migrations em produção
- [ ] Monitoramento pós-deploy
- [ ] Validação de dados migrados

---

## 🚨 PONTOS CRÍTICOS DE ATENÇÃO

1. **⚠️ OrderSoldTO sem chave primária**: Adicionar campo Id é CRÍTICO para relacionamentos
2. **⚠️ Mapeamento BidContractNumber**: Separar em BID + ContractNumber no OrderProduct
3. **⚠️ Navigation Properties**: Garantir que relacionamentos funcionem corretamente
4. **⚠️ Dados existentes**: Entidades Order podem já ter dados - não sobrescrever
5. **⚠️ Performance**: Verificar se queries continuam performáticas com novos campos

---

**📅 Data**: ${new Date().toLocaleDateString('pt-BR')}  
**👤 Responsável**: Assistente de IA  
**🎯 Status**: Pronto para implementação  
**✅ Aprovado pelo cliente**: Aguardando confirmação
