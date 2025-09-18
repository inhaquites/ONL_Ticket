-- ========================================
-- SCRIPT DE ATUALIZAÇÃO - TABELA OrderAttachment
-- MIGRAÇÃO ONL TICKET → ORDER ENTITIES
-- ========================================
-- Objetivo: Adicionar 3 campos necessários para compatibilidade com OnlTicketAttachment
-- ========================================

USE [ThinkTool]
GO

-- Adicionar campos necessários na tabela OrderAttachment
ALTER TABLE [dbo].[OrderAttachment]
ADD [FileExtension] NVARCHAR(10) NULL,
    [UpdatedDate] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(100) NULL
GO
