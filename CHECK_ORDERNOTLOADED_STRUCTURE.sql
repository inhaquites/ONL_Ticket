-- Verificar estrutura da tabela OrderNotLoaded
USE [ThinkTool]
GO

SELECT 
    COLUMN_NAME as 'Campo',
    DATA_TYPE as 'Tipo',
    CHARACTER_MAXIMUM_LENGTH as 'Tamanho',
    IS_NULLABLE as 'Permite_NULL',
    COLUMN_DEFAULT as 'Valor_Default'
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'OrderNotLoaded'
ORDER BY ORDINAL_POSITION

-- Verificar se existe tabela Customer
SELECT COUNT(*) as 'Tabela_Customer_Existe' 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'Customer'

-- Verificar constraints de FK
SELECT 
    fk.name as 'FK_Name',
    tp.name as 'Parent_Table',
    cp.name as 'Parent_Column',
    tr.name as 'Referenced_Table',
    cr.name as 'Referenced_Column'
FROM sys.foreign_keys fk
INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id
INNER JOIN sys.columns cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id
WHERE tp.name = 'OrderNotLoaded'
