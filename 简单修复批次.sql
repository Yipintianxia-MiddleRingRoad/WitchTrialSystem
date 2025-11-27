-- =======================================================================
-- 简单直接修复批次脚本
-- 直接修改现有数据，避免复杂重建
-- =======================================================================

USE WitchTrialWT;
GO

PRINT '=== 开始简单修复批次 ===';

-- 1. 查看当前状态
PRINT '';
PRINT '=== 1. 当前状态 ===';

SELECT 
    '当前批次' AS 状态,
    b.BatchID,
    b.IslandID,
    i.Name AS 岛屿名,
    b.WitchCount,
    COUNT(w.WitchID) AS 魔女数
FROM wt.Batch b
LEFT JOIN wt.Island i ON i.IslandID = b.IslandID
LEFT JOIN wt.Witch w ON w.IslandID = b.IslandID AND w.BatchID = b.BatchID
GROUP BY b.BatchID, b.IslandID, i.Name, b.WitchCount
ORDER BY b.IslandID, b.BatchID;

-- 2. 找出岛屿2对应的正确批次ID
PRINT '';
PRINT '=== 2. 确定岛屿2的批次ID ===';

DECLARE @island2Batch1 INT, @island2Batch2 INT;

-- 找到岛屿2的两个批次，编号较小的作为批次1
SELECT @island2Batch1 = MIN(BatchID), @island2Batch2 = MAX(BatchID)
FROM wt.Batch 
WHERE IslandID = 2;

PRINT '岛屿2批次ID - 批次1:' + ISNULL(CAST(@island2Batch1 AS NVARCHAR), 'NULL') + ', 批次2:' + ISNULL(CAST(@island2Batch2 AS NVARCHAR), 'NULL');

-- 3. 将684-696全部分配到岛屿2的第一个批次
PRINT '';
PRINT '=== 3. 分配魔女到正确批次 ===';

UPDATE wt.Witch
SET BatchID = @island2Batch1
WHERE PrisonerNo BETWEEN '684' AND '696';

DECLARE @witchUpdateCount INT = @@ROWCOUNT;
PRINT '魔女批次修正数量: ' + CAST(@witchUpdateCount AS NVARCHAR);

-- 4. 修正用户批次
PRINT '';
PRINT '=== 4. 修正用户批次 ===';

UPDATE wt.[User]
SET BatchID = @island2Batch1
WHERE Username BETWEEN '684' AND '696';

DECLARE @userUpdateCount INT = @@ROWCOUNT;
PRINT '用户批次修正数量: ' + CAST(@userUpdateCount AS NVARCHAR);

-- 5. 更新批次计数
PRINT '';
PRINT '=== 5. 更新批次计数 ===';

UPDATE wt.Batch
SET WitchCount = (
    SELECT COUNT(*) FROM wt.Witch 
    WHERE IslandID = 2 AND BatchID = wt.Batch.BatchID
)
WHERE IslandID = 2;

-- 6. 验证结果
PRINT '';
PRINT '=== 6. 验证结果 ===';

SELECT 
    '修复后状态' AS 状态,
    b.BatchID,
    b.IslandID,
    i.Name AS 岛屿名,
    b.WitchCount,
    COUNT(w.WitchID) AS 魔女数
FROM wt.Batch b
LEFT JOIN wt.Island i ON i.IslandID = b.IslandID
LEFT JOIN wt.Witch w ON w.IslandID = b.IslandID AND w.BatchID = b.BatchID
GROUP BY b.BatchID, b.IslandID, i.Name, b.WitchCount
ORDER BY b.IslandID, b.BatchID;

-- 显示684-696的详细信息
PRINT '';
SELECT 
    '684-696详情' AS 状态,
    w.PrisonerNo,
    w.Name,
    w.IslandID,
    w.BatchID,
    u.Username AS 用户名
FROM wt.Witch w
LEFT JOIN wt.[User] u ON u.Username = w.PrisonerNo
WHERE w.PrisonerNo BETWEEN '684' AND '696'
ORDER BY w.PrisonerNo;

PRINT '';
PRINT '=== 修复完成 ===';
PRINT '请检查批次显示是否正确';
GO