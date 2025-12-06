-- =============================================
-- 创建审判会话表 (TrialSession)
-- 用途：存储审判会话的完整信息
-- 作者：Kiro AI Assistant
-- 日期：2024-12-06
-- =============================================

USE WitchTrialWT;
GO

-- 如果表已存在则删除（仅用于开发测试）
IF OBJECT_ID('wt.TrialSession', 'U') IS NOT NULL
    DROP TABLE wt.TrialSession;
GO

-- 创建审判会话表
CREATE TABLE wt.TrialSession (
    SessionID INT IDENTITY(1,1) PRIMARY KEY,
    IslandID INT NOT NULL,
    BatchID INT NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT N'Pending',
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    VotingStartTime DATETIME2 NULL,
    VotingEndTime DATETIME2 NULL,
    ExecutionTargetWitchID INT NULL,
    ExecutionConfirmedAt DATETIME2 NULL,
    CompletedAt DATETIME2 NULL,
    
    -- 外键约束
    CONSTRAINT FK_TrialSession_Island FOREIGN KEY (IslandID) 
        REFERENCES wt.Island(IslandID),
    CONSTRAINT FK_TrialSession_Batch FOREIGN KEY (BatchID) 
        REFERENCES wt.Batch(BatchID),
    CONSTRAINT FK_TrialSession_CreatedBy FOREIGN KEY (CreatedBy) 
        REFERENCES wt.[User](UserID),
    CONSTRAINT FK_TrialSession_ExecutionTarget FOREIGN KEY (ExecutionTargetWitchID) 
        REFERENCES wt.Witch(WitchID),
    
    -- 检查约束
    CONSTRAINT CK_TrialSession_Status CHECK (Status IN (
        N'Pending',    -- 待开始
        N'Voting',     -- 投票中
        N'Confirmed',  -- 已确认处刑对象
        N'Executing',  -- 执行中
        N'Completed',  -- 已完成
        N'Cancelled'   -- 已取消
    ))
);
GO

-- 创建索引
CREATE INDEX IX_TrialSession_Island ON wt.TrialSession(IslandID);
CREATE INDEX IX_TrialSession_Status ON wt.TrialSession(IslandID, Status);
CREATE INDEX IX_TrialSession_CreatedAt ON wt.TrialSession(CreatedAt DESC);
GO

-- 添加表说明
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'审判会话表：存储魔女审判的完整流程信息', 
    @level0type = N'SCHEMA', @level0name = N'wt',
    @level1type = N'TABLE', @level1name = N'TrialSession';
GO

-- 添加字段说明
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'审判会话ID（主键）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialSession', @level2type = N'COLUMN', @level2name = N'SessionID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'所属岛屿ID', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialSession', @level2type = N'COLUMN', @level2name = N'IslandID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'批次ID', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialSession', @level2type = N'COLUMN', @level2name = N'BatchID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'审判状态：Pending/Voting/Confirmed/Executing/Completed/Cancelled', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialSession', @level2type = N'COLUMN', @level2name = N'Status';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'发起人UserID（典狱长）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialSession', @level2type = N'COLUMN', @level2name = N'CreatedBy';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialSession', @level2type = N'COLUMN', @level2name = N'CreatedAt';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'投票开始时间', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialSession', @level2type = N'COLUMN', @level2name = N'VotingStartTime';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'投票结束时间', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialSession', @level2type = N'COLUMN', @level2name = N'VotingEndTime';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'处刑对象WitchID', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialSession', @level2type = N'COLUMN', @level2name = N'ExecutionTargetWitchID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'确认处刑时间', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialSession', @level2type = N'COLUMN', @level2name = N'ExecutionConfirmedAt';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'完成时间', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialSession', @level2type = N'COLUMN', @level2name = N'CompletedAt';
GO

PRINT N'✅ 审判会话表 (wt.TrialSession) 创建成功！';
GO
