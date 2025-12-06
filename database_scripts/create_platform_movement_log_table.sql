-- =============================================
-- 创建处刑台移动记录表 (PlatformMovementLog)
-- 用途：记录处刑台的所有移动历史
-- 作者：WitchTrialSystem
-- 日期：2024-12-06
-- =============================================

USE WitchTrialWT;
GO

-- 如果表已存在则删除（仅用于开发环境）
IF OBJECT_ID('wt.PlatformMovementLog', 'U') IS NOT NULL
BEGIN
    DROP TABLE wt.PlatformMovementLog;
    PRINT '已删除旧的 wt.PlatformMovementLog 表';
END
GO

-- 创建处刑台移动记录表
CREATE TABLE wt.PlatformMovementLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    IslandID INT NOT NULL,
    PlatformID INT NOT NULL,
    PlatformNumber INT NOT NULL,           -- 处刑台编号
    FromPosition INT NOT NULL,             -- 起始位置
    ToPosition INT NOT NULL,               -- 目标位置
    ToolName NVARCHAR(100) NULL,          -- 移动时的刑具名称
    MovementTime DATETIME2 NOT NULL,      -- 移动时间（北京时间，可手动输入）
    IsManualTime BIT NOT NULL DEFAULT 0,  -- 是否手动输入时间
    MovementType NVARCHAR(20) NOT NULL,   -- 移动类型：升起/返回
    
    -- 外键约束
    CONSTRAINT FK_PlatformMovementLog_Island FOREIGN KEY (IslandID) 
        REFERENCES wt.Island(IslandID),
    CONSTRAINT FK_PlatformMovementLog_Platform FOREIGN KEY (PlatformID) 
        REFERENCES wt.ExecutionPlatform(PlatformID),
    
    -- 检查约束
    CONSTRAINT CK_PlatformMovementLog_Position CHECK (
        FromPosition BETWEEN 1 AND 50 AND ToPosition BETWEEN 1 AND 50
    ),
    CONSTRAINT CK_PlatformMovementLog_Type CHECK (MovementType IN (N'升起', N'返回'))
);
GO

-- 创建索引
CREATE INDEX IX_PlatformMovementLog_Island 
    ON wt.PlatformMovementLog(IslandID);
GO

CREATE INDEX IX_PlatformMovementLog_Platform 
    ON wt.PlatformMovementLog(PlatformID);
GO

CREATE INDEX IX_PlatformMovementLog_Time 
    ON wt.PlatformMovementLog(MovementTime DESC);
GO

-- 添加表说明
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'处刑台移动记录表：记录处刑台的所有移动历史（不记录操作人）', 
    @level0type = N'SCHEMA', @level0name = N'wt',
    @level1type = N'TABLE', @level1name = N'PlatformMovementLog';
GO

-- 添加字段说明
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'日志ID（主键）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'PlatformMovementLog', @level2type = N'COLUMN', @level2name = N'LogID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'所属岛屿ID', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'PlatformMovementLog', @level2type = N'COLUMN', @level2name = N'IslandID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'处刑台ID', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'PlatformMovementLog', @level2type = N'COLUMN', @level2name = N'PlatformID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'处刑台编号（冗余字段，便于查询）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'PlatformMovementLog', @level2type = N'COLUMN', @level2name = N'PlatformNumber';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'起始位置', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'PlatformMovementLog', @level2type = N'COLUMN', @level2name = N'FromPosition';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'目标位置', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'PlatformMovementLog', @level2type = N'COLUMN', @level2name = N'ToPosition';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'移动时的刑具名称（冗余字段）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'PlatformMovementLog', @level2type = N'COLUMN', @level2name = N'ToolName';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'移动时间（北京时间，可手动输入精确到秒）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'PlatformMovementLog', @level2type = N'COLUMN', @level2name = N'MovementTime';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'是否手动输入时间（1=手动输入，0=系统当前时间）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'PlatformMovementLog', @level2type = N'COLUMN', @level2name = N'IsManualTime';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'移动类型（升起/返回）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'PlatformMovementLog', @level2type = N'COLUMN', @level2name = N'MovementType';
GO

PRINT '✓ wt.PlatformMovementLog 表创建成功';
PRINT '✓ 索引创建成功';
PRINT '✓ 约束创建成功';
GO
