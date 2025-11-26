-- ========================================
-- 步骤 2：导入魔女详细数据 (Part 1: 658-661)
-- ========================================

USE WitchTrialWT;
GO

-- 658 樱羽艾玛
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
    Skills = '推理能力敏锐、观察力强、吃饭快速',
    Hobbies = '寻找美食店、和朋友相处',
    Dreams = '交 100 个朋友',
    Dislikes = '孤独、被排挤',
    Trauma = '旁观好友月代雪霸凌致其自杀，篡改记忆掩盖创伤',
    WitchTransformMethod = '无',
    Remarks = '无'
WHERE PrisonerNo = '658';
GO

-- 659 二阶堂希罗
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
    Skills = '成绩优异、运动全能、逻辑清晰',
    Hobbies = '写助人日记、行善事',
    Dreams = '创造无恶的纯净世界',
    Dislikes = '恶行、不公正',
    Trauma = '好友自杀后因极端正义观陷入偏执，忌恨艾玛',
    WitchTransformMethod = '无',
    Remarks = '无'
WHERE PrisonerNo = '659';
GO

-- 660 夏目安安
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
    Skills = '笔谈交流、游戏达人、自我肯定',
    Hobbies = '宅家打游戏、写小说',
    Dreams = '成为知名作家',
    Dislikes = '社交、被打扰',
    Trauma = '社交障碍，因小事自我满足，害怕与人深度接触',
    WitchTransformMethod = '让安安以为艾玛阻挠她获得自由',
    Remarks = '无'
WHERE PrisonerNo = '660';
GO

-- 661 城崎诺亚
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
    Skills = '绘画创作、街头艺术、看穿谎言',
    Hobbies = '画画、探索未知事物',
    Dreams = '画出完美作品',
    Dislikes = '创作瓶颈、束缚',
    Trauma = '对作品不满意的执念，渴望被认可',
    WitchTransformMethod = '让蕾雅杀诺亚',
    Remarks = '无'
WHERE PrisonerNo = '661';
GO

PRINT '✅ Part 1 完成 (658-661)';
GO
