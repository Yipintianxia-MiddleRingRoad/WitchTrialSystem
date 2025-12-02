-- 为 Witch 表添加时间记录字段
-- 执行时间: 2024年12月2日晚

USE WitchTrialWT;
GO

-- 检查并添加 CaptureTime 字段（抓捕时间）
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_SCHEMA = 'wt' 
      AND TABLE_NAME = 'Witch' 
      AND COLUMN_NAME = 'CaptureTime'
)
BEGIN
    ALTER TABLE wt.Witch ADD CaptureTime DATETIME2 NULL;
    PRINT N'已添加 CaptureTime 字段';
END
ELSE
BEGIN
    PRINT N'CaptureTime 字段已存在';
END
GO

-- 检查并添加 DepartureTime 字段（离开原地时间）
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_SCHEMA = 'wt' 
      AND TABLE_NAME = 'Witch' 
      AND COLUMN_NAME = 'DepartureTime'
)
BEGIN
    ALTER TABLE wt.Witch ADD DepartureTime DATETIME2 NULL;
    PRINT N'已添加 DepartureTime 字段';
END
ELSE
BEGIN
    PRINT N'DepartureTime 字段已存在';
END
GO

-- 检查并添加 ArrivalTime 字段（到达岛屿时间）
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_SCHEMA = 'wt' 
      AND TABLE_NAME = 'Witch' 
      AND COLUMN_NAME = 'ArrivalTime'
)
BEGIN
    ALTER TABLE wt.Witch ADD ArrivalTime DATETIME2 NULL;
    PRINT N'已添加 ArrivalTime 字段';
END
ELSE
BEGIN
    PRINT N'ArrivalTime 字段已存在';
END
GO

-- 检查并添加 DeathTime 字段（处刑/死亡时间）
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_SCHEMA = 'wt' 
      AND TABLE_NAME = 'Witch' 
      AND COLUMN_NAME = 'DeathTime'
)
BEGIN
    ALTER TABLE wt.Witch ADD DeathTime DATETIME2 NULL;
    PRINT N'已添加 DeathTime 字段';
END
ELSE
BEGIN
    PRINT N'DeathTime 字段已存在';
END
GO

PRINT N'时间字段添加完成！';
GO

-- 验证字段是否添加成功
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'wt' 
  AND TABLE_NAME = 'Witch'
  AND COLUMN_NAME IN ('CaptureTime', 'DepartureTime', 'ArrivalTime', 'DeathTime')
ORDER BY COLUMN_NAME;
GO
