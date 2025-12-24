-- ========================================
-- 最终修复：统一所有魔女的JSON格式
-- 执行方式：在SSMS中打开此文件并执行（F5）
-- ========================================

USE WitchTrialWT;
GO

PRINT '========================================';
PRINT '开始修复所有魔女的JSON格式';
PRINT '========================================';
PRINT '';

-- ========================================
-- 第1步：删除所有冒号后的空格
-- ========================================
PRINT '第1步：删除冒号后的空格...';

UPDATE wt.Witch
SET EducationHistory = REPLACE(EducationHistory, '": "', '":"')
WHERE EducationHistory LIKE '%": "%';

DECLARE @Count1 INT = @@ROWCOUNT;
PRINT '  教育经历：' + CAST(@Count1 AS NVARCHAR) + ' 行';

UPDATE wt.Witch
SET WorkHistory = REPLACE(WorkHistory, '": "', '":"')
WHERE WorkHistory LIKE '%": "%';

DECLARE @Count2 INT = @@ROWCOUNT;
PRINT '  工作经历：' + CAST(@Count2 AS NVARCHAR) + ' 行';
PRINT '';

-- ========================================
-- 第2步：统一教育经历键名为小写
-- ========================================
PRINT '第2步：统一教育经历键名为小写...';

UPDATE wt.Witch
SET EducationHistory = REPLACE(EducationHistory, '"School":', '"school":')
WHERE EducationHistory LIKE '%"School"%';
PRINT '  School -> school: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' 行';

UPDATE wt.Witch
SET EducationHistory = REPLACE(EducationHistory, '"Degree":', '"degree":')
WHERE EducationHistory LIKE '%"Degree"%';
PRINT '  Degree -> degree: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' 行';

UPDATE wt.Witch
SET EducationHistory = REPLACE(EducationHistory, '"Status":', '"status":')
WHERE EducationHistory LIKE '%"Status"%';
PRINT '  Status -> status: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' 行';

UPDATE wt.Witch
SET EducationHistory = REPLACE(EducationHistory, '"SpecialNote":', '"specialNote":')
WHERE EducationHistory LIKE '%"SpecialNote"%';
PRINT '  SpecialNote -> specialNote: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' 行';
PRINT '';

-- ========================================
-- 第3步：统一工作经历键名为小写
-- ========================================
PRINT '第3步：统一工作经历键名为小写...';

UPDATE wt.Witch
SET WorkHistory = REPLACE(WorkHistory, '"Company":', '"company":')
WHERE WorkHistory LIKE '%"Company"%';
PRINT '  Company -> company: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' 行';

UPDATE wt.Witch
SET WorkHistory = REPLACE(WorkHistory, '"Position":', '"position":')
WHERE WorkHistory LIKE '%"Position"%';
PRINT '  Position -> position: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' 行';
PRINT '';

-- ========================================
-- 验证结果
-- ========================================
PRINT '========================================';
PRINT '验证结果';
PRINT '========================================';

SELECT 
    '总魔女数' AS 项目,
    COUNT(*) AS 数量
FROM wt.Witch
UNION ALL
SELECT 
    '有教育经历',
    COUNT(*)
FROM wt.Witch
WHERE EducationHistory IS NOT NULL AND EducationHistory != '[]'
UNION ALL
SELECT 
    '教育经历仍有空格',
    COUNT(*)
FROM wt.Witch
WHERE EducationHistory LIKE '%": "%'
UNION ALL
SELECT 
    '教育经历仍有大写键',
    COUNT(*)
FROM wt.Witch
WHERE EducationHistory LIKE '%"School"%' OR EducationHistory LIKE '%"Degree"%';

PRINT '';
PRINT '========================================';
PRINT '示例数据（前5条）';
PRINT '========================================';

SELECT TOP 5
    PrisonerNo,
    Name,
    SUBSTRING(EducationHistory, 1, 100) AS EducationSample
FROM wt.Witch
WHERE EducationHistory IS NOT NULL AND EducationHistory != '[]'
ORDER BY PrisonerNo;

PRINT '';
PRINT '✅ 修复完成！所有JSON格式已统一为：';
PRINT '   - 小写键名（school, degree, status, specialNote）';
PRINT '   - 无空格（":"而不是": "）';
PRINT '   - 紧凑格式（无换行符）';
GO
