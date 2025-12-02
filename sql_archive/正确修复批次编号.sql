-- =======================================================================
-- 正确修复批次编号脚本
-- 重新创建正确的批次结构，避免更新标识列
-- =======================================================================

USE WitchTrialWT;
GO

PRINT '=== 开始正确修复批次编号 ===';

-- 1. 备份当前魔女数据到临时表
PRINT '';
PRINT '=== 1. 备份魔女数据 ===';

SELECT *
INTO #TempWitchBackup
FROM wt.Witch
WHERE IslandID = 2;

PRINT '备份岛屿2魔女数据完成，记录数：' + CAST(@@ROWCOUNT AS NVARCHAR);

-- 2. 删除错误的UserWitch关联
PRINT '';
PRINT '=== 2. 清理关联关系 ===';

DELETE FROM wt.UserWitch 
WHERE UserID IN (
    SELECT u.UserID FROM wt.[User] u
    WHERE u.IslandID = 2
);

PRINT '清理UserWitch关联完成';

-- 3. 删除岛屿2的错误批次数据
PRINT '';
PRINT '=== 3. 删除错误批次数据 ===';

DELETE FROM wt.Witch WHERE IslandID = 2;
PRINT '删除岛屿2魔女数据完成';

DELETE FROM wt.[User] WHERE IslandID = 2 AND Username NOT IN ('utena_regulator', 'warden2');
PRINT '删除岛屿2用户数据完成（保留管理者）';

DELETE FROM wt.Batch WHERE IslandID = 2;
PRINT '删除岛屿2批次数据完成';

-- 4. 重新创建正确的批次结构
PRINT '';
PRINT '=== 4. 重新创建批次结构 ===';

-- 插入正确的批次1和2
INSERT INTO wt.Batch (IslandID, WitchCount)
VALUES (2, 13);  -- 批次1，将包含684-696

INSERT INTO wt.Batch (IslandID, WitchCount)
VALUES (2, 0);   -- 批次2，为未来预留

PRINT '创建岛屿2批次1,2完成';

-- 5. 重新插入魔女684-696到正确的批次
PRINT '';
PRINT '=== 5. 重新插入魔女数据 ===';

-- 获取新创建的批次ID
DECLARE @batch1ID INT = (SELECT TOP 1 BatchID FROM wt.Batch WHERE IslandID = 2 ORDER BY BatchID);
DECLARE @batch2ID INT = (SELECT BatchID FROM wt.Batch WHERE IslandID = 2 AND BatchID > @batch1ID);

PRINT '新批次ID - 批次1:' + CAST(@batch1ID AS NVARCHAR) + ', 批次2:' + CAST(@batch2ID AS NVARCHAR);

-- 重新插入魔女数据
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID, DescriptionPublic)
VALUES
(N'柊舞缇娜', N'暗黑魔力操纵、变身、调教、伪装', '684', N'审判中', 'Images/684.png', 2, @batch1ID, N'邪恶组织艾诺尔米塔的新任总帅，最初渴望成为魔法少女。她因被组织欺骗而被迫成为干部，却在战斗中发现了自己的真实欲望。她享受看魔法少女痛苦挣扎的快感，这让她感到既兴奋又困惑。她的邪恶并非源于恶意，而是源于对真实自我的迷茫与探寻。她代表了那些被命运欺骗、却在黑暗中找到光芒的灵魂。'),

(N'阿良河琪舞', N'猎豹魔法：炸弹创造、遥控引爆、强爆形态真化', '685', N'死亡(魔女化)', 'Images/685.png', 2, @batch1ID, N'邪恶组织艾诺尔米塔的女干部，对自己的样貌极度自信。她身材娇小却有着独特的魅力，通过爆炸物与社交网络展开战斗。她曾因身材和性格被同学排挤，渴望得到关注与认可。加入组织后，她找到了展示自我的舞台，对舞缇娜死心塌地。她的存在感欲望既是弱点，也是她最强大的力量源泉。'),

(N'杜乃可莉丝', N'尼禄爱莉丝：玩偶活化、玩具屋结界、绒毛治疗', '686', N'死亡(正常)', 'Images/686.png', 2, @batch1ID, N'邪恶组织艾诺尔米塔的女干部，是个孤僻不爱说话的小学生。她表情丰富却缺乏言语，通过玩偶与玩具屋来表达自己的情感。因为内向而在学校难以交友，她将所有感情寄托于玩偶之身。她与舞缇娜的相遇，改变了她对"友谊"的理解。她梦想创造一个所有玩偶都能自由活动的巨大玩具王国。'),

(N'阿古屋真珠', N'洛可慕斯卡：音波冲击、共鸣破坏、洗脑之歌', '687', N'审判中', 'Images/687.png', 2, @batch1ID, N'前主旅团成员，梦想成为一名被世人认可的歌手。她多次参加偶像选拔均告落选，正常状态下歌艺平庸。然而在极度羞耻的状态下，她却能展现出一流的歌唱才华。这种扭曲的天赋让她陷入了深深的自我否定之中。在娞摩的支持与新总帅的机会下，她开始重新审视自己的价值。'),

(N'姐母娞摩', N'莱贝尔布鲁姆：影子束缚、影傀儡术、黑暗吞噬', '688', N'审判中', 'Images/688.png', 2, @batch1ID, N'前主旅团成员，与真珠自幼稚园便认识的亲密伙伴。她出身精英家族，一直活在"完美"的期望压力之下。她掌握影子魔法与策略能力，常在暗处守护真珠的梦想。她的手段不总是光彩正大，但她的初心从未改变。她正在学习如何在道德与爱之间找到自己的答案。'),

(N'花菱春香', N'品红魔法：治愈之光、品红冲击、暗黑真化-堕落医师', '689', N'审判中', 'Images/689.png', 2, @batch1ID, N'特蕾丝玛吉雅小队的队长，性格开朗而富有责任感。她是传统魔法少女的典范，拥有纯潔的心灵与坚定的正义感。她热爱料理，擅长照顾他人，是团队中不可或缺的核心支柱。然而，她也曾在被邪恶力量控制，经历了对自我失控的恐惧。在度过危机后，她对"保护"的意义有了更深层的理解。'),

(N'水神小夜', N'碧蓝魔法：激流斩、冰剑操纵、薄冰巫女真化', '690', N'死亡(正常)', 'Images/690.png', 2, @batch1ID, N'特蕾丝玛吉雅小队的格斗型成员，运动神经发达。她出身于世代经营神社的家族，传承着古老的灵力。冷静而克制的性格掩盖了她内心深处对力量的渴望。多次的战斗失败让她产生了对"惩罚"的复杂期待。她正在学习，如何在力量与温柔之间找到平衡。'),

(N'天川薰子', N'硫磺魔法：爆裂拳、防御屏障、电击天使真化', '691', N'死亡(正常)', 'Images/691.png', 2, @batch1ID, N'特蕾丝玛吉雅小队的力量型成员，性格豪爽被称作大姐头。她天生拥有超乎常人的怪力，但这份天赋曾让她感到被诅咒。她无法精准控制自己的力量，造成过多次无意的伤害。在与邪恶组织的对抗中，她逐渐学会了驾驭这份力量。她的梦想是用这份力量守护所有需要保护的人。'),

(N'相野美都', N'翠绿魔法：藤蔓缠绕、花粉催眠、森之精灵真化', '692', N'审判中', 'Images/692.png', 2, @batch1ID, N'特蕾丝玛吉雅小队的支援型成员，拥有强大的治疗能力和植物操控能力。她来自一个重视传统与自然的家庭，从小便展现出与植物沟通的天赋。她温柔善良，但有时会因过于担心他人而显得犹豫不决。在与团队的相处中，她学会了信任伙伴与承担责任。'),

(N'平良伊纲', N'琥珀魔法：火焰之箭、熔岩护盾、烈焰魔女真化', '693', N'审判中', 'Images/693.png', 2, @batch1ID, N'神秘的魔法少女，拥有强大的火焰操控能力。她性格独立，不喜与他人过多交流，常常独自行动。她的过去笼罩着谜团，只知道她似乎在寻找某种重要的东西。她的火焰魔法既可以是守护之光，也可以是毁灭之炎，全凭她内心的决断。'),

(N'江利内美智', N'白银魔法：寒冰之镜、冰封时间、雪之公主真化', '694', N'死亡(正常)', 'Images/694.png', 2, @batch1ID, N'拥有冰系魔法的神秘少女，能够操控时间与空间的微妙平衡。她出身贵族，受到严格的魔法教育，但内心向往自由。她的魔法精密而优雅，能够在战斗中创造绝对的优势。然而，她也因此承受着巨大的压力，害怕辜负众人的期望。'),

(N'椎崎咲良', N'黄金魔法：光芒之剑、净化结界、太阳女神真化', '695', N'死亡(魔女化)', 'Images/695.png', 2, @batch1ID, N'拥有光系魔法的魔法少女，性格开朗活泼，是团队中的阳光角色。她相信正义终将战胜邪恶，对未来充满希望。然而，现实的残酷让她开始质疑自己的信念。在与邪恶组织的接触中，她逐渐理解了世界的复杂性与人心的多面性。'),

(N'月出Style', N'星空魔法：星辰陨落、幻影分身、月之女神真化', '696', N'审判中', 'Images/696.png', 2, @batch1ID, N'神秘的占卜师，能够通过星象预测未来并操控暗影力量。她性格神秘，说话充满禅意，让人难以捉摸。她的魔法既可以是救赎的希望，也可以是绝望的深渊，全看她的选择。她似乎在寻找能够改变命运的关键之人。');

PRINT '重新插入684-696魔女数据完成';

-- 6. 重新创建用户账号
PRINT '';
PRINT '=== 6. 重新创建用户账号 ===';

-- 获取角色ID
DECLARE @roleWitch INT = (SELECT RoleID FROM wt.Role WHERE Name = 'Witch');

-- 插入684-696用户
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore)
VALUES 
('684', '0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976', 'Yipintianxia_MiddleRingRoad_2025', @roleWitch, 2, @batch1ID, 0),
('685', '0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976', 'Yipintianxia_MiddleRingRoad_2025', @roleWitch, 2, @batch1ID, 0),
('686', '0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976', 'Yipintianxia_MiddleRingRoad_2025', @roleWitch, 2, @batch1ID, 0),
('687', '0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976', 'Yipintianxia_MiddleRingRoad_2025', @roleWitch, 2, @batch1ID, 0),
('688', '0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976', 'Yipintianxia_MiddleRingRoad_2025', @roleWitch, 2, @batch1ID, 0),
('689', '0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976', 'Yipintianxia_MiddleRingRoad_2025', @roleWitch, 2, @batch1ID, 0),
('690', '0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976', 'Yipintianxia_MiddleRingRoad_2025', @roleWitch, 2, @batch1ID, 0),
('691', '0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976', 'Yipintianxia_MiddleRingRoad_2025', @roleWitch, 2, @batch1ID, 0),
('692', '0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976', 'Yipintianxia_MiddleRingRoad_2025', @roleWitch, 2, @batch1ID, 0),
('693', '0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976', 'Yipintianxia_MiddleRingRoad_2025', @roleWitch, 2, @batch1ID, 0),
('694', '0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976', 'Yipintianxia_MiddleRingRoad_2025', @roleWitch, 2, @batch1ID, 0),
('695', '0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976', 'Yipintianxia_MiddleRingRoad_2025', @roleWitch, 2, @batch1ID, 0),
('696', '0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976', 'Yipintianxia_MiddleRingRoad_2025', @roleWitch, 2, @batch1ID, 0);

PRINT '重新创建684-696用户账号完成';

-- 7. 重建UserWitch关联
PRINT '';
PRINT '=== 7. 重建用户-魔女关联 ===';

INSERT INTO wt.UserWitch (UserID, WitchID)
SELECT u.UserID, w.WitchID
FROM wt.[User] u
JOIN wt.Witch w ON w.PrisonerNo = u.Username
WHERE u.Username IN ('684','685','686','687','688','689','690','691','692','693','694','695','696');

PRINT '重建UserWitch关联完成';

-- 8. 验证最终结果
PRINT '';
PRINT '=== 8. 最终验证 ===';

-- 显示批次状态
SELECT 
    '最终批次状态' AS 状态,
    b.BatchID,
    b.IslandID,
    i.Name AS IslandName,
    b.WitchCount AS 设置数量,
    COUNT(w.WitchID) AS 实际数量
FROM wt.Batch b
LEFT JOIN wt.Island i ON i.IslandID = b.IslandID
LEFT JOIN wt.Witch w ON w.IslandID = b.IslandID AND w.BatchID = b.BatchID
GROUP BY b.BatchID, b.IslandID, i.Name, b.WitchCount
ORDER BY b.IslandID, b.BatchID;

-- 显示岛屿2魔女
PRINT '';
SELECT 
    '岛屿2魔女验证' AS 状态,
    w.BatchID,
    COUNT(*) AS 数量,
    STUFF((
        SELECT ', ' + w2.PrisonerNo 
        FROM wt.Witch w2 
        WHERE w2.IslandID = 2 AND w2.BatchID = w.BatchID 
        ORDER BY w2.PrisonerNo
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS 囚犯编号
FROM wt.Witch w
WHERE w.IslandID = 2
GROUP BY w.BatchID
ORDER BY w.BatchID;

-- 清理临时表
DROP TABLE #TempWitchBackup;

PRINT '';
PRINT '=== 修复完成 ===';
PRINT '现在岛屿2应该正确显示批次1和批次2';
PRINT '魔女684-696应该正确显示为岛屿2批次1';
PRINT '请重启程序验证效果';
GO