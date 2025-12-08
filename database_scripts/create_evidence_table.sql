-- =============================================
-- 证物表创建脚本
-- 生成时间: 2024-12-09
-- 用途: 创建 wt.Evidence 表
-- =============================================

USE WitchTrialWT;
GO

-- 检查表是否存在，如果存在则删除（谨慎使用）
IF OBJECT_ID('wt.Evidence', 'U') IS NOT NULL
BEGIN
    PRINT '表 wt.Evidence 已存在，正在删除...';
    DROP TABLE wt.Evidence;
    PRINT '表已删除。';
END
GO

-- 创建证物表
CREATE TABLE wt.Evidence
(
    EvidenceID INT IDENTITY(1,1) NOT NULL,
    EvidenceNo NVARCHAR(20) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    ImagePath NVARCHAR(255) NULL,
    
    CONSTRAINT PK_Evidence PRIMARY KEY CLUSTERED (EvidenceID ASC),
    CONSTRAINT UQ_Evidence_No UNIQUE (EvidenceNo)
);
GO

-- 创建索引
CREATE NONCLUSTERED INDEX IX_Evidence_No 
ON wt.Evidence (EvidenceNo ASC);
GO

PRINT '证物表创建完成！';
PRINT '表名: wt.Evidence';
PRINT '字段: EvidenceID (主键), EvidenceNo (唯一), Name, Description, ImagePath';
GO
