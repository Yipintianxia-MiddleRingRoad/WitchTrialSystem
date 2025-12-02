-- ========================================
-- 更新魔女状态为中文
-- ========================================

USE WitchTrialWT;
GO

PRINT '开始更新魔女状态...';
GO

-- 更新13位魔女的状态
UPDATE wt.Witch SET Status = '审判中' WHERE PrisonerNo = '658';  -- 樱羽艾玛
UPDATE wt.Witch SET Status = '死亡(正常)' WHERE PrisonerNo = '659';  -- 二阶堂希罗
UPDATE wt.Witch SET Status = '死亡(魔女化)' WHERE PrisonerNo = '660';  -- 夏目安安
UPDATE wt.Witch SET Status = '死亡(正常)' WHERE PrisonerNo = '661';  -- 城崎诺亚
UPDATE wt.Witch SET Status = '死亡(魔女化)' WHERE PrisonerNo = '662';  -- 莲见蕾雅
UPDATE wt.Witch SET Status = '死亡(正常)' WHERE PrisonerNo = '663';  -- 佐伯米莉亚
UPDATE wt.Witch SET Status = '审判中' WHERE PrisonerNo = '664';  -- 宝生玛格
UPDATE wt.Witch SET Status = '死亡(正常)' WHERE PrisonerNo = '665';  -- 黑部奈叶香
UPDATE wt.Witch SET Status = '死亡(正常)' WHERE PrisonerNo = '666';  -- 紫藤亚里沙
UPDATE wt.Witch SET Status = '死亡(魔女化)' WHERE PrisonerNo = '667';  -- 橘雪莉
UPDATE wt.Witch SET Status = '死亡(正常)' WHERE PrisonerNo = '668';  -- 远野汉娜
UPDATE wt.Witch SET Status = '审判中' WHERE PrisonerNo = '669';  -- 泽渡可可
UPDATE wt.Witch SET Status = '审判中' WHERE PrisonerNo = '670';  -- 冰上梅露露

PRINT '✅ 状态更新完成！';
GO

-- 验证更新结果
SELECT 
    PrisonerNo AS 囚人番号,
    Name AS 姓名,
    Status AS 状态
FROM wt.Witch
ORDER BY PrisonerNo;
GO

-- 统计各状态数量
SELECT 
    Status AS 状态,
    COUNT(*) AS 数量
FROM wt.Witch
GROUP BY Status
ORDER BY 
    CASE Status
        WHEN '待抓捕' THEN 1
        WHEN '分配至岛屿' THEN 2
        WHEN '审判中' THEN 3
        WHEN '死亡(正常)' THEN 4
        WHEN '死亡(魔女化)' THEN 5
        ELSE 6
    END;
GO

PRINT '';
PRINT '========================================';
PRINT '✅ 状态更新完成！';
PRINT '📊 审判中: 4人 (658, 664, 669, 670)';
PRINT '📊 死亡(正常): 6人 (659, 661, 663, 665, 666, 668)';
PRINT '📊 死亡(魔女化): 3人 (660, 662, 667)';
PRINT '========================================';
GO
