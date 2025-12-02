-- 检查重复的魔女数据
USE WitchTrialWT;
GO

PRINT '=== 检查重复的魔女记录 ===';

-- 检查基于PrisonerNo的重复
SELECT 
    PrisonerNo,
    COUNT(*) AS 重复数量,
    MIN(WitchID) AS 最小WitchID,
    STRING_AGG(CAST(WitchID AS NVARCHAR), ',') WITHIN GROUP (ORDER BY WitchID) AS 所有WitchID
FROM wt.Witch 
WHERE PrisonerNo IS NOT NULL
GROUP BY PrisonerNo
HAVING COUNT(*) > 1
ORDER BY PrisonerNo;

-- 检查基于Name的重复
PRINT '';
PRINT '=== 检查重复的魔女名称 ===';

SELECT 
    Name,
    COUNT(*) AS 重复数量,
    MIN(WitchID) AS 最小WitchID,
    STRING_AGG(CAST(WitchID AS NVARCHAR), ',') WITHIN GROUP (ORDER BY WitchID) AS 所有WitchID
FROM wt.Witch 
WHERE Name IS NOT NULL
GROUP BY Name
HAVING COUNT(*) > 1
ORDER BY Name;

-- 检查所有魔女的基本信息
PRINT '';
PRINT '=== 所有魔女基本信息 ===';

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
WHERE w.PrisonerNo >= '684' OR w.PrisonerNo <= '696'
ORDER BY w.PrisonerNo, w.WitchID;

PRINT '';
PRINT '=== 检查图片文件引用情况 ===';

SELECT 
    AvatarPath,
    COUNT(*) AS 使用次数,
    STRING_AGG(w.PrisonerNo, ',') WITHIN GROUP (ORDER BY w.PrisonerNo) AS 使用者
FROM wt.Witch 
WHERE AvatarPath IS NOT NULL AND AvatarPath != ''
GROUP BY AvatarPath
HAVING COUNT(*) > 1
ORDER BY AvatarPath;