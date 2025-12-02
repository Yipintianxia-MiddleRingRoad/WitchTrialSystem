-- 为User表添加AvatarPath列
USE WitchTrialWT;
GO

PRINT '=== 为User表添加AvatarPath列 ===';

-- 检查并添加AvatarPath列
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('wt.[User]') 
    AND name = 'AvatarPath'
)
BEGIN
    PRINT '正在添加AvatarPath列到User表...';
    ALTER TABLE wt.[User] 
    ADD AvatarPath NVARCHAR(255) NULL;
    PRINT '✓ 已添加AvatarPath列';
END
ELSE
BEGIN
    PRINT '✓ AvatarPath列已存在';
END;

PRINT '=== 完成 ===';
