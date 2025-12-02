-- 为 User 表添加五子棋积分字段
USE WitchTrialWT;

-- 检查字段是否已存在，不存在则添加
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'wt.[User]') 
    AND name = 'GomokuScore'
)
BEGIN
    ALTER TABLE wt.[User]
    ADD GomokuScore INT NOT NULL DEFAULT 0;
    
    PRINT '成功添加 GomokuScore 字段到 wt.[User] 表';
END
ELSE
BEGIN
    PRINT 'GomokuScore 字段已存在';
END
