-- ========================================
-- 最终状态检查 - 清晰版本
-- ========================================

USE WitchTrialWT;
GO

PRINT '';
PRINT '========================================';
PRINT '最终数据状态检查';
PRINT '========================================';
PRINT '';

-- 批次1统计
SELECT 
    '批次1' AS [批次],
    (SELECT COUNT(*) FROM wt.Witch WHERE BatchID = 1) AS [魔女数量],
    (SELECT COUNT(*) FROM wt.[User] WHERE BatchID = 1) AS [用户数量],
    (SELECT COUNT(*) FROM wt.UserWitch uw JOIN wt.[User] u ON uw.UserID = u.UserID WHERE u.BatchID = 1) AS [关联数量];
GO

-- 批次2统计
SELECT 
    '批次2' AS [批次],
    (SELECT COUNT(*) FROM wt.Witch WHERE BatchID = 2) AS [魔女数量],
    (SELECT COUNT(*) FROM wt.[User] WHERE BatchID = 2) AS [用户数量],
    (SELECT COUNT(*) FROM wt.UserWitch uw JOIN wt.[User] u ON uw.UserID = u.UserID WHERE u.BatchID = 2) AS [关联数量];
GO

-- 批次1魔女列表（显示编号范围）
SELECT 
    '批次1' AS [批次],
    MIN(PrisonerNo) AS [最小编号],
    MAX(PrisonerNo) AS [最大编号],
    COUNT(*) AS [数量]
FROM wt.Witch WHERE BatchID = 1;
GO

-- 批次2魔女列表（显示编号范围）
SELECT 
    '批次2' AS [批次],
    MIN(PrisonerNo) AS [最小编号],
    MAX(PrisonerNo) AS [最大编号],
    COUNT(*) AS [数量]
FROM wt.Witch WHERE BatchID = 2;
GO

-- 检查重复
SELECT 
    '重复检查' AS [检查项],
    COUNT(*) AS [重复数量]
FROM (
    SELECT PrisonerNo 
    FROM wt.Witch 
    GROUP BY PrisonerNo 
    HAVING COUNT(*) > 1
) AS duplicates;
GO

-- 批次2用户-魔女关联验证
SELECT 
    COUNT(*) AS [批次2关联数量],
    CASE WHEN COUNT(*) = 13 THEN '✓ 完整' ELSE '✗ 不完整' END AS [状态]
FROM wt.UserWitch uw
JOIN wt.[User] u ON uw.UserID = u.UserID
JOIN wt.Witch w ON uw.WitchID = w.WitchID
WHERE u.BatchID = 2 AND w.BatchID = 2;
GO

PRINT '';
PRINT '========================================';
PRINT '检查完成';
PRINT '========================================';
GO



