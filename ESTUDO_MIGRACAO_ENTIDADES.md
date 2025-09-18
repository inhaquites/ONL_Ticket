# ESTUDO: MIGRAÇÃO DE ENTIDADES ONL TICKET PARA ORDER

## 📋 RESUMO EXECUTIVO

O cliente solicitou a migração das entidades atuais do sistema OnlTicket para utilizar as entidades Order existentes. Este documento apresenta um estudo detalhado das mudanças necessárias, mantendo o design das telas intacto.

---

## 🔄 MAPEAMENTO DE ENTIDADES

### ENTIDADES ATUAIS → NOVAS ENTIDADES

| **Entidade Atual** | **Nova Entidade** | **Status** |
|-------------------|------------------|------------|
| `OnlTicket` | `OrderNotLoaded` | ✅ Compatível |
| `OnlTicketAttachment` | `OrderAttachment` | ⚠️ Ajustes necessários |
| `OnlTicketComment` | `OrderHistory` | ⚠️ Reestruturação |
| `OnlTicketOrderItem` | `OrderProduct` | ✅ Compatível |
| `OnlTicketSAPOrder` | *(Integrado em OrderShipTo)* | 🔄 Mudança estrutural |
| `OnlTicketShipTo` | `OrderShipTo` | ✅ Compatível |
| `OnlTicketSoldTo` | `OrderSoldTO` | ✅ Compatível |

---

## 📊 ANÁLISE DETALHADA DAS ENTIDADES

### 1. **OnlTicket → OrderNotLoaded**

#### **Campos Compatíveis:**
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
OrderStatus → OrderStatus
NFType → NFType
CustomerName → Customer
Country → Country
BU → BusinessUnit
PODate → PODate
Segment → Segment
Region → Region
UF → State
ReplacementType → RecolocationType
```

#### **Campos Novos em OrderNotLoaded:**
- `Subject` (novo campo)
- `EmailResolutionOwner` (novo campo)
- `IdOrderStatus`, `IdBusinessUnit`, `IdNFType`, `IdCustomer`, `IdSegment`, `IdOrderType`, `IdCancelReason` (chaves estrangeiras)
- `CancelReason` (novo campo)

#### **Campos Removidos:**
- `DMU`, `BillAhead`, `ISRName`, `Comentarios`, `OrderId`, `CountryId`, `SegmentId`

---

### 2. **OnlTicketAttachment → OrderAttachment**

#### **Mapeamento de Campos:**
```csharp
// OnlTicketAttachment → OrderAttachment
Id → Id
OnlTicketId → IdOrderNotLoaded
CustomerPO → PONumber
Descricao → Description
Comentarios → Comments
FileName → AttachemntFileName
CreatedDate → CreatedOn
CreatedBy → CreatedBy
```

#### **Principais Diferenças:**
- ❌ **OrderAttachment** usa `byte[] Attachment` (arquivo em binário)
- ❌ **OnlTicketAttachment** usa `FilePath + FileName` (arquivo no sistema)
- ❌ **OrderAttachment** não tem campos de auditoria (UpdatedDate, UpdatedBy, IsActive)
- ❌ **OrderAttachment** não tem FileSize, ContentType, FileExtension

---

### 3. **OnlTicketComment → OrderHistory**

#### **Mapeamento de Campos:**
```csharp
// OnlTicketComment → OrderHistory
OnlTicketId → IdOrderNotLoaded
Comment → Comments
CreatedDate → CreatedOn
CreatedBy → CreatedBy
```

#### **Campos Adicionais em OrderHistory:**
- `IdOrderStatus` + `OrderStatus` (status do pedido no momento do comentário)
- `IdProblemSubCategory` (categorização do problema)
- `EmailsCopy` (cópias de email)
- `SalesRep` (representante de vendas)

---

### 4. **OnlTicketOrderItem → OrderProduct**

#### **Mapeamento de Campos:**
```csharp
// OnlTicketOrderItem → OrderProduct
OnlTicketShipToId → IdOrderShipTo
BidContractNumber → BID + ContractNumber (separar em 2 campos)
PartNumber → PartNumber
PartNumberDescription → MTMDescription
Qty → Quantity
UnityNetPrice → UnitNetPrice
UnitGrossPrice → UnitGrossPrice
CreatedDate → CreatedOn
CreatedBy → CreatedBy
```

#### **Diferenças:**
- ✅ **OrderProduct** separa BID e ContractNumber
- ❌ **OrderProduct** não tem campos de auditoria (Id, CreatedDate, CreatedBy)

---

### 5. **OnlTicketSoldTo → OrderSoldTO**

#### **Mapeamento de Campos:**
```csharp
// OnlTicketSoldTo → OrderSoldTO
OnlTicketId → IdOrderNotLoaded
CNPJ → CompanyTaxId
Endereco → Address
Bairro → Neighborhood
Municipio → City
CEP → PostalCode
UF → State
CreatedDate → CreatedOn
CreatedBy → CreatedBy
```

#### **Diferenças:**
- ❌ **OrderSoldTO** não tem campo Id (chave primária)
- ❌ **OrderSoldTO** não tem relacionamento com ShipTo

---

### 6. **OnlTicketShipTo → OrderShipTo**

#### **Mapeamento de Campos:**
```csharp
// OnlTicketShipTo → OrderShipTo
OnlTicketSoldToId → IdOrderSoldTo
CNPJ → CompanyTaxId
Endereco → Address
Bairro → Neighborhood
Municipio → City
CEP → PostalCode
UF → State
SAPOrderNumber → SapOrder
CreatedDate → CreatedOn
CreatedBy → CreatedBy
```

#### **Campos Novos em OrderShipTo:**
- `IdOrderNotLoaded` (referência direta ao pedido)
- `SapOrderService` (novo campo para serviços SAP)

---

### 7. **OnlTicketSAPOrder → (Integrado em OrderShipTo)**

#### **Mudança Estrutural:**
- ❌ **A entidade OnlTicketSAPOrder será REMOVIDA**
- ✅ **Os campos SAP foram integrados em OrderShipTo:**
  - `SAPOrderNumber → SapOrder`
  - `DeliveryNumber, InvoiceNumber, NFNumber, NFDate, NetAmount, TotalOrderAmount` → **PERDIDOS**

---

## ⚠️ IMPACTOS E RISCOS

### **ALTO RISCO:**
1. **Perda de Dados SAP**: Campos como DeliveryNumber, InvoiceNumber, NFNumber, NFDate, NetAmount, TotalOrderAmount não existem nas novas entidades
2. **Mudança de Estrutura de Anexos**: Migração de arquivos físicos para BLOB binário
3. **Relacionamentos Quebrados**: OrderSoldTO não tem chave primária própria

### **MÉDIO RISCO:**
1. **Campos de Auditoria**: Algumas entidades perdem campos de controle (UpdatedDate, UpdatedBy, IsActive)
2. **Validações**: Necessário revisar todas as validações de negócio
3. **Histórico**: Estrutura de comentários muda para histórico com mais campos

### **BAIXO RISCO:**
1. **Nomes de Campos**: Apenas renomeações simples
2. **Tipos de Dados**: Compatíveis na maioria dos casos

---

## 🛠️ SUGESTÕES DE IMPLEMENTAÇÃO

### **FASE 1: PREPARAÇÃO**

#### **1.1 Backup e Segurança**
```sql
-- Criar backup completo das tabelas atuais
BACKUP DATABASE [ThinkTool] TO DISK = 'C:\Backup\ThinkTool_PreMigration.bak'

-- Criar tabelas de backup para dados críticos
SELECT * INTO OnlTicket_Backup FROM OnlTicket
SELECT * INTO OnlTicketSAPOrder_Backup FROM OnlTicketSAPOrder
```

#### **1.2 Extensão das Novas Entidades**
```csharp
// Adicionar campos faltantes em OrderNotLoaded
public partial class OrderNotLoaded
{
    public string? DMU { get; set; }
    public string? BillAhead { get; set; }
    public string? ISRName { get; set; }
    public string? Comentarios { get; set; }
    public string? OrderId { get; set; }
}

// Adicionar chave primária em OrderSoldTO
public partial class OrderSoldTO
{
    [Key]
    public int Id { get; set; } // ADICIONAR
}

// Criar nova entidade para dados SAP perdidos
public class OrderSAPDetails
{
    public int Id { get; set; }
    public int IdOrderShipTo { get; set; }
    public string? DeliveryNumber { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? NFNumber { get; set; }
    public DateTime? NFDate { get; set; }
    public decimal? NetAmount { get; set; }
    public decimal? TotalOrderAmount { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
}
```

### **FASE 2: MIGRAÇÃO DE DADOS**

#### **2.1 Script de Migração Principal**
```sql
-- Migrar OnlTicket → OrderNotLoaded
INSERT INTO OrderNotLoaded (
    CreatedOn, CreatedBy, UpdatedOn, UpdatedBy, 
    OrderStatus, [From], NumberOrder, AssignedTo,
    Country, BusinessUnit, NFType, PONumber, PODate,
    Customer, Segment, DMU, OrderType, RecolocationType,
    BillAhead, ISRName
)
SELECT 
    CreatedDate, CreatedBy, UpdatedDate, UpdatedBy,
    Status, EmailFrom, LogNumber, AssignedOperator,
    Country, BU, NFType, PONumber, PODate,
    CustomerName, Segment, DMU, OrderType, ReplacementType,
    BillAhead, ISRName
FROM OnlTicket

-- Migrar OnlTicketSoldTo → OrderSoldTO
INSERT INTO OrderSoldTO (
    IdOrderNotLoaded, CreatedOn, CreatedBy,
    CompanyTaxId, Address, Neighborhood, City, PostalCode, State
)
SELECT 
    onl.Id, ost.CreatedDate, ost.CreatedBy,
    ost.CNPJ, ost.Endereco, ost.Bairro, ost.Municipio, ost.CEP, ost.UF
FROM OnlTicketSoldTo ost
INNER JOIN OrderNotLoaded onl ON onl.NumberOrder = (SELECT LogNumber FROM OnlTicket WHERE Id = ost.OnlTicketId)
```

#### **2.2 Migração de Anexos**
```csharp
// Converter arquivos físicos para BLOB
public async Task MigrateAttachments()
{
    var oldAttachments = await _context.OnlTicketAttachments.ToListAsync();
    
    foreach(var oldAttachment in oldAttachments)
    {
        var filePath = Path.Combine(oldAttachment.FilePath, oldAttachment.FileName);
        if(File.Exists(filePath))
        {
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            
            var newAttachment = new OrderAttachment
            {
                IdOrderNotLoaded = GetOrderNotLoadedId(oldAttachment.OnlTicketId),
                CreatedOn = oldAttachment.CreatedDate,
                CreatedBy = oldAttachment.CreatedBy,
                PONumber = oldAttachment.CustomerPO,
                Attachment = fileBytes,
                AttachemntFileName = oldAttachment.FileName,
                Comments = oldAttachment.Comentarios,
                Description = oldAttachment.Descricao
            };
            
            _context.OrderAttachments.Add(newAttachment);
        }
    }
    
    await _context.SaveChangesAsync();
}
```

### **FASE 3: ATUALIZAÇÃO DO CÓDIGO**

#### **3.1 Repository Pattern**
```csharp
// Criar novos repositórios
public interface IOrderNotLoadedRepository
{
    Task<OrderNotLoaded> GetByIdAsync(long id);
    Task<OrderNotLoaded> CreateAsync(OrderNotLoaded order);
    Task<OrderNotLoaded> UpdateAsync(OrderNotLoaded order);
    Task<bool> DeleteAsync(long id);
    // ... outros métodos
}

// Implementar repositórios mantendo interface similar
public class OrderNotLoadedRepository : IOrderNotLoadedRepository
{
    // Implementação similar ao OnlTicketRepository
    // mas usando as novas entidades
}
```

#### **3.2 Service Layer**
```csharp
// Atualizar OnlTicketService para usar novas entidades
public class OnlTicketService
{
    private readonly IOrderNotLoadedRepository _orderRepository;
    private readonly IOrderAttachmentRepository _attachmentRepository;
    // ... outros repositórios
    
    // Manter mesmas assinaturas de métodos
    public async Task<OnlTicketViewModel> GetByIdAsync(int id)
    {
        // Buscar nas novas entidades
        var order = await _orderRepository.GetByIdAsync(id);
        
        // Mapear para ViewModel existente
        return MapToViewModel(order);
    }
    
    // Método de mapeamento
    private OnlTicketViewModel MapToViewModel(OrderNotLoaded order)
    {
        return new OnlTicketViewModel
        {
            ID = order.Id.ToString(),
            LogNumber = order.NumberOrder,
            CreatedDate = order.CreatedOn,
            CreatedBy = order.CreatedBy,
            // ... outros mapeamentos
        };
    }
}
```

#### **3.3 Controller Adjustments**
```csharp
// OnlTicketController permanece igual
// Apenas o Service interno muda para usar novas entidades
public class OnlTicketController : Controller
{
    // Nenhuma mudança necessária
    // O Service abstrai as mudanças de entidade
}
```

### **FASE 4: TESTES E VALIDAÇÃO**

#### **4.1 Testes de Migração**
```csharp
[TestMethod]
public async Task TestDataMigration()
{
    // Verificar se todos os registros foram migrados
    var oldCount = await _oldContext.OnlTickets.CountAsync();
    var newCount = await _newContext.OrderNotLoaded.CountAsync();
    
    Assert.AreEqual(oldCount, newCount, "Contagem de registros não confere");
    
    // Verificar integridade dos dados
    var oldTicket = await _oldContext.OnlTickets.FirstAsync();
    var newOrder = await _newContext.OrderNotLoaded
        .FirstAsync(o => o.NumberOrder == oldTicket.LogNumber);
    
    Assert.AreEqual(oldTicket.CreatedBy, newOrder.CreatedBy);
    Assert.AreEqual(oldTicket.Status, newOrder.OrderStatus);
    // ... outras verificações
}
```

---

## 📈 CRONOGRAMA SUGERIDO

| **Fase** | **Atividade** | **Tempo Estimado** | **Risco** |
|----------|---------------|-------------------|-----------|
| 1 | Análise e Backup | 2 dias | Baixo |
| 2 | Extensão de Entidades | 3 dias | Médio |
| 3 | Scripts de Migração | 5 dias | Alto |
| 4 | Atualização de Código | 7 dias | Médio |
| 5 | Testes e Validação | 4 dias | Alto |
| 6 | Deploy e Monitoramento | 2 dias | Alto |
| **TOTAL** | | **23 dias** | |

---

## 💡 RECOMENDAÇÕES FINAIS

### **CRÍTICAS:**
1. **⚠️ PERDA DE FUNCIONALIDADE**: A migração resultará na perda de campos importantes da funcionalidade SAP
2. **⚠️ COMPLEXIDADE**: Mudança estrutural significativa com riscos altos
3. **⚠️ TEMPO**: Projeto complexo que requer planejamento cuidadoso

### **ALTERNATIVAS SUGERIDAS:**
1. **Opção 1**: Manter entidades atuais e apenas renomear para seguir novo padrão
2. **Opção 2**: Criar entidades híbridas que combinem o melhor dos dois mundos
3. **Opção 3**: Implementar migração gradual por módulos

### **DECISÃO RECOMENDADA:**
**Implementar a migração com as extensões sugeridas** para manter toda funcionalidade existente, mas preparar um plano de rollback robusto.

---

**📅 Data do Estudo**: ${new Date().toLocaleDateString('pt-BR')}  
**👤 Responsável**: Assistente de IA  
**🎯 Status**: Aguardando aprovação do cliente
