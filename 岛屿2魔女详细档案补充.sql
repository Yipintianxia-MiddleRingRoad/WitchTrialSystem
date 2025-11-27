-- WitchTrialSystem 岛屿2魔女详细档案补充脚本
-- 为岛屿2批次3（684-696）的13位魔女补充完整的个人信息
-- 执行前请确保已完成魔女表扩展字段（add_witch_extended_fields.sql）

USE WitchTrialWT;
GO

PRINT '=== 开始补充岛屿2魔女详细档案 ===';

-- 1. 柊舞缇娜 (684)
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
WHERE PrisonerNo = '684';

PRINT '✓ 柊舞缇娜 (684) 档案更新完成';

-- 2. 阿良河琪舞 (685)
UPDATE wt.Witch
SET 
    PersonalNo = '0222-4444-6666',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2008-02-22',
    Ethnicity = '大和民族',
    Birthplace = '东京都',
    Height = 148.00,
    Weight = 46.00,
    BloodType = 'AB',
    Address = '东京都涩谷区神宫前3丁目',
    Phone = '03-5775-1111',
    Email = 'kiwi_aragaki@example.com',
    LineAccount = 'kiwi_0222',
    HighestEducation = '中学校在读',
    EducationHistory = N'[
        {
            "school": "东京都立原宿中学校",
            "degree": "中学校",
            "status": "2年生（初二）在读",
            "specialNote": "成绩下游，热衷自拍和社交网络"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2023年至今",
            "company": "邪恶组织艾诺尔米塔",
            "position": "女干部；炸弹制造与操纵",
            "salary": "时薪1200日元",
            "resignReason": "在职"
        }
    ]',
    FamilyStructure = '核心成员为母亲',
    Father = '阿良河信也（已故）',
    Mother = '阿良河玲奈，44岁，美容师，ビューティサロン「R」，年收500万',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '炸弹制造操纵、自拍构图、社交网络运营',
    Hobbies = '自拍照、逛时尚商店、收集爆炸物素材',
    Dreams = '成为最引人注目焦点',
    Dislikes = '被忽视、风头被抢、不够可爱',
    Trauma = '因身材矮小被排挤，渴望被关注',
    WitchTransformMethod = '无',
    Remarks = '邪恶组织女干部，样貌极度自信'
WHERE PrisonerNo = '685';

PRINT '✓ 阿良河琪舞 (685) 档案更新完成';

-- 3. 杜乃可莉丝 (686)
UPDATE wt.Witch
SET 
    PersonalNo = '1212-3333-7777',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2012-12-12',
    Ethnicity = '大和民族',
    Birthplace = '东京都',
    Height = 140.00,
    Weight = 38.00,
    BloodType = 'A',
    Address = '东京都练马区光丘1丁目',
    Phone = '03-3925-8888',
    Email = 'korisu_morino@example.com',
    LineAccount = 'korisu_1212',
    HighestEducation = '小学校在读',
    EducationHistory = N'[
        {
            "school": "东京都立光丘小学校",
            "degree": "小学校",
            "status": "5年生在读",
            "specialNote": "不擅长社交，手工艺天赋强"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2023年至今",
            "company": "邪恶组织艾诺尔米塔",
            "position": "女干部；玩偶侦察战斗",
            "salary": "时薪1000日元",
            "resignReason": "在职"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '杜乃正人，40岁，玩具设计师，タカラトミー株式会社，年收650万',
    Mother = '杜乃七海，38岁，绘本作家，在家工作，年收300万',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '玩偶操纵、玩具屋构造、手工艺制作',
    Hobbies = '收集制作玩偶、设计衣服、独自玩耍',
    Dreams = '创造所有玩偶都能自由活动的玩具王国',
    Dislikes = '被触碰珍视玩偶、吵闹环境',
    Trauma = '性格内向难以交友，情感寄托于玩偶',
    WitchTransformMethod = '无',
    Remarks = '邪恶组织女干部，孤僻小学生'
WHERE PrisonerNo = '686';

PRINT '✓ 杜乃可莉丝 (686) 档案更新完成';

-- 4. 阿古屋真珠 (687)
UPDATE wt.Witch
SET 
    PersonalNo = '0505-8888-2222',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2001-05-05',
    Ethnicity = '大和民族',
    Birthplace = '福冈县',
    Height = 162.00,
    Weight = 49.00,
    BloodType = 'O',
    Address = '福冈县福冈市博多区祇园1丁目',
    Phone = '092-411-9999',
    Email = 'matama_akoya@example.com',
    LineAccount = 'matama_0505',
    HighestEducation = '高等学校毕业',
    EducationHistory = N'[
        {
            "school": "福冈县立博多高等学校",
            "degree": "高等学校",
            "status": "毕业",
            "specialNote": "参加轻音部，多次选拔落选"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2020年至今",
            "company": "邪恶组织艾诺尔米塔",
            "position": "女干部主旅团；声波攻击",
            "salary": "月薪25万日元",
            "resignReason": "转投新总帅"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '阿古屋幸夫，55岁，渔业经营者，博多漁業，年收1000万',
    Mother = '阿古屋珠子，52岁，传统艺能讲师，日本舞踊「珠の会」，年收400万',
    OtherFamily1 = '好友：姊母娞摩',
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '声波魔法、歌唱、原创歌词',
    Hobbies = '唱歌、作曲、地下偶像活动',
    Dreams = '成为被认可的顶级歌手',
    Dislikes = '批评其歌艺的人、冷场',
    Trauma = '多次选拔落选，羞耻状态才能一流',
    WitchTransformMethod = '无',
    Remarks = '前主旅团成员，梦想歌手'
WHERE PrisonerNo = '687';

PRINT '✓ 阿古屋真珠 (687) 档案更新完成';

-- 5. 姊母娞摩 (688)
UPDATE wt.Witch
SET 
    PersonalNo = '1010-7777-3333',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2001-10-10',
    Ethnicity = '大和民族',
    Birthplace = '福冈县',
    Height = 165.00,
    Weight = 51.00,
    BloodType = 'B',
    Address = '福冈县福冈市中央区大名2丁目',
    Phone = '092-731-5555',
    Email = 'nemo_anemo@example.com',
    LineAccount = 'nemo_1010',
    HighestEducation = '高等学校毕业',
    EducationHistory = N'[
        {
            "school": "福冈县立福冈中央高等学校",
            "degree": "高等学校",
            "status": "毕业",
            "specialNote": "学生会长，成绩优异"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2020年至今",
            "company": "邪恶组织艾诺尔米塔",
            "position": "女干部主旅团；影子操纵",
            "salary": "月薪28万日元",
            "resignReason": "转投新总帅"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '姊母健一郎，58岁，法官，福岡地方裁判所，年收1200万',
    Mother = '姊母律子，55岁，律师，弁護士法人「姉母」，年收800万',
    OtherFamily1 = '好友：阿古屋真珠',
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '影子操纵、战略策划、人员管理、暗中运作',
    Hobbies = '阅读法律书籍、策划活动、帮助真珠',
    Dreams = '守护真珠的梦想（手段不光彩）',
    Dislikes = '背叛、让真珠伤心',
    Trauma = '活在精英期望中，羡慕真珠直率',
    WitchTransformMethod = '无',
    Remarks = '前主旅团成员，与真珠自幼稚园认识'
WHERE PrisonerNo = '688';

PRINT '✓ 姊母娞摩 (688) 档案更新完成';

-- 6. 花菱春香 (689)
UPDATE wt.Witch
SET 
    PersonalNo = '0430-1234-5678',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2007-04-30',
    Ethnicity = '大和民族',
    Birthplace = '埼玉县',
    Height = 155.00,
    Weight = 43.00,
    BloodType = 'A',
    Address = '埼玉县越谷市レイクタウン4丁目',
    Phone = '048-987-6543',
    Email = 'haruka_hanabishi@example.com',
    LineAccount = 'haruka_0430',
    HighestEducation = '中学校在读',
    EducationHistory = N'[
        {
            "school": "埼玉县立光之丘中学校",
            "degree": "中学校",
            "status": "3年生（初三）在读",
            "specialNote": "成绩优异，班级委员，领导力强"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2022年至今",
            "company": "魔法少女协会",
            "position": "魔法品红；对抗邪恶",
            "salary": "无薪（志愿）",
            "resignReason": "在职"
        }
    ]',
    FamilyStructure = '核心成员为父母及三个妹妹',
    Father = '花菱大辅，50岁，公务员，埼玉県厅，年收700万',
    Mother = '花菱绫子，47岁，兼职店员，スーパー「マルエツ」，年收200万',
    OtherFamily1 = '妹妹：花菱夏奈，12岁',
    OtherFamily2 = '妹妹：花菱秋穗，10岁',
    OtherFamily3 = '妹妹：花菱美冬，7岁',
    Skills = '长枪战斗、治愈魔法、团队指挥、烹饪',
    Hobbies = '料理、照顾妹妹、收集蘑菇物品',
    Dreams = '守护城市的和平与大家的笑容',
    Dislikes = '黄豆粉、不珍惜家人',
    Trauma = '曾被邪恶力量控制成傀儡',
    WitchTransformMethod = '无',
    Remarks = '特蕾丝玛吉雅小队队长，性格开朗'
WHERE PrisonerNo = '689';

PRINT '✓ 花菱春香 (689) 档案更新完成';

-- 7. 水神小夜 (690)
UPDATE wt.Witch
SET 
    PersonalNo = '0721-5555-8888',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2008-07-21',
    Ethnicity = '大和民族',
    Birthplace = '神奈川县',
    Height = 153.00,
    Weight = 42.00,
    BloodType = 'B',
    Address = '神奈川县横滨市港未来2丁目',
    Phone = '045-222-3333',
    Email = 'sayo_minagami@example.com',
    LineAccount = 'sayo_0721',
    HighestEducation = '中学校在读',
    EducationHistory = N'[
        {
            "school": "神奈川县立港中学校",
            "degree": "中学校",
            "status": "2年生（初二）在读",
            "specialNote": "运动神经发达，文化课稍弱"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2022年至今",
            "company": "魔法少女协会",
            "position": "魔法碧蓝；前线突击",
            "salary": "无薪（志愿）",
            "resignReason": "在职"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '水神海斗，46岁，船长，横浜漁業協同組合，年收600万',
    Mother = '水神渚，43岁，潜水员，マリンサービス「シーブルー」，年收300万',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '近身格斗、冰属性魔法、高速移动',
    Hobbies = '游泳、钓鱼、水上运动、格斗比赛',
    Dreams = '成为最强最快的魔法少女',
    Dislikes = '优柔寡断、热水',
    Trauma = '战斗败北产生对惩罚的期待',
    WitchTransformMethod = '无',
    Remarks = '特蕾丝玛吉雅成员，近战担当'
WHERE PrisonerNo = '690';

PRINT '✓ 水神小夜 (690) 档案更新完成';

-- 8. 天川薰子 (691)
UPDATE wt.Witch
SET 
    PersonalNo = '1103-9999-1111',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2007-11-03',
    Ethnicity = '大和民族',
    Birthplace = '大阪府',
    Height = 160.00,
    Weight = 48.00,
    BloodType = 'O',
    Address = '大阪府大阪市此花区桜島1丁目',
    Phone = '06-7777-8888',
    Email = 'kaoruko_amakawa@example.com',
    LineAccount = 'kaoruko_1103',
    HighestEducation = '中学校在读',
    EducationHistory = N'[
        {
            "school": "大阪府立咲洲中学校",
            "degree": "中学校",
            "status": "3年生（初三）在读",
            "specialNote": "体育万能，性格豪爽，大姐头"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2022年至今",
            "company": "魔法少女协会",
            "position": "魔法硫磺；力量输出",
            "salary": "无薪（志愿）",
            "resignReason": "在职"
        }
    ]',
    FamilyStructure = '核心成员为父母',
    Father = '天川刚志，49岁，建筑工地主任，大建工業株式会社，年收750万',
    Mother = '天川惠美，46岁，健身房教练，フィットネスクラブ「パワー」，年收350万',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    Skills = '怪力、火属性魔法、防御强化',
    Hobbies = '力量训练、吃肉、摔角、照顾小动物',
    Dreams = '用力量保护所有需要保护的人',
    Dislikes = '欺负弱小、蔬菜特别青椒',
    Trauma = '无法控制力量导致焦虑',
    WitchTransformMethod = '无',
    Remarks = '特蕾丝玛吉雅成员，力量担当'
WHERE PrisonerNo = '691';

PRINT '✓ 天川薰子 (691) 档案更新完成';

-- 9. 梓川咲太 (692)
UPDATE wt.Witch
SET 
    PersonalNo = '1223-2005-0206',
    FormerName = '无',
    Gender = '男',
    BirthDate = '1998-04-10',
    Ethnicity = '大和民族',
    Birthplace = '横滨市',
    Height = 172.00,
    Weight = 50.00,
    BloodType = 'A',
    Address = '神奈川县藤泽市片濑4-7-18 峰原高校学生公寓302号室',
    Phone = '06-7890-1230',
    Email = 'sakuta@minegahara.jp',
    LineAccount = 'azusagawa_s',
    HighestEducation = '高中在读',
    EducationHistory = N'[
        {
            "school": "峰原高中2年1班",
            "degree": "高中",
            "status": "2年级在读",
            "specialNote": "曾因"青春期症候群"被孤立"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2017至今",
            "company": "藤泽餐厅",
            "position": "服务员",
            "salary": "时薪1100日元",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '独居，照顾妹妹',
    Father = '关系疏远',
    Mother = '住院中',
    OtherFamily1 = '好友：国见佑真',
    OtherFamily2 = '初恋：牧之原翔子',
    OtherFamily3 = NULL,
    Skills = '吐槽、照顾人',
    Hobbies = '看海、撩妹',
    Dreams = '希望自己与妹妹都过上正常生活',
    Dislikes = '谣言、排挤',
    Trauma = '妹妹被霸凌',
    WitchTransformMethod = '无',
    Remarks = '本作男主，无手机'
WHERE PrisonerNo = '692';

PRINT '✓ 梓川咲太 (692) 档案更新完成';

-- 10. 樱岛麻衣 (693)
UPDATE wt.Witch
SET 
    PersonalNo = '1223-2005-0207',
    FormerName = '无',
    Gender = '女',
    BirthDate = '1997-12-02',
    Ethnicity = '大和民族',
    Birthplace = '横滨市',
    Height = 165.00,
    Weight = 40.00,
    BloodType = 'B',
    Address = '神奈川县藤泽市鹄沼海岸3-12-5 海景樱塔公寓15楼',
    Phone = '05-7890-1231',
    Email = 'mai@sakurajima.com',
    LineAccount = 'sakurajima_m',
    HighestEducation = '高中在读',
    EducationHistory = N'[
        {
            "school": "峰原高中3年1班",
            "degree": "高中",
            "status": "3年级在读",
            "specialNote": "因艺人身份常请假"
        },
        {
            "school": "艺人培训学校",
            "degree": "艺人培训证书",
            "status": "已毕业",
            "specialNote": "同时兼顾学业与演艺"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "2007至今",
            "company": "星尘事务所",
            "position": "演员",
            "salary": "年收入500万日元",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '与母亲同住',
    Father = '离异，疏远',
    Mother = '艺人经纪人',
    OtherFamily1 = '妹妹：丰滨和花',
    OtherFamily2 = '前男友：无',
    OtherFamily3 = NULL,
    Skills = '兔女郎装扮、演技、料理',
    Hobbies = '料理、阅读',
    Dreams = '成为优秀演员',
    Dislikes = '被忽视',
    Trauma = '母亲利用自己',
    WitchTransformMethod = '无',
    Remarks = '人气艺人，学姐'
WHERE PrisonerNo = '693';

PRINT '✓ 樱岛麻衣 (693) 档案更新完成';

-- 11. 古贺朋绘 (694)
UPDATE wt.Witch
SET 
    PersonalNo = '1223-2005-0208',
    FormerName = '无',
    Gender = '女',
    BirthDate = '1999-05-23',
    Ethnicity = '大和民族',
    Birthplace = '横滨市',
    Height = 165.00,
    Weight = 40.00,
    BloodType = 'O',
    Address = '神奈川县藤泽市南藤泽5-9-11 藤泽公园高地202号室',
    Phone = '02-4567-8901',
    Email = 'tomoe@hakata.jp',
    LineAccount = 'koga_tomo',
    HighestEducation = '高中在读',
    EducationHistory = N'[
        {
            "school": "峰原高中1年",
            "degree": "高中",
            "status": "1年级在读",
            "specialNote": "从福冈转学"
        }
    ]',
    WorkHistory = N'[
        {
            "period": "高一开始",
            "company": "藤泽餐厅",
            "position": "服务员",
            "salary": "时薪1000日元",
            "resignReason": "无"
        }
    ]',
    FamilyStructure = '与父母同住',
    Father = '普通职员',
    Mother = '家庭主妇',
    OtherFamily1 = '好友：上里沙希',
    OtherFamily2 = '假扮女友事件',
    OtherFamily3 = '踢屁股梗',
    Skills = '运动、吐槽',
    Hobbies = '篮球、偶像',
    Dreams = '成为可爱女生',
    Dislikes = '被误会',
    Trauma = '被表白困扰',
    WitchTransformMethod = '无',
    Remarks = '福冈口音，学妹'
WHERE PrisonerNo = '694';

PRINT '✓ 古贺朋绘 (694) 档案更新完成';

-- 12. 双叶理央 (695)
UPDATE wt.Witch
SET 
    PersonalNo = '1223-2005-0209',
    FormerName = '无',
    Gender = '女',
    BirthDate = '1998-10-23',
    Ethnicity = '大和民族',
    Birthplace = '横滨市',
    Height = 155.00,
    Weight = 38.00,
    BloodType = 'AB',
    Address = '神奈川县藤泽市江之岛2-14-7 海洋居住湘南401号室',
    Phone = '08-1234-5678',
    Email = 'rio@lab.net',
    LineAccount = 'futaba_rio',
    HighestEducation = '高中在读',
    EducationHistory = N'[
        {
            "school": "峰原高中2年",
            "degree": "高中",
            "status": "2年级在读",
            "specialNote": "科学社唯一成员"
        },
        {
            "school": "科学夏令营",
            "degree": "夏令营证书",
            "status": "已结业",
            "specialNote": "对量子物理感兴趣"
        }
    ]',
    WorkHistory = N'[]',
    FamilyStructure = '独居',
    Father = '研究员',
    Mother = '大学教授',
    OtherFamily1 = '好友：梓川咲太',
    OtherFamily2 = '量子物理解释',
    OtherFamily3 = NULL,
    Skills = '科学分析',
    Hobbies = '实验、看书',
    Dreams = '理解世界真理',
    Dislikes = '无逻辑的事',
    Trauma = '自我认同障碍',
    WitchTransformMethod = '无',
    Remarks = '科学社怪才，喜欢穿白大褂'
WHERE PrisonerNo = '695';

PRINT '✓ 双叶理央 (695) 档案更新完成';

-- 13. 梓川枫 (696)
UPDATE wt.Witch
SET 
    PersonalNo = '1223-2005-0210',
    FormerName = '花枫',
    Gender = '女',
    BirthDate = '2002-11-05',
    Ethnicity = '大和民族',
    Birthplace = '横滨市',
    Height = 162.00,
    Weight = 38.00,
    BloodType = 'O',
    Address = '神奈川县藤泽市片濑4-7-18 峰原高校学生公寓302号室（与哥哥同住）',
    Phone = '09-1234-5679',
    Email = 'kaede@home.jp',
    LineAccount = 'kaede_chi',
    HighestEducation = '初中在读',
    EducationHistory = N'[
        {
            "school": "峰原初中3年",
            "degree": "初中",
            "status": "初一年级在读",
            "specialNote": "因霸凌休学两年"
        }
    ]',
    WorkHistory = N'[]',
    FamilyStructure = '与哥哥同住',
    Father = '已故（虚构）',
    Mother = '住院中',
    OtherFamily1 = '双重人格设定',
    OtherFamily2 = '曾改名',
    OtherFamily3 = NULL,
    Skills = '画画、写日记',
    Hobbies = '熊猫、哥哥',
    Dreams = '成为普通女生',
    Dislikes = '霸凌、出门',
    Trauma = '校园霸凌',
    WitchTransformMethod = '无',
    Remarks = '家里蹲妹妹，喜欢熊猫'
WHERE PrisonerNo = '696';

PRINT '✓ 梓川枫 (696) 档案更新完成';

PRINT '=== 岛屿2魔女详细档案补充完成 ===';
PRINT '✓ 已为13位魔女补充完整的28个扩展字段';
PRINT '✓ 包含个人信息、家庭背景、教育经历、工作经历等';
PRINT '✓ 数据格式符合数据库字段要求';
PRINT '';
PRINT '下一步：验证数据完整性';
