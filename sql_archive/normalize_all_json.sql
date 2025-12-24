USE WitchTrialWT;
GO

PRINT '开始统一所有魔女的JSON格式...';
PRINT '';

-- 步骤1：删除所有空格（冒号后的空格）
UPDATE wt.Witch
SET EducationHistory = REPLACE(EducationHistory, '": "', '":"'),
    WorkHistory = REPLACE(WorkHistory, '": "', '":"')
WHERE EducationHistory IS NOT NULL OR WorkHistory IS NOT NULL;

PRINT '步骤1完成：已删除冒号后的空格';
PRINT '影响行数：' + CAST(@@ROWCOUNT AS NVARCHAR);
PRINT '';

-- 步骤2：统一为小写键名（将大写转为小写）
UPDATE wt.Witch
SET EducationHistory = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    EducationHistory,
    '"School":', '"school":'),
    '"Degree":', '"degree":'),
    '"Major":', '"major":'),
    '"StartDate":', '"startDate":'),
    '"EndDate":', '"endDate":'),
    '"Status":', '"status":'),
    '"SpecialNote":', '"specialNote":'
)
WHERE EducationHistory IS NOT NULL;

PRINT '步骤2完成：已统一教育经历键名为小写';
PRINT '影响行数：' + CAST(@@ROWCOUNT AS NVARCHAR);
PRINT '';

-- 步骤3：统一工作经历键名
UPDATE wt.Witch
SET WorkHistory = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
    WorkHistory,
    '"Company":', '"company":'),
    '"Position":', '"position":'),
    '"StartDate":', '"startDate":'),
    '"EndDate":', '"endDate":'),
    '"Department":', '"department":'),
    '"Responsibilities":', '"responsibilities":'),
    '"Achievements":', '"achievements":'),
    '"Salary":', '"salary":'),
    '"ReasonForLeaving":', '"reasonForLeaving":'
)
WHERE WorkHistory IS NOT NULL AND WorkHistory != '[]';

PRINT '步骤3完成：已统一工作经历键名为小写';
PRINT '影响行数：' + CAST(@@ROWCOUNT AS NVARCHAR);
PRINT '';

-- 验证结果
PRINT '========== 验证结果 ==========';
SELECT 
    COUNT(*) AS 总数,
    SUM(CASE WHEN EducationHistory LIKE '%": "%' THEN 1 ELSE 0 END) AS 教育经历有空格,
    SUM(CASE WHEN WorkHistory LIKE '%": "%' THEN 1 ELSE 0 END) AS 工作经历有空格,
    SUM(CASE WHEN EducationHistory LIKE '%School%' OR EducationHistory LIKE '%Degree%' THEN 1 ELSE 0 END) AS 教育经历有大写键,
    SUM(CASE WHEN WorkHistory LIKE '%Company%' OR WorkHistory LIKE '%Position%' THEN 1 ELSE 0 END) AS 工作经历有大写键
FROM wt.Witch
WHERE EducationHistory IS NOT NULL OR WorkHistory IS NOT NULL;

PRINT '';
PRINT '========== 示例数据 ==========';
SELECT TOP 5
    PrisonerNo,
    Name,
    LEFT(EducationHistory, 100) AS EducationSample
FROM wt.Witch
WHERE EducationHistory IS NOT NULL AND EducationHistory != '[]'
ORDER BY PrisonerNo;

PRINT '';
PRINT '✅ JSON格式统一完成！';
GO
