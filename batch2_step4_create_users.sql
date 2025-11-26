-- ========================================
-- 批次2 - 步骤4：创建用户账号
-- 账号名：囚犯编号（671-683）
-- 密码：123456（统一）
-- ========================================

USE WitchTrialWT;
GO

PRINT '========================================';
PRINT '批次2 - 步骤4：创建用户账号';
PRINT '========================================';
GO

DECLARE @islandId INT = 1;
DECLARE @batchId INT = 2;
DECLARE @roleWitch INT = (SELECT RoleID FROM wt.Role WHERE Name = N'Witch');

-- 检查是否已存在批次2的用户
IF NOT EXISTS (SELECT 1 FROM wt.[User] WHERE Username = '671')
BEGIN
    -- 批量创建13个用户账号
    INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore)
    VALUES
        ('671', N'PENDING', N'PENDING', @roleWitch, @islandId, @batchId, 0),
        ('672', N'PENDING', N'PENDING', @roleWitch, @islandId, @batchId, 0),
        ('673', N'PENDING', N'PENDING', @roleWitch, @islandId, @batchId, 0),
        ('674', N'PENDING', N'PENDING', @roleWitch, @islandId, @batchId, 0),
        ('675', N'PENDING', N'PENDING', @roleWitch, @islandId, @batchId, 0),
        ('676', N'PENDING', N'PENDING', @roleWitch, @islandId, @batchId, 0),
        ('677', N'PENDING', N'PENDING', @roleWitch, @islandId, @batchId, 0),
        ('678', N'PENDING', N'PENDING', @roleWitch, @islandId, @batchId, 0),
        ('679', N'PENDING', N'PENDING', @roleWitch, @islandId, @batchId, 0),
        ('680', N'PENDING', N'PENDING', @roleWitch, @islandId, @batchId, 0),
        ('681', N'PENDING', N'PENDING', @roleWitch, @islandId, @batchId, 0),
        ('682', N'PENDING', N'PENDING', @roleWitch, @islandId, @batchId, 0),
        ('683', N'PENDING', N'PENDING', @roleWitch, @islandId, @batchId, 0);
    
    PRINT '✅ 13个用户账号创建完成';
    PRINT '   账号名：671-683（囚犯编号）';
    PRINT '   密码：PENDING（待设置为123456）';
END
ELSE
BEGIN
    PRINT '⚠️  批次2用户账号已存在';
END
GO

-- 创建用户-魔女关联
IF NOT EXISTS (SELECT 1 FROM wt.UserWitch uw JOIN wt.[User] u ON uw.UserID = u.UserID WHERE u.Username = '671')
BEGIN
    INSERT INTO wt.UserWitch (UserID, WitchID)
    SELECT u.UserID, w.WitchID
    FROM wt.[User] u
    JOIN wt.Witch w ON u.Username = w.PrisonerNo
    WHERE u.BatchID = 2 AND w.BatchID = 2;
    
    PRINT '✅ 用户-魔女关联创建完成';
END
ELSE
BEGIN
    PRINT '⚠️  用户-魔女关联已存在';
END
GO

-- 验证用户创建结果
SELECT 
    u.UserID,
    u.Username,
    u.RoleID,
    r.Name AS RoleName,
    u.IslandID,
    u.BatchID,
    w.Name AS WitchName,
    w.PrisonerNo
FROM wt.[User] u
JOIN wt.Role r ON u.RoleID = r.RoleID
LEFT JOIN wt.UserWitch uw ON u.UserID = uw.UserID
LEFT JOIN wt.Witch w ON uw.WitchID = w.WitchID
WHERE u.BatchID = 2
ORDER BY u.Username;
GO

PRINT '';
PRINT '========================================';
PRINT '✅ 步骤4完成！';
PRINT '   13个用户账号已创建';
PRINT '   用户-魔女关联已建立';
PRINT '========================================';
GO

PRINT '';
PRINT '⚠️  注意：密码需要通过应用程序设置为 123456';
PRINT '   或者运行密码设置脚本';
GO
