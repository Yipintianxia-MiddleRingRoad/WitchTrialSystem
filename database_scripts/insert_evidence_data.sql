-- =============================================
-- 证物数据导入脚本
-- 生成时间: 2024-12-09
-- 用途: 导入20条证物数据到 wt.Evidence 表
-- 前置条件: 需要先运行 create_evidence_table.sql 创建表
-- =============================================

USE WitchTrialWT;
GO

-- 检查表是否存在
IF OBJECT_ID('wt.Evidence', 'U') IS NULL
BEGIN
    PRINT '错误: wt.Evidence 表不存在！';
    PRINT '请先运行 create_evidence_table.sql 创建表。';
    RETURN;
END
GO

-- 清空现有数据（可选，如果需要重新导入）
-- DELETE FROM wt.Evidence;
-- DBCC CHECKIDENT ('wt.Evidence', RESEED, 0);
-- GO

-- 插入证物数据
INSERT INTO wt.Evidence (EvidenceNo, Name, Description, ImagePath) VALUES
(0, N'监牢的设计图', N'似乎是监牢的老图纸。可在地图中确认其内容。', N'Images\Evidence\Clue_005_000.png'),
(1, N'仪器', N'一把漆桶精美的剑。此前曾持有的检寻。没有使用过的迹象。', N'Images\Evidence\Clue_005_001.png'),
(2, N'奈叶香的枪', N'在冷冻室找到的，奈叶香持有的枪支。最大装弹量为 6 发，残余弹数为 3 发。是过去存在于监牢内的少女...', N'Images\Evidence\Clue_005_002.png'),
(3, N'弹痕', N'在地下冷冻室发现的弹痕。子弹打进了冰块之中，周围散落有小块的冰碎片。现场留有 1 发子弹。', N'Images\Evidence\Clue_005_003.png'),
(4, N'液氮', N'-196℃的液体。化学式为 N2。由于会引起缺氧或冻伤，因此使用时需要注意。', N'Images\Evidence\Clue_005_004.png'),
(5, N'透明的液体', N'放在保险箱中的不明液体。有使用过的痕迹。', N'Images\Evidence\Clue_005_005.png'),
(6, N'手枪', N'放在保险箱中的手枪。装弹量为 6 发，没有使用过的迹象。', N'Images\Evidence\Clue_005_006.png'),
(7, N'资料', N'写有岛外与宅邸的物资交易，以及在押少女们的情报的大量资料。上面也有艾艾玛她们所有人的人物资料和人...', N'Images\Evidence\Clue_005_007.png'),
(8, N'便条纸', N'奈叶香尸体的口袋中找到的便条。上面潦草写着这串数字。', N'Images\Evidence\Clue_005_008.png'),
(9, N'处刑台通用门', N'位于地下冷冻室深处的大门。靠人力不可能打开。下方设有小窗，往里探看可看见处刑台。小窗大小有限，只...', N'Images\Evidence\Clue_005_009.png'),
(10, N'保险箱', N'被打开的保险箱。是数字密码锁，数字键盘处沾有血迹。', N'Images\Evidence\Clue_005_010.png'),
(11, N'救急箱', N'存放消火栓和防灾用品的箱子。里面装着破门用的斧头，以及缝纫工具等。', N'Images\Evidence\Clue_005_011.png'),
(12, N'通风口', N'连接浴室与处刑台的通风口。大小勉强可容纳一人通过。盖子的固定装置被破坏了。似乎是遭撞击破坏的，...', N'Images\Evidence\Clue_005_012.png'),
(13, N'奈叶香的尸体照片', N'在审判庭的冷冻室中发现。身体表面有许多结冰。未观察到致命性的严重外伤。口腔上颚部分有疑似被利器割...', N'Images\Evidence\Clue_005_013.png'),
(14, N'操作室', N'用来操作地下设施和处刑台的房间。设置有处刑台的操作面板。非常冷。', N'Images\Evidence\Clue_005_014.png'),
(15, N'地下冷冻室', N'保管处刑用的尸体，以及冻结酷刑的魔法少女身体的地点。位于监牢的地下。', N'Images\Evidence\Clue_005_015.png'),
(16, N'处刑台', N'位于审判庭中央的处刑台装置。目前中央没有实体。在替换放置于处刑台的处刑器具之时，会将周围的装置...', N'Images\Evidence\Clue_005_016.png'),
(17, N'控制面板', N'位于地下的控制面板。可以用来操作处刑台。似乎也可以用来开关地下设施的门，但具体使用方法不明。昨天...', N'Images\Evidence\Clue_005_017.png'),
(18, N'亚里沙的尸体照片', N'在审判庭的处刑台上，以坐在电椅上的状态被发现。脸上有蝴蝶形状的烧伤痕迹，身体已呈现魔文化的征兆。头...', N'Images\Evidence\Clue_005_018.png'),
(19, N'希罗的钢笔', N'二师团希罗原本持有的钢笔。', N'Images\Evidence\Clue_005_019.png');

GO

-- 验证插入结果
SELECT COUNT(*) AS '插入的证物数量' FROM wt.Evidence;
GO

-- 显示所有证物数据
SELECT 
    EvidenceID,
    EvidenceNo,
    Name,
    LEFT(Description, 50) + '...' AS Description,
    ImagePath
FROM wt.Evidence
ORDER BY EvidenceNo;
GO

PRINT '证物数据导入完成！';
PRINT '共导入 20 条证物记录';
PRINT '图片路径格式: Images\Evidence\Clue_005_XXX.png';
GO


SELECT EvidenceID, EvidenceNo, Name, ImagePath 
FROM wt.Evidence 
ORDER BY EvidenceNo;
