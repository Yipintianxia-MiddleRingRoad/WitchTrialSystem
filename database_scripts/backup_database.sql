-- 备份数据库到 .bak 文件
-- 在 SSMS 中执行此脚本

USE master;
GO

-- 备份数据库
BACKUP DATABASE [WitchTrialWT]
TO DISK = 'D:\WitchTrialWT_完整备份.bak'
WITH FORMAT,
     MEDIANAME = 'WitchTrialWT_Backup',
     NAME = 'WitchTrialWT 完整备份';
GO

PRINT '备份完成！文件位置: D:\WitchTrialWT_完整备份.bak';
PRINT '请将此 .bak 文件分享给队友';
