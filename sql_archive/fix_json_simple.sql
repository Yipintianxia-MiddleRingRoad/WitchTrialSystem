USE WitchTrialWT;
GO

-- 删除冒号后的空格
UPDATE wt.Witch
SET EducationHistory = REPLACE(EducationHistory, N'": "', N'":"')
WHERE EducationHistory LIKE N'%": "%';

SELECT '教育经历空格删除：' + CAST(@@ROWCOUNT AS NVARCHAR) + ' 行';

UPDATE wt.Witch
SET WorkHistory = REPLACE(WorkHistory, N'": "', N'":"')
WHERE WorkHistory LIKE N'%": "%';

SELECT '工作经历空格删除：' + CAST(@@ROWCOUNT AS NVARCHAR) + ' 行';

-- 统一为小写键名
UPDATE wt.Witch
SET EducationHistory = 
    REPLACE(REPLACE(REPLACE(REPLACE(
        EducationHistory,
        N'"School":', N'"school":'),
        N'"Degree":', N'"degree":'),
        N'"Status":', N'"status":'),
        N'"SpecialNote":', N'"specialNote":'
    )
WHERE EducationHistory LIKE N'%"School"%' OR EducationHistory LIKE N'%"Degree"%';

SELECT '教育经历键名统一：' + CAST(@@ROWCOUNT AS NVARCHAR) + ' 行';
GO

-- 验证
SELECT TOP 10
    PrisonerNo,
    Name,
    SUBSTRING(EducationHistory, 1, 80) AS Sample
FROM wt.Witch
WHERE EducationHistory IS NOT NULL AND EducationHistory != '[]'
ORDER BY PrisonerNo;
GO
