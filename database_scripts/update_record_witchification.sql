-- =============================================
-- 记录数据修正脚本
-- 生成时间: 2024-12-09
-- 用途: 修正"魔女化"记录的Content字段错误
-- 问题: Content字段错误写成了"魔文化.md"，应为"魔女化.md"
-- =============================================

USE WitchTrialWT;
GO

-- 检查表是否存在
IF OBJECT_ID('wt.Record', 'U') IS NULL
BEGIN
    PRINT '错误: wt.Record 表不存在！';
    RETURN;
END
GO

PRINT '开始修正"魔女化"记录...';
GO

-- 显示修正前的数据
PRINT '修正前的数据：';
SELECT RecordID, Title, Content 
FROM wt.Record 
WHERE Title = N'魔女化';
GO

-- 更新"魔女化"记录的Content字段
UPDATE wt.Record 
SET Content = N'Images/Records/魔女化.md'
WHERE Title = N'魔女化';
GO

-- 显示修正后的数据
PRINT '修正后的数据：';
SELECT RecordID, Title, Content 
FROM wt.Record 
WHERE Title = N'魔女化';
GO

-- 验证所有记录数据
PRINT '所有记录数据：';
SELECT RecordID, Title, Content 
FROM wt.Record 
ORDER BY RecordID;
GO

PRINT '记录数据修正完成！';
PRINT '"魔女化"记录的Content字段已从"魔文化.md"更正为"魔女化.md"';
GO
