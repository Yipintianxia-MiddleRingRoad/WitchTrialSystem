-- =======================================================================
-- 双岛屿扩展问题修复脚本
-- 1. 清理重复账号
-- 2. 修复密码哈希（密码：123456）
-- 3. 补全魔女信息（IslandID, BatchID等）
-- 版本: 1.0
-- 创建时间: 2025-11-26
-- =======================================================================

USE WitchTrialWT;
GO

PRINT '=== 开始双岛屿扩展问题修复 ===';

-- 正确的密码哈希值（123456 + Yipintianxia_MiddleRingRoad_2025）
DECLARE @salt NVARCHAR(64) = 'Yipintianxia_MiddleRingRoad_2025';
DECLARE @hash NVARCHAR(64) = '0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976';

-- 获取角色ID
DECLARE @roleRegulator INT = (SELECT RoleID FROM wt.Role WHERE Name = 'Meruru');
DECLARE @roleWarden INT = (SELECT RoleID FROM wt.Role WHERE Name = 'Warden');
DECLARE @roleWitch INT = (SELECT RoleID FROM wt.Role WHERE Name = 'Witch');

PRINT N'角色ID查询结果:';
PRINT N'- Meruru(监管员): ' + ISNULL(CAST(@roleRegulator AS NVARCHAR), 'NULL');
PRINT N'- Warden(典狱长): ' + ISNULL(CAST(@roleWarden AS NVARCHAR), 'NULL');
PRINT N'- Witch(魔女): ' + ISNULL(CAST(@roleWitch AS NVARCHAR), 'NULL');

-- 1. 清理重复的监管员和典狱长账号
PRINT '';
PRINT '=== 1. 清理重复账号 ===';

-- 删除重复的监管员（保留最小的UserID）
WITH RegulatorDuplicates AS (
    SELECT Username, ROW_NUMBER() OVER (PARTITION BY Username ORDER BY UserID) AS RowNum
    FROM wt.[User] u
    JOIN wt.Role r ON r.RoleID = u.RoleID
    WHERE r.Name = 'Meruru' AND Username IN ('utena_regulator')
)
DELETE FROM wt.[User]
WHERE Username IN (SELECT Username FROM RegulatorDuplicates WHERE RowNum > 1);

-- 删除重复的典狱长（保留最小的UserID）
WITH WardenDuplicates AS (
    SELECT Username, ROW_NUMBER() OVER (PARTITION BY Username ORDER BY UserID) AS RowNum
    FROM wt.[User] u
    JOIN wt.Role r ON r.RoleID = u.RoleID
    WHERE r.Name = 'Warden' AND Username IN ('warden2')
)
DELETE FROM wt.[User]
WHERE Username IN (SELECT Username FROM WardenDuplicates WHERE RowNum > 1);

-- 删除重复的魔女账号
WITH WitchDuplicates AS (
    SELECT Username, ROW_NUMBER() OVER (PARTITION BY Username ORDER BY UserID) AS RowNum
    FROM wt.[User] 
    WHERE Username IN ('684','685','686','687','688','689','690','691','692','693','694','695','696')
)
DELETE FROM wt.[User]
WHERE Username IN (SELECT Username FROM WitchDuplicates WHERE RowNum > 1);

PRINT '重复账号清理完成';

-- 2. 确保必要的账号存在并修复密码
PRINT '';
PRINT '=== 2. 确保账号存在并修复密码 ===';

-- 确保监管员账号存在
IF NOT EXISTS (SELECT 1 FROM wt.[User] WHERE Username = 'utena_regulator' AND RoleID = @roleRegulator)
BEGIN
    INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, GomokuScore)
    VALUES ('utena_regulator', @hash, @salt, @roleRegulator, 0);
    PRINT N'创建监管员账号: utena_regulator';
END

-- 确保典狱长账号存在
IF NOT EXISTS (SELECT 1 FROM wt.[User] WHERE Username = 'warden2' AND RoleID = @roleWarden)
BEGIN
    INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, GomokuScore)
    VALUES ('warden2', @hash, @salt, @roleWarden, 0);
    PRINT N'创建典狱长账号: warden2';
END

-- 确保魔女账号存在（684-696）
DECLARE @i INT = 684;
WHILE @i <= 696
BEGIN
    DECLARE @username NVARCHAR(50) = CAST(@i AS NVARCHAR(50));
    
    IF NOT EXISTS (SELECT 1 FROM wt.[User] WHERE Username = @username AND RoleID = @roleWitch)
    BEGIN
        INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore)
        VALUES (@username, @hash, @salt, @roleWitch, 2, 1, 0);
        PRINT N'创建魔女账号: ' + @username + N' (岛屿2批次1)';
    END
    
    SET @i = @i + 1;
END

-- 3. 修复密码哈希
PRINT '';
PRINT '=== 3. 修复密码哈希 ===';

-- 批量更新所有新建账号的密码
UPDATE wt.[User] 
SET Salt = @salt, PasswordHash = @hash
WHERE Username IN ('utena_regulator', 'warden2') 
  AND RoleID IN (@roleRegulator, @roleWarden)
  AND (Salt = 'PENDING' OR PasswordHash = 'PENDING' OR Salt != @salt);

UPDATE wt.[User] 
SET Salt = @salt, PasswordHash = @hash
WHERE Username IN ('684','685','686','687','688','689','690','691','692','693','694','695','696')
  AND RoleID = @roleWitch
  AND (Salt = 'PENDING' OR PasswordHash = 'PENDING' OR Salt != @salt);

DECLARE @passwordUpdateCount INT = @@ROWCOUNT;
PRINT N'密码修复完成，更新账号数量: ' + CAST(@passwordUpdateCount AS NVARCHAR);

-- 4. 补全IslandID和BatchID信息
PRINT '';
PRINT '=== 4. 补全IslandID和BatchID信息 ===';

-- 为监管员和典狱长设置IslandID=2（岛屿2）
UPDATE wt.[User] 
SET IslandID = 2, BatchID = NULL
WHERE Username IN ('utena_regulator', 'warden2') 
  AND RoleID IN (@roleRegulator, @roleWarden);

-- 为魔女设置IslandID=2, BatchID=1（岛屿2批次1）
UPDATE wt.[User] 
SET IslandID = 2, BatchID = 1
WHERE Username IN ('684','685','686','687','688','689','690','691','692','693','694','695','696')
  AND RoleID = @roleWitch;

DECLARE @infoUpdateCount INT = @@ROWCOUNT;
PRINT N'信息补全完成，更新账号数量: ' + CAST(@infoUpdateCount AS NVARCHAR);

-- 5. 确保权限表记录存在
PRINT '';
PRINT '=== 5. 确保权限表记录存在 ===';

-- 确保监管员在IslandRegulator表中
DECLARE @regulatorUserId INT = (SELECT UserID FROM wt.[User] WHERE Username = 'utena_regulator');
IF @regulatorUserId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM wt.IslandRegulator WHERE UserID = @regulatorUserId AND IslandID = 2)
    BEGIN
        INSERT INTO wt.IslandRegulator (UserID, IslandID)
        VALUES (@regulatorUserId, 2);
        PRINT N'创建监管员权限记录: utena_regulator -> 岛屿2';
    END
END

-- 确保典狱长在IslandWarden表中
DECLARE @wardenUserId INT = (SELECT UserID FROM wt.[User] WHERE Username = 'warden2');
IF @wardenUserId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM wt.IslandWarden WHERE UserID = @wardenUserId AND IslandID = 2)
    BEGIN
        INSERT INTO wt.IslandWarden (UserID, IslandID, WardenName)
        VALUES (@wardenUserId, 2, '典狱长2');
        PRINT N'创建典狱长权限记录: warden2 -> 岛屿2';
    END
END

-- 6. 验证结果
PRINT '';
PRINT '=== 6. 验证修复结果 ===';

PRINT '';
PRINT N'当前用户状态:';
SELECT 
    u.UserID,
    u.Username,
    r.Name AS RoleName,
    u.IslandID,
    u.BatchID,
    CASE 
        WHEN u.PasswordHash = 'PENDING' THEN '待设置'
        ELSE '已设置'
    END AS PasswordStatus,
    CASE 
        WHEN ir.UserID IS NOT NULL THEN '岛屿' + CAST(ir.IslandID AS NVARCHAR) + '监管员'
        WHEN iw.UserID IS NOT NULL THEN '岛屿' + CAST(iw.IslandID AS NVARCHAR) + '典狱长'
        WHEN r.Name = 'Witch' THEN '岛屿' + ISNULL(CAST(u.IslandID AS NVARCHAR), '?') + '批次' + ISNULL(CAST(u.BatchID AS NVARCHAR), '?') + '魔女'
        ELSE r.Name
    END AS Permission
FROM wt.[User] u
LEFT JOIN wt.Role r ON r.RoleID = u.RoleID
LEFT JOIN wt.IslandRegulator ir ON ir.UserID = u.UserID
LEFT JOIN wt.IslandWarden iw ON iw.UserID = u.UserID
WHERE u.Username IN ('admin', 'meruru_regulator', 'utena_regulator', 'warden2', '684','685','686','687','688','689','690','691','692','693','694','695','696')
ORDER BY u.UserID;

PRINT '';
PRINT '=== 修复完成 ===';
PRINT '';
PRINT N'测试账号信息:';
PRINT N'- 管理员: admin / (原密码)';
PRINT N'- 监管员1: meruru_regulator / (原密码) - 管理岛屿1';
PRINT N'- 监管员2: utena_regulator / 123456 - 管理岛屿2';
PRINT N'- 典狱长2: warden2 / 123456 - 管理岛屿2';
PRINT N'- 魔女: 684-696 / 123456 - 岛屿2批次1';
PRINT '';
PRINT N'权限说明:';
PRINT N'- admin: 可以管理所有岛屿和批次';
PRINT N'- meruru_regulator: 只能管理岛屿1的所有批次';
PRINT N'- utena_regulator: 只能管理岛屿2的所有批次';
PRINT N'- warden2: 只能管理岛屿2，受utena_regulator控制';
PRINT N'- 684-696: 只能看到岛屿2批次1的信息';
PRINT '';
PRINT N'五子棋功能: 所有账号都可以跨岛屿对战';
GO