# Estudo de Mapeamento de Entidades - ONL Ticket

## Visão Geral
Este documento apresenta o estudo completo do mapeamento de entidades necessárias para persistir os dados do formulário de criação de ONL Tickets no banco de dados.

## Análise da Estrutura da Tela

### 1. Seções Identificadas
A tela de criação possui as seguintes seções principais:

1. **ONL Ticket Information** - Informações básicas do ticket
2. **Order Information - Header** - Informações do pedido (cabeçalho)
3. **Attachments** - Sistema completo de anexos com metadados
4. **Order Information - Item** - Estrutura hierárquica principal (Sold To → Ship To → Order Items)
5. **Extra Order Information - Audit Section** - Seção de auditoria

### 1.1. Sistema de Anexos Detalhado
O sistema de anexos permite:
- **Upload múltiplo**: Usuário pode adicionar quantos anexos quiser
- **Metadados por anexo**: Cada anexo possui Customer PO, Description e Comments
- **Validações**: Tipo de arquivo e tamanho máximo (10MB)
- **Formatos aceitos**: PDF, DOC, DOCX, XLS, XLSX, JPG, JPEG, PNG, TXT
- **Interface**: Formulário de adição + tabela de visualização

### 1.2. Sistema de SAP Orders Dinâmico
O sistema de SAP Orders permite:
- **Múltiplos SAP Orders**: Usuário pode adicionar quantos SAP Orders quiser
- **Campos completos**: SAP Order Number, Delivery Number, Invoice Number, NF Number, NF Date, Net Amount, Total Order Amount
- **Interface dinâmica**: Botão "Add SAP Order" + cards numerados
- **Remoção individual**: Cada SAP Order pode ser removido independentemente
- **Renumeração automática**: Números são atualizados quando itens são removidos

### 2. Estrutura Hierárquica Principal
```
ONL Ticket
├── Attachments (1:N)
├── SAP Orders (1:N)
├── Comments (1:N)
└── Sold To (1:N)
    └── Ship To (1:N)
        └── Order Items (1:N)
```

## Entidades Identificadas

### 1. OnlTicket (Entidade Principal)
**Propósito**: Armazena as informações básicas do ticket ONL

**Campos**:
- `Id` (int, PK, Identity)
- `LogNumber` (string, 50, NOT NULL, UNIQUE)
- `CreatedDate` (datetime2, NOT NULL)
- `CreatedBy` (string, 100, NOT NULL)
- `UpdatedDate` (datetime2, NULL)
- `UpdatedBy` (string, 100, NULL)
- `Status` (string, 50, NOT NULL)
- `EmailFrom` (string, 255, NULL)
- `AssignedOperator` (string, 100, NULL)
- `PONumber` (string, 100, NULL)
- `OrderType` (string, 50, NULL)
- `OrderStatus` (string, 50, NULL)
- `NFType` (string, 50, NULL)
- `CustomerName` (string, 255, NULL)
- `DMU` (string, 50, NULL)
- `Country` (string, 100, NULL)
- `CountryId` (uniqueidentifier, NULL) - FK para Countries
- `BU` (string, 50, NULL)
- `PODate` (datetime2, NULL)
- `Segment` (string, 50, NULL)
- `SegmentId` (int, NULL) - FK para CustomerSegment
- `BillAhead` (string, 50, NULL)
- `ISRName` (string, 100, NULL)
- `Region` (string, 100, NULL)
- `UF` (string, 2, NULL)
- `ReplacementType` (string, 50, NULL)
- `Comentarios` (text, NULL)
- `OrderId` (string, 100, NULL) - ID do pedido

### 2. OnlTicketAttachment
**Propósito**: Armazena anexos do ticket com metadados completos

**Campos**:
- `Id` (int, PK, Identity)
- `OnlTicketId` (int, FK, NOT NULL)
- `CustomerPO` (string, 100, NULL) - Customer PO do anexo
- `Descricao` (string, 500, NULL) - Descrição do anexo
- `Comentarios` (text, NULL) - Comentários do anexo
- `FilePath` (string, 500, NULL) - Caminho do arquivo no servidor
- `FileName` (string, 255, NOT NULL) - Nome original do arquivo
- `FileSize` (bigint, NOT NULL) - Tamanho do arquivo em bytes
- `ContentType` (string, 100, NOT NULL) - Tipo MIME do arquivo
- `FileExtension` (string, 10, NULL) - Extensão do arquivo
- `IsActive` (bit, NOT NULL, DEFAULT 1) - Indica se o anexo está ativo
- `CreatedDate` (datetime2, NOT NULL)
- `CreatedBy` (string, 100, NOT NULL)
- `UpdatedDate` (datetime2, NULL)
- `UpdatedBy` (string, 100, NULL)

### 3. OnlTicketSoldTo
**Propósito**: Armazena endereços Sold To do ticket

**Campos**:
- `Id` (int, PK, Identity)
- `OnlTicketId` (int, FK, NOT NULL)
- `CNPJ` (string, 18, NULL)
- `Endereco` (string, 255, NULL)
- `Bairro` (string, 100, NULL)
- `Municipio` (string, 100, NULL)
- `CEP` (string, 10, NULL)
- `UF` (string, 2, NULL)
- `CreatedDate` (datetime2, NOT NULL)
- `CreatedBy` (string, 100, NOT NULL)

### 4. OnlTicketShipTo
**Propósito**: Armazena endereços Ship To do ticket

**Campos**:
- `Id` (int, PK, Identity)
- `OnlTicketSoldToId` (int, FK, NOT NULL)
- `CNPJ` (string, 18, NULL)
- `Endereco` (string, 255, NULL)
- `Bairro` (string, 100, NULL)
- `Municipio` (string, 100, NULL)
- `CEP` (string, 10, NULL)
- `UF` (string, 2, NULL)
- `SAPOrderNumber` (string, 100, NULL)
- `CreatedDate` (datetime2, NOT NULL)
- `CreatedBy` (string, 100, NOT NULL)

### 5. OnlTicketOrderItem
**Propósito**: Armazena itens do pedido

**Campos**:
- `Id` (int, PK, Identity)
- `OnlTicketShipToId` (int, FK, NOT NULL)
- `BidContractNumber` (string, 100, NULL)
- `PartNumber` (string, 100, NULL)
- `PartNumberDescription` (string, 500, NULL)
- `Qty` (int, NULL)
- `UnityNetPrice` (decimal(18,2), NULL)
- `UnitGrossPrice` (decimal(18,2), NULL)
- `CreatedDate` (datetime2, NOT NULL)
- `CreatedBy` (string, 100, NOT NULL)

### 6. OnlTicketSAPOrder
**Propósito**: Armazena informações de pedidos SAP

**Campos**:
- `Id` (int, PK, Identity)
- `OnlTicketId` (int, FK, NOT NULL)
- `SAPOrderNumber` (string, 100, NULL)
- `DeliveryNumber` (string, 100, NULL)
- `InvoiceNumber` (string, 100, NULL)
- `NFNumber` (string, 100, NULL)
- `NFDate` (datetime2, NULL)
- `NetAmount` (decimal(18,2), NULL)
- `TotalOrderAmount` (decimal(18,2), NULL)
- `CreatedDate` (datetime2, NOT NULL)
- `CreatedBy` (string, 100, NOT NULL)

### 7. OnlTicketComment
**Propósito**: Armazena comentários do ticket

**Campos**:
- `Id` (int, PK, Identity)
- `OnlTicketId` (int, FK, NOT NULL)
- `Comment` (text, NOT NULL)
- `CreatedDate` (datetime2, NOT NULL)
- `CreatedBy` (string, 100, NOT NULL)

## Entidades de Referência (Já Existentes)

### 8. Country
**Propósito**: Entidade de referência para países (já existente no sistema)

**Campos**:
- `Id` (uniqueidentifier, PK)
- `Name` (string, NOT NULL)
- `IsActive` (bit, NOT NULL)
- `IsDeleted` (bit, NOT NULL)
- `CreatedAt` (datetime2, NOT NULL)
- `CreatedBy` (string, NOT NULL)
- `UpdatedAt` (datetime2, NULL)
- `UpdatedBy` (string, NULL)
- `RecordStatus` (byte, NOT NULL)
- `Abbreviation` (string, NULL)
- `CountryId` (int, NOT NULL)

### 9. CustomerSegment
**Propósito**: Entidade de referência para segmentos de cliente (já existente no sistema)

**Campos**:
- `Id` (int, PK, Identity)
- `Name` (string, NOT NULL)

## Relacionamentos

### 1. OnlTicket (1) → OnlTicketAttachment (N)
- **Tipo**: One-to-Many
- **Chave Estrangeira**: `OnlTicketId` em `OnlTicketAttachment`
- **Comportamento**: CASCADE DELETE

### 2. OnlTicket (1) → OnlTicketSoldTo (N)
- **Tipo**: One-to-Many
- **Chave Estrangeira**: `OnlTicketId` em `OnlTicketSoldTo`
- **Comportamento**: CASCADE DELETE

### 3. OnlTicketSoldTo (1) → OnlTicketShipTo (N)
- **Tipo**: One-to-Many
- **Chave Estrangeira**: `OnlTicketSoldToId` em `OnlTicketShipTo`
- **Comportamento**: CASCADE DELETE

### 4. OnlTicketShipTo (1) → OnlTicketOrderItem (N)
- **Tipo**: One-to-Many
- **Chave Estrangeira**: `OnlTicketShipToId` em `OnlTicketOrderItem`
- **Comportamento**: CASCADE DELETE

### 5. OnlTicket (1) → OnlTicketSAPOrder (N)
- **Tipo**: One-to-Many
- **Chave Estrangeira**: `OnlTicketId` em `OnlTicketSAPOrder`
- **Comportamento**: CASCADE DELETE

### 6. OnlTicket (1) → OnlTicketComment (N)
- **Tipo**: One-to-Many
- **Chave Estrangeira**: `OnlTicketId` em `OnlTicketComment`
- **Comportamento**: CASCADE DELETE

### 7. OnlTicket (N) → Country (1)
- **Tipo**: Many-to-One
- **Chave Estrangeira**: `CountryId` em `OnlTicket`
- **Comportamento**: NO ACTION (referência de integridade)

### 8. OnlTicket (N) → CustomerSegment (1)
- **Tipo**: Many-to-One
- **Chave Estrangeira**: `SegmentId` em `OnlTicket`
- **Comportamento**: NO ACTION (referência de integridade)

## Índices Recomendados

### 1. Índices Únicos
- `IX_OnlTicket_LogNumber` (UNIQUE) - Para busca rápida por Log Number
- `IX_OnlTicket_OrderId` (UNIQUE) - Para busca rápida por Order ID

### 2. Índices de Performance
- `IX_OnlTicket_CreatedDate` - Para ordenação por data de criação
- `IX_OnlTicket_Status` - Para filtros por status
- `IX_OnlTicket_CreatedBy` - Para filtros por usuário
- `IX_OnlTicketSoldTo_OnlTicketId` - Para joins com Sold To
- `IX_OnlTicketShipTo_OnlTicketSoldToId` - Para joins com Ship To
- `IX_OnlTicketOrderItem_OnlTicketShipToId` - Para joins com Order Items

## Validações e Regras de Negócio

### 1. Validações de Anexos
- **Tamanho máximo**: 10MB por arquivo
- **Tipos permitidos**: PDF, DOC, DOCX, XLS, XLSX, JPG, JPEG, PNG, TXT
- **Arquivo obrigatório**: Cada anexo deve ter um arquivo
- **Metadados opcionais**: Customer PO, Descrição e Comentários são opcionais

### 2. Validações de Estrutura Hierárquica
- **Sold To obrigatório**: Pelo menos um Sold To deve ser adicionado
- **Ship To obrigatório**: Cada Sold To deve ter pelo menos um Ship To
- **Order Items opcionais**: Ship To pode existir sem Order Items

### 3. Validações de Campos Obrigatórios
- **LogNumber**: Deve ser único no sistema
- **CreatedBy**: Obrigatório para auditoria
- **Status**: Obrigatório para controle de fluxo
- **PONumber**: Obrigatório para identificação do pedido
- **CustomerName**: Obrigatório para identificação do cliente

## Considerações de Design

### 1. Normalização
- As entidades seguem a 3ª Forma Normal
- Evita redundância de dados
- Facilita manutenção e atualizações

### 2. Auditoria
- Todas as entidades possuem campos de auditoria (`CreatedDate`, `CreatedBy`)
- Entidades principais possuem também `UpdatedDate` e `UpdatedBy`
- Permite rastreamento completo de alterações
- Facilita debugging e compliance

### 3. Flexibilidade
- Estrutura hierárquica permite múltiplos Sold To por ticket
- Cada Sold To pode ter múltiplos Ship To
- Cada Ship To pode ter múltiplos Order Items
- Suporte a múltiplos anexos com metadados completos
- Suporte a múltiplos comentários e pedidos SAP

### 4. Performance
- Índices estratégicos para consultas frequentes
- Chaves estrangeiras para integridade referencial
- CASCADE DELETE para limpeza automática
- Campos de auditoria indexados para consultas temporais

### 5. Segurança de Arquivos
- Validação de tipo MIME para prevenir uploads maliciosos
- Controle de tamanho para evitar sobrecarga do servidor
- Armazenamento seguro de arquivos com nomes únicos
- Soft delete para anexos (campo `IsActive`)

### 6. Padronização de Interface
- **Idioma único**: Toda a interface em inglês para consistência
- **Campos de formulário**: Labels, placeholders e validações em inglês
- **Mensagens de sistema**: Alertas, erros e confirmações em inglês
- **Tabelas e botões**: Cabeçalhos e textos de ação em inglês
- **Comentários de código**: JavaScript e HTML com comentários em inglês

### 7. Sistema de Dropdowns Implementado
- **Country Dropdown**: 
  - Utiliza entidade `Country` existente no banco
  - Filtros: `IsActive = true` e `IsDeleted = false`
  - Ordenação: Por nome alfabético
  - Padrão: Repository → Service → Controller → View
  - Mapeamento: AutoMapper para `CountryViewModel`
  
- **Segment Dropdown**:
  - Utiliza entidade `CustomerSegment` existente no banco
  - Aplicado tanto na tela de Create quanto na Index (filtros)
  - Padrão: Repository → Service → Controller → View
  - Mapeamento: AutoMapper para `OnlTicketCustomerSegmentViewModel`
  
- **Outros Dropdowns**:
  - OrderType, OrderStatus, NFType: Seguem o mesmo padrão
  - Todos utilizam entidades existentes no banco
  - Implementação consistente em toda a aplicação

### 8. Experiência do Usuário (UX)
- **Botões de remoção intuitivos**: Texto "Remove" + ícone de lixeira
- **Tooltips informativos**: Dicas contextuais para cada ação
- **Efeitos hover**: Feedback visual ao interagir com elementos
- **Renumeração automática**: Números atualizados após remoções
- **Espaçamento consistente**: Margens e paddings padronizados
- **Campo ID oculto**: Interface mais limpa na seção Order Information

### 9. Implementação de Persistência
- **Entidades criadas**: 7 entidades principais com Data Annotations
- **Relacionamentos configurados**: Foreign Keys e Navigation Properties
- **Repository Pattern**: CRUD completo implementado
- **Service Layer**: Mapeamento e lógica de negócio
- **Controller atualizado**: Salvamento e listagem funcionais
- **AutoMapper configurado**: Conversão automática entre ViewModels e Entities
- **Dados reais**: Listagem na home agora usa dados do banco de dados
- **Salvamento hierárquico**: Sold To, Ship To, Order Items, SAP Orders, Comments e Attachments

### 10. Funcionalidade Details/Edit
- **Navegação**: Home → Details → Edit → Home
- **Carregamento de dados**: Busca ticket por ID com todos os relacionamentos
- **Mapeamento completo**: Entity → ViewModel com estrutura hierárquica
- **Interface adaptativa**: Título e botões mudam conforme modo (Create/Edit)
- **Validação**: Mesmas validações do Create aplicadas no Edit
- **Atualização**: Salva alterações no banco de dados
- **Reutilização de view**: Create.cshtml serve para ambos os modos
- **Campos hidden**: ID preservado no formulário para edição

## Scripts de Criação

Os scripts SQL completos para criação das entidades estão disponíveis no arquivo:
**`SCRIPTS_CRIACAO_ENTIDADES_ONL_TICKET.sql`**

Este arquivo contém:
- Criação de todas as 7 tabelas principais
- Foreign Keys para todos os relacionamentos
- Índices únicos e de performance
- Referências para entidades existentes (Countries, CustomerSegment)

## Interface de Usuário Implementada

### 1. Estrutura Visual Hierárquica
- **Sold To**: Cards azuis com padding lateral (20px)
- **Ship To**: Cards verdes com padding lateral (10px)  
- **Order Items**: Cards cinza com padding lateral (5px)
- **SAP Orders**: Cards azuis com borda azul (consistente com botão)
- **Consistência de cores**: Botões seguem a cor da borda do bloco que adicionam
- **Botões de remoção**: Padrão unificado com texto "Remove" + tooltips

### 2. Sistema de Anexos
- **Formulário de adição**: Layout em 2 linhas
  - Linha 1: File input (largura completa)
  - Linha 2: Customer PO, Description, Comments (3 colunas iguais)
- **Tabela de visualização**: 7 colunas com informações completas
- **Validações em tempo real**: Tipo e tamanho de arquivo
- **Interface responsiva**: Adapta-se a diferentes tamanhos de tela
- **Interface em inglês**: Todos os campos e mensagens em inglês

### 3. Funcionalidades JavaScript
- **Adição dinâmica**: Sold To, Ship To, Order Items, SAP Orders e Comments
- **Remoção com renumeração**: Atualização automática de índices
- **Validações client-side**: Campos obrigatórios e estrutura hierárquica
- **Gerenciamento de arquivos**: Upload, validação e remoção
- **Formatação automática**: Tamanho de arquivo e tipo
- **Mensagens em inglês**: Todos os alertas e validações em inglês
- **Botões de remoção aprimorados**: Texto "Remove" + tooltips + efeitos hover

### 4. Validações de Interface
- **Campos obrigatórios**: LogNumber, CreatedBy, Status, PONumber, CustomerName
- **Estrutura mínima**: Pelo menos 1 Sold To e 1 Ship To por Sold To
- **Arquivos**: Validação de tipo e tamanho antes do upload
- **Feedback visual**: Alertas e mensagens de erro claras
- **Idioma**: Interface completamente em inglês para consistência

## Conclusão

Este mapeamento de entidades fornece uma base sólida para persistir todos os dados coletados no formulário de criação de ONL Tickets. A estrutura hierárquica permite máxima flexibilidade enquanto mantém a integridade dos dados e performance adequada.

### Características Principais:
- **Estrutura hierárquica completa**: Sold To → Ship To → Order Items
- **Sistema de anexos robusto**: Com metadados e validações
- **Sistema de SAP Orders dinâmico**: Múltiplos SAP Orders com interface intuitiva
- **Sistema de dropdowns integrado**: Country e Segment com dados reais do banco
- **Interface intuitiva**: Cores consistentes e layout organizado
- **Validações abrangentes**: Client-side e server-side
- **Auditoria completa**: Rastreamento de todas as alterações
- **Interface em inglês**: Consistência linguística em toda a aplicação
- **UX aprimorada**: Botões de remoção intuitivos e feedback visual
- **Padrão de arquitetura**: Repository → Service → Controller → View consistente
- **Persistência funcional**: Salvamento e listagem com dados reais do banco
- **Mapeamento automático**: AutoMapper para conversão entre camadas
- **Funcionalidade Details/Edit**: Visualização e edição de tickets existentes

### Implementação Realizada:
- ✅ **Configuração de relacionamentos no DbContext**: Todas as entidades adicionadas
- ✅ **Mapeamento de entidades com Data Annotations**: 7 entidades principais criadas
- ✅ **Implementação de repositórios**: OnlTicketRepository com CRUD completo
- ✅ **Validações de negócio**: Implementadas nas entidades e ViewModels
- ✅ **Tratamento de transações**: Gerenciado pelo Entity Framework
- ✅ **Sistema de upload de arquivos**: Estrutura preparada para implementação
- ✅ **Soft delete para anexos**: Campo IsActive implementado
- ✅ **Dropdowns com dados reais**: Country e CustomerSegment integrados
- ✅ **Padrão Repository-Service**: Arquitetura consistente implementada
- ✅ **AutoMapper**: Mapeamento automático entre camadas configurado
- ✅ **Filtros de dados**: Aplicados nos dropdowns e consultas
- ✅ **Salvamento funcional**: Create e Index com dados reais do banco
- ✅ **Funcionalidade Details/Edit**: Visualização e edição de tickets existentes
- ✅ **Interface adaptativa**: Título e botões mudam conforme modo
- ✅ **Mapeamento bidirecional**: Entity ↔ ViewModel completo
- ✅ **Salvamento hierárquico completo**: Sold To, Ship To, Order Items, SAP Orders, Comments e Attachments
- ✅ **Attachments com campos hidden**: JavaScript envia dados via campos hidden para o servidor
- ✅ **Validação de Attachments**: Salva se pelo menos um campo (CustomerPO, Descricao ou Comentarios) estiver preenchido
- ✅ **Propriedade Comentarios**: Adicionada à OnlTicketAttachmentViewModel para suporte completo
- ✅ **Mapeamento completo de Attachments**: Controller mapeia Id, CustomerPO, Descricao e Comentarios
- ✅ **Carregamento de Attachments existentes**: JavaScript popula tabela com dados do servidor
- ✅ **Exibição de dados hierárquicos**: OrderInformationItem.cshtml exibe Sold To, Ship To e Order Items existentes
- ✅ **Correção de índices JavaScript**: Inicialização correta dos índices para dados existentes
- ✅ **Mensagem de sucesso condicional**: Só exibe no modo Create, não no Details
