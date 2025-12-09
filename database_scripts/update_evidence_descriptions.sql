-- =============================================
-- 证物描述更新脚本
-- 生成时间: 2024-12-09
-- 用途: 修正 wt.Evidence 表中的描述字段错误
-- =============================================

USE WitchTrialWT;
GO

-- 检查表是否存在
IF OBJECT_ID('wt.Evidence', 'U') IS NULL
BEGIN
    PRINT '错误: wt.Evidence 表不存在！';
    RETURN;
END
GO

PRINT '开始更新证物描述...';
GO

-- 更新证物描述（按 EvidenceNo 顺序）
UPDATE wt.Evidence SET Description = N'似乎是监牢的老图纸。可在地图中确认其内容。' WHERE EvidenceNo = 0;
UPDATE wt.Evidence SET Description = N'一把装饰精美的剑。此前放在了保险箱中。没有使用过的迹象。' WHERE EvidenceNo = 1;
UPDATE wt.Evidence SET Description = N'在冷冻室找到的，奈叶香曾持有的枪支。最大装弹量为 6 发，残余弹数为 3 发。是过去存在于监牢内的少女用魔法制成的武器。在日期变更之时，会自动装填 1 发子弹。' WHERE EvidenceNo = 2;
UPDATE wt.Evidence SET Description = N'在地下冷冻室发现的弹痕。子弹打进了冰块之中，周围散落有小块的冰碎片。现场留有 1 发子弹。' WHERE EvidenceNo = 3;
UPDATE wt.Evidence SET Description = N'-196℃的液体。化学式为 N2。由于会引起缺氧或冻伤，因此使用时需要注意。' WHERE EvidenceNo = 4;
UPDATE wt.Evidence SET Description = N'放在保险箱中的不明液体。有使用过的痕迹。' WHERE EvidenceNo = 5;
UPDATE wt.Evidence SET Description = N'放在保险箱中的手枪。装弹量为 6 发，没有使用过的迹象。' WHERE EvidenceNo = 6;
UPDATE wt.Evidence SET Description = N'写有岛外与宅邸的物资交易，以及在押少女们的情报的大量资料。上面也有艾玛她们所有人的人物资料和人脸照片。解读似乎需要耗费大量时间。' WHERE EvidenceNo = 7;
UPDATE wt.Evidence SET Description = N'奈叶香尸体的口袋中找到的便条。上面潦草写着这串数字。' WHERE EvidenceNo = 8;
UPDATE wt.Evidence SET Description = N'位于地下冷冻室深处的大门。靠人力不可能打开。下方设有小窗，往里探看可看见处刑台。小窗大小有限，只够小动物通过。开启时会发出巨大声响，因此本次案件中没有开启过的迹象。' WHERE EvidenceNo = 9;
UPDATE wt.Evidence SET Description = N'被打开的保险箱。是数字密码锁，数字键盘处沾有血迹。' WHERE EvidenceNo = 10;
UPDATE wt.Evidence SET Description = N'存放消火栓和防灾用品的箱子。里面装着破门用的斧头，以及缝纫工具等。' WHERE EvidenceNo = 11;
UPDATE wt.Evidence SET Description = N'连结操作室与冷冻室的通风口。大小勉强可容纳一人通过。盖子的固定装置被破坏了。似乎是遭枪击破坏的，并留有 1 发子弹。' WHERE EvidenceNo = 12;
UPDATE wt.Evidence SET Description = N'在审判庭的处刑台上发现。身体表面有少许结冰。未观察到致命性的严重外伤。口腔上颚部分有疑似被利器割伤的痕迹，且有血液自口中渗出。此外，指尖也沾有血迹。' WHERE EvidenceNo = 13;
UPDATE wt.Evidence SET Description = N'用来操作地下设施和处刑台的房间。设置有处刑台的操作面板。非常冷。' WHERE EvidenceNo = 14;
UPDATE wt.Evidence SET Description = N'保管处刑后的尸体，以及受冻结刑的魔女化少女身体的地点。位于监牢的地下。' WHERE EvidenceNo = 15;
UPDATE wt.Evidence SET Description = N'位于审判庭中央的处刑台装置。目前中央设置有电椅。在替换设置在处刑台的处刑器具之时，会将周围的装置整体下降到地下冷冻室。据留存的记录可知，昨晚 21 时曾移动数次，今早 9 时也曾上升过。不可能自地下冷冻室一侧侵入。' WHERE EvidenceNo = 16;
UPDATE wt.Evidence SET Description = N'位于地下的控制面板。可以用来操作处刑台。似乎也可以用来开关地下设施的门，但具体使用方法不明。昨天艾玛本应把它摔坏了，但现在已得到换新。' WHERE EvidenceNo = 17;
UPDATE wt.Evidence SET Description = N'在审判庭的处刑台上，以坐在电椅上的状态被发现。脸上有蝴蝶形状的烧伤痕，身体已呈现魔女化的征候。头发和衣服沾湿了。' WHERE EvidenceNo = 18;
UPDATE wt.Evidence SET Description = N'二阶堂希罗原本持有的钢笔。' WHERE EvidenceNo = 19;

GO

-- 验证更新结果
PRINT '更新完成！显示更新后的数据：';
GO

SELECT 
    EvidenceID,
    EvidenceNo,
    Name,
    Description,
    ImagePath
FROM wt.Evidence
ORDER BY EvidenceNo;
GO

PRINT '证物描述更新完成！';
PRINT '共更新 20 条证物记录的描述字段';
GO
