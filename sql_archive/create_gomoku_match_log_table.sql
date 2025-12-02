-- 创建五子棋对局日志表
USE WitchTrialWT;
GO

-- 检查表是否已存在
IF NOT EXISTS (
    SELECT * FROM sys.tables 
    WHERE object_id = OBJECT_ID(N'wt.GomokuMatchLog') 
    AND type = 'U'
)
BEGIN
    CREATE TABLE wt.GomokuMatchLog (
        MatchID INT IDENTITY(1,1) PRIMARY KEY,
        Player1Username NVARCHAR(50) NOT NULL,
        Player1Name NVARCHAR(50) NOT NULL,
        Player2Username NVARCHAR(50) NOT NULL,
        Player2Name NVARCHAR(50) NOT NULL,
        StartTime DATETIME2 NOT NULL,
        EndTime DATETIME2 NOT NULL,
        Player1Result NVARCHAR(20) NOT NULL,  -- 'Win', 'Lose', 'Draw'
        Player1ScoreChange INT NOT NULL,
        Player2Result NVARCHAR(20) NOT NULL,  -- 'Win', 'Lose', 'Draw'
        Player2ScoreChange INT NOT NULL,
        TotalMoves INT NOT NULL DEFAULT 0,
        Duration INT NOT NULL DEFAULT 0,  -- 对局时长（秒）
        CONSTRAINT FK_GomokuMatchLog_Player1 FOREIGN KEY(Player1Username) REFERENCES wt.[User](Username),
        CONSTRAINT FK_GomokuMatchLog_Player2 FOREIGN KEY(Player2Username) REFERENCES wt.[User](Username)
    );
    
    -- 创建索引以提高查询性能
    CREATE INDEX IX_GomokuMatchLog_Player1 ON wt.GomokuMatchLog(Player1Username);
    CREATE INDEX IX_GomokuMatchLog_Player2 ON wt.GomokuMatchLog(Player2Username);
    CREATE INDEX IX_GomokuMatchLog_StartTime ON wt.GomokuMatchLog(StartTime DESC);
    
    PRINT N'成功创建 wt.GomokuMatchLog 表';
END
ELSE
BEGIN
    PRINT N'wt.GomokuMatchLog 表已存在';
END
GO
