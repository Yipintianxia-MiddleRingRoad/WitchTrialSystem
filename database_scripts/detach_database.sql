-- ========================================
-- 分离数据库（用于复制数据库文件）
-- ========================================
-- 使用场景：
-- 当你需要复制 .mdf 和 .ldf 文件时，必须先分离数据库
-- 分离后文件才能被复制
-- ========================================

USE master;
GO

PRINT '========================================';
PRINT '准备分离数据库 WitchTrialWT';
PRINT '========================================';
GO

-- 检查数据库是否存在
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'WitchTrialWT')
BEGIN
    PRINT '❌ 错误：数据库 WitchTrialWT 不存在';
    RETURN;
END
GO

-- 关闭所有连接
PRINT '正在关闭所有连接...';
ALTER DATABASE [WitchTrialWT] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- 分离数据库
PRINT '正在分离数据库...';
EXEC sp_detach_db 'WitchTrialWT', 'true';
GO

PRINT '';
PRINT '========================================';
PRINT '✅ 数据库已分离！';
PRINT '========================================';
PRINT '';
PRINT '现在你可以：';
PRINT '1. 复制数据库文件到项目的 Data/ 文件夹';
PRINT '   文件位置：D:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\';
PRINT '   - WitchTrialWT.mdf';
PRINT '   - WitchTrialWT_log.ldf';
PRINT '';
PRINT '2. 复制完成后，重新附加数据库：';
PRINT '   执行 attach_database.sql 脚本';
PRINT '';
GO
