-- =============================================
-- 创建处刑台表 (ExecutionPlatform)
-- 用途：管理每个岛屿的处刑台位置和刑具信息
-- 作者：WitchTrialSystem
-- 日期：2024-12-06
-- =============================================

USE WitchTrialWT;
GO

-- 如果表已存在则删除（仅用于开发环境）
IF OBJECT_ID('wt.ExecutionPlatform', 'U') IS NOT NULL
BEGIN
    DROP TABLE wt.ExecutionPlatform;
    PRINT '已删除旧的 wt.ExecutionPlatform 表';
END
GO

-- 创建处刑台表
CREATE TABLE wt.ExecutionPlatform (
    PlatformID INT PRIMARY KEY IDENTITY(1,1),
    IslandID INT NOT NULL,
    PlatformNumber INT NOT NULL,           -- 处刑台编号 (1-49)
    HomePosition INT NOT NULL,             -- 原位位置 (1-49)
    CurrentPosition INT NOT NULL,          -- 当前位置 (1-50)
    ToolName NVARCHAR(100) NULL,          -- 刑具名称
    ToolType NVARCHAR(50) NULL,           -- 刑具类型
    ToolDescription NVARCHAR(500) NULL,   -- 刑具描述
    Status NVARCHAR(20) NOT NULL DEFAULT N'空闲',  -- 状态：空闲/使用中
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- 外键约束
    CONSTRAINT FK_ExecutionPlatform_Island FOREIGN KEY (IslandID) 
        REFERENCES wt.Island(IslandID),
    
    -- 唯一约束：每个岛屿的处刑台编号唯一
    CONSTRAINT UQ_ExecutionPlatform_Island_Number UNIQUE (IslandID, PlatformNumber),
    
    -- 检查约束
    CONSTRAINT CK_ExecutionPlatform_Number CHECK (PlatformNumber BETWEEN 1 AND 49),
    CONSTRAINT CK_ExecutionPlatform_HomePosition CHECK (HomePosition BETWEEN 1 AND 49),
    CONSTRAINT CK_ExecutionPlatform_CurrentPosition CHECK (CurrentPosition BETWEEN 1 AND 50),
    CONSTRAINT CK_ExecutionPlatform_Status CHECK (Status IN (N'空闲', N'使用中'))
);
GO

-- 创建索引
CREATE INDEX IX_ExecutionPlatform_Island 
    ON wt.ExecutionPlatform(IslandID);
GO

CREATE INDEX IX_ExecutionPlatform_CurrentPosition 
    ON wt.ExecutionPlatform(IslandID, CurrentPosition);
GO

-- 添加表说明
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'处刑台表：管理每个岛屿的处刑台位置和刑具信息', 
    @level0type = N'SCHEMA', @level0name = N'wt',
    @level1type = N'TABLE', @level1name = N'ExecutionPlatform';
GO

-- 添加字段说明
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'处刑台ID（主键）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'ExecutionPlatform', @level2type = N'COLUMN', @level2name = N'PlatformID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'所属岛屿ID', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'ExecutionPlatform', @level2type = N'COLUMN', @level2name = N'IslandID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'处刑台编号（1-49）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'ExecutionPlatform', @level2type = N'COLUMN', @level2name = N'PlatformNumber';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'原位位置（1-49），处刑台的固定归属位置', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'ExecutionPlatform', @level2type = N'COLUMN', @level2name = N'HomePosition';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'当前位置（1-50），50表示在审判庭', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'ExecutionPlatform', @level2type = N'COLUMN', @level2name = N'CurrentPosition';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'刑具名称', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'ExecutionPlatform', @level2type = N'COLUMN', @level2name = N'ToolName';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'刑具类型', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'ExecutionPlatform', @level2type = N'COLUMN', @level2name = N'ToolType';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'刑具描述', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'ExecutionPlatform', @level2type = N'COLUMN', @level2name = N'ToolDescription';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'状态（空闲/使用中）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'ExecutionPlatform', @level2type = N'COLUMN', @level2name = N'Status';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'ExecutionPlatform', @level2type = N'COLUMN', @level2name = N'CreatedAt';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'更新时间', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'ExecutionPlatform', @level2type = N'COLUMN', @level2name = N'UpdatedAt';
GO

PRINT '✓ wt.ExecutionPlatform 表创建成功';
PRINT '✓ 索引创建成功';
PRINT '✓ 约束创建成功';
GO
