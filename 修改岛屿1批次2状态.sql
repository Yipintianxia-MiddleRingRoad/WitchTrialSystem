-- 修改岛屿1批次2魔女状态
-- 671-682号状态修改为"分配至岛屿"，683号状态修改为"审判中"
USE WitchTrialWT;
GO

PRINT '=== 开始修改岛屿1批次2魔女状态 ===';

-- 1. 获取岛屿1批次2的信息
DECLARE @island1ID INT = (SELECT TOP 1 IslandID FROM wt.Island WHERE Name = N'魔女岛·壹');
DECLARE @batch2ID INT = (SELECT TOP 1 BatchID FROM wt.Batch WHERE IslandID = @island1ID AND BatchID = 2);

IF @island1ID IS NULL
BEGIN
    RAISERROR(N'错误：找不到岛屿一(魔女岛·壹)的记录', 16, 1);
    RETURN;
END

IF @batch2ID IS NULL
BEGIN
    RAISERROR(N'错误：找不到岛屿一的批次2记录', 16, 1);
    RETURN;
END

PRINT N'岛屿1 ID: ' + CAST(@island1ID AS NVARCHAR);
PRINT N'批次2 ID: ' + CAST(@batch2ID AS NVARCHAR);

-- 2. 显示当前状态
PRINT '';
PRINT '=== 修改前的状态 ===';
SELECT 
    PrisonerNo AS 囚犯编号,
    Name AS 姓名,
    [Status] AS 当前状态
FROM wt.Witch 
WHERE IslandID = @island1ID 
  AND BatchID = @batch2ID
  AND PrisonerNo BETWEEN '671' AND '683'
ORDER BY PrisonerNo;

-- 3. 批量更新671-682号状态为"分配至岛屿"
UPDATE wt.Witch
SET [Status] = N'分配至岛屿'
WHERE IslandID = @island1ID 
  AND BatchID = @batch2ID
  AND PrisonerNo BETWEEN '671' AND '682';

DECLARE @updated1 INT = @@ROWCOUNT;
PRINT N'';
PRINT N'✓ 已更新671-682号魔女状态为"分配至岛屿"，影响行数: ' + CAST(@updated1 AS NVARCHAR);

-- 4. 更新683号状态为"审判中"
UPDATE wt.Witch
SET [Status] = N'审判中'
WHERE IslandID = @island1ID 
  AND BatchID = @batch2ID
  AND PrisonerNo = '683';

DECLARE @updated2 INT = @@ROWCOUNT;
PRINT N'✓ 已更新683号魔女状态为"审判中"，影响行数: ' + CAST(@updated2 AS NVARCHAR);

-- 5. 显示修改后的状态
PRINT '';
PRINT '=== 修改后的状态 ===';
SELECT 
    PrisonerNo AS 囚犯编号,
    Name AS 姓名,
    [Status] AS 新状态
FROM wt.Witch 
WHERE IslandID = @island1ID 
  AND BatchID = @batch2ID
  AND PrisonerNo BETWEEN '671' AND '683'
ORDER BY PrisonerNo;

-- 6. 统计各状态的魔女数量
PRINT '';
PRINT '=== 状态统计 ===';
SELECT 
    [Status] AS 状态,
    COUNT(*) AS 数量,
    STRING_AGG(PrisonerNo, ',') WITHIN GROUP (ORDER BY PrisonerNo) AS 囚犯编号列表
FROM wt.Witch 
WHERE IslandID = @island1ID 
  AND BatchID = @batch2ID
  AND PrisonerNo BETWEEN '671' AND '683'
GROUP BY [Status]
ORDER BY [Status];

PRINT '';
PRINT '=== 操作完成 ===';
PRINT N'✓ 岛屿1批次2状态修改完成';
PRINT N'  - 671-682号: 分配至岛屿';
PRINT N'  - 683号: 审判中';
