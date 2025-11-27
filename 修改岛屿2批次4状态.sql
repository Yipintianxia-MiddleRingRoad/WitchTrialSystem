-- 修改岛屿2批次4魔女状态
-- 684-685号状态修改为"审判中"
USE WitchTrialWT;
GO

PRINT '=== 开始修改岛屿2批次4魔女状态 ===';

-- 1. 获取岛屿2批次4的信息
DECLARE @island2ID INT = (SELECT TOP 1 IslandID FROM wt.Island WHERE Name = N'魔女岛·贰');
DECLARE @batch4ID INT = (SELECT TOP 1 BatchID FROM wt.Batch WHERE IslandID = @island2ID AND BatchID = 4);

IF @island2ID IS NULL
BEGIN
    RAISERROR(N'错误：找不到岛屿二(魔女岛·贰)的记录', 16, 1);
    RETURN;
END

IF @batch4ID IS NULL
BEGIN
    RAISERROR(N'错误：找不到岛屿二的批次4记录', 16, 1);
    RETURN;
END

PRINT N'岛屿2 ID: ' + CAST(@island2ID AS NVARCHAR);
PRINT N'批次4 ID: ' + CAST(@batch4ID AS NVARCHAR);

-- 2. 显示当前状态
PRINT '';
PRINT '=== 修改前的状态 ===';
SELECT 
    PrisonerNo AS 囚犯编号,
    Name AS 姓名,
    [Status] AS 当前状态
FROM wt.Witch 
WHERE IslandID = @island2ID 
  AND BatchID = @batch4ID
  AND PrisonerNo BETWEEN '684' AND '685'
ORDER BY PrisonerNo;

-- 3. 批量更新684-685号状态为"审判中"
UPDATE wt.Witch
SET [Status] = N'审判中'
WHERE IslandID = @island2ID 
  AND BatchID = @batch4ID
  AND PrisonerNo BETWEEN '684' AND '685';

DECLARE @updatedCount INT = @@ROWCOUNT;
PRINT N'';
PRINT N'✓ 已更新684-685号魔女状态为"审判中"，影响行数: ' + CAST(@updatedCount AS NVARCHAR);

-- 4. 显示修改后的状态
PRINT '';
PRINT '=== 修改后的状态 ===';
SELECT 
    PrisonerNo AS 囚犯编号,
    Name AS 姓名,
    [Status] AS 新状态
FROM wt.Witch 
WHERE IslandID = @island2ID 
  AND BatchID = @batch4ID
  AND PrisonerNo BETWEEN '684' AND '685'
ORDER BY PrisonerNo;

-- 5. 统计批次4中各状态的魔女数量
PRINT '';
PRINT '=== 批次4状态统计 ===';
SELECT 
    [Status] AS 状态,
    COUNT(*) AS 数量,
    STRING_AGG(PrisonerNo, ',') WITHIN GROUP (ORDER BY PrisonerNo) AS 囚犯编号列表
FROM wt.Witch 
WHERE IslandID = @island2ID 
  AND BatchID = @batch4ID
GROUP BY [Status]
ORDER BY 
    CASE [Status] 
        WHEN N'审判中' THEN 1
        WHEN N'分配至岛屿' THEN 2
        ELSE 3
    END;

PRINT '';
PRINT '=== 操作完成 ===';
PRINT N'✓ 岛屿2批次4状态修改完成';
PRINT N'  - 684-685号: 审判中';
