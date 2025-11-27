-- 为所有管理员和监管员设置头像
USE WitchTrialWT;
GO

PRINT '=== 开始设置管理员和监管员头像 ===';

-- 1. 首先检查并添加AvatarPath列到User表（如果不存在）
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
GO

-- 2. 更新admin头像
UPDATE wt.[User] 
SET AvatarPath = 'Images/Jailer.png'
WHERE Username = 'admin'
AND RoleID = (SELECT RoleID FROM wt.Role WHERE Name = 'Admin');

IF @@ROWCOUNT > 0
    PRINT '✓ 已更新admin账号头像为: Images/Jailer.png';
ELSE
    PRINT '⚠ 未找到admin账号或未更新';

-- 3. 更新meruru_regulator头像
UPDATE wt.[User] 
SET AvatarPath = 'Images/meruru_regulator.png'
WHERE Username = 'meruru_regulator';

IF @@ROWCOUNT > 0
    PRINT '✓ 已更新meruru_regulator账号头像为: Images/meruru_regulator.png';
ELSE
    PRINT '⚠ 未找到meruru_regulator账号或未更新';

-- 4. 更新utena_regulator头像
UPDATE wt.[User] 
SET AvatarPath = 'Images/utena_regulator.png'
WHERE Username = 'utena_regulator';

IF @@ROWCOUNT > 0
    PRINT '✓ 已更新utena_regulator账号头像为: Images/utena_regulator.png';
ELSE
    PRINT '⚠ 未找到utena_regulator账号或未更新';

-- 5. 更新warden头像
UPDATE wt.[User] 
SET AvatarPath = 'Images/warden.jpg'
WHERE Username = 'warden'
AND RoleID = (SELECT RoleID FROM wt.Role WHERE Name = 'Warden');

IF @@ROWCOUNT > 0
    PRINT '✓ 已更新warden账号头像为: Images/warden.jpg';
ELSE
    PRINT '⚠ 未找到warden账号或未更新';

-- 6. 更新warden2头像
UPDATE wt.[User] 
SET AvatarPath = 'Images/warden2.png'
WHERE Username = 'warden2'
AND RoleID = (SELECT RoleID FROM wt.Role WHERE Name = 'Warden');

IF @@ROWCOUNT > 0
    PRINT '✓ 已更新warden2账号头像为: Images/warden2.png';
ELSE
    PRINT '⚠ 未找到warden2账号或未更新';

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
    u.Username IN ('admin', 'meruru_regulator', 'utena_regulator', 'warden', 'warden2')
ORDER BY 
    u.Username;

PRINT '';
PRINT '=== 操作完成 ===';
PRINT '请确保以下图片文件已放置在 Images/ 目录:';
PRINT '- Jailer.png (admin头像)';
PRINT '- meruru_regulator.png (meruru_regulator头像)';
PRINT '- utena_regulator.png (utena_regulator头像)';
PRINT '- warden.jpg (warden头像)';
PRINT '- warden2.png (warden2头像)';
PRINT '';
PRINT '重新编译并运行程序后，这些账号将显示对应的头像。';
