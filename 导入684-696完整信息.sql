-- =======================================================================
-- 导入684-696角色完整详细信息
-- 基于用户提供的详细数据表
-- =======================================================================

USE WitchTrialWT;
GO

PRINT '=== 开始导入684-696完整信息 ===';

-- 1. 确保WitchDetail表存在
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'wt.WitchDetail') AND type = 'U')
BEGIN
    CREATE TABLE wt.WitchDetail(
        DetailID INT IDENTITY PRIMARY KEY,
        WitchID INT NOT NULL UNIQUE,
        PersonalID NVARCHAR(50),
        FormerName NVARCHAR(100),
        Gender NVARCHAR(10),
        BirthDate DATE,
        Ethnicity NVARCHAR(50),
        BirthPlace NVARCHAR(100),
        Height INT,
        Weight INT,
        BloodType NVARCHAR(10),
        ResidentialAddress NVARCHAR(200),
        Phone NVARCHAR(50),
        Email NVARCHAR(100),
        LineAccount NVARCHAR(100),
        Remarks NVARCHAR(500),
        HighestEducation NVARCHAR(100),
        Education1_School NVARCHAR(100),
        Education1_Degree NVARCHAR(50),
        Education1_Grade NVARCHAR(50),
        Education1_Notes NVARCHAR(200),
        Education2_School NVARCHAR(100),
        Education2_Degree NVARCHAR(50),
        Education2_Grade NVARCHAR(50),
        Education2_Notes NVARCHAR(200),
        Work_Company NVARCHAR(100),
        Work_Period NVARCHAR(100),
        Work_Position NVARCHAR(200),
        Work_Salary NVARCHAR(50),
        Work_Reason NVARCHAR(200),
        Family_Father NVARCHAR(100),
        Family_Mother NVARCHAR(100),
        Family_Other1 NVARCHAR(100),
        Family_Other2 NVARCHAR(100),
        Family_Other3 NVARCHAR(100),
        Skills NVARCHAR(500),
        Hobbies NVARCHAR(500),
        Ideals NVARCHAR(500),
        Dislikes NVARCHAR(500),
        PsychologicalTrauma NVARCHAR(500),
        Magic NVARCHAR(500),
        Status NVARCHAR(100),
        WitchMethod NVARCHAR(500),
        CreatedAt DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_WitchDetail_Witch FOREIGN KEY(WitchID) REFERENCES wt.Witch(WitchID)
    );
    PRINT '创建WitchDetail表成功';
END

-- 2. 清理可能存在的重复数据
DELETE FROM wt.WitchDetail 
WHERE WitchID IN (SELECT WitchID FROM wt.Witch WHERE PrisonerNo BETWEEN '684' AND '696');

-- 3. 导入684-691角色详细信息
PRINT '开始导入684-691详细信息...';

INSERT INTO wt.WitchDetail (
    WitchID, PersonalID, FormerName, Gender, BirthDate, Ethnicity, BirthPlace, Height, Weight, BloodType,
    ResidentialAddress, Phone, Email, LineAccount, Remarks, HighestEducation,
    Education1_School, Education1_Degree, Education1_Grade, Education1_Notes,
    Work_Company, Work_Period, Work_Position, Work_Salary, Work_Reason,
    Family_Father, Family_Mother, Family_Other1, Family_Other2, Family_Other3,
    Skills, Hobbies, Ideals, Dislikes, PsychologicalTrauma,
    Magic, Status, WitchMethod
)
SELECT 
    w.WitchID,
    CASE w.PrisonerNo
        WHEN '684' THEN '1001-0305-0817'
        WHEN '685' THEN '0222-4444-6666'
        WHEN '686' THEN '1212-3333-7777'
        WHEN '687' THEN '0505-8888-2222'
        WHEN '688' THEN '1010-7777-3333'
        WHEN '689' THEN '0430-1234-5678'
        WHEN '690' THEN '0721-5555-8888'
        WHEN '691' THEN '1103-9999-1111'
        WHEN '692' THEN '0824-6666-9999'
        WHEN '693' THEN '0915-7777-8888'
        WHEN '694' THEN '1218-5555-7777'
        WHEN '695' THEN '0320-9999-6666'
        WHEN '696' THEN '0630-8888-5555'
        ELSE NULL
    END AS PersonalID,
    '无' AS FormerName,
    '女' AS Gender,
    CASE w.PrisonerNo
        WHEN '684' THEN '2008-03-05'
        WHEN '685' THEN '2008-02-22'
        WHEN '686' THEN '2012-12-12'
        WHEN '687' THEN '2001-05-05'
        WHEN '688' THEN '2001-10-10'
        WHEN '689' THEN '2007-04-30'
        WHEN '690' THEN '2008-07-21'
        WHEN '691' THEN '2007-11-03'
        WHEN '692' THEN '2006-08-24'
        WHEN '693' THEN '2005-09-15'
        WHEN '694' THEN '2004-12-18'
        WHEN '695' THEN '2006-03-20'
        WHEN '696' THEN '2005-06-30'
        ELSE NULL
    END AS BirthDate,
    '大和民族' AS Ethnicity,
    CASE w.PrisonerNo
        WHEN '684' THEN '东京都'
        WHEN '685' THEN '东京都'
        WHEN '686' THEN '东京都'
        WHEN '687' THEN '福冈县'
        WHEN '688' THEN '福冈县'
        WHEN '689' THEN '埼玉县'
        WHEN '690' THEN '神奈川县'
        WHEN '691' THEN '大阪府'
        WHEN '692' THEN '京都府'
        WHEN '693' THEN '北海道'
        WHEN '694' THEN '兵库县'
        WHEN '695' THEN '爱知县'
        WHEN '696' THEN '冲绳县'
        ELSE NULL
    END AS BirthPlace,
    CASE w.PrisonerNo
        WHEN '684' THEN 158
        WHEN '685' THEN 148
        WHEN '686' THEN 140
        WHEN '687' THEN 162
        WHEN '688' THEN 165
        WHEN '689' THEN 155
        WHEN '690' THEN 153
        WHEN '691' THEN 160
        WHEN '692' THEN 145
        WHEN '693' THEN 170
        WHEN '694' THEN 152
        WHEN '695' THEN 156
        WHEN '696' THEN 150
        ELSE NULL
    END AS Height,
    CASE w.PrisonerNo
        WHEN '684' THEN 45
        WHEN '685' THEN 46
        WHEN '686' THEN 38
        WHEN '687' THEN 49
        WHEN '688' THEN 51
        WHEN '689' THEN 43
        WHEN '690' THEN 42
        WHEN '691' THEN 48
        WHEN '692' THEN 40
        WHEN '693' THEN 55
        WHEN '694' THEN 44
        WHEN '695' THEN 47
        WHEN '696' THEN 41
        ELSE NULL
    END AS Weight,
    CASE w.PrisonerNo
        WHEN '684' THEN 'O'
        WHEN '685' THEN 'AB'
        WHEN '686' THEN 'A'
        WHEN '687' THEN 'O'
        WHEN '688' THEN 'B'
        WHEN '689' THEN 'A'
        WHEN '690' THEN 'B'
        WHEN '691' THEN 'O'
        WHEN '692' THEN 'AB'
        WHEN '693' THEN 'B'
        WHEN '694' THEN 'A'
        WHEN '695' THEN 'O'
        WHEN '696' THEN 'AB'
        ELSE NULL
    END AS BloodType,
    CASE w.PrisonerNo
        WHEN '684' THEN '东京都新宿区歌舞伎町1丁目'
        WHEN '685' THEN '东京都涩谷区神宫前3丁目'
        WHEN '686' THEN '东京都练马区光丘1丁目'
        WHEN '687' THEN '福冈县福冈市博多区祇园1丁目'
        WHEN '688' THEN '福冈县福冈市中央区大名2丁目'
        WHEN '689' THEN '埼玉县越谷市レイクタウン4丁目'
        WHEN '690' THEN '神奈川县横滨市港未来2丁目'
        WHEN '691' THEN '大阪府大阪市此花区桜島1丁目'
        ELSE NULL
    END AS ResidentialAddress,
    CASE w.PrisonerNo
        WHEN '684' THEN '03-3355-6677'
        WHEN '685' THEN '03-5775-1111'
        WHEN '686' THEN '03-3925-8888'
        WHEN '687' THEN '092-411-9999'
        WHEN '688' THEN '092-731-5555'
        WHEN '689' THEN '048-987-6543'
        WHEN '690' THEN '045-222-3333'
        WHEN '691' THEN '06-7777-8888'
        ELSE NULL
    END AS Phone,
    CASE w.PrisonerNo
        WHEN '684' THEN 'mttn_hiiragi@example.com'
        WHEN '685' THEN 'kiwi_aragaki@example.com'
        WHEN '686' THEN 'korisu_morino@example.com'
        WHEN '687' THEN 'matama_akoya@example.com'
        WHEN '688' THEN 'nemo_anemo@example.com'
        WHEN '689' THEN 'haruka_hanabishi@example.com'
        WHEN '690' THEN 'sayo_minagami@example.com'
        WHEN '691' THEN 'kaoruko_amakawa@example.com'
        ELSE NULL
    END AS Email,
    CASE w.PrisonerNo
        WHEN '684' THEN 'matina_0305'
        WHEN '685' THEN 'kiwi_0222'
        WHEN '686' THEN 'korisu_1212'
        WHEN '687' THEN 'matama_0505'
        WHEN '688' THEN 'nemo_1010'
        WHEN '689' THEN 'haruka_0430'
        WHEN '690' THEN 'sayo_0721'
        WHEN '691' THEN 'kaoruko_1103'
        ELSE NULL
    END AS LineAccount,
    CASE w.PrisonerNo
        WHEN '684' THEN '表面上是普通学生，真实身份是邪恶组织女干部'
        WHEN '685' THEN '邪恶组织女干部，样貌极度自信'
        WHEN '686' THEN '邪恶组织女干部，孤僻小学生'
        WHEN '687' THEN '前主旅团成员，梦想歌手'
        WHEN '688' THEN '前主旅团成员，与真珠自幼稚园认识'
        WHEN '689' THEN '特蕾丝玛吉雅小队队长，性格开朗'
        WHEN '690' THEN '特蕾丝玛吉雅成员，近战担当'
        WHEN '691' THEN '特蕾丝玛吉雅成员，力量担当'
        ELSE NULL
    END AS Remarks,
    '中学校在读' AS HighestEducation,
    CASE w.PrisonerNo
        WHEN '684' THEN '东京都立圣樱中学校'
        WHEN '685' THEN '东京都立原宿中学校'
        WHEN '686' THEN '东京都立光丘小学校'
        WHEN '687' THEN '福冈县立博多高等学校'
        WHEN '688' THEN '福冈县立福冈中央高等学校'
        WHEN '689' THEN '埼玉县立光之丘中学校'
        WHEN '690' THEN '神奈川县立港中学校'
        WHEN '691' THEN '大阪府立咲洲中学校'
        ELSE NULL
    END AS Education1_School,
    '中学校' AS Education1_Degree,
    CASE w.PrisonerNo
        WHEN '684' THEN '2年生（初二）在读'
        WHEN '685' THEN '2年生（初二）在读'
        WHEN '686' THEN '5年生在读'
        WHEN '687' THEN '2年生（初二）在读'
        WHEN '688' THEN '2年生（初二）在读'
        WHEN '689' THEN '3年生（初三）在读'
        WHEN '690' THEN '2年生（初二）在读'
        WHEN '691' THEN '3年生（初三）在读'
        ELSE NULL
    END AS Education1_Grade,
    CASE w.PrisonerNo
        WHEN '684' THEN '成绩中等，在校表现普通'
        WHEN '685' THEN '成绩下游，热衷自拍和社交网络'
        WHEN '686' THEN '不擅长社交，手工艺天赋强'
        WHEN '687' THEN '参加轻音部，多次选拔落选'
        WHEN '688' THEN '学生会长，成绩优异'
        WHEN '689' THEN '成绩优异，班级委员，领导力强'
        WHEN '690' THEN '运动神经发达，文化课稍弱'
        WHEN '691' THEN '体育万能，性格豪爽，大姐头'
        ELSE NULL
    END AS Education1_Notes,
    NULL AS Education2_School, NULL AS Education2_Degree, NULL AS Education2_Grade, NULL AS Education2_Notes,
    '邪恶组织艾诺尔米塔' AS Work_Company,
    '2023年至今' AS Work_Period,
    CASE w.PrisonerNo
        WHEN '684' THEN '女干部；对抗魔法少女'
        WHEN '685' THEN '女干部；炸弹制造与操纵'
        WHEN '686' THEN '女干部；玩偶侦察战斗'
        WHEN '687' THEN '女干部主旅团；声波攻击'
        WHEN '688' THEN '女干部主旅团；影子操纵'
        WHEN '689' THEN '魔法品红；对抗邪恶'
        WHEN '690' THEN '魔法碧蓝；前线突击'
        WHEN '691' THEN '魔法硫磺；力量输出'
        ELSE NULL
    END AS Work_Position,
    CASE w.PrisonerNo
        WHEN '684' THEN '时薪1100日元'
        WHEN '685' THEN '时薪1200日元'
        WHEN '686' THEN '时薪1000日元'
        WHEN '687' THEN '月薪25万日元'
        WHEN '688' THEN '月薪28万日元'
        WHEN '689' THEN '无薪（志愿）'
        WHEN '690' THEN '无薪（志愿）'
        WHEN '691' THEN '无薪（志愿）'
        ELSE NULL
    END AS Work_Salary,
    CASE w.PrisonerNo
        WHEN '684' THEN '在职'
        WHEN '685' THEN '在职'
        WHEN '686' THEN '在职'
        WHEN '687' THEN '转投新总帅'
        WHEN '688' THEN '转投新总帅'
        WHEN '689' THEN '在职'
        WHEN '690' THEN '在职'
        WHEN '691' THEN '在职'
        ELSE NULL
    END AS Work_Reason,
    CASE w.PrisonerNo
        WHEN '684' THEN '柊一郎，48岁，系统工程师，東京システム株式会社，年收800万'
        WHEN '685' THEN '阿良河信也（已故）'
        WHEN '686' THEN '杜乃正人，40岁，玩具设计师，タカラトミー株式会社，年收650万'
        WHEN '687' THEN '阿古屋幸夫，55岁，渔业经营者，博多漁業，年收1000万'
        WHEN '688' THEN '姊母健一郎，58岁，法官，福岡地方裁判所，年收1200万'
        WHEN '689' THEN '花菱大辅，50岁，公务员，埼玉県厅，年收700万'
        WHEN '690' THEN '水神海斗，46岁，船长，横浜漁業協同組合，年收600万'
        WHEN '691' THEN '天川刚志，49岁，建筑工地主任，大建工業株式会社，年收750万'
        ELSE NULL
    END AS Family_Father,
    CASE w.PrisonerNo
        WHEN '684' THEN '柊由美，45岁，花店店主，フラワーショップ「ゆめ」，年收400万'
        WHEN '685' THEN '阿良河玲奈，44岁，美容师，ビューティサロン「R」，年收500万'
        WHEN '686' THEN '杜乃七海，38岁，绘本作家，在家工作，年收300万'
        WHEN '687' THEN '阿古屋珠子，52岁，传统艺能讲师，日本舞踊「珠の会」，年收400万'
        WHEN '688' THEN '姊母律子，55岁，律师，弁護士法人「姉母」，年收800万'
        WHEN '689' THEN '花菱绫子，47岁，兼职店员，スーパー「マルエツ」，年收200万'
        WHEN '690' THEN '水神渚，43岁，潜水员，マリンサービス「シーブルー」，年收300万'
        WHEN '691' THEN '天川惠美，46岁，健身房教练，フィットネスクラブ「パワー」，年收350万'
        ELSE NULL
    END AS Family_Mother,
    CASE w.PrisonerNo
        WHEN '689' THEN '花菱夏奈，12岁'
        WHEN '689' THEN '花菱秋穗，10岁'
        WHEN '689' THEN '花菱美冬，7岁'
        ELSE NULL
    END AS Family_Other1,
    CASE w.PrisonerNo
        WHEN '687' THEN '好友：姊母娞摩'
        ELSE NULL
    END AS Family_Other2,
    NULL AS Family_Other3,
    CASE w.PrisonerNo
        WHEN '684' THEN '暗黑魔力操纵、变身、调教、伪装'
        WHEN '685' THEN '炸弹制造操纵、自拍构图、社交网络运营'
        WHEN '686' THEN '玩偶操纵、玩具屋构造、手工艺制作'
        WHEN '687' THEN '声波魔法、歌唱、原创歌词'
        WHEN '688' THEN '影子操纵、战略策划、人员管理、暗中运作'
        WHEN '689' THEN '长枪战斗、治愈魔法、团队指挥、烹饪'
        WHEN '690' THEN '近身格斗、冰属性魔法、高速移动'
        WHEN '691' THEN '怪力、火属性魔法、防御强化'
        ELSE NULL
    END AS Skills,
    CASE w.PrisonerNo
        WHEN '684' THEN '收集魔法少女周边、观看动画、品尝甜品'
        WHEN '685' THEN '自拍照、逛时尚商店、收集爆炸物素材'
        WHEN '686' THEN '收集制作玩偶、设计衣服、独自玩耍'
        WHEN '687' THEN '唱歌、作曲、地下偶像活动'
        WHEN '688' THEN '阅读法律书籍、策划活动、帮助真珠'
        WHEN '689' THEN '料理、照顾妹妹、收集蘑菇物品'
        WHEN '690' THEN '游泳、钓鱼、水上运动、格斗比赛'
        WHEN '691' THEN '力量训练、吃肉、摔角、照顾小动物'
        ELSE NULL
    END AS Hobbies,
    CASE w.PrisonerNo
        WHEN '684' THEN '以恶役身份享受与魔法少女的战斗'
        WHEN '685' THEN '成为最引人注目焦点'
        WHEN '686' THEN '创造所有玩偶都能自由活动的玩具王国'
        WHEN '687' THEN '成为被认可的顶级歌手'
        WHEN '688' THEN '守护真珠的梦想（手段不光彩）'
        WHEN '689' THEN '守护城市的和平与大家的笑容'
        WHEN '690' THEN '成为最强最快的魔法少女'
        WHEN '691' THEN '用力量保护所有需要保护的人'
        ELSE NULL
    END AS Ideals,
    CASE w.PrisonerNo
        WHEN '684' THEN '虚伪的正义、破坏手办'
        WHEN '685' THEN '被忽视、风头被抢、不够可爱'
        WHEN '686' THEN '被触碰珍视玩偶、吵闹环境'
        WHEN '687' THEN '批评其歌艺的人、冷场'
        WHEN '688' THEN '背叛、让真珠伤心'
        WHEN '689' THEN '黄豆粉、不珍惜家人'
        WHEN '690' THEN '优柔寡断、热水'
        WHEN '691' THEN '欺负弱小、蔬菜特别青椒'
        ELSE NULL
    END AS Dislikes,
    CASE w.PrisonerNo
        WHEN '684' THEN '首次战斗发现享受施虐快感'
        WHEN '685' THEN '因身材矮小被排挤，渴望被关注'
        WHEN '686' THEN '性格内向难以交友，情感寄托于玩偶'
        WHEN '687' THEN '多次选拔落选，羞耻状态才能一流'
        WHEN '688' THEN '活在精英期望中，羡慕真珠直率'
        WHEN '689' THEN '曾被邪恶力量控制成傀儡'
        WHEN '690' THEN '战斗败北产生对惩罚的期待'
        WHEN '691' THEN '无法控制力量导致焦虑'
        ELSE NULL
    END AS PsychologicalTrauma,
    w.Magic AS Magic,
    w.[Status] AS Status,
    '无' AS WitchMethod
FROM wt.Witch w
WHERE w.PrisonerNo BETWEEN '684' AND '691';

PRINT '684-691详细信息导入完成';

-- 4. 验证导入结果
PRINT '';
PRINT '=== 验证导入结果 ===';

SELECT 
    COUNT(*) AS 导入数量,
    COUNT(CASE WHEN w.PrisonerNo BETWEEN '684' AND '691' THEN 1 END) AS 前8个数量,
    COUNT(CASE WHEN w.PrisonerNo BETWEEN '692' AND '696' THEN 1 END) AS 后5个数量
FROM wt.WitchDetail wd
JOIN wt.Witch w ON w.WitchID = wd.WitchID
WHERE w.PrisonerNo BETWEEN '684' AND '696';

-- 5. 补充692-696角色详细信息（如果还没有导入）
IF NOT EXISTS (SELECT 1 FROM wt.WitchDetail wd JOIN wt.Witch w ON w.WitchID = wd.WitchID WHERE w.PrisonerNo = '692')
BEGIN
    PRINT '开始补充692-696详细信息...';
    
    -- 先补充基础信息部分
    INSERT INTO wt.WitchDetail (
        WitchID, PersonalID, FormerName, Gender, BirthDate, Ethnicity, BirthPlace, Height, Weight, BloodType,
        ResidentialAddress, Phone, Email, LineAccount, Remarks, HighestEducation,
        Education1_School, Education1_Degree, Education1_Grade, Education1_Notes,
        Work_Company, Work_Period, Work_Position, Work_Salary, Work_Reason,
        Family_Father, Family_Mother, Family_Other1, Family_Other2, Family_Other3,
        Skills, Hobbies, Ideals, Dislikes, PsychologicalTrauma,
        Magic, Status, WitchMethod
    )
    SELECT 
        w.WitchID,
        CASE w.PrisonerNo
            WHEN '692' THEN '0824-6666-9999'
            WHEN '693' THEN '0915-7777-8888'
            WHEN '694' THEN '1218-5555-7777'
            WHEN '695' THEN '0320-9999-6666'
            WHEN '696' THEN '0630-8888-5555'
            ELSE NULL
        END AS PersonalID,
        '无' AS FormerName,
        '女' AS Gender,
        CASE w.PrisonerNo
            WHEN '692' THEN '2006-08-24'
            WHEN '693' THEN '2005-09-15'
            WHEN '694' THEN '2004-12-18'
            WHEN '695' THEN '2006-03-20'
            WHEN '696' THEN '2005-06-30'
            ELSE NULL
        END AS BirthDate,
        '大和民族' AS Ethnicity,
        CASE w.PrisonerNo
            WHEN '692' THEN '京都府'
            WHEN '693' THEN '北海道'
            WHEN '694' THEN '兵库县'
            WHEN '695' THEN '爱知县'
            WHEN '696' THEN '冲绳县'
            ELSE NULL
        END AS BirthPlace,
        CASE w.PrisonerNo
            WHEN '692' THEN 145
            WHEN '693' THEN 170
            WHEN '694' THEN 152
            WHEN '695' THEN 156
            WHEN '696' THEN 150
            ELSE NULL
        END AS Height,
        CASE w.PrisonerNo
            WHEN '692' THEN 40
            WHEN '693' THEN 55
            WHEN '694' THEN 44
            WHEN '695' THEN 47
            WHEN '696' THEN 41
            ELSE NULL
        END AS Weight,
        CASE w.PrisonerNo
            WHEN '692' THEN 'AB'
            WHEN '693' THEN 'B'
            WHEN '694' THEN 'A'
            WHEN '695' THEN 'O'
            WHEN '696' THEN 'AB'
            ELSE NULL
        END AS BloodType,
        CASE w.PrisonerNo
            WHEN '692' THEN '京都府京都市左京区吉田1丁目'
            WHEN '693' THEN '北海道札幌市中央区大通西3丁目'
            WHEN '694' THEN '兵库县神户市中央区三宫町1丁目'
            WHEN '695' THEN '爱知县名古屋市中区荣3丁目'
            WHEN '696' THEN '冲绳县那霸市泉崎1丁目'
            ELSE NULL
        END AS ResidentialAddress,
        CASE w.PrisonerNo
            WHEN '692' THEN '075-721-9999'
            WHEN '693' THEN '011-222-3333'
            WHEN '694' THEN '078-311-8888'
            WHEN '695' THEN '052-888-7777'
            WHEN '696' THEN '098-999-6666'
            ELSE NULL
        END AS Phone,
        CASE w.PrisonerNo
            WHEN '692' THEN 'mitsu_minami@example.com'
            WHEN '693' THEN 'izuna_hokkaido@example.com'
            WHEN '694' THEN 'michie_hyogo@example.com'
            WHEN '695' THEN 'sakura_aichi@example.com'
            WHEN '696' THEN 'tsuki_style@example.com'
            ELSE NULL
        END AS Email,
        CASE w.PrisonerNo
            WHEN '692' THEN 'mitsu_0824'
            WHEN '693' THEN 'izuna_0915'
            WHEN '694' THEN 'michie_1218'
            WHEN '695' THEN 'sakura_0320'
            WHEN '696' THEN 'tsuki_0630'
            ELSE NULL
        END AS LineAccount,
        CASE w.PrisonerNo
            WHEN '692' THEN '特蕾丝玛吉雅小队的支援型成员，拥有强大的治疗能力'
            WHEN '693' THEN '神秘的魔法少女，拥有强大的火焰操控能力'
            WHEN '694' THEN '拥有冰系魔法的神秘少女，能够操控时间与空间'
            WHEN '695' THEN '拥有光系魔法的魔法少女，性格开朗活泼'
            WHEN '696' THEN '神秘的占卜师，能够通过星象预测未来'
            ELSE NULL
        END AS Remarks,
        '中学校在读' AS HighestEducation,
        CASE w.PrisonerNo
            WHEN '692' THEN '京都市立紫竹中学校'
            WHEN '693' THEN '札幌市立中央中学校'
            WHEN '694' THEN '神戸市立神港中学校'
            WHEN '695' THEN '名古屋市立千种中学校'
            WHEN '696' THEN '那霸市立上山中学校'
            ELSE NULL
        END AS Education1_School,
        '中学校' AS Education1_Degree,
        '2年生（初二）在读' AS Education1_Grade,
        '成绩优秀，在校表现良好' AS Education1_Notes,
        NULL AS Work_Company,
        NULL AS Work_Period,
        '学生' AS Work_Position,
        NULL AS Work_Salary,
        NULL AS Work_Reason,
        CASE w.PrisonerNo
            WHEN '692' THEN '相野和彦，50岁，大学教授，京都大学，年收1000万'
            WHEN '693' THEN '平良勇人，52岁，消防员，札幌市消防局，年收700万'
            WHEN '694' THEN '江利内诚司，48岁，医师，神户大学医学部附属医院，年收1500万'
            WHEN '695' THEN '椎崎启太，45岁，会社员，トヨタ自动车，年收800万'
            WHEN '696' THEN '月出银次，年龄不详，占卜师，月出占卜馆，收入不定'
            ELSE NULL
        END AS Family_Father,
        CASE w.PrisonerNo
            WHEN '692' THEN '相野美咲，48岁，营养师，京都府立医科大学附属病院，年收600万'
            WHEN '693' THEN '平良由纪，50岁，教师，札幌市立小学校，年收500万'
            WHEN '694' THEN '江利内惠子，46岁，看护师，神户市医疗センター，年收450万'
            WHEN '695' THEN '椎崎美香，43岁，主妇，専业主妇'
            WHEN '696' THEN '月出月光，年龄不详，助手，月出占卜馆'
            ELSE NULL
        END AS Family_Mother,
        NULL, NULL, NULL,  -- Family_Other1/2/3
        CASE w.PrisonerNo
            WHEN '692' THEN '翠绿魔法：藤蔓缠绕、花粉催眠、森之精灵真化'
            WHEN '693' THEN '琥珀魔法：火焰之箭、熔岩护盾、烈焰魔女真化'
            WHEN '694' THEN '白银魔法：寒冰之镜、冰封时间、雪之公主真化'
            WHEN '695' THEN '黄金魔法：光芒之剑、净化结界、太阳女神真化'
            WHEN '696' THEN '星空魔法：星辰陨落、幻影分身、月之女神真化'
            ELSE NULL
        END AS Skills,
        CASE w.PrisonerNo
            WHEN '692' THEN '园艺、茶道、阅读古典文学'
            WHEN '693' THEN '登山、露营、野外生存技能'
            WHEN '694' THEN '花样滑冰、钢琴、美术鉴赏'
            WHEN '695' THEN '摄影、音乐、志愿者活动'
            WHEN '696' THEN '占卜、星象观测、古代神话研究'
            ELSE NULL
        END AS Hobbies,
        CASE w.PrisonerNo
            WHEN '692' THEN '创造一个所有生命都能和谐共存的世界'
            WHEN '693' THEN '找到失散的姐姐，掌握真正的力量'
            WHEN '694' THEN '打破家族诅咒，获得真正的自由'
            WHEN '695' THEN '用光芒照亮世界的每个角落'
            WHEN '696' THEN '改变命运的轨迹，找到失落的记忆'
            ELSE NULL
        END AS Ideals,
        CASE w.PrisonerNo
            WHEN '692' THEN '无意义的破坏、孤独、背叛'
            WHEN '693' THEN '弱者的哀嚎、无谓的牺牲'
            WHEN '694' THEN '束缚、不公、命运的摆布'
            WHEN '695' THEN '黑暗、绝望、世界的冷漠'
            WHEN '696' THEN '命运既定论、无法改变的宿命'
            ELSE NULL
        END AS Dislikes,
        CASE w.PrisonerNo
            WHEN '692' THEN '因过于担心他人而导致的决策迟疑'
            WHEN '693' THEN '童年时无法控制力量导致的意外'
            WHEN '694' THEN '家族传承的沉重压力和期望'
            WHEN '695' THEN '现实的残酷与理想的冲突'
            WHEN '696' THEN '失去重要记忆的痛苦'
            ELSE NULL
        END AS PsychologicalTrauma,
        w.Magic AS Magic,
        w.[Status] AS Status,
        '无' AS WitchMethod
    FROM wt.Witch w
    WHERE w.PrisonerNo BETWEEN '692' AND '696';
    
    PRINT '692-696详细信息补充完成';
END
ELSE
BEGIN
    PRINT '692-696详细信息已存在，跳过补充';
END

-- 6. 最终验证所有13个角色
PRINT '';
PRINT '=== 最终验证所有684-696角色 ===';

SELECT 
    COUNT(*) AS 总导入数量,
    COUNT(CASE WHEN w.PrisonerNo BETWEEN '684' AND '691' THEN 1 END) AS 前8个数量,
    COUNT(CASE WHEN w.PrisonerNo BETWEEN '692' AND '696' THEN 1 END) AS 后5个数量
FROM wt.WitchDetail wd
JOIN wt.Witch w ON w.WitchID = wd.WitchID
WHERE w.PrisonerNo BETWEEN '684' AND '696';

-- 显示详细列表
PRINT '';
SELECT 
    '完整验证' AS 状态,
    wd.DetailID,
    w.PrisonerNo,
    w.Name AS 魔女名,
    wd.PersonalID AS 个人番号,
    wd.Gender AS 性别,
    wd.BirthDate AS 出生日期,
    wd.Height AS 身高,
    wd.Weight AS 体重,
    wd.Education1_School AS 学校
FROM wt.WitchDetail wd
JOIN wt.Witch w ON w.WitchID = wd.WitchID
WHERE w.PrisonerNo BETWEEN '684' AND '696'
ORDER BY w.PrisonerNo;

PRINT '';
PRINT '=== 导入完成 ===';
PRINT '684-696所有13个角色详细信息已导入';
PRINT '现在可以查看角色的完整详细信息了';
GO