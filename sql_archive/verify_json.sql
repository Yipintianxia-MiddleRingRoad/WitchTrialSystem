USE WitchTrialWT;
GO

SELECT COUNT(*) AS 有空格的教育经历
FROM wt.Witch
WHERE EducationHistory LIKE '%": "%';

SELECT COUNT(*) AS 有大写键的教育经历
FROM wt.Witch
WHERE EducationHistory LIKE '%"School"%' OR EducationHistory LIKE '%"Degree"%';

SELECT TOP 3
    PrisonerNo,
    Name,
    SUBSTRING(EducationHistory, 1, 100) AS Sample
FROM wt.Witch
WHERE PrisonerNo IN ('658', '666', '677')
ORDER BY PrisonerNo;
GO
