-- ========================================
-- 批次2 - 步骤3：导入魔女详细信息（第3部分）
-- 681-683
-- ========================================

USE WitchTrialWT;
GO

PRINT '========================================';
PRINT '批次2 - 步骤3：导入详细信息（第3部分）';
PRINT '========================================';
GO

-- 681 席拉
UPDATE wt.Witch SET
    PersonalNo = '2143-6587-9013',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '1994-07-08',
    Ethnicity = N'大和民族',
    Birthplace = N'和平国家罗贝塔',
    Height = 157,
    Weight = 46,
    BloodType = 'AB',
    Address = N'罗贝塔共和国郊外暗影小屋',
    Phone = '06-1234-5679',
    Email = 'shira_dark@example.com',
    LineAccount = 'dark_witch0708',
    HighestEducation = N'魔女最高位（等同高等教育）',
    EducationHistory = N'罗贝塔共和国立夜校（初等教育，毕业）：自幼习惯夜间活动，对黑暗有天然亲和力，12岁因能操控暗影被推荐至魔法界\n维多利加修行道场（魔女认证，毕业）：16岁拜维多利加为师，与芙兰同期修行，性格跳脱常捉弄芙兰，19岁获魔女名"暗夜魔女"，擅长隐秘行动',
    WorkHistory = N'2014年至今：旅行魔女、魔法委托承接者，擅长处理隐秘魔法事件',
    FamilyStructure = N'核心成员为师父与师姐，性格活泼爱闹，关键时刻极为可靠',
    Father = N'希拉·克劳德，65岁，退休军人，罗贝塔共和国自卫队，已退休',
    Mother = N'安娜·克劳德，63岁，花店店主，罗贝塔共和国中央区花店，家庭年收入300万日元',
    OtherFamily1 = N'师父：维多利加（灰之魔女），48岁，前旅行魔女',
    OtherFamily2 = N'师妹：芙兰（星辰魔女），32岁，魔女导师',
    OtherFamily3 = N'好友：伊雷娜（灰之魔女），20岁，旅行魔女',
    Skills = N'暗影魔法专精、潜行侦查、隐秘行动、开锁技巧、危机预判',
    Hobbies = N'夜间散步、捉弄芙兰、收集暗影相关魔法道具、品尝苦味咖啡',
    Dreams = N'用暗影魔法守护弱小，与芙兰一起完成师父的嘱托',
    Dislikes = N'强光环境（影响魔法效果）、无聊等待、芙兰的"说教"',
    Trauma = N'童年因夜间出行被误解为"怪物"，产生短暂自卑，后在维多利加引导下接纳自身能力',
    WitchTransformMethod = N'无',
    Remarks = N'芙兰的师妹，"暗夜魔女"'
WHERE PrisonerNo = '681';
PRINT '✅ 681 席拉 详细信息已更新';
GO

-- 682 琪琪
UPDATE wt.Witch SET
    PersonalNo = '1985-0202-3456',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '1985-02-02',
    Ethnicity = N'魔法民族',
    Birthplace = N'科里科',
    Height = 157,
    Weight = 46,
    BloodType = 'O',
    Address = N'科里科小镇海边面包店楼上',
    Phone = '0120-554-789',
    Email = 'kiki.delivery@example.com',
    LineAccount = 'kiki_0202',
    HighestEducation = N'初中在读',
    EducationHistory = N'科里科乡村魔法学校（初中，在读）：魔法家族培养，独立生活中不断成长',
    WorkHistory = N'2001年至今：琪琪宅急便，快递员兼老板；运送各类物品',
    FamilyStructure = N'核心成员为父母及宠物黑猫',
    Father = N'索波罗（父亲），45岁，魔法研究员，魔法协会，年收入800万日元',
    Mother = N'可琪莉（母亲），43岁，魔女，独立从业，年收入600万日元',
    OtherFamily1 = N'宠物：吉吉（黑猫，能说话）',
    OtherFamily2 = N'好友：蜻蜓、索娜',
    OtherFamily3 = N'好友：乌露丝拉',
    Skills = N'飞行魔法、物品配送、空中视野、应急处理',
    Hobbies = N'飞行、观察城市、品尝美食、与吉吉聊天',
    Dreams = N'成为独立自主的成年魔女，守护人类与魔女的友谊',
    Dislikes = N'孤独、被否定、失去魔法',
    Trauma = N'第一次飞行失败导致自信心动摇',
    WitchTransformMethod = N'无',
    Remarks = N'独立创业"魔女宅急便"，13岁的年轻魔女'
WHERE PrisonerNo = '682';
PRINT '✅ 682 琪琪 详细信息已更新';
GO

-- 683 冰上梅露露
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9023',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2005-12-25',
    Ethnicity = N'大和民族',
    Birthplace = N'不明',
    Height = 158,
    Weight = 48,
    BloodType = N'不明',
    Address = N'魔女监牢岛中央区监牢街',
    Phone = N'不明',
    Email = 'mizore_meruru@yahoo.co.jp',
    LineAccount = 'meruru_mizore1225',
    HighestEducation = N'无',
    EducationHistory = N'无',
    WorkHistory = N'无',
    FamilyStructure = N'无',
    Skills = N'治愈、再生、植物培育（香料）',
    Hobbies = N'培育香料植物、独处阅读',
    Dreams = N'与大魔女重逢',
    Dislikes = N'人群、背叛',
    Trauma = N'作为幕后黑手的自责与痛苦，渴望被认可',
    WitchTransformMethod = N'无',
    Remarks = N'无'
WHERE PrisonerNo = '683';
PRINT '✅ 683 冰上梅露露 详细信息已更新';
GO

PRINT '';
PRINT '========================================';
PRINT '✅ 第3部分完成！（681-683）';
PRINT '========================================';
GO

-- 验证所有详细信息
SELECT 
    PrisonerNo,
    Name,
    PersonalNo,
    Gender,
    BirthDate,
    Height,
    Weight,
    Phone,
    Email
FROM wt.Witch
WHERE BatchID = 2
ORDER BY PrisonerNo;
GO

PRINT '';
PRINT '========================================';
PRINT '✅ 批次2所有详细信息导入完成！';
PRINT '========================================';
GO
