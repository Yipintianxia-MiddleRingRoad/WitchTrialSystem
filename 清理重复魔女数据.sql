-- =======================================================================
-- 清理重复魔女数据脚本
-- 1. 删除重复的魔女记录（保留最小的WitchID）
-- 2. 修复用户-魔女关联
-- 3. 验证清理结果
-- =======================================================================

USE WitchTrialWT;
GO

PRINT '=== 开始清理重复魔女数据 ===';

-- 1. 查找并删除重复的魔女记录
PRINT '';
PRINT '=== 1. 查找重复记录 ===';

-- 显示即将删除的重复记录
SELECT 
    '重复记录将被删除' AS 状态,
    w.WitchID,
    w.PrisonerNo,
    w.Name,
    w.IslandID,
    w.BatchID,
    w.AvatarPath
FROM wt.Witch w
WHERE w.PrisonerNo IN (
    SELECT PrisonerNo
    FROM wt.Witch 
    WHERE PrisonerNo IS NOT NULL
    GROUP BY PrisonerNo
    HAVING COUNT(*) > 1
)
AND w.WitchID NOT IN (
    SELECT MIN(WitchID)
    FROM wt.Witch w2
    WHERE w2.PrisonerNo = w.PrisonerNo
    GROUP BY w2.PrisonerNo
)
ORDER BY w.PrisonerNo, w.WitchID;

PRINT '';
PRINT '=== 2. 执行删除重复记录 ===';

-- 创建临时表存储要保留的记录
SELECT 
    MIN(WitchID) AS KeepWitchID,
    PrisonerNo
INTO #TempKeepRecords
FROM wt.Witch 
WHERE PrisonerNo IS NOT NULL
GROUP BY PrisonerNo;

-- 删除重复记录（保留最小WitchID的记录）
DELETE FROM wt.Witch
WHERE PrisonerNo IN (SELECT PrisonerNo FROM #TempKeepRecords)
AND WitchID NOT IN (SELECT KeepWitchID FROM #TempKeepRecords);

DECLARE @deletedCount INT = @@ROWCOUNT;
PRINT N'删除重复记录数量: ' + CAST(@deletedCount AS NVARCHAR);

-- 清理临时表
DROP TABLE #TempKeepRecords;

-- 3. 修复用户-魔女关联（如果有问题的话）
PRINT '';
PRINT '=== 3. 检查用户-魔女关联 ===';

-- 检查UserWitch表中的WitchID是否还有效
UPDATE uw
SET WitchID = NULL
FROM wt.UserWitch uw
LEFT JOIN wt.Witch w ON w.WitchID = uw.WitchID
WHERE uw.WitchID IS NOT NULL AND w.WitchID IS NULL;

PRINT '用户-魔女关联修复完成';

-- 4. 验证清理结果
PRINT '';
PRINT '=== 4. 验证清理结果 ===';

-- 检查是否还有重复
SELECT 
    '剩余重复检查' AS 状态,
    PrisonerNo,
    COUNT(*) AS 数量
FROM wt.Witch 
WHERE PrisonerNo IS NOT NULL
GROUP BY PrisonerNo
HAVING COUNT(*) > 1
ORDER BY PrisonerNo;

-- 显示所有魔女的最终状态
SELECT 
    w.WitchID,
    w.PrisonerNo,
    w.Name,
    w.IslandID,
    w.BatchID,
    w.[Status],
    w.AvatarPath,
    u.Username AS 对应用户名,
    u.UserID AS 用户ID
FROM wt.Witch w
LEFT JOIN wt.[User] u ON u.Username = w.PrisonerNo
ORDER BY 
    CASE 
        WHEN w.IslandID = 1 AND w.BatchID = 1 THEN 1
        WHEN w.IslandID = 1 AND w.BatchID = 2 THEN 2  
        WHEN w.IslandID = 2 AND w.BatchID = 1 THEN 3
        WHEN w.IslandID = 2 AND w.BatchID = 2 THEN 4
        ELSE 5
    END,
    w.PrisonerNo;

-- 检查图片路径是否重复
PRINT '';
PRINT '=== 5. 检查图片路径重复情况 ===';

SELECT 
    AvatarPath,
    COUNT(*) AS 使用次数,
    STUFF((
        SELECT ', ' + w2.PrisonerNo 
        FROM wt.Witch w2 
        WHERE w2.AvatarPath = w1.AvatarPath 
        ORDER BY w2.PrisonerNo
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 使用者
FROM wt.Witch w1
WHERE AvatarPath IS NOT NULL AND AvatarPath != ''
GROUP BY AvatarPath
HAVING COUNT(*) > 1
ORDER BY AvatarPath;

PRINT '';
PRINT '=== 清理完成 ===';
PRINT '现在图鉴界面应该不再显示重复的缩略图了';
GO