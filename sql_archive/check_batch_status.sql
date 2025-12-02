-- ========================================
-- 批次数据完整性检查脚本
-- 检查魔女、用户、关联关系
-- ========================================

USE WitchTrialWT;
GO

PRINT '';
PRINT '========================================';
PRINT '批次数据完整性检查';
PRINT '========================================';
PRINT '';

-- ========================================
-- 1. 批次信息总览
-- ========================================
PRINT '【1. 批次信息总览】';
SELECT 
    b.BatchID AS [批次ID],
    i.Name AS [岛屿名称],
    b.WitchCount AS [魔女数量（批次表）],
    b.CreatedDate AS [创建日期]
FROM wt.Batch b
JOIN wt.Island i ON b.IslandID = i.IslandID
ORDER BY b.BatchID;
GO

-- ========================================
-- 2. 各批次魔女数量统计
-- ========================================
PRINT '';
PRINT '【2. 各批次魔女数量统计】';
SELECT 
    BatchID AS [批次ID],
    COUNT(*) AS [实际魔女数量],
    MIN(PrisonerNo) AS [最小编号],
    MAX(PrisonerNo) AS [最大编号],
    STRING_AGG(PrisonerNo, ', ') WITHIN GROUP (ORDER BY PrisonerNo) AS [囚犯编号列表]
FROM wt.Witch
GROUP BY BatchID
ORDER BY BatchID;
GO

-- ========================================
-- 3. 批次1魔女详情
-- ========================================
PRINT '';
PRINT '【3. 批次1魔女详情（应该有13个）】';
SELECT 
    PrisonerNo AS [囚犯编号],
    Name AS [姓名],
    Magic AS [魔法],
    BatchID AS [批次ID],
    CASE WHEN AvatarPath LIKE '%' + PrisonerNo + '%' THEN '✓' ELSE '✗' END AS [头像路径正确]
FROM wt.Witch
WHERE BatchID = 1
ORDER BY CAST(PrisonerNo AS INT);
GO

-- ========================================
-- 4. 批次2魔女详情
-- ========================================
PRINT '';
PRINT '【4. 批次2魔女详情（应该有13个：671-683）】';
SELECT 
    PrisonerNo AS [囚犯编号],
    Name AS [姓名],
    Magic AS [魔法],
    BatchID AS [批次ID],
    CASE WHEN AvatarPath LIKE '%' + PrisonerNo + '%' THEN '✓' ELSE '✗' END AS [头像路径正确]
FROM wt.Witch
WHERE BatchID = 2
ORDER BY CAST(PrisonerNo AS INT);
GO

-- ========================================
-- 5. 检查重复的囚犯编号
-- ========================================
PRINT '';
PRINT '【5. 检查重复的囚犯编号】';
SELECT 
    PrisonerNo AS [囚犯编号],
    COUNT(*) AS [出现次数],
    STRING_AGG(CAST(BatchID AS NVARCHAR), ', ') WITHIN GROUP (ORDER BY BatchID) AS [所在批次],
    STRING_AGG(Name, ', ') WITHIN GROUP (ORDER BY BatchID) AS [姓名列表]
FROM wt.Witch
GROUP BY PrisonerNo
HAVING COUNT(*) > 1;
GO

-- 如果没有重复，显示提示
IF NOT EXISTS (SELECT 1 FROM wt.Witch GROUP BY PrisonerNo HAVING COUNT(*) > 1)
BEGIN
    PRINT '  ✓ 未发现重复的囚犯编号';
END
GO

-- ========================================
-- 6. 各批次用户账号数量统计
-- ========================================
PRINT '';
PRINT '【6. 各批次用户账号数量统计】';
SELECT 
    u.BatchID AS [批次ID],
    COUNT(*) AS [用户账号数量],
    MIN(u.Username) AS [最小账号],
    MAX(u.Username) AS [最大账号],
    SUM(CASE WHEN u.PasswordHash = N'PENDING' THEN 1 ELSE 0 END) AS [待设置密码数量],
    SUM(CASE WHEN u.PasswordHash != N'PENDING' THEN 1 ELSE 0 END) AS [已设置密码数量]
FROM wt.[User] u
GROUP BY u.BatchID
ORDER BY u.BatchID;
GO

-- ========================================
-- 7. 批次1用户账号列表
-- ========================================
PRINT '';
PRINT '【7. 批次1用户账号列表（应该有13个）】';
SELECT 
    Username AS [账号],
    BatchID AS [批次ID],
    CASE WHEN PasswordHash = N'PENDING' THEN '待设置' ELSE '已设置' END AS [密码状态],
    GomokuScore AS [五子棋分数]
FROM wt.[User]
WHERE BatchID = 1
ORDER BY Username;
GO

-- ========================================
-- 8. 批次2用户账号列表
-- ========================================
PRINT '';
PRINT '【8. 批次2用户账号列表（应该有13个：671-683）】';
SELECT 
    Username AS [账号],
    BatchID AS [批次ID],
    CASE WHEN PasswordHash = N'PENDING' THEN '待设置' ELSE '已设置' END AS [密码状态],
    GomokuScore AS [五子棋分数]
FROM wt.[User]
WHERE BatchID = 2
ORDER BY CAST(Username AS INT);
GO

-- ========================================
-- 9. 用户-魔女关联统计
-- ========================================
PRINT '';
PRINT '【9. 用户-魔女关联统计】';
SELECT 
    u.BatchID AS [批次ID],
    COUNT(*) AS [关联数量]
FROM wt.UserWitch uw
JOIN wt.[User] u ON uw.UserID = u.UserID
GROUP BY u.BatchID
ORDER BY u.BatchID;
GO

-- ========================================
-- 10. 批次2用户-魔女关联详情
-- ========================================
PRINT '';
PRINT '【10. 批次2用户-魔女关联详情（应该有13条）】';
SELECT 
    u.Username AS [用户账号],
    w.PrisonerNo AS [囚犯编号],
    w.Name AS [魔女姓名],
    u.BatchID AS [用户批次],
    w.BatchID AS [魔女批次],
    CASE WHEN u.BatchID = w.BatchID THEN '✓' ELSE '✗ 批次不匹配' END AS [批次匹配]
FROM wt.[User] u
JOIN wt.UserWitch uw ON u.UserID = uw.UserID
JOIN wt.Witch w ON uw.WitchID = w.WitchID
WHERE u.BatchID = 2
ORDER BY CAST(u.Username AS INT);
GO

-- ========================================
-- 11. 检查缺少关联的用户
-- ========================================
PRINT '';
PRINT '【11. 检查缺少关联的用户】';
SELECT 
    u.Username AS [用户账号],
    u.BatchID AS [批次ID],
    '缺少魔女关联' AS [问题]
FROM wt.[User] u
WHERE NOT EXISTS (
    SELECT 1 FROM wt.UserWitch uw WHERE uw.UserID = u.UserID
);
GO

-- ========================================
-- 12. 检查缺少用户的魔女
-- ========================================
PRINT '';
PRINT '【12. 检查缺少用户账号的魔女】';
SELECT 
    w.PrisonerNo AS [囚犯编号],
    w.Name AS [魔女姓名],
    w.BatchID AS [批次ID],
    '缺少用户账号' AS [问题]
FROM wt.Witch w
WHERE NOT EXISTS (
    SELECT 1 
    FROM wt.[User] u
    JOIN wt.UserWitch uw ON u.UserID = uw.UserID
    WHERE uw.WitchID = w.WitchID
);
GO

-- ========================================
-- 13. 数据完整性总结
-- ========================================
PRINT '';
PRINT '========================================';
PRINT '数据完整性总结';
PRINT '========================================';

DECLARE @batch1WitchCount INT = (SELECT COUNT(*) FROM wt.Witch WHERE BatchID = 1);
DECLARE @batch2WitchCount INT = (SELECT COUNT(*) FROM wt.Witch WHERE BatchID = 2);
DECLARE @batch1UserCount INT = (SELECT COUNT(*) FROM wt.[User] WHERE BatchID = 1);
DECLARE @batch2UserCount INT = (SELECT COUNT(*) FROM wt.[User] WHERE BatchID = 2);
DECLARE @batch1AssocCount INT = (SELECT COUNT(*) FROM wt.UserWitch uw JOIN wt.[User] u ON uw.UserID = u.UserID WHERE u.BatchID = 1);
DECLARE @batch2AssocCount INT = (SELECT COUNT(*) FROM wt.UserWitch uw JOIN wt.[User] u ON uw.UserID = u.UserID WHERE u.BatchID = 2);
DECLARE @duplicateCount INT = (SELECT COUNT(*) FROM (SELECT PrisonerNo FROM wt.Witch GROUP BY PrisonerNo HAVING COUNT(*) > 1) AS dup);

PRINT '';
PRINT '批次1：';
PRINT CONCAT('  魔女数量：', @batch1WitchCount, ' / 13', CASE WHEN @batch1WitchCount = 13 THEN ' ✓' ELSE ' ✗' END);
PRINT CONCAT('  用户数量：', @batch1UserCount, ' / 13', CASE WHEN @batch1UserCount = 13 THEN ' ✓' ELSE ' ✗' END);
PRINT CONCAT('  关联数量：', @batch1AssocCount, ' / 13', CASE WHEN @batch1AssocCount = 13 THEN ' ✓' ELSE ' ✗' END);

PRINT '';
PRINT '批次2：';
PRINT CONCAT('  魔女数量：', @batch2WitchCount, ' / 13', CASE WHEN @batch2WitchCount = 13 THEN ' ✓' ELSE ' ✗' END);
PRINT CONCAT('  用户数量：', @batch2UserCount, ' / 13', CASE WHEN @batch2UserCount = 13 THEN ' ✗' END);
PRINT CONCAT('  关联数量：', @batch2AssocCount, ' / 13', CASE WHEN @batch2AssocCount = 13 THEN ' ✓' ELSE ' ✗' END);

PRINT '';
PRINT '数据质量：';
PRINT CONCAT('  重复囚犯编号：', @duplicateCount, CASE WHEN @duplicateCount = 0 THEN ' ✓' ELSE ' ✗ 存在重复' END);

PRINT '';
PRINT '========================================';
GO
