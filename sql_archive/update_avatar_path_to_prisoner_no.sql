-- ========================================
-- 更新头像路径为囚犯编号命名
-- 从姓名命名改为编号命名
-- ========================================

USE WitchTrialWT;
GO

PRINT '========================================';
PRINT '开始更新头像路径';
PRINT '========================================';
GO

-- 备份当前数据（可选）
IF OBJECT_ID('tempdb..#WitchAvatarBackup') IS NOT NULL
    DROP TABLE #WitchAvatarBackup;

SELECT WitchID, PrisonerNo, Name, AvatarPath
INTO #WitchAvatarBackup
FROM wt.Witch;

PRINT '✅ 已创建临时备份表 #WitchAvatarBackup';
GO

-- 更新为编号命名
UPDATE wt.Witch
SET AvatarPath = 'Images/' + PrisonerNo + '.png'
WHERE PrisonerNo IS NOT NULL;

PRINT '✅ 头像路径已更新为编号命名';
GO

-- 显示更新结果
PRINT '';
PRINT '更新结果：';
SELECT 
    WitchID,
    PrisonerNo,
    Name,
    AvatarPath AS NewAvatarPath
FROM wt.Witch
ORDER BY PrisonerNo;
GO

PRINT '';
PRINT '========================================';
PRINT '✅ 更新完成！';
PRINT '========================================';
GO

-- 验证：检查是否有空路径
DECLARE @nullCount INT = (SELECT COUNT(*) FROM wt.Witch WHERE AvatarPath IS NULL);
IF @nullCount > 0
BEGIN
    PRINT '';
    PRINT '⚠️  警告：发现 ' + CAST(@nullCount AS NVARCHAR) + ' 个魔女的头像路径为空';
    SELECT WitchID, PrisonerNo, Name 
    FROM wt.Witch 
    WHERE AvatarPath IS NULL;
END
ELSE
BEGIN
    PRINT '';
    PRINT '✅ 所有魔女都有头像路径';
END
GO
