-- 检查JSON有效性
USE WitchTrialWT;
GO

SELECT 
    PrisonerNo,
    Name,
    ISJSON(EducationHistory) AS IsValidJSON,
    LEN(EducationHistory) AS JsonLength,
    CASE 
        WHEN EducationHistory LIKE '%"%' THEN '包含全角引号'
        WHEN EducationHistory LIKE '%''%' THEN '包含全角单引号'
        ELSE '正常'
    END AS QuoteCheck
FROM wt.Witch 
WHERE PrisonerNo IN ('675', '677', '678', '679', '680', '681', '692');
GO
