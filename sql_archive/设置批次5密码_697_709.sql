-- WitchTrialSystem 批次5魔女密码设置脚本
-- 为批次5（697-709）的13位魔女设置密码：123456
-- 执行前请确保已完成批次5魔女导入

USE WitchTrialWT;
GO

PRINT '=== 开始设置批次5魔女密码 ===';

-- 1. 获取角色ID
DECLARE @roleWitch INT = (SELECT RoleID FROM wt.Role WHERE Name = N'Witch');

-- 2. 设置密码参数（默认密码：123456）
DECLARE @password NVARCHAR(100) = N'123456';
DECLARE @salt NVARCHAR(64) = N'Yipintianxia_MiddleRingRoad_2025';
DECLARE @hash NVARCHAR(64) = N'0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976';

-- 3. 批量更新批次5魔女密码
DECLARE @UpdatedCount INT = 0;

-- 使用循环逐个更新，确保只更新批次5的魔女
DECLARE @CurrentUsername NVARCHAR(50);
DECLARE @i INT = 697;

WHILE @i <= 709
BEGIN
    SET @CurrentUsername = CAST(@i AS NVARCHAR(50));
    
    UPDATE wt.[User]
    SET Salt = @salt, PasswordHash = @hash
    WHERE Username = @CurrentUsername 
      AND RoleID = @roleWitch 
      AND IslandID = 2
      AND BatchID = 5
      AND (Salt = 'PENDING' OR PasswordHash = 'PENDING');
    
    SET @UpdatedCount = @UpdatedCount + @@ROWCOUNT;
    
    PRINT N'更新密码: ' + @CurrentUsername + N' -> 123456';
    
    SET @i = @i + 1;
END

PRINT N'';
PRINT N'批次5密码设置完成，更新账号数量: ' + CAST(@UpdatedCount AS NVARCHAR);

-- 4. 验证设置结果
SELECT 
    BatchID,
    COUNT(*) AS [账号总数],
    SUM(CASE WHEN PasswordHash = N'PENDING' THEN 1 ELSE 0 END) AS [待设置密码数量],
    SUM(CASE WHEN PasswordHash != N'PENDING' THEN 1 ELSE 0 END) AS [已设置密码数量]
FROM wt.[User] u
WHERE IslandID = 2 AND BatchID = 5
GROUP BY BatchID;

PRINT N'';
PRINT N'=== 批次5密码设置脚本执行完成 ===';
PRINT N'✓ 批次5魔女（697-709）密码已设置为：123456';
PRINT N'✓ 这些账号现在可以正常登录系统';
PRINT N'';
PRINT N'登录账号列表：';
PRINT N'697, 698, 699, 700, 701, 702, 703, 704, 705, 706, 707, 708, 709';
PRINT N'默认密码：123456';
