-- 删除岛屿2的批次3（魔女684-696）
-- 此脚本将删除重复的批次3魔女记录

USE WitchTrialWT;
GO

BEGIN TRANSACTION;

PRINT '=== 开始删除重复的批次3魔女 ===';

-- 1. 获取岛屿2的ID
DECLARE @island2ID INT = (SELECT TOP 1 IslandID FROM wt.Island WHERE Name = N'魔女岛·贰');
IF @island2ID IS NULL
BEGIN
    RAISERROR(N'错误：找不到岛屿二(魔女岛·贰)的记录', 16, 1);
    ROLLBACK;
    RETURN;
END

-- 2. 获取批次3的ID
DECLARE @batch3ID INT = (SELECT TOP 1 BatchID FROM wt.Batch WHERE IslandID = @island2ID AND BatchID = 3);
IF @batch3ID IS NULL
BEGIN
    RAISERROR(N'错误：找不到岛屿二的批次3记录', 16, 1);
    ROLLBACK;
    RETURN;
END

-- 3. 显示将要删除的魔女信息
PRINT '即将删除以下魔女记录：';
SELECT 
    WitchID AS 魔女ID,
    PrisonerNo AS 囚犯编号,
    Name AS 姓名,
    Magic AS 魔法
FROM wt.Witch 
WHERE BatchID = @batch3ID
ORDER BY PrisonerNo;

-- 4. 删除批次3的魔女记录
DELETE FROM wt.Witch 
WHERE BatchID = @batch3ID;

-- 5. 删除批次3记录
DELETE FROM wt.Batch 
WHERE BatchID = @batch3ID;

-- 6. 更新批次1和2的魔女计数（确保触发器已更新）
UPDATE b
SET WitchCount = (SELECT COUNT(*) FROM wt.Witch WHERE BatchID = b.BatchID)
FROM wt.Batch b
WHERE b.IslandID = @island2ID;

-- 7. 验证删除结果
PRINT '\n删除后的批次信息：';
SELECT 
    i.Name AS 岛屿名称,
    b.BatchID AS 批次ID,
    b.WitchCount AS 魔女数量,
    STRING_AGG(ISNULL(w.PrisonerNo, '无'), ',') AS 囚犯编号
FROM wt.Batch b
JOIN wt.Island i ON b.IslandID = i.IslandID
LEFT JOIN wt.Witch w ON b.BatchID = w.BatchID
WHERE i.IslandID = @island2ID
GROUP BY i.Name, b.BatchID, b.WitchCount
ORDER BY b.BatchID;

-- 8. 检查是否还有批次3的记录
IF EXISTS (SELECT 1 FROM wt.Batch WHERE BatchID = 3)
    PRINT '\n警告：批次3仍然存在，请检查数据库完整性！';
ELSE
    PRINT '\n成功：批次3已完全删除';

COMMIT TRANSACTION;

PRINT '\n=== 操作完成 ===';

-- 显示最终的所有批次信息
SELECT 
    i.IslandID AS 岛屿ID,
    i.Name AS 岛屿名称,
    b.BatchID AS 批次ID,
    b.WitchCount AS 魔女数量,
    (SELECT STRING_AGG(PrisonerNo, ',') 
     FROM wt.Witch w 
     WHERE w.BatchID = b.BatchID) AS 囚犯编号
FROM wt.Batch b
JOIN wt.Island i ON b.IslandID = i.IslandID
ORDER BY i.IslandID, b.BatchID;


----------------------------------------------------------------更新后
-- 删除岛屿2的批次3（魔女684-696）
-- 此脚本将删除重复的批次3魔女记录

USE WitchTrialWT;
GO

BEGIN TRANSACTION;

PRINT '=== 开始删除重复的批次3魔女 ===';

-- 1. 获取岛屿2的ID
DECLARE @island2ID INT = (SELECT TOP 1 IslandID FROM wt.Island WHERE Name = N'魔女岛·贰');
IF @island2ID IS NULL
BEGIN
    RAISERROR(N'错误：找不到岛屿二(魔女岛·贰)的记录', 16, 1);
    ROLLBACK;
    RETURN;
END

-- 2. 获取批次3的ID
DECLARE @batch3ID INT = (SELECT TOP 1 BatchID FROM wt.Batch WHERE IslandID = @island2ID AND BatchID = 3);
IF @batch3ID IS NULL
BEGIN
    RAISERROR(N'错误：找不到岛屿二的批次3记录', 16, 1);
    ROLLBACK;
    RETURN;
END

-- 3. 显示将要删除的魔女信息
PRINT '即将删除以下魔女记录：';
SELECT 
    w.WitchID AS 魔女ID,
    w.PrisonerNo AS 囚犯编号,
    w.Name AS 姓名,
    w.Magic AS 魔法
FROM wt.Witch w
WHERE w.BatchID = @batch3ID
ORDER BY w.PrisonerNo;

-- 4. 先删除UserWitch表中的关联记录
DELETE FROM wt.UserWitch
WHERE WitchID IN (SELECT WitchID FROM wt.Witch WHERE BatchID = @batch3ID);

PRINT '已删除UserWitch表中的关联记录';

-- 5. 删除批次3的魔女记录
DELETE FROM wt.Witch 
WHERE BatchID = @batch3ID;

-- 6. 删除批次3记录
DELETE FROM wt.Batch 
WHERE BatchID = @batch3ID;

-- 7. 更新批次1和2的魔女计数
UPDATE b
SET WitchCount = (SELECT COUNT(*) FROM wt.Witch WHERE BatchID = b.BatchID)
FROM wt.Batch b
WHERE b.IslandID = @island2ID;

-- 8. 验证删除结果
PRINT '删除后的批次信息：';
SELECT 
    i.Name AS 岛屿名称,
    b.BatchID AS 批次ID,
    b.WitchCount AS 魔女数量,
    STRING_AGG(ISNULL(w.PrisonerNo, '无'), ',') AS 囚犯编号
FROM wt.Batch b
JOIN wt.Island i ON b.IslandID = i.IslandID
LEFT JOIN wt.Witch w ON b.BatchID = w.BatchID
WHERE i.IslandID = @island2ID
GROUP BY i.Name, b.BatchID, b.WitchCount
ORDER BY b.BatchID;

-- 9. 检查是否还有批次3的记录
IF EXISTS (SELECT 1 FROM wt.Batch WHERE BatchID = 3)
    PRINT '警告：批次3仍然存在，请检查数据库完整性！';
ELSE
    PRINT '成功：批次3已完全删除';

COMMIT TRANSACTION;

PRINT '=== 操作完成 ===';

-- 显示最终的所有批次信息
SELECT 
    i.IslandID AS 岛屿ID,
    i.Name AS 岛屿名称,
    b.BatchID AS 批次ID,
    b.WitchCount AS 魔女数量,
    (SELECT STRING_AGG(PrisonerNo, ',') 
     FROM wt.Witch w 
     WHERE w.BatchID = b.BatchID) AS 囚犯编号
FROM wt.Batch b
JOIN wt.Island i ON b.IslandID = i.IslandID
ORDER BY i.IslandID, b.BatchID;