-- ========================================
-- 批次2 - 步骤3：导入魔女详细信息（第2部分）
-- 676-680
-- ========================================

USE WitchTrialWT;
GO

PRINT '========================================';
PRINT '批次2 - 步骤3：导入详细信息（第2部分）';
PRINT '========================================';
GO

-- 676 七宫智音
UPDATE wt.Witch SET
    PersonalNo = '5678-1234-0992',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2002-01-03',
    Ethnicity = N'大和民族',
    Birthplace = N'千叶县',
    Height = 150,
    Weight = 40,
    BloodType = 'O',
    Address = N'千叶县千叶市中央区',
    Phone = '043-4567-8902',
    Email = 'shichimiya_tomo@example.com',
    LineAccount = 'tomo_shichimiya0103',
    HighestEducation = N'中学毕业',
    EducationHistory = N'不明中学校（中学校，毕业）：原勇太的青梅竹马，魔法小提琴手',
    WorkHistory = N'无',
    FamilyStructure = N'无',
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

-- 677 伊雷娜
UPDATE wt.Witch SET
    PersonalNo = '9012-3456-7890',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2003-10-17',
    Ethnicity = N'大和民族',
    Birthplace = N'和平国家罗贝塔',
    Height = 150,
    Weight = 42,
    BloodType = 'O',
    Address = N'罗贝塔共和国中央区魔女大道7丁目',
    Phone = '06-7890-1234',
    Email = 'irena_witch@example.com',
    LineAccount = 'ashen_witch1017',
    HighestEducation = N'魔女最高位（等同高等教育）',
    EducationHistory = N'罗贝塔共和国立初等学校（初等学校，毕业）：自幼阅读母亲著作《妮可冒险记》，憧憬魔女职业，在母亲指导下自学基础魔法\n星辰魔女芙兰的修行道场（魔女认证，毕业）：14岁成为见习魔女，1年完成修行，15岁获魔女名"灰之魔女"，随即开启环游世界之旅',
    WorkHistory = N'2018年至今：旅行魔女、游记撰写者，偶尔承接魔法委托',
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

-- 678 维多利加
UPDATE wt.Witch SET
    PersonalNo = '4321-8765-2345',
    FormerName = N'妮可（笔名）',
    Gender = N'女',
    BirthDate = '1977-05-22',
    Ethnicity = N'大和民族',
    Birthplace = N'和平国家罗贝塔',
    Height = 158,
    Weight = 47,
    BloodType = 'O',
    Address = N'罗贝塔共和国中央区魔女大道7丁目',
    Phone = '06-7890-1235',
    Email = 'victorica_nicole@example.com',
    LineAccount = 'nicole_adventure0522',
    HighestEducation = N'魔女最高位（等同博士学历）',
    EducationHistory = N'罗贝塔共和国立孤儿院附属学校（初等教育，毕业）：故乡遭洪水摧毁后入住孤儿院，8岁被发现魔法天赋，展现出惊人学习能力\n白之魔女修行道场（魔女认证，毕业）：10岁擅自出国旅行，被白之魔女搭救后拜师，15岁成为见习魔女，18岁获魔女称号，以"妮可"为名撰写《妮可冒险记》',
    WorkHistory = N'1995-2008年：旅行魔女、魔法导师，培养芙兰与希拉两名弟子',
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

-- 679 沙耶
UPDATE wt.Witch SET
    PersonalNo = '5678-9012-6789',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2003-08-15',
    Ethnicity = N'大和民族',
    Birthplace = N'极东地区',
    Height = 155,
    Weight = 45,
    BloodType = 'B',
    Address = N'极东地区咲樱市魔法町3丁目',
    Phone = '03-4567-8901',
    Email = 'saya_broom@example.com',
    LineAccount = 'broom_witch0815',
    HighestEducation = N'魔女中位（等同高等教育）',
    EducationHistory = N'极东地区咲樱市立中学（中学校，毕业）：普通家庭出身，偶然目睹伊雷娜飞行魔法后立志成为魔女，学习刻苦但缺乏天赋\n极东魔法协会修行道场（魔女认证，毕业）：受伊雷娜鼓励坚持修行，开发出专属扫帚魔法，20岁获魔女名"扫帚魔女"，擅长团队协作',
    WorkHistory = N'2023年至今：魔法统合协会极东支部，魔女职员，负责魔法事件协调与新人指导',
    FamilyStructure = N'核心成员为父母，性格开朗热情，重视友情',
    Father = N'沙耶一郎，52岁，公务员，极东地区市政厅，家庭年收入600万日元',
    Mother = N'沙耶良子，50岁，护士，咲樱市综合医院，家庭年收入450万日元',
    OtherFamily1 = N'挚友：伊雷娜（灰之魔女），20岁，旅行魔女',
    OtherFamily2 = N'同事：艾姆妮西亚，21岁，魔法统合协会职员',
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

-- 680 芙兰
UPDATE wt.Witch SET
    PersonalNo = '2143-6587-9012',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '1993-03-10',
    Ethnicity = N'大和民族',
    Birthplace = N'和平国家罗贝塔',
    Height = 160,
    Weight = 48,
    BloodType = 'A',
    Address = N'罗贝塔共和国郊外星辰道场',
    Phone = '06-1234-5678',
    Email = 'fran_star@example.com',
    LineAccount = 'star_witch0310',
    HighestEducation = N'魔女最高位（等同高等教育）',
    EducationHistory = N'罗贝塔共和国立初等学校（初等学校，毕业）：天生拥有星辰感知力，幼时能与星空共鸣，被视为"怪孩子"\n维多利加修行道场（魔女认证，毕业）：15岁拜维多利加为师，与希拉同期修行，因发色被师父故意取"星辰魔女"称号，20岁正式独立开设道场',
    WorkHistory = N'2013年至今：星辰魔女修行道场，魔女导师，负责培养魔女见习生，承接星辰观测相关委托',
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

PRINT '';
PRINT '========================================';
PRINT '✅ 第2部分完成！（676-680）';
PRINT '========================================';
GO
