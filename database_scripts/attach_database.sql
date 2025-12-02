-- ========================================
-- 附加数据库文件到 SQL Server
-- ========================================
-- 使用方法：
-- 1. 将项目 Data/ 文件夹中的 WitchTrialWT.mdf 和 WitchTrialWT_log.ldf 
--    复制到你的 SQL Server 数据目录
-- 2. 修改下面的路径为你实际复制的位置
-- 3. 在 SSMS 中执行此脚本
-- ========================================

USE master;
GO

PRINT '========================================';
PRINT '开始附加数据库 WitchTrialWT';
PRINT '========================================';
GO

-- 如果数据库已存在，先分离
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'WitchTrialWT')
BEGIN
    PRINT '检测到数据库已存在，正在分离...';
    ALTER DATABASE [WitchTrialWT] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    EXEC sp_detach_db 'WitchTrialWT';
    PRINT '数据库已分离';
END
GO

-- ========================================
-- ⚠️ 重要：修改下面的路径为你的实际路径
-- ========================================
-- 常见路径：
-- C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\
-- D:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\
-- ========================================

DECLARE @DataPath NVARCHAR(500) = 'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\';
DECLARE @MdfFile NVARCHAR(500) = @DataPath + 'WitchTrialWT.mdf';
DECLARE @LdfFile NVARCHAR(500) = @DataPath + 'WitchTrialWT_log.ldf';

PRINT '数据文件路径: ' + @MdfFile;
PRINT '日志文件路径: ' + @LdfFile;
PRINT '';

-- 检查文件是否存在
DECLARE @FileExists INT;
EXEC master.dbo.xp_fileexist @MdfFile, @FileExists OUTPUT;

IF @FileExists = 0
BEGIN
    PRINT '❌ 错误：找不到数据文件！';
    PRINT '请确认：';
    PRINT '1. 已将 Data/ 文件夹中的 .mdf 和 .ldf 文件复制到上述路径';
    PRINT '2. 路径设置正确（注意盘符和文件夹名称）';
    PRINT '';
    RAISERROR('数据文件不存在，附加失败', 16, 1);
    RETURN;
END

-- 附加数据库
PRINT '正在附加数据库...';
EXEC('
CREATE DATABASE [WitchTrialWT] ON 
(FILENAME = ''' + @MdfFile + '''),
(FILENAME = ''' + @LdfFile + ''')
FOR ATTACH;
');

PRINT '';
PRINT '========================================';
PRINT '✅ 数据库附加完成！';
PRINT '========================================';
PRINT '';
PRINT '下一步：';
PRINT '1. 验证数据：SELECT COUNT(*) FROM WitchTrialWT.wt.Witch;';
PRINT '2. 配置 appsettings.json 中的连接字符串';
PRINT '3. 运行程序，使用 admin/123456 登录';
PRINT '';
GO
