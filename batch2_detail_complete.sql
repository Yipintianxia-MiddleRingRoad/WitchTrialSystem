-- ========================================
-- 批次2 - 完整详细信息导入脚本
-- 包含13位魔女的全部详细信息（671-683）
-- ========================================

USE WitchTrialWT;
GO

PRINT '';
PRINT '╔════════════════════════════════════════╗';
PRINT '║   批次2 - 详细信息导入开始             ║';
PRINT '║   13位魔女（671-683）                 ║';
PRINT '╚════════════════════════════════════════╝';
PRINT '';
GO

-- ========================================
-- 671 小鸟游六花
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '5678-1234-0987',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2001-06-12',
    Ethnicity = N'大和民族',
    Birthplace = N'富山县',
    Height = 150.00,
    Weight = 47.00,
    BloodType = N'AB',
    Address = N'富山县中央市大字山田町',
    Phone = '076-555-6677',
    Email = 'takanashi_rikka@example.com',
    LineAccount = 'rikka_chan0612',
    HighestEducation = N'高中在读',
    EducationHistory = N'[
        {
            "school": "富山县立大森中学",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "初中时期因父亲去世陷入中二病，社交圈较窄"
        },
        {
            "school": "私立银杏学园高等学校",
            "degree": "高等学校",
            "status": "在读",
            "specialNote": "进入高中后仍保持中二设定，与同伴形成社团"
        }
    ]',
    WorkHistory = N'[]',
    FamilyStructure = N'父亲已故，核心成员为母姐',
    Father = N'小鸟游宗太（已故，原建筑师）',
    Mother = N'小鸟游澄子（家庭主妇）',
    OtherFamily1 = N'姐姐：小鸟游十花（职业厨师）',
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = N'中二病设定构建（漆黑泽克斯原始型第二自动伞使用、幻想战斗技能）、观察力敏锐（对"不可视境界线"的感知）',
    Hobbies = N'中二行为扮演、收集单边眼罩、动漫游相关',
    Dreams = N'找到"不可视境界线"，解开世界真相',
    Dislikes = N'常识、被当成"奇怪的人"',
    Trauma = N'父亲去世后因无法接受现实，通过中二病逃避，形成心理防御机制',
    WitchTransformMethod = N'无',
    Remarks = N'无'
WHERE PrisonerNo = '671';
PRINT '✅ 671 小鸟游六花 详细信息已更新';
GO

-- ========================================
-- 672 富樫勇太
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '5678-1234-0988',
    FormerName = N'无',
    Gender = N'男',
    BirthDate = '2001-08-01',
    Ethnicity = N'大和民族',
    Birthplace = N'千叶县',
    Height = 170.00,
    Weight = 60.00,
    BloodType = N'O',
    Address = N'千叶县千叶市中央区',
    Phone = '043-1234-5678',
    Email = 'togashi_yuta@example.com',
    LineAccount = 'yuta_togashi0801',
    HighestEducation = N'高中在读',
    EducationHistory = N'[
        {
            "school": "私立银杏学园中等部",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "初中时因中二病被孤立，社交恐惧"
        },
        {
            "school": "私立银杏学园高等学校",
            "degree": "高等学校",
            "status": "在读",
            "specialNote": "与六花等人组成远东魔法午睡结社之夏"
        }
    ]',
    WorkHistory = N'[]',
    FamilyStructure = N'核心成员为父母',
    Father = N'富樫幸太郎，48岁，公司职员',
    Mother = N'富樫育江，46岁，家庭主妇',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = N'社交能力（隐藏中二后）、家务熟练、情绪管理（对六花）',
    Hobbies = N'游戏、阅读、隐藏中二痕迹',
    Dreams = N'成为普通的社会人',
    Dislikes = N'中二黑历史被揭露',
    Trauma = N'初中中二病被孤立，产生社交恐惧，极力隐藏过往',
    WitchTransformMethod = N'无',
    Remarks = N'无'
WHERE PrisonerNo = '672';
PRINT '✅ 672 富樫勇太 详细信息已更新';
GO

-- ========================================
-- 673 丹生谷森夏
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '5678-1234-0989',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2001-12-20',
    Ethnicity = N'大和民族',
    Birthplace = N'千叶县',
    Height = 165.00,
    Weight = 57.00,
    BloodType = N'A',
    Address = N'千叶县千叶市美滨区',
    Phone = '043-2345-6789',
    Email = 'niwa_takanashi@example.com',
    LineAccount = 'takanashi_niwa1220',
    HighestEducation = N'高中在读',
    EducationHistory = N'[
        {
            "school": "私立银杏学园中等部",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "成绩优异，班级干部，隐藏中二过往"
        },
        {
            "school": "私立银杏学园高等学校",
            "degree": "高等学校",
            "status": "在读",
            "specialNote": "负责管理结社，协调成员关系"
        }
    ]',
    WorkHistory = N'[]',
    FamilyStructure = N'核心成员为父母',
    Father = N'丹生谷修，47岁，公司高管',
    Mother = N'丹生谷芙美，45岁，教师',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = N'班级管理、情报收集、情绪洞察',
    Hobbies = N'cosplay、社团运营、八卦',
    Dreams = N'成为完美的现充领袖',
    Dislikes = N'被当成幼稚的人',
    Trauma = N'初中时的中二黑历史，努力塑造现充形象以掩盖',
    WitchTransformMethod = N'无',
    Remarks = N'无'
WHERE PrisonerNo = '673';
PRINT '✅ 673 丹生谷森夏 详细信息已更新';
GO

-- ========================================
-- 674 五月七日茴香
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '5678-1234-0990',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2002-03-25',
    Ethnicity = N'大和民族',
    Birthplace = N'千叶县',
    Height = 155.00,
    Weight = 45.00,
    BloodType = N'AB',
    Address = N'千叶县千叶市若叶区',
    Phone = '043-3456-7890',
    Email = 'itsuki_mayoi@example.com',
    LineAccount = 'mayoi_itsuki0325',
    HighestEducation = N'高中在读',
    EducationHistory = N'[
        {
            "school": "私立银杏学园中等部",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "占卜爱好者，性格温和，观察力敏锐"
        },
        {
            "school": "私立银杏学园高等学校",
            "degree": "高等学校",
            "status": "在读",
            "specialNote": "结社的占卜师，提供神秘学支持"
        }
    ]',
    WorkHistory = N'[]',
    FamilyStructure = N'核心成员为父母',
    Father = N'五月七日博臣，46岁，公务员',
    Mother = N'五月七日铃，44岁，护士',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = N'占卜、观察入微、氛围营造',
    Hobbies = N'占卜、做点心、照顾他人',
    Dreams = N'成为知名占卜师',
    Dislikes = N'不吉利的事物',
    Trauma = N'无（性格天然，无明显创伤）',
    WitchTransformMethod = N'无',
    Remarks = N'无'
WHERE PrisonerNo = '674';
PRINT '✅ 674 五月七日茴香 详细信息已更新';
GO

-- ========================================
-- 675 凸守早苗
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '5678-1234-0991',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2002-08-09',
    Ethnicity = N'大和民族',
    Birthplace = N'千叶县',
    Height = 143.00,
    Weight = 45.00,
    BloodType = N'B',
    Address = N'千叶县千叶市绿区',
    Phone = '043-4567-8901',
    Email = 'dekomori_sanae@example.com',
    LineAccount = 'sanae_dekomori0809',
    HighestEducation = N'高中在读',
    EducationHistory = N'[
        {
            "school": "私立银杏学园中等部",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "元气满满，六花的忠实随从，中二病重度"
        },
        {
            "school": "私立银杏学园高等学校",
            "degree": "高等学校",
            "status": "在读",
            "specialNote": "结社的战斗员，魔法名为"雷之征服者""
        }
    ]',
    WorkHistory = N'[]',
    FamilyStructure = N'核心成员为父母',
    Father = N'凸守源五郎，47岁，企业家',
    Mother = N'凸守早苗（母，同名）',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = N'中二设定构建、体力充沛、对六花绝对忠诚',
    Hobbies = N'中二扮演、追随六花、吃点心',
    Dreams = N'成为六花的最强骑士',
    Dislikes = N'六花被欺负、无聊',
    Trauma = N'无（天生元气，沉浸中二）',
    WitchTransformMethod = N'无',
    Remarks = N'无'
WHERE PrisonerNo = '675';
PRINT '✅ 675 凸守早苗 详细信息已更新';
GO

-- ========================================
-- 676 七宫智音
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '5678-1234-0992',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2002-01-03',
    Ethnicity = N'大和民族',
    Birthplace = N'千叶县',
    Height = 150.00,
    Weight = 40.00,
    BloodType = N'O',
    Address = N'千叶县千叶市中央区',
    Phone = '043-4567-8902',
    Email = 'shichimiya_tomo@example.com',
    LineAccount = 'tomo_shichimiya0103',
    HighestEducation = N'中学毕业',
    EducationHistory = N'[
        {
            "school": "不明中学校",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "原勇太的青梅竹马，魔法小提琴手"
        }
    ]',
    WorkHistory = N'[]',
    FamilyStructure = NULL,
    Father = NULL,
    Mother = NULL,
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = N'小提琴演奏、音乐创作、情感表达（通过音乐）',
    Hobbies = N'拉小提琴、作曲、寻找勇太',
    Dreams = N'用音乐传达心意给勇太',
    Dislikes = N'被遗忘、孤独',
    Trauma = N'被勇太遗忘的过往，内心深处的孤独感',
    WitchTransformMethod = N'无',
    Remarks = N'无'
WHERE PrisonerNo = '676';
PRINT '✅ 676 七宫智音 详细信息已更新';
GO

-- ========================================
-- 677 伊雷娜
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '9012-3456-7890',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2003-10-17',
    Ethnicity = N'大和民族',
    Birthplace = N'和平国家罗贝塔',
    Height = 150.00,
    Weight = 42.00,
    BloodType = N'O',
    Address = N'罗贝塔共和国中央区魔女大道7丁目',
    Phone = '06-7890-1234',
    Email = 'irena_witch@example.com',
    LineAccount = 'ashen_witch1017',
    HighestEducation = N'魔女最高位（等同高等教育）',
    EducationHistory = N'[
        {
            "school": "罗贝塔共和国立初等学校",
            "degree": "初等学校",
            "status": "毕业",
            "specialNote": "自幼阅读母亲著作《妮可冒险记》，憧憬魔女职业，在母亲指导下自学基础魔法"
        },
        {
            "school": "星辰魔女芙兰的修行道场",
            "degree": "魔女认证",
            "status": "毕业",
            "specialNote": "14岁成为见习魔女，1年完成修行，15岁获魔女名"灰之魔女"，随即开启环游世界之旅"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2018年至今",
            "company": "无（自由职业）",
            "position": "旅行魔女、游记撰写者，偶尔承接魔法委托",
            "salary": "不定（魔法服务/游记稿费/合理盈利手段）",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = N'核心成员为父母，家庭氛围和睦',
    Father = N'伊雷娜·马克斯，50岁，商人，罗贝塔贸易株式会社，家庭年收入800万日元，备注：极度溺爱女儿',
    Mother = N'维多利加，48岁，前魔女旅行者，无固定单位，备注：《妮可冒险记》作者，魔女名"灰之魔女"',
    OtherFamily1 = N'师父：芙兰（星辰魔女），32岁，魔女导师',
    OtherFamily2 = N'好友：沙耶（扫帚魔女），同龄，魔法统合协会职员',
    OtherFamily3 = N'好友：艾姆妮西亚（魔法使），拥有记忆相关能力',
    Skills = N'精通各类魔法、厨艺（炖菜专长）、旅行规划、多语言沟通、游记撰写，逻辑思维缜密',
    Hobbies = N'旅行、阅读、品尝牛角面包、撰写游记、收集各地特色小物件',
    Dreams = N'环游世界，记录所有国家的故事与风景，理解"世界的多样性"',
    Dislikes = N'菇类、猫（过敏）、下雨、不礼貌的人、被过度干涉自由',
    Trauma = N'旅行中目睹过多人性的复杂与矛盾，对"永恒美好"有理性认知，无严重创伤',
    WitchTransformMethod = N'无',
    Remarks = N'15岁获最高位魔女称号的天才'
WHERE PrisonerNo = '677';
PRINT '✅ 677 伊雷娜 详细信息已更新';
GO

-- ========================================
-- 678 维多利加
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '4321-8765-2345',
    FormerName = N'妮可（笔名）',
    Gender = N'女',
    BirthDate = '1977-05-22',
    Ethnicity = N'大和民族',
    Birthplace = N'和平国家罗贝塔',
    Height = 158.00,
    Weight = 47.00,
    BloodType = N'O',
    Address = N'罗贝塔共和国中央区魔女大道7丁目',
    Phone = '06-7890-1235',
    Email = 'victorica_nicole@example.com',
    LineAccount = 'nicole_adventure0522',
    HighestEducation = N'魔女最高位（等同博士学历）',
    EducationHistory = N'[
        {
            "school": "罗贝塔共和国立孤儿院附属学校",
            "degree": "初等教育",
            "status": "毕业",
            "specialNote": "故乡遭洪水摧毁后入住孤儿院，8岁被发现魔法天赋，展现出惊人学习能力"
        },
        {
            "school": "白之魔女修行道场",
            "degree": "魔女认证",
            "status": "毕业",
            "specialNote": "10岁擅自出国旅行，被白之魔女搭救后拜师，15岁成为见习魔女，18岁获魔女称号，以"妮可"为名撰写《妮可冒险记》"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "1995-2008年",
            "company": "无（自由职业）",
            "position": "旅行魔女、魔法导师，培养芙兰与希拉两名弟子",
            "salary": "高额（魔法委托/著作版权费）",
            "resignReason": "为陪伴待产的丈夫与幼年伊雷娜，选择回归家庭"
        }
    ]',
    FamilyStructure = N'核心成员为丈夫与女儿，曾是传奇旅行魔女，现为家庭主妇',
    Father = N'姓名不详（已故，原农夫，洪水灾害中遇难）',
    Mother = N'姓名不详（已故，原家庭主妇，洪水灾害中遇难）',
    OtherFamily1 = N'弟子：芙兰（星辰魔女），32岁，魔女导师',
    OtherFamily2 = N'弟子：希拉（暗夜魔女），31岁，旅行魔女',
    OtherFamily3 = N'女儿：伊雷娜（灰之魔女），20岁，旅行魔女',
    Skills = N'精通高阶魔法、魔法教学、游记撰写、商业谈判（擅长盈利）、危机应对',
    Hobbies = N'撰写旅行故事、指导后辈魔法、品尝甜食、捉弄丈夫与女儿',
    Dreams = N'通过文字与弟子传承魔女精神，守护女儿的旅行梦想',
    Dislikes = N'菇类、浪费钱财、无礼之人、被人识破真实身份（早期）',
    Trauma = N'幼年经历洪水失去双亲，留下对"离别"的隐性敏感，后通过旅行与写作释怀',
    WitchTransformMethod = N'无',
    Remarks = N'伊雷娜之母，芙兰与希拉的师父'
WHERE PrisonerNo = '678';
PRINT '✅ 678 维多利加 详细信息已更新';
GO

-- ========================================
-- 679 沙耶
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '5678-9012-6789',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2003-08-15',
    Ethnicity = N'大和民族',
    Birthplace = N'极东地区',
    Height = 155.00,
    Weight = 45.00,
    BloodType = N'B',
    Address = N'极东地区咲樱市魔法町3丁目',
    Phone = '03-4567-8901',
    Email = 'saya_broom@example.com',
    LineAccount = 'broom_witch0815',
    HighestEducation = N'魔女中位（等同高等教育）',
    EducationHistory = N'[
        {
            "school": "极东地区咲樱市立中学",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "普通家庭出身，偶然目睹伊雷娜飞行魔法后立志成为魔女，学习刻苦但缺乏天赋"
        },
        {
            "school": "极东魔法协会修行道场",
            "degree": "魔女认证",
            "status": "毕业",
            "specialNote": "受伊雷娜鼓励坚持修行，开发出专属扫帚魔法，20岁获魔女名"扫帚魔女"，擅长团队协作"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2023年至今",
            "company": "魔法统合协会极东支部",
            "position": "魔女职员，负责魔法事件协调与新人指导",
            "salary": "固定月薪35万日元",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = N'核心成员为父母，性格开朗热情，重视友情',
    Father = N'沙耶一郎，52岁，公务员，极东地区市政厅，家庭年收入600万日元',
    Mother = N'沙耶良子，50岁，护士，咲樱市综合医院，家庭年收入450万日元',
    OtherFamily1 = N'挚友：伊雷娜（灰之魔女），20岁，旅行魔女',
    OtherFamily2 = N'同事：艾姆妮西亚，21岁，魔法统合协会职员',
    OtherFamily3 = NULL,
    Skills = N'扫帚魔法专精、团队协调、新人指导、家常菜烹饪，耐力极强',
    Hobbies = N'擦拭扫帚、练习飞行魔法、与伊雷娜通信、制作便当',
    Dreams = N'成为极东地区最优秀的魔女，与伊雷娜一起旅行世界',
    Dislikes = N'被人说"平庸"、魔法失误、与伊雷娜久别不见',
    Trauma = N'曾因魔法天赋不足被嘲笑，产生自我怀疑，后在伊雷娜鼓励下建立自信',
    WitchTransformMethod = N'无',
    Remarks = N'伊雷娜挚友，"扫帚魔女"'
WHERE PrisonerNo = '679';
PRINT '✅ 679 沙耶 详细信息已更新';
GO

-- ========================================
-- 680 芙兰
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '2143-6587-9012',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '1993-03-10',
    Ethnicity = N'大和民族',
    Birthplace = N'和平国家罗贝塔',
    Height = 160.00,
    Weight = 48.00,
    BloodType = N'A',
    Address = N'罗贝塔共和国郊外星辰道场',
    Phone = '06-1234-5678',
    Email = 'fran_star@example.com',
    LineAccount = 'star_witch0310',
    HighestEducation = N'魔女最高位（等同高等教育）',
    EducationHistory = N'[
        {
            "school": "罗贝塔共和国立初等学校",
            "degree": "初等学校",
            "status": "毕业",
            "specialNote": "天生拥有星辰感知力，幼时能与星空共鸣，被视为"怪孩子""
        },
        {
            "school": "维多利加修行道场",
            "degree": "魔女认证",
            "status": "毕业",
            "specialNote": "15岁拜维多利加为师，与希拉同期修行，因发色被师父故意取"星辰魔女"称号，20岁正式独立开设道场"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2013年至今",
            "company": "星辰魔女修行道场",
            "position": "魔女导师，负责培养魔女见习生，承接星辰观测相关委托",
            "salary": "不定（学费/魔法委托费）",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = N'核心成员为弟子，性格沉稳，略带懒散，重视师徒情谊',
    Father = N'芙兰·琼斯，70岁，退休教师，罗贝塔共和国公立学校，已退休',
    Mother = N'玛丽·琼斯，68岁，退休护士，罗贝塔共和国中央医院，已退休',
    OtherFamily1 = N'师父：维多利加（灰之魔女），48岁，前旅行魔女',
    OtherFamily2 = N'师姐：希拉（暗夜魔女），31岁，旅行魔女',
    OtherFamily3 = N'弟子：伊雷娜（灰之魔女），20岁，旅行魔女',
    Skills = N'星辰魔法专精、魔法教学、星空观测、预言（有限）、手工制作魔法道具',
    Hobbies = N'观测星空、酿造果酒、指导弟子、与希拉拌嘴（友好型）',
    Dreams = N'培养出更多优秀魔女，解开星辰魔法的终极秘密',
    Dislikes = N'噪音、乌云天气（无法观测星空）、希拉的恶作剧',
    Trauma = N'幼时因特殊能力被孤立，后在维多利加引导下接纳自身特质',
    WitchTransformMethod = N'无',
    Remarks = N'伊雷娜与希拉的师父，"星辰魔女"'
WHERE PrisonerNo = '680';
PRINT '✅ 680 芙兰 详细信息已更新';
GO

-- ========================================
-- 681 席拉
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '2143-6587-9013',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '1994-07-08',
    Ethnicity = N'大和民族',
    Birthplace = N'和平国家罗贝塔',
    Height = 157.00,
    Weight = 46.00,
    BloodType = N'AB',
    Address = N'罗贝塔共和国郊外暗影小屋',
    Phone = '06-1234-5679',
    Email = 'shira_dark@example.com',
    LineAccount = 'dark_witch0708',
    HighestEducation = N'魔女最高位（等同高等教育）',
    EducationHistory = N'[
        {
            "school": "罗贝塔共和国立夜校",
            "degree": "初等教育",
            "status": "毕业",
            "specialNote": "自幼习惯夜间活动，对黑暗有天然亲和力，12岁因能操控暗影被推荐至魔法界"
        },
        {
            "school": "维多利加修行道场",
            "degree": "魔女认证",
            "status": "毕业",
            "specialNote": "16岁拜维多利加为师，与芙兰同期修行，性格跳脱常捉弄芙兰，19岁获魔女名"暗夜魔女"，擅长隐秘行动"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2014年至今",
            "company": "无（自由职业）",
            "position": "旅行魔女、魔法委托承接者，擅长处理隐秘魔法事件",
            "salary": "高额（按委托难度计算，单次5-50万日元）",
            "resignReason": "无"
        }
    ]',
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

-- ========================================
-- 682 琪琪
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '1985-0202-3456',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '1985-02-02',
    Ethnicity = N'魔法民族',
    Birthplace = N'科里科',
    Height = 157.00,
    Weight = 46.00,
    BloodType = N'O',
    Address = N'科里科小镇海边面包店楼上',
    Phone = '0120-554-789',
    Email = 'kiki.delivery@example.com',
    LineAccount = 'kiki_0202',
    HighestEducation = N'初中在读',
    EducationHistory = N'[
        {
            "school": "科里科乡村魔法学校",
            "degree": "初中",
            "status": "在读",
            "specialNote": "魔法家族培养，独立生活中不断成长"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2001年至今",
            "company": "琪琪宅急便",
            "position": "快递员兼老板；运送各类物品",
            "salary": "月收入约30万日元",
            "resignReason": "创业中"
        }
    ]',
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

-- ========================================
-- 683 冰上梅露露
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9023',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2005-12-25',
    Ethnicity = N'大和民族',
    Birthplace = N'不明',
    Height = 158.00,
    Weight = 48.00,
    BloodType = NULL,  -- 表格显示"不明"
    Address = N'魔女监牢岛中央区监牢街',
    Phone = NULL,  -- 表格显示"不明"
    Email = 'mizore_meruru@yahoo.co.jp',
    LineAccount = 'meruru_mizore1225',
    HighestEducation = N'无',
    EducationHistory = N'[]',
    WorkHistory = N'[]',
    FamilyStructure = NULL,
    Father = NULL,
    Mother = NULL,
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
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

-- ========================================
-- 最终验证
-- ========================================
PRINT '';
PRINT '╔════════════════════════════════════════╗';
PRINT '║   导入完成 - 验证结果                  ║';
PRINT '╚════════════════════════════════════════╝';
PRINT '';

-- 统计已更新的魔女数量
SELECT 
    COUNT(*) AS [已更新魔女数量],
    SUM(CASE WHEN PersonalNo IS NOT NULL THEN 1 ELSE 0 END) AS [有个人编号],
    SUM(CASE WHEN BirthDate IS NOT NULL THEN 1 ELSE 0 END) AS [有出生日期],
    SUM(CASE WHEN EducationHistory IS NOT NULL THEN 1 ELSE 0 END) AS [有教育经历]
FROM wt.Witch
WHERE BatchID = 2;

-- 显示所有批次2魔女的基本信息
SELECT 
    PrisonerNo AS [囚犯编号],
    Name AS [姓名],
    PersonalNo AS [个人编号],
    Gender AS [性别],
    BirthDate AS [出生日期],
    HighestEducation AS [最高学历]
FROM wt.Witch
WHERE BatchID = 2
ORDER BY CAST(PrisonerNo AS INT);

PRINT '';
PRINT '╔════════════════════════════════════════╗';
PRINT '║   ✅ 批次2详细信息导入完成！           ║';
PRINT '║   13位魔女的详细档案已全部更新         ║';
PRINT '╚════════════════════════════════════════╝';
PRINT '';
GO
