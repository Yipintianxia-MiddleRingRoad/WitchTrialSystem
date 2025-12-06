-- =============================================
-- 创建审判通知表 (TrialNotification)
-- 用途：存储审判通知消息
-- 作者：Kiro AI Assistant
-- 日期：2024-12-06
-- =============================================

USE WitchTrialWT;
GO

-- 如果表已存在则删除（仅用于开发测试）
IF OBJECT_ID('wt.TrialNotification', 'U') IS NOT NULL
    DROP TABLE wt.TrialNotification;
GO

-- 创建审判通知表
CREATE TABLE wt.TrialNotification (
    NotificationID INT IDENTITY(1,1) PRIMARY KEY,
    SessionID INT NOT NULL,
    UserID INT NOT NULL,
    Message NVARCHAR(500) NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- 外键约束
    CONSTRAINT FK_TrialNotification_Session FOREIGN KEY (SessionID) 
        REFERENCES wt.TrialSession(SessionID) ON DELETE CASCADE,
    CONSTRAINT FK_TrialNotification_User FOREIGN KEY (UserID) 
        REFERENCES wt.[User](UserID)
);
GO

-- 创建索引
CREATE INDEX IX_TrialNotification_User ON wt.TrialNotification(UserID, IsRead);
CREATE INDEX IX_TrialNotification_Session ON wt.TrialNotification(SessionID);
CREATE INDEX IX_TrialNotification_CreatedAt ON wt.TrialNotification(CreatedAt DESC);
GO

-- 添加表说明
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'审判通知表：存储审判通知消息', 
    @level0type = N'SCHEMA', @level0name = N'wt',
    @level1type = N'TABLE', @level1name = N'TrialNotification';
GO

-- 添加字段说明
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'通知ID（主键）', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialNotification', @level2type = N'COLUMN', @level2name = N'NotificationID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'审判会话ID', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialNotification', @level2type = N'COLUMN', @level2name = N'SessionID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'接收通知的UserID', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialNotification', @level2type = N'COLUMN', @level2name = N'UserID';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'通知消息内容', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialNotification', @level2type = N'COLUMN', @level2name = N'Message';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'是否已读', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialNotification', @level2type = N'COLUMN', @level2name = N'IsRead';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'wt', @level1type = N'TABLE', @level1name = N'TrialNotification', @level2type = N'COLUMN', @level2name = N'CreatedAt';
GO

PRINT N'✅ 审判通知表 (wt.TrialNotification) 创建成功！';
GO
