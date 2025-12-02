-- 更新管理员和监管员头像路径
USE WitchTrialWT;
GO

PRINT '=== 开始更新管理员和监管员头像 ===';

-- 1. 更新国家层管理员(admin)头像
UPDATE wt.[User] 
SET AvatarPath = 'Images/Jailer.png'
WHERE Username = 'admin'
AND RoleID = (SELECT RoleID FROM wt.Role WHERE Name = 'Admin');

IF @@ROWCOUNT > 0
    PRINT '✓ 已更新admin账号头像为: Images/Jailer.png';
ELSE
    PRINT '⚠ 未找到admin账号或未更新';

-- 2. 更新meruru_regulator头像
UPDATE wt.[User] 
SET AvatarPath = 'Images/meruru_regulator.png'
WHERE Username = 'meruru_regulator';

IF @@ROWCOUNT > 0
    PRINT '✓ 已更新meruru_regulator账号头像为: Images/meruru_regulator.png';
ELSE
    PRINT '⚠ 未找到meruru_regulator账号或未更新';

-- 3. 更新utena_regulator头像
UPDATE wt.[User] 
SET AvatarPath = 'Images/utena_regulator.png'
WHERE Username = 'utena_regulator';

IF @@ROWCOUNT > 0
    PRINT '✓ 已更新utena_regulator账号头像为: Images/utena_regulator.png';
ELSE
    PRINT '⚠ 未找到utena_regulator账号或未更新';

-- 验证更新结果
PRINT '';
PRINT '=== 头像更新结果验证 ===';

SELECT 
    u.UserID,
    u.Username,
    r.Name AS RoleName,
    u.AvatarPath
FROM 
    wt.[User] u
JOIN 
    wt.Role r ON u.RoleID = r.RoleID
WHERE 
    u.Username IN ('admin', 'meruru_regulator', 'utena_regulator')
ORDER BY 
    u.Username;

PRINT '';
PRINT '=== 操作完成 ===';
PRINT '请确保以下图片文件已放置在 Images/ 目录:';
PRINT '- Jailer.png (admin头像)';
PRINT '- meruru_regulator.png (meruru_regulator头像)';
PRINT '- utena_regulator.png (utena_regulator头像)';
