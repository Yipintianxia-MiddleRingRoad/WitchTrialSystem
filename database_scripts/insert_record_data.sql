-- =============================================
-- 记录数据导入脚本
-- 生成时间: 2024-12-09
-- 用途: 导入12条记录数据到 wt.Record 表
-- 前置条件: 需要先运行 create_record_table.sql 创建表
-- =============================================

USE WitchTrialWT;
GO

-- 检查表是否存在
IF OBJECT_ID('wt.Record', 'U') IS NULL
BEGIN
    PRINT '错误: wt.Record 表不存在！';
    PRINT '请先运行 create_record_table.sql 创建表。';
    RETURN;
END
GO

-- 清空现有数据（可选，如果需要重新导入）
-- DELETE FROM wt.Record;
-- DBCC CHECKIDENT ('wt.Record', RESEED, 0);
-- GO

-- 插入记录数据
INSERT INTO wt.Record (Title, Content) VALUES
(N'处刑', N'Images/Records/处刑.md'),
(N'残骸', N'Images/Records/残骸.md'),
(N'监牢', N'Images/Records/监牢.md'),
(N'研究人员的调查书', N'Images/Records/研究人员的调查书.md'),
(N'艾玛的笔记1', N'Images/Records/艾玛的笔记1.md'),
(N'艾玛的笔记2', N'Images/Records/艾玛的笔记2.md'),
(N'艾玛的笔记3', N'Images/Records/艾玛的笔记3.md'),
(N'艾玛的笔记4', N'Images/Records/艾玛的笔记4.md'),
(N'过去囚犯留下的信', N'Images/Records/过去囚犯留下的信.md'),
(N'魔女', N'Images/Records/魔女.md'),
(N'魔女化', N'Images/Records/魔文化.md'),
(N'魔女因子', N'Images/Records/魔女因子.md');

GO

-- 验证插入结果
SELECT COUNT(*) AS '插入的记录数量' FROM wt.Record;
GO

-- 显示所有记录数据
SELECT 
    RecordID,
    Title,
    Content
FROM wt.Record
ORDER BY RecordID;
GO

PRINT '记录数据导入完成！';
PRINT '共导入 12 条记录';
PRINT 'Content字段存储Markdown文件路径: Images/Records/*.md';
GO
