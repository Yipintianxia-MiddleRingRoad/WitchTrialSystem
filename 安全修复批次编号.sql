-- =======================================================================
-- 安全修复批次编号脚本
-- 清理外键约束冲突，正确修正批次编号
-- =======================================================================

USE WitchTrialWT;
GO

PRINT '=== 开始安全修复批次编号 ===';

-- 1. 显示当前状态
PRINT '';
PRINT '=== 1. 当前批次状态 ===';

SELECT 
    b.BatchID,
    b.IslandID,
    i.Name AS IslandName,
    b.WitchCount AS 设置数量,
    COUNT(w.WitchID) AS 实际数量
FROM wt.Batch b
LEFT JOIN wt.Island i ON i.IslandID = b.IslandID
LEFT JOIN wt.Witch w ON w.IslandID = b.IslandID AND w.BatchID = b.BatchID
GROUP BY b.BatchID, b.IslandID, i.Name, b.WitchCount
ORDER BY b.IslandID, b.BatchID;

-- 2. 修正现有错误批次的编号
PRINT '';
PRINT '=== 2. 修正错误批次编号 ===';

-- 将批次3->1, 4->2, 5和6如果存在则删除
BEGIN TRANSACTION;

-- 更新批次3为批次1
UPDATE wt.Witch 
SET BatchID = 1
WHERE IslandID = 2 AND BatchID = 3;

UPDATE wt.[User] 
SET BatchID = 1
WHERE IslandID = 2 AND BatchID = 3;

UPDATE wt.Batch 
SET BatchID = 1, WitchCount = 13
WHERE IslandID = 2 AND BatchID = 3;

-- 更新批次4为批次2
UPDATE wt.Witch 
SET BatchID = 2
WHERE IslandID = 2 AND BatchID = 4;

UPDATE wt.[User] 
SET BatchID = 2
WHERE IslandID = 2 AND BatchID = 4;

UPDATE wt.Batch 
SET BatchID = 2, WitchCount = 0
WHERE IslandID = 2 AND BatchID = 4;

-- 删除多余的批次5和6（如果存在）
DELETE FROM wt.UserWitch 
WHERE UserID IN (
    SELECT u.UserID FROM wt.[User] u
    WHERE u.IslandID = 2 AND u.BatchID IN (5,6)
);

DELETE FROM wt.[User] 
WHERE IslandID = 2 AND BatchID IN (5,6);

DELETE FROM wt.Witch 
WHERE IslandID = 2 AND BatchID IN (5,6);

DELETE FROM wt.Batch 
WHERE IslandID = 2 AND BatchID IN (5,6);

COMMIT TRANSACTION;
PRINT '批次编号修正完成';

-- 3. 确保岛屿2有正确的批次
PRINT '';
PRINT '=== 3. 确保批次存在 ===';

IF NOT EXISTS (SELECT 1 FROM wt.Batch WHERE IslandID = 2 AND BatchID = 1)
BEGIN
    INSERT INTO wt.Batch (IslandID, WitchCount)
    VALUES (2, 13);
    PRINT '创建岛屿2批次1';
END

IF NOT EXISTS (SELECT 1 FROM wt.Batch WHERE IslandID = 2 AND BatchID = 2)
BEGIN
    INSERT INTO wt.Batch (IslandID, WitchCount)
    VALUES (2, 0);
    PRINT '创建岛屿2批次2';
END

-- 4. 修正684-696用户的批次为1
PRINT '';
PRINT '=== 4. 修正用户批次信息 ===';

UPDATE wt.[User]
SET BatchID = 1
WHERE Username IN ('684','685','686','687','688','689','690','691','692','693','694','695','696')
AND IslandID = 2;

PRINT '用户批次修正完成';

-- 5. 重建UserWitch关联
PRINT '';
PRINT '=== 5. 重建用户-魔女关联 ===';

-- 删除可能的错误关联
DELETE FROM wt.UserWitch
WHERE UserID IN (
    SELECT u.UserID FROM wt.[User] u
    WHERE u.Username IN ('684','685','686','687','688','689','690','691','692','693','694','695','696')
);

-- 重新建立正确的关联
INSERT INTO wt.UserWitch (UserID, WitchID)
SELECT u.UserID, w.WitchID
FROM wt.[User] u
JOIN wt.Witch w ON w.PrisonerNo = u.Username
WHERE u.Username IN ('684','685','686','687','688','689','690','691','692','693','694','695','696');

PRINT '用户-魔女关联重建完成';

-- 6. 验证修复结果
PRINT '';
PRINT '=== 6. 最终验证 ===';

-- 显示修正后的批次状态
SELECT 
    '修正后批次状态' AS 状态,
    b.BatchID,
    b.IslandID,
    i.Name AS IslandName,
    b.WitchCount AS 设置数量,
    COUNT(w.WitchID) AS 实际数量
FROM wt.Batch b
LEFT JOIN wt.Island i ON i.IslandID = b.IslandID
LEFT JOIN wt.Witch w ON w.IslandID = b.IslandID AND w.BatchID = b.BatchID
GROUP BY b.BatchID, b.IslandID, i.Name, b.WitchCount
ORDER BY b.IslandID, b.BatchID;

-- 显示岛屿2魔女分布
PRINT '';
SELECT 
    '岛屿2魔女最终分布' AS 状态,
    w.BatchID,
    COUNT(*) AS 魔女数量,
    STUFF((
        SELECT ', ' + w2.PrisonerNo 
        FROM wt.Witch w2 
        WHERE w2.IslandID = w.IslandID AND w2.BatchID = w.BatchID 
        ORDER BY w2.PrisonerNo
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 囚犯编号列表
FROM wt.Witch w
WHERE w.IslandID = 2
GROUP BY w.IslandID, w.BatchID
ORDER BY w.BatchID;

-- 显示684-696的详细信息
PRINT '';
SELECT 
    '684-696详细信息' AS 状态,
    w.PrisonerNo,
    w.Name,
    w.IslandID,
    w.BatchID,
    u.Username AS 用户名,
    CASE 
        WHEN uw.WitchID IS NOT NULL THEN '已关联'
        ELSE '未关联'
    END AS UserWitch状态
FROM wt.Witch w
LEFT JOIN wt.[User] u ON u.Username = w.PrisonerNo
LEFT JOIN wt.UserWitch uw ON uw.UserID = u.UserID AND uw.WitchID = w.WitchID
WHERE w.PrisonerNo BETWEEN '684' AND '696'
ORDER BY w.PrisonerNo;

PRINT '';
PRINT '=== 修复完成 ===';
PRINT '现在岛屿2应该显示批次1和批次2了';
PRINT '魔女684-696应该正确显示为岛屿2批次1';
PRINT '请重启程序验证效果';
GO