-- ========================================
-- 批次2 - 步骤3：导入魔女详细信息（第1部分）
-- 671-675
-- ========================================

USE WitchTrialWT;
GO

PRINT '========================================';
PRINT '批次2 - 步骤3：导入详细信息（第1部分）';
PRINT '========================================';
GO

-- 671 小鸟游六花
UPDATE wt.Witch SET
    PersonalNo = '5678-1234-0987',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2001-06-12',
    Ethnicity = N'大和民族',
    Birthplace = N'富山县',
    Height = 150,
    Weight = 47,
    BloodType = 'AB',
    Address = N'富山县中央市大字山田町',
    Phone = '076-555-6677',
    Email = 'takanashi_rikka@example.com',
    LineAccount = 'rikka_chan0612',
    HighestEducation = N'高中在读',
    EducationHistory = N'富山县立大森中学（中学校，毕业）：初中时期因父亲去世陷入中二病，社交圈较窄\n私立银杏学园高等学校（高等学校，在读）：进入高中后仍保持中二设定，与同伴形成社团',
    WorkHistory = N'无',
    FamilyStructure = N'父亲已故，核心成员为母姐',
    Father = N'小鸟游宗太（已故，原建筑师）',
    Mother = N'小鸟游澄子（家庭主妇）',
    OtherFamily1 = N'姐姐：小鸟游十花（职业厨师）',
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

-- 672 富樫勇太
UPDATE wt.Witch SET
    PersonalNo = '5678-1234-0988',
    FormerName = N'无',
    Gender = N'男',
    BirthDate = '2001-08-01',
    Ethnicity = N'大和民族',
    Birthplace = N'千叶县',
    Height = 170,
    Weight = 60,
    BloodType = 'O',
    Address = N'千叶县千叶市中央区',
    Phone = '043-1234-5678',
    Email = 'togashi_yuta@example.com',
    LineAccount = 'yuta_togashi0801',
    HighestEducation = N'高中在读',
    EducationHistory = N'私立银杏学园中等部（中学校，毕业）：初中时因中二病被孤立，社交恐惧\n私立银杏学园高等学校（高等学校，在读）：与六花等人组成远东魔法午睡结社之夏',
    WorkHistory = N'无',
    FamilyStructure = N'核心成员为父母',
    Father = N'富樫幸太郎，48岁，公司职员',
    Mother = N'富樫育江，46岁，家庭主妇',
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

-- 673 丹生谷森夏
UPDATE wt.Witch SET
    PersonalNo = '5678-1234-0989',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2001-12-20',
    Ethnicity = N'大和民族',
    Birthplace = N'千叶县',
    Height = 165,
    Weight = 57,
    BloodType = 'A',
    Address = N'千叶县千叶市美滨区',
    Phone = '043-2345-6789',
    Email = 'niwa_takanashi@example.com',
    LineAccount = 'takanashi_niwa1220',
    HighestEducation = N'高中在读',
    EducationHistory = N'私立银杏学园中等部（中学校，毕业）：成绩优异，班级干部，隐藏中二过往\n私立银杏学园高等学校（高等学校，在读）：负责管理结社，协调成员关系',
    WorkHistory = N'无',
    FamilyStructure = N'核心成员为父母',
    Father = N'丹生谷修，47岁，公司高管',
    Mother = N'丹生谷芙美，45岁，教师',
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

-- 674 五月七日茴香
UPDATE wt.Witch SET
    PersonalNo = '5678-1234-0990',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2002-03-25',
    Ethnicity = N'大和民族',
    Birthplace = N'千叶县',
    Height = 155,
    Weight = 45,
    BloodType = 'AB',
    Address = N'千叶县千叶市若叶区',
    Phone = '043-3456-7890',
    Email = 'itsuki_mayoi@example.com',
    LineAccount = 'mayoi_itsuki0325',
    HighestEducation = N'高中在读',
    EducationHistory = N'私立银杏学园中等部（中学校，毕业）：占卜爱好者，性格温和，观察力敏锐\n私立银杏学园高等学校（高等学校，在读）：结社的占卜师，提供神秘学支持',
    WorkHistory = N'无',
    FamilyStructure = N'核心成员为父母',
    Father = N'五月七日博臣，46岁，公务员',
    Mother = N'五月七日铃，44岁，护士',
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

-- 675 凸守早苗
UPDATE wt.Witch SET
    PersonalNo = '5678-1234-0991',
    FormerName = N'无',
    Gender = N'女',
    BirthDate = '2002-08-09',
    Ethnicity = N'大和民族',
    Birthplace = N'千叶县',
    Height = 143,
    Weight = 45,
    BloodType = 'B',
    Address = N'千叶县千叶市绿区',
    Phone = '043-4567-8901',
    Email = 'dekomori_sanae@example.com',
    LineAccount = 'sanae_dekomori0809',
    HighestEducation = N'高中在读',
    EducationHistory = N'私立银杏学园中等部（中学校，毕业）：元气满满，六花的忠实随从，中二病重度\n私立银杏学园高等学校（高等学校，在读）：结社的战斗员，魔法名为"雷之征服者"',
    WorkHistory = N'无',
    FamilyStructure = N'核心成员为父母',
    Father = N'凸守源五郎，47岁，企业家',
    Mother = N'凸守早苗（母，同名）',
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

PRINT '';
PRINT '========================================';
PRINT '✅ 第1部分完成！（671-675）';
PRINT '========================================';
GO
