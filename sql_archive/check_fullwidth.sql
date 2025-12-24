USE WitchTrialWT;
GO

-- 检查是否还有全角字符
SELECT 
    COUNT(*) AS 仍有全角引号的教育经历
FROM wt.Witch
WHERE EducationHistory LIKE N'%"%' 
   OR EducationHistory LIKE N'%"%'
   OR EducationHistory LIKE N'%'%'
   OR EducationHistory LIKE N'%'%'
   OR EducationHistory LIKE N'%：%';

-- 检查特定魔女的JSON格式
SELECT 
    PrisonerNo,
    Name,
    CASE 
        WHEN EducationHistory LIKE N'%"%' OR EducationHistory LIKE N'%"%' THEN '有全角引号'
        WHEN EducationHistory LIKE N'%：%' THEN '有全角冒号'
        ELSE '格式正常'
    END AS JSON格式状态
FROM wt.Witch
WHERE PrisonerNo IN ('675', '677', '678', '679', '680', '681', '692')
ORDER BY PrisonerNo;
GO
