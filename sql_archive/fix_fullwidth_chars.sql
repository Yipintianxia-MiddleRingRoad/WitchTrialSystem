USE WitchTrialWT;
GO

-- 清理所有全角字符，替换为半角字符
PRINT '开始清理全角字符...';

-- 教育经历：替换全角引号、冒号、逗号等
UPDATE wt.Witch
SET EducationHistory = 
    REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
        EducationHistory,
        N'"', '"'),      -- 全角左引号
        N'"', '"'),      -- 全角右引号
        N''', ''''),     -- 全角左单引号
        N''', ''''),     -- 全角右单引号
        N'：', ':'),     -- 全角冒号
        N'，', ','),     -- 全角逗号
        N'【', '['),     -- 全角左方括号
        N'】', ']')      -- 全角右方括号
    )
WHERE EducationHistory IS NOT NULL;

PRINT '教育经历清理完成：' + CAST(@@ROWCOUNT AS NVARCHAR) + ' 行';

-- 工作经历：同样处理
UPDATE wt.Witch
SET WorkHistory = 
    REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
        WorkHistory,
        N'"', '"'),
        N'"', '"'),
        N''', ''''),
        N''', ''''),
        N'：', ':'),
        N'，', ','),
        N'【', '['),
        N'】', ']')
    )
WHERE WorkHistory IS NOT NULL;

PRINT '工作经历清理完成：' + CAST(@@ROWCOUNT AS NVARCHAR) + ' 行';

-- 验证692号魔女的数据
PRINT '';
PRINT '验证692号魔女数据：';
SELECT 
    PrisonerNo,
    Name,
    EducationHistory
FROM wt.Witch
WHERE PrisonerNo = '692';

-- 检查是否还有全角字符
PRINT '';
PRINT '检查是否还有全角字符：';
SELECT 
    COUNT(*) AS 仍有全角引号的教育经历
FROM wt.Witch
WHERE EducationHistory LIKE N'%"%' OR EducationHistory LIKE N'%"%';

SELECT 
    COUNT(*) AS 仍有全角冒号的教育经历
FROM wt.Witch
WHERE EducationHistory LIKE N'%：%';

PRINT '';
PRINT '✅ 全角字符清理完成！';
GO
