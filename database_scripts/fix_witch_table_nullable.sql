-- ========================================
-- 修复脚本：允许 Witch 表的 IslandID 和 BatchID 为 NULL
-- 功能：支持"待分配"状态的魔女
-- ========================================

USE WitchTrialWT;
GO

PRINT N'========================================';
PRINT N'开始修复 Witch 表结构';
PRINT N'========================================';
PRINT N'';

-- 检查当前列的可空性
PRINT N'检查当前列的可空性...';
SELECT 
    COLUMN_NAME AS 列名,
    IS_NULLABLE AS 可为空,
    DATA_TYPE AS 数据类型
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'wt' 
    AND TABLE_NAME = 'Witch'
    AND COLUMN_NAME IN ('IslandID', 'BatchID');
GO

PRINT N'';
PRINT N'修改列为可空...';

-- 修改 IslandID 为可空
ALTER TABLE wt.Witch
ALTER COLUMN IslandID INT NULL;

PRINT N'✓ IslandID 已修改为可空';

-- 修改 BatchID 为可空
ALTER TABLE wt.Witch
ALTER COLUMN BatchID INT NULL;

PRINT N'✓ BatchID 已修改为可空';

PRINT N'';
PRINT N'验证修改结果...';

-- 验证修改
SELECT 
    COLUMN_NAME AS 列名,
    IS_NULLABLE AS 可为空,
    DATA_TYPE AS 数据类型
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'wt' 
    AND TABLE_NAME = 'Witch'
    AND COLUMN_NAME IN ('IslandID', 'BatchID');
GO

PRINT N'';
PRINT N'========================================';
PRINT N'✅ Witch 表结构修复完成';
PRINT N'   IslandID 和 BatchID 现在可以为 NULL';
PRINT N'   支持"待分配"状态的魔女';
PRINT N'========================================';
