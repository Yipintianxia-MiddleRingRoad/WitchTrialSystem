-- ========================================
-- 添加时间戳字段到 Witch 表
-- 功能：追踪魔女的流转时间
-- 日期：2024-12-02
-- ========================================

USE WitchTrialWT;
GO

PRINT N'========================================';
PRINT N'开始添加时间戳字段...';
PRINT N'========================================';

-- 检查并添加 CaptureTime（被抓捕时间）
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'wt.Witch') AND name = 'CaptureTime')
BEGIN
    ALTER TABLE wt.Witch ADD CaptureTime DATETIME2 NULL;
    PRINT N'✓ 已添加字段: CaptureTime (被抓捕时间)';
END
ELSE
BEGIN
    PRINT N'ℹ 字段已存在: CaptureTime';
END

-- 检查并添加 DepartureTime（离开囚牢时间）
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'wt.Witch') AND name = 'DepartureTime')
BEGIN
    ALTER TABLE wt.Witch ADD DepartureTime DATETIME2 NULL;
    PRINT N'✓ 已添加字段: DepartureTime (离开囚牢时间)';
END
ELSE
BEGIN
    PRINT N'ℹ 字段已存在: DepartureTime';
END

-- 检查并添加 ArrivalTime（抵达魔女岛时间）
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'wt.Witch') AND name = 'ArrivalTime')
BEGIN
    ALTER TABLE wt.Witch ADD ArrivalTime DATETIME2 NULL;
    PRINT N'✓ 已添加字段: ArrivalTime (抵达魔女岛时间)';
END
ELSE
BEGIN
    PRINT N'ℹ 字段已存在: ArrivalTime';
END

-- 检查并添加 DeathTime（死亡时间）
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'wt.Witch') AND name = 'DeathTime')
BEGIN
    ALTER TABLE wt.Witch ADD DeathTime DATETIME2 NULL;
    PRINT N'✓ 已添加字段: DeathTime (死亡时间)';
END
ELSE
BEGIN
    PRINT N'ℹ 字段已存在: DeathTime';
END

PRINT N'';
PRINT N'========================================';
PRINT N'时间戳字段添加完成！';
PRINT N'========================================';
PRINT N'';
PRINT N'新增字段说明：';
PRINT N'  - CaptureTime: 魔女被抓捕的时间';
PRINT N'  - DepartureTime: 离开原囚牢前往魔女岛的时间';
PRINT N'  - ArrivalTime: 抵达魔女岛的时间';
PRINT N'  - DeathTime: 死亡时间（如果死亡）';
PRINT N'';
PRINT N'所有字段均为可空（NULL），可以后续补充';
PRINT N'';

GO
