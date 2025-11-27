-- WitchTrialSystem 批次5魔女完整导入脚本
-- 新增岛屿2批次5（697-709）的13位魔女，包含完整的28个扩展字段
-- 执行前请确保已完成魔女表扩展字段（add_witch_extended_fields.sql）

USE WitchTrialWT;
GO

PRINT '=== 开始导入批次5魔女（697-709） ===';

-- 1. 获取岛屿2的批次5 ID
DECLARE @island2ID INT = (SELECT TOP 1 IslandID FROM wt.Island WHERE Name = N'魔女岛·贰');
DECLARE @batch5ID INT;

-- 检查批次5是否已存在，如果不存在则创建
IF NOT EXISTS (SELECT 1 FROM wt.Batch WHERE IslandID = @island2ID AND BatchID = 5)
BEGIN
    -- 获取当前最大批次ID
    DECLARE @maxBatchID INT = (SELECT ISNULL(MAX(BatchID), 0) FROM wt.Batch WHERE IslandID = @island2ID);
    SET @batch5ID = @maxBatchID + 1;
    
    INSERT INTO wt.Batch(IslandID, WitchCount) VALUES (@island2ID, 0);
    SET @batch5ID = SCOPE_IDENTITY();
    
    PRINT N'创建批次5 (ID: ' + CAST(@batch5ID AS NVARCHAR) + N')';
END
ELSE
BEGIN
    SET @batch5ID = 5;
    PRINT N'批次5已存在，使用现有批次ID: ' + CAST(@batch5ID AS NVARCHAR);
END

-- 2. 导入批次5魔女基础信息
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID, DescriptionPublic)
VALUES
(N'鹿目圆', N'净化魔法、希望之力、魔力增幅', '697', N'分配至岛屿', 'Images/697.png', @island2ID, @batch5ID, N'性格温柔善良，拥有强大的希望之力，是魔法少女小队的核心。为了守护同伴与城市和平，愿意挺身而出，即使面对绝望也不放弃，魔法能净化邪恶、给予他人力量。'),

(N'晓美焰', N'时间回溯、魔力枪械、屏障防御', '698', N'分配至岛屿', 'Images/698.png', @island2ID, @batch5ID, N'沉默冷静的转校生，拥有时间回溯的魔法能力。为了改变悲剧命运，独自背负着多次轮回的记忆，战斗风格果断凌厉，看似冷漠实则内心极度珍视与同伴的羁绊。'),

(N'巴麻美', N'缎带束缚、魔力子弹、多重枪械召唤', '699', N'分配至岛屿', 'Images/699.png', @island2ID, @batch5ID, N'成熟优雅的魔法少女前辈，擅长用缎带与魔力枪械战斗。主动承担起保护后辈的责任，性格温柔可靠，喜欢制作点心与品茶，看似完美的外表下，藏着对同伴陪伴的渴望。'),

(N'美树沙耶香', N'剑术魔法、快速愈合、声波攻击', '700', N'分配至岛屿', 'Images/700.png', @island2ID, @batch5ID, N'充满正义感的少女，为了守护重要的人成为魔法少女。擅长近战剑术，性格直率热情，始终坚守自己的正义信念，即使面对困难也不退缩，重视友情与承诺。'),

(N'佐仓杏子', N'锁链操控、能量爆破、结界防御', '701', N'分配至岛屿', 'Images/701.png', @island2ID, @batch5ID, N'外表叛逆、内心细腻的魔法少女，擅长锁链魔法与生存战斗。因过往经历对他人保持距离，喜欢吃零食，看似独来独往，实则在关键时刻会挺身而出保护同伴，坚守自己的生存之道。'),

(N'百江渚', N'吞噬魔法、能量补给、小型结界', '702', N'分配至岛屿', 'Images/702.png', @island2ID, @batch5ID, N'外表如同小学生的魔法少女，性格纯真可爱，极度喜欢甜点。拥有吞噬魔法的特殊能力，能为同伴提供能量补给，看似弱小，却在战斗中发挥着重要的辅助作用，是团队中的"小太阳"。'),

(N'环伊吕波', N'净化魔法、魔力护盾、同伴强化', '703', N'分配至岛屿', 'Images/703.png', @island2ID, @batch5ID, N'神滨市魔法少女小队的队长，性格温柔且富有责任感。从最初的迷茫逐渐成长为可靠的领导者，魔法擅长净化与同伴强化，始终将同伴的安全放在首位，用温柔的力量守护城市。'),

(N'七海八千代', N'水流操控、防御结界、魔力箭雨', '704', N'分配至岛屿', 'Images/704.png', @island2ID, @batch5ID, N'神滨市经验丰富的魔法少女前辈，性格冷静理智，战斗风格沉稳。擅长水流魔法与结界构建，战斗经验丰富，始终保持着谨慎的态度，默默守护着神滨市与后辈，内心深处渴望同伴的信任。'),

(N'深月菲莉希', N'力量强化、近战重击、快速恢复', '705', N'分配至岛屿', 'Images/705.png', @island2ID, @batch5ID, N'活泼开朗的魔法少女，热爱战斗与美食，擅长近战突袭与力量强化。性格直率，喜欢和同伴打闹，战斗时充满活力，看似大大咧咧，实则非常重视同伴，愿意为保护他人全力以赴。'),

(N'天音月夜', N'治愈魔法、同伴强化、情绪感知', '706', N'分配至岛屿', 'Images/706.png', @island2ID, @batch5ID, N'性格温柔内敛的魔法少女，擅长治愈与辅助强化魔法。不擅长表达自己的想法，却总是默默为同伴付出，能敏锐感知他人情绪，用温柔的魔法治愈同伴的伤痛，是团队中不可或缺的辅助力量。'),

(N'水波玲奈', N'水流操控、水盾防御、水中强化、水弹攻击', '707', N'分配至岛屿', 'Images/707.png', @island2ID, @batch5ID, N'开朗活泼的魔法少女，热爱水相关的一切，擅长水属性魔法与水中战斗。在水中能发挥出超强的战斗力，性格乐观，喜欢和同伴一起行动，用灵动的水魔法守护城市与海洋的和平。'),

(N'由比鹤乃', N'火焰操控、火焰冲击、近战火刃、鼓舞强化', '708', N'分配至岛屿', 'Images/708.png', @island2ID, @batch5ID, N'热情活力的魔法少女，像太阳一样温暖耀眼，擅长火属性魔法与近战攻击。性格乐观积极，总是主动鼓励同伴，用火焰般的热情感染身边的人，在战斗中勇往直前，守护城市的和平与大家的笑容。'),

(N'柊舞缇娜', N'邪恶魔法：暗黑变身、束缚枷锁、魔力吸收、支配之鞭', '709', N'审判中', 'Images/709.png', @island2ID, @batch5ID, N'邪恶组织艾诺尔米塔的新任总帅，最初渴望成为魔法少女。她因被组织欺骗而被迫成为干部，却在战斗中发现了自己的真实欲望。她享受看魔法少女痛苦挣扎的快感，这让她感到既兴奋又困惑。她的邪恶并非源于恶意，而是源于对真实自我的迷茫与探寻。她代表了那些被命运欺骗、却在黑暗中找到光芒的灵魂。');

PRINT '✓ 批次5魔女基础信息导入完成';

-- 3. 为批次5魔女创建用户账号
DECLARE @roleWitch INT = (SELECT RoleID FROM wt.Role WHERE Name = N'Witch');

INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore)
VALUES
('697', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch5ID, 0),
('698', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch5ID, 0),
('699', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch5ID, 0),
('700', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch5ID, 0),
('701', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch5ID, 0),
('702', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch5ID, 0),
('703', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch5ID, 0),
('704', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch5ID, 0),
('705', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch5ID, 0),
('706', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch5ID, 0),
('707', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch5ID, 0),
('708', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch5ID, 0),
('709', N'PENDING', N'PENDING', @roleWitch, @island2ID, @batch5ID, 0);

PRINT '✓ 批次5用户账号创建完成';

-- 4. 建立用户-魔女关联关系
INSERT INTO wt.UserWitch(UserID, WitchID)
SELECT u.UserID, w.WitchID
FROM wt.[User] u
JOIN wt.Witch w ON w.PrisonerNo = u.Username
WHERE u.Username IN ('697','698','699','700','701','702','703','704','705','706','707','708','709');

PRINT '✓ 批次5用户-魔女关联关系建立完成';

-- 5. 更新批次5的魔女数量
UPDATE wt.Batch SET WitchCount = (SELECT COUNT(*) FROM wt.Witch WHERE BatchID = @batch5ID) WHERE BatchID = @batch5ID;

PRINT '✓ 批次5魔女数量更新完成';

-- 6. 补充批次5魔女的详细档案信息

-- 6.1 鹿目圆 (697)
UPDATE wt.Witch
SET 
    PersonalNo = '1001-0305-0818',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2003-10-03',
    Ethnicity = '大和民族',
    Birthplace = '日本见泷原市',
    Height = 152.00,
    Weight = 45.00,
    BloodType = 'A',
    Address = '见泷原市某住宅区',
    Phone = '078-1234-5678',
    Email = 'kaname_madoka@example.com',
    LineAccount = 'madoka_1003',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[
        {
            "school": "见泷原市立中学",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "性格温柔善良，乐于助人，班级人缘好"
        },
        {
            "school": "见泷原市立高等学校",
            "degree": "高等学校",
            "status": "在读",
            "specialNote": "高中期间成为魔法少女，兼顾学业与战斗"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2021年至今",
            "company": "魔法少女协会",
            "position": "魔法少女；对抗魔女、守护城市",
            "salary": "无薪（志愿）",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '鹿目知久，46岁，公司职员，见泷原市某株式会社',
    Mother = '鹿目询子，44岁，家庭主妇',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '共情能力强、领导力、牺牲精神',
    Hobbies = '阅读、泡茶、照顾家人',
    Dreams = '守护见泷原市的和平与同伴',
    Dislikes = '魔女、绝望、离别',
    Trauma = '目睹同伴牺牲，因责任与愧疚陷入自我压力',
    WitchTransformMethod = '无',
    Remarks = '魔法少女小队核心，性格温柔'
WHERE PrisonerNo = '697';

-- 6.2 晓美焰 (698)
UPDATE wt.Witch
SET 
    PersonalNo = '1001-0305-0819',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2003-02-22',
    Ethnicity = '大和民族',
    Birthplace = '日本见泷原市',
    Height = 159.00,
    Weight = 42.00,
    BloodType = 'AB',
    Address = '见泷原市某住宅区',
    Phone = '078-2345-6789',
    Email = 'akemi_homura@example.com',
    LineAccount = 'homura_0222',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[
        {
            "school": "见泷原市立中学",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "转校生，性格内向，做事严谨，擅长制定计划"
        },
        {
            "school": "见泷原市立高等学校",
            "degree": "高等学校",
            "status": "在读",
            "specialNote": "为拯救鹿目圆多次回溯时间，隐藏过往经历"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2021年至今",
            "company": "魔法少女协会",
            "position": "魔法少女；时间操控、近战突击",
            "salary": "无薪（志愿）",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '晓美隆，47岁，研究员，见泷原市科研机构',
    Mother = '晓美瑞穗，45岁，教师，见泷原市某中学',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '时间操控、枪械使用、格斗术、战略规划',
    Hobbies = '独处、阅读、锻炼',
    Dreams = '拯救鹿目圆，改变绝望命运',
    Dislikes = '失控的时间、同伴牺牲',
    Trauma = '多次时间回溯的痛苦记忆，害怕再次失去同伴',
    WitchTransformMethod = '无',
    Remarks = '沉默寡言，擅长规划'
WHERE PrisonerNo = '698';

-- 6.3 巴麻美 (699)
UPDATE wt.Witch
SET 
    PersonalNo = '1001-0305-0820',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2002-06-05',
    Ethnicity = '大和民族',
    Birthplace = '日本见泷原市',
    Height = 165.00,
    Weight = 50.00,
    BloodType = 'A',
    Address = '见泷原市某公寓',
    Phone = '078-3456-7890',
    Email = 'tomoe_mami@example.com',
    LineAccount = 'mami_0605',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[
        {
            "school": "见泷原市立中学",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "成绩优异，气质优雅，主动保护后辈魔法少女"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2020年至今",
            "company": "魔法少女协会",
            "position": "魔法少女；远程攻击、团队引导",
            "salary": "无薪（志愿）",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '巴麻刚，48岁，企业家，见泷原市贸易公司',
    Mother = '巴麻道子，46岁，设计师，自由职业',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '缎带魔法操控、枪械魔力转换、领导力',
    Hobbies = '品茶、园艺、制作点心',
    Dreams = '成为可靠的前辈，守护后辈与城市',
    Dislikes = '孤独、背叛、魔法失控',
    Trauma = '曾失去同伴，内心隐藏着对孤独的恐惧',
    WitchTransformMethod = '无',
    Remarks = '成熟稳重，魔法少女前辈'
WHERE PrisonerNo = '699';

-- 6.4 美树沙耶香 (700)
UPDATE wt.Witch
SET 
    PersonalNo = '1001-0305-0821',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2003-10-06',
    Ethnicity = '大和民族',
    Birthplace = '日本见泷原市',
    Height = 157.00,
    Weight = 42.00,
    BloodType = 'B',
    Address = '见泷原市某住宅区',
    Phone = '078-4567-8901',
    Email = 'mitsuki_sayaka@example.com',
    LineAccount = 'sayaka_1006',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[
        {
            "school": "见泷原市立中学",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "运动神经发达，喜欢帮助他人，有强烈的正义感"
        },
        {
            "school": "见泷原市立高等学校",
            "degree": "高等学校",
            "status": "在读",
            "specialNote": "为守护青梅竹马成为魔法少女，重视友情与正义"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2021年至今",
            "company": "魔法少女协会",
            "position": "魔法少女；近战格斗、sword魔法",
            "salary": "无薪（志愿）",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '美树健，45岁，公务员，见泷原市政厅',
    Mother = '美树静，43岁，护士，见泷原市医院',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '剑术格斗、快速移动、正义感强',
    Hobbies = '练习剑道、听音乐、和朋友相处',
    Dreams = '成为守护他人的英雄，坚守正义',
    Dislikes = '谎言、背叛、懦弱',
    Trauma = '因正义观与现实冲突，曾陷入自我怀疑',
    WitchTransformMethod = '无',
    Remarks = '热爱正义，憧憬英雄'
WHERE PrisonerNo = '700';

-- 6.5 佐仓杏子 (701)
UPDATE wt.Witch
SET 
    PersonalNo = '1001-0305-0822',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2002-11-03',
    Ethnicity = '大和民族',
    Birthplace = '日本见泷原市近郊',
    Height = 156.00,
    Weight = 41.00,
    BloodType = 'O',
    Address = '见泷原市某街区',
    Phone = '078-5678-9012',
    Email = 'sakura_kyoko@example.com',
    LineAccount = 'kyoko_1103',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[
        {
            "school": "见泷原市近郊中学",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "性格叛逆，独自生活，擅长在困境中生存"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2019年至今",
            "company": "魔法少女协会",
            "position": "魔法少女；锁链攻击、生存战斗",
            "salary": "无薪（志愿）",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '核心成员为父母（已故）',
    Father = '佐仓浩，已故，原公司职员',
    Mother = '佐仓美咲，已故，原家庭主妇',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '锁链魔法、高速移动、生存技巧、近战突袭',
    Hobbies = '吃零食、独自行动、看漫画',
    Dreams = '按自己的意愿活下去，保护值得的人',
    Dislikes = '规则束缚、虚伪、被欺骗',
    Trauma = '父母去世后独自生存，对他人保持警惕',
    WitchTransformMethod = '无',
    Remarks = '独来独往，擅长生存'
WHERE PrisonerNo = '701';

-- 6.6 百江渚 (702)
UPDATE wt.Witch
SET 
    PersonalNo = '1001-0305-0823',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2005-08-12',
    Ethnicity = '大和民族',
    Birthplace = '日本见泷原市',
    Height = 110.00,
    Weight = 25.00,
    BloodType = 'O',
    Address = '见泷原市某结界',
    Phone = '078-6789-0123',
    Email = 'momoe_nagisa@example.com',
    LineAccount = 'nagisa_0812',
    HighestEducation = '小学校在读',
    EducationHistory = N'[
        {
            "school": "见泷原市立小学",
            "degree": "小学校",
            "status": "在读",
            "specialNote": "外表像小学生，性格纯真，喜欢甜点"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2022年至今",
            "company": "魔法少女协会",
            "position": "魔法少女；吞噬魔法、辅助战斗",
            "salary": "无薪（志愿）",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '百江俊介，43岁，甜点师，见泷原市甜品店',
    Mother = '百江优子，41岁，店员，见泷原市甜品店',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '魔法吞噬、能量转化、辅助治愈、伪装',
    Hobbies = '吃甜点、玩耍、帮助他人',
    Dreams = '吃到所有美味甜点，和同伴快乐相处',
    Dislikes = '苦味食物、孤独、魔女',
    Trauma = '害怕独自面对危险，依赖同伴的陪伴',
    WitchTransformMethod = '无',
    Remarks = '外表年幼，内心纯真'
WHERE PrisonerNo = '702';

-- 6.7 环伊吕波 (703)
UPDATE wt.Witch
SET 
    PersonalNo = '1001-0305-0824',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2003-08-22',
    Ethnicity = '大和民族',
    Birthplace = '日本神滨市',
    Height = 154.00,
    Weight = 44.00,
    BloodType = 'A',
    Address = '神滨市某住宅区',
    Phone = '06-7890-1234',
    Email = 'tamaki_irubo@example.com',
    LineAccount = 'irubo_0822',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[
        {
            "school": "神滨市立中学",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "性格温柔，责任感强，逐渐成长为魔法少女小队队长"
        },
        {
            "school": "神滨市立高等学校",
            "degree": "高等学校",
            "status": "在读",
            "specialNote": "在神滨市组建魔法少女小队，对抗特殊魔女"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2021年至今",
            "company": "魔法少女协会神滨支部",
            "position": "魔法少女；小队队长、净化辅助",
            "salary": "无薪（志愿）",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '环健太，47岁，公司职员，神滨市某株式会社',
    Mother = '环顺子，45岁，家庭主妇',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '团队协调、净化魔法、魔力感知、领导力',
    Hobbies = '照顾家人、园艺、烹饪',
    Dreams = '守护神滨市的和平，保护同伴不受伤害',
    Dislikes = '魔女、分离、无力感',
    Trauma = '曾因无力保护同伴陷入自责，后逐渐变得坚韧',
    WitchTransformMethod = '无',
    Remarks = '温柔坚韧，团队核心'
WHERE PrisonerNo = '703';

-- 6.8 七海八千代 (704)
UPDATE wt.Witch
SET 
    PersonalNo = '1001-0305-0825',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2002-11-07',
    Ethnicity = '大和民族',
    Birthplace = '日本神滨市',
    Height = 167.00,
    Weight = 48.00,
    BloodType = 'AB',
    Address = '神滨市某公寓',
    Phone = '06-8901-2345',
    Email = 'nanami_yachiyo@example.com',
    LineAccount = 'yachiyo_1107',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[
        {
            "school": "神滨市立中学",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "魔法少女前辈，战斗经验丰富，性格冷静理智"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2019年至今",
            "company": "魔法少女协会神滨支部",
            "position": "魔法少女；远程攻击、结界构建",
            "salary": "无薪（志愿）",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '七海功，49岁，建筑师，神滨市建筑公司',
    Mother = '七海美绪，47岁，律师，神滨市法律事务所',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '水流魔法、结界构建、远程攻击、战斗规划',
    Hobbies = '品茶、阅读、独自训练',
    Dreams = '保护神滨市，引导后辈魔法少女',
    Dislikes = '鲁莽、无谋的战斗、背叛',
    Trauma = '曾失去同伴，变得谨慎冷静，不轻易相信他人',
    WitchTransformMethod = '无',
    Remarks = '冷静成熟，经验丰富'
WHERE PrisonerNo = '704';

-- 6.9 深月菲莉希 (705)
UPDATE wt.Witch
SET 
    PersonalNo = '1001-0305-0826',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2003-02-17',
    Ethnicity = '大和民族',
    Birthplace = '日本见泷原市',
    Height = 153.00,
    Weight = 40.00,
    BloodType = 'O',
    Address = '见泷原市某街区',
    Phone = '078-7890-1234',
    Email = 'fukutsuki_felicia@example.com',
    LineAccount = 'felicia_0217',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[
        {
            "school": "见泷原市立中学",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "性格活泼，喜欢战斗，擅长近战突袭"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2020年至今",
            "company": "魔法少女协会",
            "position": "魔法少女；近战突袭、力量强化",
            "salary": "无薪（志愿）",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '深月洋，46岁，运动员，见泷原市体育协会',
    Mother = '深月百合，44岁，教练，见泷原市体育协会',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '力量强化、近战格斗、快速反应、突袭技巧',
    Hobbies = '吃美食、战斗训练、和朋友打闹',
    Dreams = '享受战斗的快乐，保护同伴与城市',
    Dislikes = '无聊、懦弱、魔女',
    Trauma = '无明显严重创伤，天生热爱战斗与冒险',
    WitchTransformMethod = '无',
    Remarks = '活泼开朗，热爱战斗'
WHERE PrisonerNo = '705';

-- 6.10 天音月夜 (706)
UPDATE wt.Witch
SET 
    PersonalNo = '1001-0305-0827',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2003-12-18',
    Ethnicity = '大和民族',
    Birthplace = '日本见泷原市周边',
    Height = 160.00,
    Weight = 46.00,
    BloodType = 'A',
    Address = '见泷原市某住宅区',
    Phone = '078-8901-2345',
    Email = 'amane_tsukiyo@example.com',
    LineAccount = 'tsukiyo_1218',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[
        {
            "school": "见泷原市周边中学",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "性格温柔内敛，不擅长表达，擅长辅助魔法"
        },
        {
            "school": "见泷原市立高等学校",
            "degree": "高等学校",
            "status": "在读",
            "specialNote": "默默支持同伴，在战斗中负责辅助与治疗"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2021年至今",
            "company": "魔法少女协会",
            "position": "魔法少女；治愈魔法、辅助强化",
            "salary": "无薪（志愿）",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '天音健，45岁，公务员，见泷原市周边市政厅',
    Mother = '天音美雪，43岁，护士，见泷原市周边医院',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '治愈魔法、魔力强化、感知他人情绪、辅助战斗',
    Hobbies = '听音乐、阅读、照顾小动物',
    Dreams = '用辅助魔法帮助同伴，守护大家的笑容',
    Dislikes = '冲突、吵闹、他人受伤',
    Trauma = '看到同伴受伤会陷入自责，渴望变强保护他人',
    WitchTransformMethod = '无',
    Remarks = '温柔内敛，擅长辅助'
WHERE PrisonerNo = '706';

-- 6.11 水波玲奈 (707)
UPDATE wt.Witch
SET 
    PersonalNo = '1001-0305-0828',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2003-12-27',
    Ethnicity = '大和民族',
    Birthplace = '日本见泷原市',
    Height = 158.00,
    Weight = 43.00,
    BloodType = 'B',
    Address = '见泷原市某住宅区',
    Phone = '078-9012-3456',
    Email = 'minami_reina@example.com',
    LineAccount = 'reina_1227',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[
        {
            "school": "见泷原市立中学",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "性格开朗，喜欢水相关的事物，擅长水属性魔法"
        },
        {
            "school": "见泷原市立高等学校",
            "degree": "高等学校",
            "status": "在读",
            "specialNote": "在水中战斗能力极强，喜欢和同伴一起行动"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2021年至今",
            "company": "魔法少女协会",
            "position": "魔法少女；水属性魔法、水中战斗",
            "salary": "无薪（志愿）",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '水波洋介，47岁，渔民，见泷原市渔业协会',
    Mother = '水波由纪子，45岁，家庭主妇',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '水属性魔法操控、水中高速移动、水盾防御、水流攻击',
    Hobbies = '游泳、钓鱼、玩水、和朋友野餐',
    Dreams = '成为水中最强的魔法少女，守护海洋与城市',
    Dislikes = '干旱、火焰魔法、被束缚',
    Trauma = '无明显严重创伤，天生热爱水与自由',
    WitchTransformMethod = '无',
    Remarks = '开朗活泼，擅长水魔法'
WHERE PrisonerNo = '707';

-- 6.12 由比鹤乃 (708)
UPDATE wt.Witch
SET 
    PersonalNo = '1001-0305-0829',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2003-08-07',
    Ethnicity = '大和民族',
    Birthplace = '日本见泷原市',
    Height = 157.00,
    Weight = 45.00,
    BloodType = 'A',
    Address = '见泷原市某住宅区',
    Phone = '078-0123-4567',
    Email = 'yuhi_tsuruno@example.com',
    LineAccount = 'tsuruno_0807',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[
        {
            "school": "见泷原市立中学",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "性格热情活力，像太阳一样温暖，擅长火属性魔法"
        },
        {
            "school": "见泷原市立高等学校",
            "degree": "高等学校",
            "status": "在读",
            "specialNote": "积极主动参与战斗，总是鼓励同伴"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2021年至今",
            "company": "魔法少女协会",
            "position": "魔法少女；火属性魔法、近战攻击",
            "salary": "无薪（志愿）",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '由比正，46岁，商人，见泷原市贸易公司',
    Mother = '由比静香，44岁，设计师，见泷原市设计事务所',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '火属性魔法、近战格斗、热情感染力、鼓舞同伴',
    Hobbies = '运动、吃美食、鼓励他人、参加活动',
    Dreams = '用火焰般的热情守护同伴与城市，传递快乐',
    Dislikes = '寒冷、消极、魔女',
    Trauma = '无明显严重创伤，天生热情乐观，充满活力',
    WitchTransformMethod = '无',
    Remarks = '热情活力，擅长火魔法'
WHERE PrisonerNo = '708';

-- 6.13 柊舞缇娜 (709) - 注意这个与684重复，需要特殊处理
UPDATE wt.Witch
SET 
    PersonalNo = '1001-0305-0817',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2008-03-05',
    Ethnicity = '大和民族',
    Birthplace = '东京都',
    Height = 158.00,
    Weight = 45.00,
    BloodType = 'O',
    Address = '东京都新宿区歌舞伎町1丁目',
    Phone = '03-3355-6677',
    Email = 'mttn_hiiragi@example.com',
    LineAccount = 'matina_0305',
    HighestEducation = '中学校在读',
    EducationHistory = N'[
        {
            "school": "东京都立圣樱中学校",
            "degree": "中学校",
            "status": "2年生（初二）在读",
            "specialNote": "成绩中等，在校表现普通"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2023年至今",
            "company": "邪恶组织艾诺尔米塔",
            "position": "女干部；对抗魔法少女",
            "salary": "时薪1100日元",
            "resignReason": "在职"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '柊一郎，48岁，系统工程师，东京システム株式会社，年收800万',
    Mother = '柊由美，45岁，花店店主，フラワーショップ「ゆめ」，年收400万',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '暗黑魔力操纵、变身、调教、伪装',
    Hobbies = '收集魔法少女周边、观看动画、品尝甜品',
    Dreams = '以恶役身份享受与魔法少女的战斗',
    Dislikes = '虚伪的正义、破坏手办',
    Trauma = '首次战斗发现享受施虐快感',
    WitchTransformMethod = '无',
    Remarks = '表面上是普通学生，真实身份是邪恶组织女干部'
WHERE PrisonerNo = '709';

PRINT '✓ 批次5魔女详细档案补充完成';

PRINT '=== 批次5魔女完整导入脚本执行完成 ===';
PRINT '✓ 已导入13位魔女（697-709）';
PRINT '✓ 包含完整的28个扩展字段';
PRINT '✓ 用户账号和关联关系已建立';
PRINT '✓ 批次信息已更新';
PRINT '';
PRINT '下一步：';
PRINT '1. 导入头像图片文件（697.png - 709.png）';
PRINT '2. 设置用户密码（使用Security.cs）';
PRINT '3. 测试批次5魔女账号登录';
PRINT '4. 验证魔女图鉴显示完整性';
