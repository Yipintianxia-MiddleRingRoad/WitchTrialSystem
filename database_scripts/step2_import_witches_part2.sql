-- ========================================
-- 步骤 2：导入魔女详细数据 (Part 2: 662-665)
-- ========================================

USE WitchTrialWT;
GO

-- 662 莲见蕾雅
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9015',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2010-09-03',
    Ethnicity = '大和民族',
    Birthplace = '东京都',
    Height = 163.00,
    Weight = 51.00,
    BloodType = 'A',
    Address = '东京都港区赤坂',
    Phone = '03-5678-9012',
    Email = 'hasumi_rea@yahoo.co.jp',
    LineAccount = 'rea_hasumi0903',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[{"school":"东京都立港区中学校","degree":"中学校","status":"毕业","specialNote":"剧团演员，独立坚韧，隐藏压力"}]',
    WorkHistory = N'[]',
    FamilyStructure = '核心成员为父母',
    Father = '莲见裕介，47 岁，导演，东京剧团',
    Mother = '莲见纱织，45 岁，演员，东京剧团',
    Skills = '表演能力、团队协调、压力承受',
    Hobbies = '剧团排练',
    Dreams = '成为顶级演员',
    Dislikes = '失败、依赖他人',
    Trauma = '因独立性格背负压力，隐藏真实情绪',
    WitchTransformMethod = '让蕾雅嫉妒诺亚的名气',
    Remarks = '无'
WHERE PrisonerNo = '662';
GO

-- 663 佐伯米莉亚
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9016',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2010-10-12',
    Ethnicity = '大和民族',
    Birthplace = '埼玉县',
    Height = 155.00,
    Weight = 46.00,
    BloodType = 'B',
    Address = '埼玉县埼玉市大宫区桜木町',
    Phone = '048-6789-0123',
    Email = 'saeki_miria@yahoo.co.jp',
    LineAccount = 'miria_saeki1012',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[{"school":"埼玉县立埼玉中学校","degree":"中学校","status":"毕业","specialNote":"白辣妹，为合群发私密照被传播，后与律师互换身体"}]',
    WorkHistory = N'[]',
    FamilyStructure = '核心成员为父母',
    Father = '佐伯浩二，46 岁，商人，埼玉贸易公司',
    Mother = '佐伯理惠，44 岁，设计师，埼玉设计事务所',
    Skills = '照顾他人、老电影鉴赏、法律知识（受律师影响）',
    Hobbies = '看老电影、穿搭打扮',
    Dreams = '成为可靠的大姐姐',
    Dislikes = '麻烦、过度关注',
    Trauma = '为合群发私密照被传播，害怕不合群，内心阴沉',
    WitchTransformMethod = '无',
    Remarks = '无'
WHERE PrisonerNo = '663';
GO

-- 664 宝生玛格
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9017',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2010-11-05',
    Ethnicity = '大和民族',
    Birthplace = '东京都',
    Height = 158.00,
    Weight = 49.00,
    BloodType = 'AB',
    Address = '东京都中央区银座',
    Phone = '03-4567-8901',
    Email = 'hoshou_marg@yahoo.co.jp',
    LineAccount = 'marg_hoshou1105',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[{"school":"东京都立中央区中学校","degree":"中学校","status":"毕业","specialNote":"诈骗专家，观察力敏锐，不信任他人"}]',
    WorkHistory = N'[]',
    FamilyStructure = '核心成员为单亲',
    Father = '宝生智，47 岁，无业（原诈骗犯）',
    Mother = '无（母亲弃养）',
    Skills = '诈骗技巧、口才出众、观察力敏锐',
    Hobbies = '戏弄他人、掌握对话主导权',
    Dreams = '不相信任何事，掌控一切',
    Dislikes = '真诚、被信任',
    Trauma = '童年受虐，导致不信任他人，以诈骗为乐',
    WitchTransformMethod = '无',
    Remarks = '无'
WHERE PrisonerNo = '664';
GO

-- 665 黑部奈叶香
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9018',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2010-12-01',
    Ethnicity = '大和民族',
    Birthplace = '京都府',
    Height = 154.00,
    Weight = 44.00,
    BloodType = 'O',
    Address = '京都府京都市东山区清水',
    Phone = '075-5678-9012',
    Email = 'kurobe_natsuka@yahoo.co.jp',
    LineAccount = 'natsuka_kurobe1201',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[{"school":"京都府立京中学校","degree":"中学校","status":"毕业","specialNote":"了解监牢机制，曾伪装月代雪"}]',
    WorkHistory = N'[]',
    FamilyStructure = '核心成员为父母',
    Father = '黑部达也，46 岁，学者，京都大学',
    Mother = '黑部千夏，44 岁，编辑，京都出版社',
    Skills = '监牢机制研究、信息收集、伪装能力',
    Hobbies = '单独行动、调查秘密',
    Dreams = '揭开监牢和魔女审判真相',
    Dislikes = '被妨碍、社交',
    Trauma = '失去姐姐后性格大变，伪装成月代雪',
    WitchTransformMethod = '无',
    Remarks = '无'
WHERE PrisonerNo = '665';
GO

PRINT '✅ Part 2 完成 (662-665)';
GO
