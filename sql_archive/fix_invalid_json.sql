USE WitchTrialWT;
GO

-- 修复包含全角引号的JSON
UPDATE wt.Witch 
SET EducationHistory = REPLACE(REPLACE(REPLACE(REPLACE(EducationHistory, N'"', '"'), N'"', '"'), N''', ''''), N''', '''')
WHERE ISJSON(EducationHistory) = 0 AND EducationHistory IS NOT NULL;

PRINT '已修复教育经历 ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' 条记录';

UPDATE wt.Witch 
SET WorkHistory = REPLACE(REPLACE(REPLACE(REPLACE(WorkHistory, N'"', '"'), N'"', '"'), N''', ''''), N''', '''')
WHERE ISJSON(WorkHistory) = 0 AND WorkHistory IS NOT NULL AND WorkHistory != '[]';

PRINT '已修复工作经历 ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' 条记录';
GO

-- 验证修复结果
SELECT 
    PrisonerNo,
    Name,
    CASE WHEN ISJSON(EducationHistory) = 1 THEN 'OK' ELSE 'FAIL' END AS EducationStatus,
    CASE WHEN ISJSON(WorkHistory) = 1 OR WorkHistory IS NULL OR WorkHistory = '[]' THEN 'OK' ELSE 'FAIL' END AS WorkStatus
FROM wt.Witch
WHERE ISJSON(EducationHistory) = 0 OR (ISJSON(WorkHistory) = 0 AND WorkHistory IS NOT NULL AND WorkHistory != '[]')
ORDER BY PrisonerNo;
GO
