-- ========================================
-- 步骤 3：验证数据导入
-- ========================================

USE WitchTrialWT;
GO

-- 查看所有魔女的基本信息
SELECT 
    PrisonerNo AS 囚人番号,
    Name AS 姓名,
    Gender AS 性别,
    BirthDate AS 出生日期,
    DATEDIFF(YEAR, BirthDate, GETDATE()) AS 年龄,
    Height AS 身高,
    Weight AS 体重,
    BloodType AS 血型,
    HighestEducation AS 最高学历,
    Magic AS 魔法,
    Status AS 状态
FROM wt.Witch
ORDER BY PrisonerNo;
GO

-- 检查哪些字段已填写
SELECT 
    PrisonerNo AS 囚人番号,
    Name AS 姓名,
    CASE WHEN PersonalNo IS NOT NULL THEN '✅' ELSE '❌' END AS 个人番号,
    CASE WHEN Gender IS NOT NULL THEN '✅' ELSE '❌' END AS 性别,
    CASE WHEN BirthDate IS NOT NULL THEN '✅' ELSE '❌' END AS 出生日期,
    CASE WHEN Email IS NOT NULL THEN '✅' ELSE '❌' END AS 邮箱,
    CASE WHEN Father IS NOT NULL THEN '✅' ELSE '❌' END AS 父亲,
    CASE WHEN Skills IS NOT NULL THEN '✅' ELSE '❌' END AS 技能
FROM wt.Witch
ORDER BY PrisonerNo;
GO

-- 查看特定魔女的完整档案（示例：樱羽艾玛）
SELECT * FROM wt.Witch WHERE PrisonerNo = '658';
GO

PRINT '========================================';
PRINT '✅ 验证完成！';
PRINT '📊 请检查上面的查询结果';
PRINT '========================================';
GO
