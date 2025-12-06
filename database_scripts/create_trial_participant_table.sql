-- =============================================
-- 创建审判参与者表 (TrialParticipant)
-- 用途：存储每个参与魔女的投票和确认状态
-- 作者：Kiro AI Assistant
-- 日期：2024-12-06
-- =============================================

USE WitchTrialWT;
GO

-- 如果表已存在则删除（仅用于开发测试）
IF OBJECT_ID('wt.TrialParticipant', 'U') IS NOT NULL
    DROP TABLE wt.TrialParticipant;
GO

-- 创建审判参与者表
CREATE TABLE wt.TrialParticipant (
    ParticipantID INT IDENTITY(1,1) PRIMARY KEY,
    SessionID INT NOT NULL,
    WitchID INT NOT NULL,
    UserID INT NOT NULL,
    HasVoted BIT NOT NULL DEFAULT 0,
    VotedForWitchID INT NULL,
    VotedAt DATETIME2 NULL,
    HasConfirmedExecution BIT NOT NULL DEFAULT 0,
    ExecutionConfirmedAt DATETIME2 NULL,
    
    -- 外键约束
    CONSTRAINT FK_TrialParticipant_Session FOREIGN KEY (SessionID) 
        REFERENCES wt.TrialSession(SessionID) ON DELETE CASCADE,
    CONSTRAINT FK_TrialParticipant_Witch FOREIGN KEY (WitchID) 
        REFERENCES wt.Witch(WitchID),
    CONSTRAINT FK_TrialParticipant_User FOREIGN KEY (UserID) 
        REFERENCES wt.[User](UserID),
    CONSTRAINT FK_TrialParticipant_VotedFor FOREIGN KEY (VotedForWitchID) 
        REFERENCES wt.Witch(WitchID),
    
    -- 唯一约束：同一会话中每个魔女只能参与一次
    CONSTRAINT UQ_TrialParticipant_Session_Witch UNIQUE (SessionID, WitchID)
);
GO

-- 创建索引
CREATE INDEX IX_TrialParticipant_Session ON wt.TrialParticipant(SessionID);
CREATE INDEX IX_TrialParticipant_User ON wt.TrialParticipant(UserID);
CREATE INDEX IX_TrialParticipant_Witch ON wt.TrialParticipant(WitchID);
CREATE INDEX IX_TrialParticipant_HasVoted ON wt.TrialParticipant(SessionID, HasVoted);
CREATE INDEX IX_TrialParticipant_HasConfirmed ON wt.TrialParticipant(SessionID, HasConfirmedExecution);
GO

-- 添加表说明
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'审判参与者表：存储每个参与魔女的投票和确认状态', 
    @level0type = N'SCHEMA', @level0name = N'wt',
    @level1type = N'TABLE', @level1name = N'TrialParticipant';
GO

-- 添加字段说明
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'参与者ID（主键）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialParticipant', @level2type = N'COLUMN', @level2name = N'ParticipantID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'审判会话ID', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialParticipant', @level2type = N'COLUMN', @level2name = N'SessionID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'参与魔女的WitchID', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialParticipant', @level2type = N'COLUMN', @level2name = N'WitchID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'参与魔女的UserID', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialParticipant', @level2type = N'COLUMN', @level2name = N'UserID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'是否已投票', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialParticipant', @level2type = N'COLUMN', @level2name = N'HasVoted';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'投给谁（WitchID）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialParticipant', @level2type = N'COLUMN', @level2name = N'VotedForWitchID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'投票时间', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialParticipant', @level2type = N'COLUMN', @level2name = N'VotedAt';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'是否已确认处刑（点击处刑按钮）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialParticipant', @level2type = N'COLUMN', @level2name = N'HasConfirmedExecution';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'确认处刑时间', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialParticipant', @level2type = N'COLUMN', @level2name = N'ExecutionConfirmedAt';
GO

PRINT N'✅ 审判参与者表 (wt.TrialParticipant) 创建成功！';
GO
