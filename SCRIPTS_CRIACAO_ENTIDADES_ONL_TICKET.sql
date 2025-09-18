-- =====================================================
-- Scripts de Criação das Entidades - ONL Ticket System
-- =====================================================

-- =====================================================
-- 1. TABELA PRINCIPAL - OnlTicket
-- =====================================================
CREATE TABLE OnlTicket (
    Id int IDENTITY(1,1) PRIMARY KEY,
    LogNumber nvarchar(50) NOT NULL UNIQUE,
    CreatedDate datetime2 NOT NULL,
    CreatedBy nvarchar(100) NOT NULL,
    UpdatedDate datetime2 NULL,
    UpdatedBy nvarchar(100) NULL,
    Status nvarchar(50) NOT NULL,
    EmailFrom nvarchar(255) NULL,
    AssignedOperator nvarchar(100) NULL,
    PONumber nvarchar(100) NULL,
    OrderType nvarchar(50) NULL,
    OrderStatus nvarchar(50) NULL,
    NFType nvarchar(50) NULL,
    CustomerName nvarchar(255) NULL,
    DMU nvarchar(50) NULL,
    Country nvarchar(100) NULL,
    CountryId uniqueidentifier NULL,
    BU nvarchar(50) NULL,
    PODate datetime2 NULL,
    Segment nvarchar(50) NULL,
    SegmentId int NULL,
    BillAhead nvarchar(50) NULL,
    ISRName nvarchar(100) NULL,
    Region nvarchar(100) NULL,
    UF nvarchar(2) NULL,
    ReplacementType nvarchar(50) NULL,
    Comentarios ntext NULL,
    OrderId nvarchar(100) NULL
);

-- =====================================================
-- 2. TABELA DE ANEXOS - OnlTicketAttachment
-- =====================================================
CREATE TABLE OnlTicketAttachment (
    Id int IDENTITY(1,1) PRIMARY KEY,
    OnlTicketId int NOT NULL,
    CustomerPO nvarchar(100) NULL,
    Descricao nvarchar(500) NULL,
    Comentarios ntext NULL,
    FilePath nvarchar(500) NULL,
    FileName nvarchar(255) NOT NULL,
    FileSize bigint NOT NULL,
    ContentType nvarchar(100) NOT NULL,
    FileExtension nvarchar(10) NULL,
    IsActive bit NOT NULL DEFAULT 1,
    CreatedDate datetime2 NOT NULL,
    CreatedBy nvarchar(100) NOT NULL,
    UpdatedDate datetime2 NULL,
    UpdatedBy nvarchar(100) NULL
);

-- =====================================================
-- 3. TABELA SOLD TO - OnlTicketSoldTo
-- =====================================================
CREATE TABLE OnlTicketSoldTo (
    Id int IDENTITY(1,1) PRIMARY KEY,
    OnlTicketId int NOT NULL,
    CNPJ nvarchar(18) NULL,
    Endereco nvarchar(255) NULL,
    Bairro nvarchar(100) NULL,
    Municipio nvarchar(100) NULL,
    CEP nvarchar(10) NULL,
    UF nvarchar(2) NULL,
    CreatedDate datetime2 NOT NULL,
    CreatedBy nvarchar(100) NOT NULL
);

-- =====================================================
-- 4. TABELA SHIP TO - OnlTicketShipTo
-- =====================================================
CREATE TABLE OnlTicketShipTo (
    Id int IDENTITY(1,1) PRIMARY KEY,
    OnlTicketSoldToId int NOT NULL,
    CNPJ nvarchar(18) NULL,
    Endereco nvarchar(255) NULL,
    Bairro nvarchar(100) NULL,
    Municipio nvarchar(100) NULL,
    CEP nvarchar(10) NULL,
    UF nvarchar(2) NULL,
    SAPOrderNumber nvarchar(100) NULL,
    CreatedDate datetime2 NOT NULL,
    CreatedBy nvarchar(100) NOT NULL
);

-- =====================================================
-- 5. TABELA ORDER ITEMS - OnlTicketOrderItem
-- =====================================================
CREATE TABLE OnlTicketOrderItem (
    Id int IDENTITY(1,1) PRIMARY KEY,
    OnlTicketShipToId int NOT NULL,
    BidContractNumber nvarchar(100) NULL,
    PartNumber nvarchar(100) NULL,
    PartNumberDescription nvarchar(500) NULL,
    Qty int NULL,
    UnityNetPrice decimal(18,2) NULL,
    UnitGrossPrice decimal(18,2) NULL,
    CreatedDate datetime2 NOT NULL,
    CreatedBy nvarchar(100) NOT NULL
);

-- =====================================================
-- 6. TABELA SAP ORDERS - OnlTicketSAPOrder
-- =====================================================
CREATE TABLE OnlTicketSAPOrder (
    Id int IDENTITY(1,1) PRIMARY KEY,
    OnlTicketId int NOT NULL,
    SAPOrderNumber nvarchar(100) NULL,
    DeliveryNumber nvarchar(100) NULL,
    InvoiceNumber nvarchar(100) NULL,
    NFNumber nvarchar(100) NULL,
    NFDate datetime2 NULL,
    NetAmount decimal(18,2) NULL,
    TotalOrderAmount decimal(18,2) NULL,
    CreatedDate datetime2 NOT NULL,
    CreatedBy nvarchar(100) NOT NULL
);

-- =====================================================
-- 7. TABELA COMENTÁRIOS - OnlTicketComment
-- =====================================================
CREATE TABLE OnlTicketComment (
    Id int IDENTITY(1,1) PRIMARY KEY,
    OnlTicketId int NOT NULL,
    Comment ntext NOT NULL,
    CreatedDate datetime2 NOT NULL,
    CreatedBy nvarchar(100) NOT NULL
);

-- =====================================================
-- 8. FOREIGN KEYS
-- =====================================================

-- OnlTicketAttachment
ALTER TABLE OnlTicketAttachment 
ADD CONSTRAINT FK_OnlTicketAttachment_OnlTicket 
FOREIGN KEY (OnlTicketId) REFERENCES OnlTicket(Id) ON DELETE CASCADE;

-- OnlTicketSoldTo
ALTER TABLE OnlTicketSoldTo 
ADD CONSTRAINT FK_OnlTicketSoldTo_OnlTicket 
FOREIGN KEY (OnlTicketId) REFERENCES OnlTicket(Id) ON DELETE CASCADE;

-- OnlTicketShipTo
ALTER TABLE OnlTicketShipTo 
ADD CONSTRAINT FK_OnlTicketShipTo_OnlTicketSoldTo 
FOREIGN KEY (OnlTicketSoldToId) REFERENCES OnlTicketSoldTo(Id) ON DELETE CASCADE;

-- OnlTicketOrderItem
ALTER TABLE OnlTicketOrderItem 
ADD CONSTRAINT FK_OnlTicketOrderItem_OnlTicketShipTo 
FOREIGN KEY (OnlTicketShipToId) REFERENCES OnlTicketShipTo(Id) ON DELETE CASCADE;

-- OnlTicketSAPOrder
ALTER TABLE OnlTicketSAPOrder 
ADD CONSTRAINT FK_OnlTicketSAPOrder_OnlTicket 
FOREIGN KEY (OnlTicketId) REFERENCES OnlTicket(Id) ON DELETE CASCADE;

-- OnlTicketComment
ALTER TABLE OnlTicketComment 
ADD CONSTRAINT FK_OnlTicketComment_OnlTicket 
FOREIGN KEY (OnlTicketId) REFERENCES OnlTicket(Id) ON DELETE CASCADE;

-- OnlTicket - Referências para entidades existentes
ALTER TABLE OnlTicket 
ADD CONSTRAINT FK_OnlTicket_Country 
FOREIGN KEY (CountryId) REFERENCES Countries(Id);

ALTER TABLE OnlTicket 
ADD CONSTRAINT FK_OnlTicket_CustomerSegment 
FOREIGN KEY (SegmentId) REFERENCES CustomerSegment(Id);

-- =====================================================
-- 9. ÍNDICES ÚNICOS
-- =====================================================
CREATE UNIQUE INDEX IX_OnlTicket_LogNumber ON OnlTicket(LogNumber);
CREATE UNIQUE INDEX IX_OnlTicket_OrderId ON OnlTicket(OrderId) WHERE OrderId IS NOT NULL;

-- =====================================================
-- 10. ÍNDICES DE PERFORMANCE
-- =====================================================

-- OnlTicket
CREATE INDEX IX_OnlTicket_CreatedDate ON OnlTicket(CreatedDate);
CREATE INDEX IX_OnlTicket_Status ON OnlTicket(Status);
CREATE INDEX IX_OnlTicket_CreatedBy ON OnlTicket(CreatedBy);
CREATE INDEX IX_OnlTicket_CountryId ON OnlTicket(CountryId);
CREATE INDEX IX_OnlTicket_SegmentId ON OnlTicket(SegmentId);
CREATE INDEX IX_OnlTicket_PONumber ON OnlTicket(PONumber);
CREATE INDEX IX_OnlTicket_CustomerName ON OnlTicket(CustomerName);

-- OnlTicketAttachment
CREATE INDEX IX_OnlTicketAttachment_OnlTicketId ON OnlTicketAttachment(OnlTicketId);
CREATE INDEX IX_OnlTicketAttachment_IsActive ON OnlTicketAttachment(IsActive);
CREATE INDEX IX_OnlTicketAttachment_CreatedDate ON OnlTicketAttachment(CreatedDate);

-- OnlTicketSoldTo
CREATE INDEX IX_OnlTicketSoldTo_OnlTicketId ON OnlTicketSoldTo(OnlTicketId);
CREATE INDEX IX_OnlTicketSoldTo_CNPJ ON OnlTicketSoldTo(CNPJ);

-- OnlTicketShipTo
CREATE INDEX IX_OnlTicketShipTo_OnlTicketSoldToId ON OnlTicketShipTo(OnlTicketSoldToId);
CREATE INDEX IX_OnlTicketShipTo_CNPJ ON OnlTicketShipTo(CNPJ);
CREATE INDEX IX_OnlTicketShipTo_SAPOrderNumber ON OnlTicketShipTo(SAPOrderNumber);

-- OnlTicketOrderItem
CREATE INDEX IX_OnlTicketOrderItem_OnlTicketShipToId ON OnlTicketOrderItem(OnlTicketShipToId);
CREATE INDEX IX_OnlTicketOrderItem_PartNumber ON OnlTicketOrderItem(PartNumber);
CREATE INDEX IX_OnlTicketOrderItem_BidContractNumber ON OnlTicketOrderItem(BidContractNumber);

-- OnlTicketSAPOrder
CREATE INDEX IX_OnlTicketSAPOrder_OnlTicketId ON OnlTicketSAPOrder(OnlTicketId);
CREATE INDEX IX_OnlTicketSAPOrder_SAPOrderNumber ON OnlTicketSAPOrder(SAPOrderNumber);
CREATE INDEX IX_OnlTicketSAPOrder_InvoiceNumber ON OnlTicketSAPOrder(InvoiceNumber);

-- OnlTicketComment
CREATE INDEX IX_OnlTicketComment_OnlTicketId ON OnlTicketComment(OnlTicketId);
CREATE INDEX IX_OnlTicketComment_CreatedDate ON OnlTicketComment(CreatedDate);
