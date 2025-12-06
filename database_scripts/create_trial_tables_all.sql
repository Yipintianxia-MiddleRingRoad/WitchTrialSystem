-- =============================================
-- 一键创建审判投票流程系统所有表
-- 包含：TrialSession, TrialParticipant, TrialNotification
-- 作者：Kiro AI Assistant
-- 日期：2024-12-06
-- =============================================

USE WitchTrialWT;
GO

PRINT N'========================================';
PRINT N'开始创建审判投票流程系统数据库表...';
PRINT N'========================================';
PRINT N'';

-- =============================================
-- 1. 创建审判会话表 (TrialSession)
-- =============================================
PRINT N'[1/3] 创建审判会话表 (TrialSession)...';

IF OBJECT_ID('wt.TrialSession', 'U') IS NOT NULL
BEGIN
    PRINT N'  ⚠️  表已存在，正在删除...';
    DROP TABLE wt.TrialSession;
END

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
    
    CONSTRAINT FK_TrialSession_Island FOREIGN KEY (IslandID) 
        REFERENCES wt.Island(IslandID),
    CONSTRAINT FK_TrialSession_Batch FOREIGN KEY (BatchID) 
        REFERENCES wt.Batch(BatchID),
    CONSTRAINT FK_TrialSession_CreatedBy FOREIGN KEY (CreatedBy) 
        REFERENCES wt.[User](UserID),
    CONSTRAINT FK_TrialSession_ExecutionTarget FOREIGN KEY (ExecutionTargetWitchID) 
        REFERENCES wt.Witch(WitchID),
    CONSTRAINT CK_TrialSession_Status CHECK (Status IN (
        N'Pending', N'Voting', N'Confirmed', N'Executing', N'Completed', N'Cancelled'
    ))
);

CREATE INDEX IX_TrialSession_Island ON wt.TrialSession(IslandID);
CREATE INDEX IX_TrialSession_Status ON wt.TrialSession(IslandID, Status);
CREATE INDEX IX_TrialSession_CreatedAt ON wt.TrialSession(CreatedAt DESC);

PRINT N'  ✅ 审判会话表创建成功！';
PRINT N'';

-- =============================================
-- 2. 创建审判参与者表 (TrialParticipant)
-- =============================================
PRINT N'[2/3] 创建审判参与者表 (TrialParticipant)...';

IF OBJECT_ID('wt.TrialParticipant', 'U') IS NOT NULL
BEGIN
    PRINT N'  ⚠️  表已存在，正在删除...';
    DROP TABLE wt.TrialParticipant;
END

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
    
    CONSTRAINT FK_TrialParticipant_Session FOREIGN KEY (SessionID) 
        REFERENCES wt.TrialSession(SessionID) ON DELETE CASCADE,
    CONSTRAINT FK_TrialParticipant_Witch FOREIGN KEY (WitchID) 
        REFERENCES wt.Witch(WitchID),
    CONSTRAINT FK_TrialParticipant_User FOREIGN KEY (UserID) 
        REFERENCES wt.[User](UserID),
    CONSTRAINT FK_TrialParticipant_VotedFor FOREIGN KEY (VotedForWitchID) 
        REFERENCES wt.Witch(WitchID),
    CONSTRAINT UQ_TrialParticipant_Session_Witch UNIQUE (SessionID, WitchID)
);

CREATE INDEX IX_TrialParticipant_Session ON wt.TrialParticipant(SessionID);
CREATE INDEX IX_TrialParticipant_User ON wt.TrialParticipant(UserID);
CREATE INDEX IX_TrialParticipant_Witch ON wt.TrialParticipant(WitchID);
CREATE INDEX IX_TrialParticipant_HasVoted ON wt.TrialParticipant(SessionID, HasVoted);
CREATE INDEX IX_TrialParticipant_HasConfirmed ON wt.TrialParticipant(SessionID, HasConfirmedExecution);

PRINT N'  ✅ 审判参与者表创建成功！';
PRINT N'';

-- =============================================
-- 3. 创建审判通知表 (TrialNotification)
-- =============================================
PRINT N'[3/3] 创建审判通知表 (TrialNotification)...';

IF OBJECT_ID('wt.TrialNotification', 'U') IS NOT NULL
BEGIN
    PRINT N'  ⚠️  表已存在，正在删除...';
    DROP TABLE wt.TrialNotification;
END

CREATE TABLE wt.TrialNotification (
    NotificationID INT IDENTITY(1,1) PRIMARY KEY,
    SessionID INT NOT NULL,
    UserID INT NOT NULL,
    Message NVARCHAR(500) NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_TrialNotification_Session FOREIGN KEY (SessionID) 
        REFERENCES wt.TrialSession(SessionID) ON DELETE CASCADE,
    CONSTRAINT FK_TrialNotification_User FOREIGN KEY (UserID) 
        REFERENCES wt.[User](UserID)
);

CREATE INDEX IX_TrialNotification_User ON wt.TrialNotification(UserID, IsRead);
CREATE INDEX IX_TrialNotification_Session ON wt.TrialNotification(SessionID);
CREATE INDEX IX_TrialNotification_CreatedAt ON wt.TrialNotification(CreatedAt DESC);

PRINT N'  ✅ 审判通知表创建成功！';
PRINT N'';

-- =============================================
-- 完成
-- =============================================
PRINT N'========================================';
PRINT N'✅ 所有表创建完成！';
PRINT N'========================================';
PRINT N'';
PRINT N'已创建的表：';
PRINT N'  1. wt.TrialSession       - 审判会话表';
PRINT N'  2. wt.TrialParticipant   - 审判参与者表';
PRINT N'  3. wt.TrialNotification  - 审判通知表';
PRINT N'';
PRINT N'下一步：';
PRINT N'  1. 验证表结构：SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = ''wt'' AND TABLE_NAME LIKE ''Trial%''';
PRINT N'  2. 开始实现数据模型类（Models/TrialModels.cs）';
PRINT N'';
GO
