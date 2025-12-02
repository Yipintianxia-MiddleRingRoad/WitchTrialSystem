-- ========================================
-- 步骤 2：导入魔女详细数据 (Part 3: 666-670)
-- ========================================

USE WitchTrialWT;
GO

-- 666 紫藤亚里沙
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9019',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2011-02-20',
    Ethnicity = '大和民族',
    Birthplace = '兵库县',
    Height = 157.00,
    Weight = 46.00,
    BloodType = 'A',
    Address = '兵库县神戸市中央区元町',
    Phone = '078-7890-1234',
    Email = 'shidou_arisa@yahoo.co.jp',
    LineAccount = 'arisa_shidou0220',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[{"school":"兵库县立神戸中学校","degree":"中学校","status":"毕业","specialNote":"离家出走太妹，易怒，情绪管理困难"}]',
    WorkHistory = N'[]',
    FamilyStructure = '核心成员为父母',
    Father = '紫藤英树，47 岁，消防员，兵库消防局',
    Mother = '紫藤绫子，45 岁，护士，兵库医院',
    Skills = '情绪爆发、力量控制（发火）、直率',
    Hobbies = '发泄情绪、吃辣食',
    Dreams = '随心所欲，不受束缚',
    Dislikes = '约束、压抑',
    Trauma = '易怒导致人际关系紧张，害怕被排斥',
    WitchTransformMethod = '无',
    Remarks = '无'
WHERE PrisonerNo = '666';
GO

-- 667 橘雪莉
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9020',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2011-04-15',
    Ethnicity = '大和民族',
    Birthplace = '千叶县',
    Height = 165.00,
    Weight = 52.00,
    BloodType = 'B',
    Address = '千叶县千叶市中央区站前',
    Phone = '043-8901-2345',
    Email = 'tachibana_sherry@yahoo.co.jp',
    LineAccount = 'sherry_tachibana0415',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[{"school":"千叶县立千叶中学校","degree":"中学校","status":"毕业","specialNote":"怪力，性格直率，体力超群"}]',
    WorkHistory = N'[]',
    FamilyStructure = '核心成员为父母',
    Father = '橘正雄，46 岁，运动员，千叶体育协会',
    Mother = '橘早苗，44 岁，教练，千叶体育协会',
    Skills = '怪力、体力超群、直率可靠',
    Hobbies = '健身、吃美食、保护朋友',
    Dreams = '成为最强的守护者',
    Dislikes = '弱小、被轻视',
    Trauma = '因怪力被孤立，渴望被正常对待',
    WitchTransformMethod = '让雪莉承受杀害汉娜的压力',
    Remarks = '无'
WHERE PrisonerNo = '667';
GO

-- 668 远野汉娜
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9021',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2011-06-08',
    Ethnicity = '大和民族',
    Birthplace = '北海道',
    Height = 153.00,
    Weight = 43.00,
    BloodType = 'AB',
    Address = '北海道札幌市中央区大通西',
    Phone = '011-3456-7890',
    Email = 'touno_hanna@yahoo.co.jp',
    LineAccount = 'hanna_touno0608',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[{"school":"北海道立札幌中学校","degree":"中学校","status":"毕业","specialNote":"浮游能力，性格温和，渴望自由"}]',
    WorkHistory = N'[]',
    FamilyStructure = '核心成员为父母',
    Father = '远野健一，47 岁，渔民，北海道渔业',
    Mother = '远野里美，45 岁，家庭主妇',
    Skills = '浮游、自由飞翔、温柔善良',
    Hobbies = '眺望天空、和朋友野餐',
    Dreams = '自由自在环游世界',
    Dislikes = '束缚、重力',
    Trauma = '渴望自由却被囚禁，害怕失去飞行能力',
    WitchTransformMethod = '被雪莉杀害',
    Remarks = '无'
WHERE PrisonerNo = '668';
GO

-- 669 泽渡可可
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9022',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2011-08-22',
    Ethnicity = '大和民族',
    Birthplace = '爱知县',
    Height = 150.00,
    Weight = 42.00,
    BloodType = 'O',
    Address = '爱知县名古屋市中区荣',
    Phone = '052-5678-9012',
    Email = 'sawatari_coco@yahoo.co.jp',
    LineAccount = 'coco_sawatari0822',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[{"school":"爱知县立名古屋中学校","degree":"中学校","status":"毕业","specialNote":"千里眼，信息收集者，洞察力强"}]',
    WorkHistory = N'[]',
    FamilyStructure = '核心成员为父母',
    Father = '泽渡宏，46 岁，记者，爱知电视台',
    Mother = '泽渡千夏，44 岁，编辑，爱知出版社',
    Skills = '千里眼、信息收集、洞察力强',
    Hobbies = '观察他人、收集情报',
    Dreams = '掌握所有信息，洞察一切',
    Dislikes = '秘密被隐瞒、无知',
    Trauma = '因知晓太多秘密而孤独，害怕被利用',
    WitchTransformMethod = '无',
    Remarks = '无'
WHERE PrisonerNo = '669';
GO

-- 670 冰上梅露露
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9023',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2005-12-25',
    Ethnicity = '大和民族',
    Birthplace = '不明',
    Height = 158.00,
    Weight = 48.00,
    BloodType = '不明',
    Address = '魔女监牢岛中央区监牢街',
    Phone = '不明',
    Email = 'mizore_meruru@yahoo.co.jp',
    LineAccount = 'meruru_mizore1225',
    HighestEducation = '无',
    EducationHistory = N'[]',
    WorkHistory = N'[]',
    FamilyStructure = '无',
    Father = '无',
    Mother = '无',
    Skills = '治愈、再生、植物培育（香料）',
    Hobbies = '培育香料植物、独处阅读',
    Dreams = '与大魔女重逢',
    Dislikes = '人群、背叛',
    Trauma = '作为幕后黑手的自责与痛苦，渴望被认可',
    WitchTransformMethod = '无',
    Remarks = '监牢幕后黑手，原人类，被大魔女赋予魔法'
WHERE PrisonerNo = '670';
GO

PRINT '✅ Part 3 完成 (666-670)';
PRINT '========================================';
PRINT '✅ 所有 13 位魔女的详细档案已导入完成！';
PRINT '========================================';
GO
