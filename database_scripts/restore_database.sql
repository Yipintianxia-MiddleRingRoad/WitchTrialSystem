-- 队友使用：从 .bak 文件恢复数据库
-- 在 SSMS 中执行此脚本

USE master;
GO

-- 如果数据库已存在，先删除
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'WitchTrialWT')
BEGIN
    ALTER DATABASE [WitchTrialWT] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [WitchTrialWT];
END
GO

-- 从备份文件恢复数据库
-- 注意：需要修改 FROM DISK 路径为实际的 .bak 文件位置
RESTORE DATABASE [WitchTrialWT]
FROM DISK = 'D:\WitchTrialWT_完整备份.bak'
WITH MOVE 'WitchTrialWT' TO 'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\WitchTrialWT.mdf',
     MOVE 'WitchTrialWT_log' TO 'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\WitchTrialWT_log.ldf',
     REPLACE;
GO

PRINT '数据库恢复完成！';
PRINT '现在可以运行 WitchTrialSystem 程序了';
