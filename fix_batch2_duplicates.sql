-- ========================================
-- 批次2重复数据清理和修复脚本
-- ========================================

USE WitchTrialWT;
GO

PRINT '';
PRINT '========================================';
PRINT '批次2数据清理和修复';
PRINT '========================================';
PRINT '';

-- ========================================
-- 步骤1：删除批次2的重复魔女数据
-- 保留每个囚犯编号的第一条记录
-- ========================================
PRINT '【步骤1】删除批次2的重复魔女数据...';
GO

-- 使用CTE找出要保留的记录（每个PrisonerNo的第一条）
WITH Duplicates AS (
    SELECT 
        WitchID,
        PrisonerNo,
        ROW_NUMBER() OVER (PARTITION BY PrisonerNo ORDER BY WitchID) AS RowNum
    FROM wt.Witch
    WHERE BatchID = 2
)
-- 删除除了第一条之外的所有重复记录
DELETE FROM wt.Witch
WHERE BatchID = 2
  AND WitchID NOT IN (
      SELECT WitchID 
      FROM Duplicates 
      WHERE RowNum = 1
  );
GO

DECLARE @deletedCount INT = @@ROWCOUNT;
PRINT CONCAT('  已删除 ', @deletedCount, ' 条重复记录');
GO

-- 验证：检查批次2魔女数量（应该是13个）
DECLARE @batch2WitchCount INT = (SELECT COUNT(*) FROM wt.Witch WHERE BatchID = 2);
IF @batch2WitchCount = 13
    PRINT CONCAT('  ✓ 批次2魔女数量正确：', @batch2WitchCount);
ELSE
    PRINT CONCAT('  ✗ 批次2魔女数量异常：', @batch2WitchCount, '（应该是13）');
GO

-- ========================================
-- 步骤2：创建批次2的用户-魔女关联
-- ========================================
PRINT '';
PRINT '【步骤2】创建批次2的用户-魔女关联...';
GO

-- 先删除可能存在的旧关联（如果有）
DELETE FROM wt.UserWitch 
WHERE UserID IN (SELECT UserID FROM wt.[User] WHERE BatchID = 2);
GO

-- 创建新的关联（按Username = PrisonerNo匹配）
INSERT INTO wt.UserWitch (UserID, WitchID)
SELECT 
    u.UserID,
    w.WitchID
FROM wt.[User] u
JOIN wt.Witch w ON u.Username = w.PrisonerNo
WHERE u.BatchID = 2 
  AND w.BatchID = 2
  AND NOT EXISTS (
      SELECT 1 
      FROM wt.UserWitch uw 
      WHERE uw.UserID = u.UserID AND uw.WitchID = w.WitchID
  );
GO

DECLARE @assocCount INT = @@ROWCOUNT;
PRINT CONCAT('  已创建 ', @assocCount, ' 条关联记录');
GO

-- 验证关联
IF @assocCount = 13
    PRINT CONCAT('  ✓ 关联数量正确：', @assocCount);
ELSE
    PRINT CONCAT('  ✗ 关联数量异常：', @assocCount, '（应该是13）');
GO

-- ========================================
-- 步骤3：更新批次表的魔女数量
-- ========================================
PRINT '';
PRINT '【步骤3】更新批次表的魔女数量...';
GO

UPDATE wt.Batch
SET WitchCount = (SELECT COUNT(*) FROM wt.Witch WHERE BatchID = 2)
WHERE BatchID = 2;
GO

PRINT '  ✓ 批次魔女数量已更新';
GO

-- ========================================
-- 最终验证
-- ========================================
PRINT '';
PRINT '========================================';
PRINT '最终验证结果';
PRINT '========================================';
GO

-- 批次2魔女列表
SELECT 
    PrisonerNo AS [囚犯编号],
    Name AS [姓名],
    Magic AS [魔法]
FROM wt.Witch
WHERE BatchID = 2
ORDER BY CAST(PrisonerNo AS INT);
GO

-- 批次2用户-魔女关联
SELECT 
    u.Username AS [用户账号],
    w.PrisonerNo AS [囚犯编号],
    w.Name AS [魔女姓名]
FROM wt.[User] u
JOIN wt.UserWitch uw ON u.UserID = uw.UserID
JOIN wt.Witch w ON uw.WitchID = w.WitchID
WHERE u.BatchID = 2
ORDER BY CAST(u.Username AS INT);
GO

-- 统计信息
DECLARE @finalWitchCount INT = (SELECT COUNT(*) FROM wt.Witch WHERE BatchID = 2);
DECLARE @finalUserCount INT = (SELECT COUNT(*) FROM wt.[User] WHERE BatchID = 2);
DECLARE @finalAssocCount INT = (SELECT COUNT(*) FROM wt.UserWitch uw JOIN wt.[User] u ON uw.UserID = u.UserID WHERE u.BatchID = 2);
DECLARE @finalDuplicateCount INT = (SELECT COUNT(*) FROM (SELECT PrisonerNo FROM wt.Witch WHERE BatchID = 2 GROUP BY PrisonerNo HAVING COUNT(*) > 1) AS dup);

PRINT '';
PRINT '批次2数据统计：';
PRINT CONCAT('  魔女数量：', @finalWitchCount, ' / 13', CASE WHEN @finalWitchCount = 13 THEN ' ✓' ELSE ' ✗' END);
PRINT CONCAT('  用户数量：', @finalUserCount, ' / 13', CASE WHEN @finalUserCount = 13 THEN ' ✓' ELSE ' ✗' END);
PRINT CONCAT('  关联数量：', @finalAssocCount, ' / 13', CASE WHEN @finalAssocCount = 13 THEN ' ✓' ELSE ' ✗' END);
PRINT CONCAT('  重复数量：', @finalDuplicateCount, CASE WHEN @finalDuplicateCount = 0 THEN ' ✓' ELSE ' ✗' END);
PRINT '';
PRINT '========================================';
PRINT '✅ 修复完成！';
PRINT '========================================';
GO



