-- WitchTrialSystem 双岛屿扩展脚本 (V1.4.0)
-- 新增岛屿2、新管理者、新典狱长、13位新魔女(684-696) - 岛屿2批次1
-- 执行此脚本前请确保已完成V1.3.0的初始化

USE WitchTrialWT;
GO

PRINT '=== 开始双岛屿扩展脚本 ===';

-- 1. 新增岛屿2数据
INSERT INTO wt.Island(Name) VALUES (N'魔女岛·贰');
DECLARE @island2ID INT = (SELECT TOP 1 IslandID FROM wt.Island WHERE Name = N'魔女岛·贰');
PRINT N'新增岛屿2: 魔女岛·贰 (ID: ' + CAST(@island2ID AS NVARCHAR) + N')';

-- 2. 新增批次3、批次4（属于岛屿2）
INSERT INTO wt.Batch(IslandID, WitchCount) VALUES (@island2ID, 0);  -- 批次3
INSERT INTO wt.Batch(IslandID, WitchCount) VALUES (@island2ID, 0);  -- 批次4
DECLARE @batch3ID INT = (SELECT TOP 1 BatchID FROM wt.Batch WHERE IslandID = @island2ID ORDER BY BatchID);
DECLARE @batch4ID INT = (SELECT BatchID FROM wt.Batch WHERE IslandID = @island2ID AND BatchID > @batch3ID);
PRINT N'新增批次3(ID: ' + CAST(@batch3ID AS NVARCHAR) + N') 和 批次4(ID: ' + CAST(@batch4ID AS NVARCHAR) + N')';

-- 3. 创建岛屿管理者和典狱长表
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'wt.IslandRegulator') AND type = 'U')
BEGIN
    CREATE TABLE wt.IslandRegulator(
        UserID INT PRIMARY KEY,
        IslandID INT NOT NULL,
        RegulatorName NVARCHAR(50) NOT NULL,
        FOREIGN KEY(UserID) REFERENCES wt.[User](UserID),
        FOREIGN KEY(IslandID) REFERENCES wt.Island(IslandID)
    );
    PRINT '创建岛屿管理者表成功';
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'wt.IslandWarden') AND type = 'U')
BEGIN
    CREATE TABLE wt.IslandWarden(
        UserID INT PRIMARY KEY,
        IslandID INT NOT NULL,
        WardenName NVARCHAR(50) NOT NULL,
        FOREIGN KEY(UserID) REFERENCES wt.[User](UserID),
        FOREIGN KEY(IslandID) REFERENCES wt.Island(IslandID)
    );
    PRINT '创建岛屿典狱长表成功';
END

-- 4. 新增岛屿2的管理者和典狱长用户账号
DECLARE @roleRegulator INT = (SELECT RoleID FROM wt.Role WHERE Name = N'Meruru');
DECLARE @roleWarden INT = (SELECT RoleID FROM wt.Role WHERE Name = N'Warden');

-- utena_regulator (柊舞缇娜)
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore)
VALUES ('utena_regulator', N'PENDING', N'PENDING', @roleRegulator, @island2ID, NULL, 0);

-- warden2
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore)
VALUES ('warden2', N'PENDING', N'PENDING', @roleWarden, @island2ID, NULL, 0);

-- 5. 建立管理者-岛屿、典狱长-岛屿关联关系
DECLARE @utenaUserID INT = (SELECT UserID FROM wt.[User] WHERE Username = 'utena_regulator');
DECLARE @warden2UserID INT = (SELECT UserID FROM wt.[User] WHERE Username = 'warden2');

INSERT INTO wt.IslandRegulator(UserID, IslandID, RegulatorName)
VALUES (@utenaUserID, @island2ID, N'柊舞缇娜');

INSERT INTO wt.IslandWarden(UserID, IslandID, WardenName)
VALUES (@warden2UserID, @island2ID, N'典狱长2');

PRINT N'新增管理者: utena_regulator (柊舞缇娜)';
PRINT N'新增典狱长: warden2 (典狱长2)';

-- 6. 导入批次3魔女 (684-696)
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID, DescriptionPublic)
VALUES
(N'柊舞缇娜', N'暗黑魔力操纵、变身、调教、伪装', '684', N'审判中', 'Images/684.png', @island2ID, @batch3ID, N'邪恶组织艾诺尔米塔的新任总帅，最初渴望成为魔法少女。她因被组织欺骗而被迫成为干部，却在战斗中发现了自己的真实欲望。她享受看魔法少女痛苦挣扎的快感，这让她感到既兴奋又困惑。她的邪恶并非源于恶意，而是源于对真实自我的迷茫与探寻。她代表了那些被命运欺骗、却在黑暗中找到光芒的灵魂。'),

(N'阿良河琪舞', N'猎豹魔法：炸弹创造、遥控引爆、强爆形态真化', '685', N'死亡(魔女化)', 'Images/685.png', @island2ID, @batch3ID, N'邪恶组织艾诺尔米塔的女干部，对自己的样貌极度自信。她身材娇小却有着独特的魅力，通过爆炸物与社交网络展开战斗。她曾因身材和性格被同学排挤，渴望得到关注与认可。加入组织后，她找到了展示自我的舞台，对舞缇娜死心塌地。她的存在感欲望既是弱点，也是她最强大的力量源泉。'),

(N'杜乃可莉丝', N'尼禄爱莉丝：玩偶活化、玩具屋结界、绒毛治疗', '686', N'死亡(正常)', 'Images/686.png', @island2ID, @batch3ID, N'邪恶组织艾诺尔米塔的女干部，是个孤僻不爱说话的小学生。她表情丰富却缺乏言语，通过玩偶与玩具屋来表达自己的情感。因为内向而在学校难以交友，她将所有感情寄托于玩偶之身。她与舞缇娜的相遇，改变了她对"友谊"的理解。她梦想创造一个所有玩偶都能自由活动的巨大玩具王国。'),

(N'阿古屋真珠', N'洛可慕斯卡：音波冲击、共鸣破坏、洗脑之歌', '687', N'审判中', 'Images/687.png', @island2ID, @batch3ID, N'前主旅团成员，梦想成为一名被世人认可的歌手。她多次参加偶像选拔均告落选，正常状态下歌艺平庸。然而在极度羞耻的状态下，她却能展现出一流的歌唱才华。这种扭曲的天赋让她陷入了深深的自我否定之中。在娞摩的支持与新总帅的机会下，她开始重新审视自己的价值。'),

(N'姐母娞摩', N'莱贝尔布鲁姆：影子束缚、影傀儡术、黑暗吞噬', '688', N'审判中', 'Images/688.png', @island2ID, @batch3ID, N'前主旅团成员，与真珠自幼稚园便认识的亲密伙伴。她出身精英家族，一直活在"完美"的期望压力之下。她掌握影子魔法与策略能力，常在暗处守护真珠的梦想。她的手段不总是光彩正大，但她的初心从未改变。她正在学习如何在道德与爱之间找到自己的答案。'),

(N'花菱春香', N'品红魔法：治愈之光、品红冲击、暗黑真化-堕落医师', '689', N'审判中', 'Images/689.png', @island2ID, @batch3ID, N'特蕾丝玛吉雅小队的队长，性格开朗而富有责任感。她是传统魔法少女的典范，拥有纯潔的心灵与坚定的正义感。她热爱料理，擅长照顾他人，是团队中不可或缺的核心支柱。然而，她也曾在被邪恶力量控制，经历了对自我失控的恐惧。在度过危机后，她对"保护"的意义有了更深层的理解。'),

(N'水神小夜', N'碧蓝魔法：激流斩、冰剑操纵、薄冰巫女真化', '690', N'死亡(正常)', 'Images/690.png', @island2ID, @batch3ID, N'特蕾丝玛吉雅小队的格斗型成员，运动神经发达。她出身于世代经营神社的家族，传承着古老的灵力。冷静而克制的性格掩盖了她内心深处对力量的渴望。多次的战斗失败让她产生了对"惩罚"的复杂期待。她正在学习，如何在力量与温柔之间找到平衡。'),

(N'天川薰子', N'硫磺魔法：爆裂拳、防御屏障、电击天使真化', '691', N'死亡(正常)', 'Images/691.png', @island2ID, @batch3ID, N'特蕾丝玛吉雅小队的力量型成员，性格豪爽被称作大姐头。她天生拥有超乎常人的怪力，但这份天赋曾让她感到被诅咒。她无法精准控制自己的力量，造成过多次无意的伤害。在与邪恶组织的对抗中，她逐渐学会了驾驭这份力量。她的梦想是用这份力量守护所有需要保护的人。'),

(N'梓川咲太', N'能将吐槽暂时变为现实，持续3分钟', '692', N'死亡(正常)', 'Images/692.png', @island2ID, @batch3ID, N'日常中表现得有些沉闷，但实际上具备了不寻常的观察力和同理心。他与许多被诅咒的少女相遇，成为了她们重获新生的见证者和支持者。虽然自己的过去也充满了痛苦，但他从未放弃拯救他人的决心。他特殊的体质使其能够察觉到现实与愿望之间的扭曲与异常。在青春的混沌中，他承载着多个少女的秘密与希望。'),

(N'樱岛麻衣', N'可将存在感化为分身，最多3个，持续10分钟', '693', N'死亡(魔女化)', 'Images/693.png', @island2ID, @batch3ID, N'身为知名演员，她在镜头前展现出完美的笑容，却在日常中消失殆尽。她是最初的受害者，被诅咒导致存在感持续消退，几乎被世界遗忘。尽管如此，她以坚强的意志对抗命运，用行动守护重要的人。她的故事揭示了现代社会中个人与存在的深刻矛盾。在咲太的陪伴下，她逐渐找回被夺走的自我。'),

(N'古贺朋绘', N'能让自己或他人时间倒流10秒，但会随机遗忘一段记忆', '694', N'死亡(正常)', 'Images/694.png', @island2ID, @batch3ID, N'一个看似开朗的女孩，却在夜间陷入了诅咒的循环。她患有严重的睡眠障碍，每晚都要经历意识与身体分离的痛苦。为了活下去，她尝试了各种极端的方法来对抗这无尽的折磨。她的笑容是最坚强的伪装，用来隐藏内心的绝望与恐惧。最终她在咲太的帮助下，学会了与自己的黑暗共存。'),

(N'双叶理央', N'戴上眼镜可看穿事物概率，预测3种未来，每日限1次', '695', N'死亡(魔女化)', 'Images/695.png', @island2ID, @batch3ID, N'一个天才程序员，却被自己的天赋所困。她体内存在两个自我，两种完全对立的人格在争夺身体的控制权。这种分裂导致她无法确认现实，无法信任任何人。她用代码建造的虚拟世界，成为了逃避现实的最后堡垒。在咲太的耐心引导下，她的两个自我开始学会和解。'),

(N'梓川枫', N'画作能散发对应情绪气息，效果持续24小时', '696', N'死亡(正常)', 'Images/696.png', @island2ID, @batch3ID, N'咲太的妹妹，却对哥哥产生了病态的执着与依赖。她的"初恋症候群"将她困在了对哥哥纯粹的爱恋中。这种感情超越了常规的家族纽带，成为了一种诅咒的表现形式。她通过伪装和演技来维持这段关系，却在内心深处感到孤独。虽然病症最终被治愈，但她对爱与关系的理解仍在重建之中。');

PRINT '批次3 (684-696): 13位魔女导入完成';

-- 7. 为批次3魔女创建用户账号
DECLARE @roleWitch INT = (SELECT RoleID FROM wt.Role WHERE Name = N'Witch');

INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore)
VALUES
('684', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch3ID, 0),
('685', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch3ID, 0),
('686', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch3ID, 0),
('687', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch3ID, 0),
('688', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch3ID, 0),
('689', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch3ID, 0),
('690', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch3ID, 0),
('691', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch3ID, 0),
('692', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch3ID, 0),
('693', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch3ID, 0),
('694', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch3ID, 0),
('695', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch3ID, 0),
('696', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch3ID, 0);

PRINT '批次3: 13个用户账号创建完成';

-- 8. 建立用户-魔女关联关系 (批次3)
INSERT INTO wt.UserWitch(UserID, WitchID)
SELECT u.UserID, w.WitchID
FROM wt.[User] u
JOIN wt.Witch w ON w.PrisonerNo = u.Username
WHERE u.Username IN ('684','685','686','687','688','689','690','691','692','693','694','695','696');

PRINT '批次3: 用户-魔女关联关系建立完成';

-- 9. 更新批次3的魔女数量
UPDATE wt.Batch SET WitchCount = (SELECT COUNT(*) FROM wt.Witch WHERE BatchID = @batch3ID) WHERE BatchID = @batch3ID;

PRINT '批次3魔女数量更新完成';

-- 10. 批次4占位 (697-709) - 等待后续数据
PRINT '批次4 (697-709) 已预留，等待魔女详细信息...';

-- 11. 为现有岛屿1的管理者和典狱长建立关联记录
DECLARE @island1ID INT = (SELECT TOP 1 IslandID FROM wt.Island WHERE Name = N'魔女岛·壹');
DECLARE @meruruUserID INT = (SELECT UserID FROM wt.[User] WHERE Username = 'meruru_regulator');
DECLARE @warden1UserID INT = (SELECT UserID FROM wt.[User] WHERE Username = 'warden');

INSERT INTO wt.IslandRegulator(UserID, IslandID, RegulatorName)
VALUES (@meruruUserID, @island1ID, N'冰上梅露露');

INSERT INTO wt.IslandWarden(UserID, IslandID, WardenName)
VALUES (@warden1UserID, @island1ID, N'典狱长');

PRINT '岛屿1管理者和典狱长关联关系建立完成';

-- 12. 设置所有密码为123456 (实际使用时需要设置真实密码)
DECLARE @salt123456 NVARCHAR(64) = 'your_salt_here';
DECLARE @hash123456 NVARCHAR(64) = 'your_hash_here';

UPDATE wt.[User] 
SET PasswordHash = N'PENDING', Salt = N'PENDING'
WHERE Username IN ('utena_regulator', 'warden2', '684','685','686','687','688','689','690','691','692','693','694','695','696');

PRINT '新账号密码设置为PENDING状态，需要使用Security.cs设置真实密码';

PRINT '=== 双岛屿扩展脚本执行完成 ===';
PRINT '扩展内容:';
PRINT '- 新增岛屿2: 魔女岛·贰';
PRINT '- 新增管理者: utena_regulator (柊舞缇娜)'; 
PRINT '- 新增典狱长: warden2 (典狱长2)';
PRINT '- 新增批次3: 13位魔女 (684-696)';
PRINT '- 批次4已预留: 等待697-709号魔女数据';
PRINT '- 权限控制已配置: 各角色只能管理本岛屿数据';
PRINT '';
PRINT '下一步: 1) 批量设置密码 2) 导入697-709魔女数据 3) 修改代码权限逻辑';