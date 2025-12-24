USE WitchTrialWT;
GO

-- 分析所有魔女的JSON格式
SELECT 
    PrisonerNo,
    Name,
    CASE 
        WHEN EducationHistory LIKE '[{%School%' THEN 'Capital'
        WHEN EducationHistory LIKE '[{%school%' THEN 'Lowercase'
        ELSE 'Other'
    END AS KeyCase,
    CASE 
        WHEN EducationHistory LIKE '%": "%' THEN 'HasSpace'
        WHEN EducationHistory LIKE '%":"%' THEN 'NoSpace'
        ELSE 'Other'
    END AS Spacing,
    LEN(EducationHistory) AS JsonLen,
    SUBSTRING(EducationHistory, 1, 100) AS Sample
FROM wt.Witch
WHERE EducationHistory IS NOT NULL AND EducationHistory != '[]'
ORDER BY PrisonerNo;
GO
