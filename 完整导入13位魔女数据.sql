-- ========================================
-- 完整导入13位魔女详细档案数据
-- 使用说明：在 SSMS 中打开此文件，按 F5 执行
-- ========================================

USE WitchTrialWT;
GO

PRINT '开始导入13位魔女的详细档案数据...';
GO

-- ========================================
-- 658 樱羽艾玛
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9011',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2010-03-05',
    Ethnicity = '大和民族',
    Birthplace = '东京都',
    Height = 156.00,
    Weight = 48.00,
    BloodType = 'A',
    Address = '东京都涩谷区道玄坂 2 丁目',
    Phone = '03-1234-5678',
    Email = 'sakuraba_ema@yahoo.co.jp',
    LineAccount = 'ema_sakura0305',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[{"school":"东京都立樱丘中学校","degree":"中学校","status":"毕业","specialNote":"初中时旁观好友月代雪霸凌致其自杀，篡改记忆掩盖创伤"},{"school":"东京都立樱丘高等学校","degree":"高等学校","status":"未入学","specialNote":"高中开学前一日被抓至魔女岛"}]',
    WorkHistory = N'[]',
    FamilyStructure = '核心成员为父母',
    Father = '樱羽健一，45 岁，会社社员，东京商事株式会社',
    Mother = '樱羽静香，43 岁，家庭主妇',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '推理能力敏锐、观察力强、吃饭快速',
    Hobbies = '寻找美食店、和朋友相处',
    Dreams = '交 100 个朋友',
    Dislikes = '孤独、被排挤',
    Trauma = '旁观好友月代雪霸凌致其自杀，篡改记忆掩盖创伤',
    WitchTransformMethod = '无',
    Remarks = '无'
WHERE PrisonerNo = '658';
PRINT '✅ 658 樱羽艾玛 - 完成';
GO

-- ========================================
-- 659 二阶堂希罗
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9012',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2010-01-10',
    Ethnicity = '大和民族',
    Birthplace = '神奈川县',
    Height = 157.00,
    Weight = 50.00,
    BloodType = 'B',
    Address = '神奈川县横浜市中区山下町',
    Phone = '045-6789-0123',
    Email = 'nikaido_hiro@yahoo.co.jp',
    LineAccount = 'hiro_nikaido0110',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[{"school":"东京都立樱丘中学校","degree":"中学校","status":"毕业","specialNote":"成绩优异，运动全能，因极端正义观陷入偏执"},{"school":"东京都立樱丘高等学校","degree":"高等学校","status":"未入学","specialNote":"高中开学前一日被抓至魔女岛"}]',
    WorkHistory = N'[]',
    FamilyStructure = '核心成员为父母',
    Father = '二阶堂隆，46 岁，律师，神奈川法律事务所',
    Mother = '二阶堂美咲，44 岁，教师，神奈川中学',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '成绩优异、运动全能、逻辑清晰',
    Hobbies = '写助人日记、行善事',
    Dreams = '创造无恶的纯净世界',
    Dislikes = '恶行、不公正',
    Trauma = '好友自杀后因极端正义观陷入偏执，忌恨艾玛',
    WitchTransformMethod = '无',
    Remarks = '无'
WHERE PrisonerNo = '659';
PRINT '✅ 659 二阶堂希罗 - 完成';
GO

-- ========================================
-- 660 夏目安安
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9013',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2010-03-28',
    Ethnicity = '大和民族',
    Birthplace = '东京都',
    Height = 152.00,
    Weight = 45.00,
    BloodType = 'AB',
    Address = '东京都新宿区西新宿',
    Phone = '无（笔谈交流）',
    Email = 'natsume_an@yahoo.co.jp',
    LineAccount = 'an_natsume0328',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[{"school":"东京都立新宿中学校","degree":"中学校","status":"毕业","specialNote":"家里蹲，社交障碍，仅能笔谈交流"}]',
    WorkHistory = N'[]',
    FamilyStructure = '核心成员为父母',
    Father = '夏目悟，47 岁，程序员，东京 IT 公司',
    Mother = '夏目由佳，45 岁，漫画家',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '笔谈交流、游戏达人、自我肯定',
    Hobbies = '宅家打游戏、写小说',
    Dreams = '成为知名作家',
    Dislikes = '社交、被打扰',
    Trauma = '社交障碍，因小事自我满足，害怕与人深度接触',
    WitchTransformMethod = '让安安以为艾玛阻挠她获得自由',
    Remarks = '无'
WHERE PrisonerNo = '660';
PRINT '✅ 660 夏目安安 - 完成';
GO

-- ========================================
-- 661 城崎诺亚
-- ========================================
UPDATE wt.Witch SET
    PersonalNo = '1234-5678-9014',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2010-07-07',
    Ethnicity = '大和民族',
    Birthplace = '大阪府',
    Height = 160.00,
    Weight = 47.00,
    BloodType = 'O',
    Address = '大阪府大阪市北区梅田',
    Phone = '06-7890-1234',
    Email = 'shirosaki_noa@yahoo.co.jp',
    LineAccount = 'noa_shirosaki0707',
    HighestEducation = '中学校毕业',
    EducationHistory = N'[{"school":"大阪府立梅田中学校","degree":"中学校","status":"毕业","specialNote":"街头艺术家，作品全球知名，看穿谎言"}]',
    WorkHistory = N'[]',
    FamilyStructure = '核心成员为父母',
    Father = '城崎刚，46 岁，画家，大阪艺术工作室',
    Mother = '城崎明美，44 岁，策展人，大阪美术馆',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '绘画创作、街头艺术、看穿谎言',
    Hobbies = '画画、探索未知事物',
    Dreams = '画出完美作品',
    Dislikes = '创作瓶颈、束缚',
    Trauma = '对作品不满意的执念，渴望被认可',
    WitchTransformMethod = '让蕾雅杀诺亚',
    Remarks = '无'
WHERE PrisonerNo = '661';
PRINT '✅ 661 城崎诺亚 - 完成';
GO

-- ========================================
-- 662 莲见蕾雅
-- ========================================
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
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '表演能力、团队协调、压力承受',
    Hobbies = '剧团排练',
    Dreams = '成为顶级演员',
    Dislikes = '失败、依赖他人',
    Trauma = '因独立性格背负压力，隐藏真实情绪',
    WitchTransformMethod = '让蕾雅嫉妒诺亚的名气',
    Remarks = '无'
WHERE PrisonerNo = '662';
PRINT '✅ 662 莲见蕾雅 - 完成';
GO

-- ========================================
-- 663 佐伯米莉亚
-- ========================================
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
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '照顾他人、老电影鉴赏、法律知识（受律师影响）',
    Hobbies = '看老电影、穿搭打扮',
    Dreams = '成为可靠的大姐姐',
    Dislikes = '麻烦、过度关注',
    Trauma = '为合群发私密照被传播，害怕不合群，内心阴沉',
    WitchTransformMethod = '无',
    Remarks = '无'
WHERE PrisonerNo = '663';
PRINT '✅ 663 佐伯米莉亚 - 完成';
GO

-- ========================================
-- 664 宝生玛格
-- ========================================
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
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '诈骗技巧、口才出众、观察力敏锐',
    Hobbies = '戏弄他人、掌握对话主导权',
    Dreams = '不相信任何事，掌控一切',
    Dislikes = '真诚、被信任',
    Trauma = '童年受虐，导致不信任他人，以诈骗为乐',
    WitchTransformMethod = '无',
    Remarks = '无'
WHERE PrisonerNo = '664';
PRINT '✅ 664 宝生玛格 - 完成';
GO

-- ========================================
-- 665 黑部奈叶香
-- ========================================
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
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '监牢机制研究、信息收集、伪装能力',
    Hobbies = '单独行动、调查秘密',
    Dreams = '揭开监牢和魔女审判真相',
    Dislikes = '被妨碍、社交',
    Trauma = '失去姐姐后性格大变，伪装成月代雪',
    WitchTransformMethod = '无',
    Remarks = '无'
WHERE PrisonerNo = '665';
PRINT '✅ 665 黑部奈叶香 - 完成';
GO

-- ========================================
-- 666 紫藤亚里沙
-- ========================================
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
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '情绪爆发、力量控制（发火）、直率',
    Hobbies = '发泄情绪、吃辣食',
    Dreams = '随心所欲，不受束缚',
    Dislikes = '约束、压抑',
    Trauma = '易怒导致人际关系紧张，害怕被排斥',
    WitchTransformMethod = '无',
    Remarks = '无'
WHERE PrisonerNo = '666';
PRINT '✅ 666 紫藤亚里沙 - 完成';
GO

-- ========================================
-- 667 橘雪莉
-- ========================================
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
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '怪力、体力超群、直率可靠',
    Hobbies = '健身、吃美食、保护朋友',
    Dreams = '成为最强的守护者',
    Dislikes = '弱小、被轻视',
    Trauma = '因怪力被孤立，渴望被正常对待',
    WitchTransformMethod = '让雪莉承受杀害汉娜的压力',
    Remarks = '无'
WHERE PrisonerNo = '667';
PRINT '✅ 667 橘雪莉 - 完成';
GO

-- ========================================
-- 668 远野汉娜
-- ========================================
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
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '浮游、自由飞翔、温柔善良',
    Hobbies = '眺望天空、和朋友野餐',
    Dreams = '自由自在环游世界',
    Dislikes = '束缚、重力',
    Trauma = '渴望自由却被囚禁，害怕失去飞行能力',
    WitchTransformMethod = '被雪莉杀害',
    Remarks = '无'
WHERE PrisonerNo = '668';
PRINT '✅ 668 远野汉娜 - 完成';
GO

-- ========================================
-- 669 泽渡可可
-- ========================================
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
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '千里眼、信息收集、洞察力强',
    Hobbies = '观察他人、收集情报',
    Dreams = '掌握所有信息，洞察一切',
    Dislikes = '秘密被隐瞒、无知',
    Trauma = '因知晓太多秘密而孤独，害怕被利用',
    WitchTransformMethod = '无',
    Remarks = '无'
WHERE PrisonerNo = '669';
PRINT '✅ 669 泽渡可可 - 完成';
GO

-- ========================================
-- 670 冰上梅露露
-- ========================================
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
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '治愈、再生、植物培育（香料）',
    Hobbies = '培育香料植物、独处阅读',
    Dreams = '与大魔女重逢',
    Dislikes = '人群、背叛',
    Trauma = '作为幕后黑手的自责与痛苦，渴望被认可',
    WitchTransformMethod = '无',
    Remarks = '监牢幕后黑手，原人类，被大魔女赋予魔法'
WHERE PrisonerNo = '670';
PRINT '✅ 670 冰上梅露露 - 完成';
GO

-- ========================================
-- 验证导入结果
-- ========================================
PRINT '';
PRINT '========================================';
PRINT '导入完成！正在验证...';
PRINT '========================================';
GO

-- 统计已导入的魔女数量
SELECT 
    COUNT(*) AS 已导入数量,
    13 AS 总数量,
    CASE WHEN COUNT(*) = 13 THEN '✅ 全部导入成功' ELSE '❌ 部分导入失败' END AS 状态
FROM wt.Witch
WHERE PersonalNo IS NOT NULL;
GO

-- 显示所有魔女的基本信息
SELECT 
    PrisonerNo AS 囚人番号,
    Name AS 姓名,
    PersonalNo AS 个人番号,
    Gender AS 性别,
    CONVERT(VARCHAR(10), BirthDate, 120) AS 出生日期,
    Height AS 身高,
    Weight AS 体重,
    BloodType AS 血型,
    Email AS 邮箱
FROM wt.Witch
ORDER BY PrisonerNo;
GO

PRINT '';
PRINT '========================================';
PRINT '✅ 所有操作完成！';
PRINT '📊 请检查上面的验证结果';
PRINT '========================================';
GO
