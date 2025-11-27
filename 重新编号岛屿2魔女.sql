-- 重新编号岛屿2魔女的WitchID（从109-121改为29-41）
-- 执行前请先备份数据库！

BEGIN TRANSACTION;

-- 创建临时表存储映射关系
CREATE TABLE #WitchIDMapping (
    OldWitchID INT,
    NewWitchID INT,
    PrisonerNo VARCHAR(50)
);

-- 插入映射关系（109-121 -> 29-41）
INSERT INTO #WitchIDMapping (OldWitchID, NewWitchID, PrisonerNo)
VALUES 
(109, 29, '684'),
(110, 30, '685'),
(111, 31, '686'),
(112, 32, '687'),
(113, 33, '688'),
(114, 34, '689'),
(115, 35, '690'),
(116, 36, '691'),
(117, 37, '692'),
(118, 38, '693'),
(119, 39, '694'),
(120, 40, '695'),
(121, 41, '696');

-- 显示映射关系
SELECT '映射关系：' AS Info;
SELECT * FROM #WitchIDMapping ORDER BY OldWitchID;

-- 更新UserWitch表
UPDATE uw
SET uw.WitchID = m.NewWitchID
FROM wt.UserWitch uw
INNER JOIN #WitchIDMapping m ON uw.WitchID = m.OldWitchID;

-- 更新Witch表
UPDATE w
SET w.WitchID = m.NewWitchID
FROM wt.Witch w
INNER JOIN #WitchIDMapping m ON w.WitchID = m.OldWitchID;

-- 验证结果
SELECT '更新后的岛屿2魔女：' AS Info;
SELECT WitchID, PrisonerNo, Name, IslandID, BatchID 
FROM wt.Witch 
WHERE IslandID = 2 
ORDER BY WitchID;

SELECT '更新后的UserWitch关联：' AS Info;
SELECT uw.UserID, u.Username, uw.WitchID, w.PrisonerNo, w.Name
FROM wt.UserWitch uw
INNER JOIN wt.[User] u ON uw.UserID = u.UserID
INNER JOIN wt.Witch w ON uw.WitchID = w.WitchID
WHERE w.IslandID = 2
ORDER BY uw.WitchID;

-- 清理
DROP TABLE #WitchIDMapping;

COMMIT TRANSACTION;

-- 重置自增ID（可选）
-- DBCC CHECKIDENT ('wt.Witch', RESEED, 41);
