-- =============================================
-- 记录表创建脚本
-- 生成时间: 2024-12-09
-- 用途: 创建 wt.Record 表
-- =============================================

USE WitchTrialWT;
GO

-- 检查表是否存在，如果存在则删除（谨慎使用）
IF OBJECT_ID('wt.Record', 'U') IS NOT NULL
BEGIN
    PRINT '表 wt.Record 已存在，正在删除...';
    DROP TABLE wt.Record;
    PRINT '表已删除。';
END
GO

-- 创建记录表
CREATE TABLE wt.Record
(
    RecordID INT IDENTITY(1,1) NOT NULL,
    Title NVARCHAR(100) NOT NULL,
    Content NVARCHAR(255) NULL,  -- 存储Markdown文件路径
    
    CONSTRAINT PK_Record PRIMARY KEY CLUSTERED (RecordID ASC)
);
GO

-- 创建索引
CREATE NONCLUSTERED INDEX IX_Record_Title 
ON wt.Record (Title ASC);
GO

PRINT '记录表创建完成！';
PRINT '表名: wt.Record';
PRINT '字段: RecordID (主键), Title, Content (Markdown文件路径)';
GO
