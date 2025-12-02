-- =======================================================================
-- 补充692-696角色详细信息
-- 针对导入684-696完整信息.sql中缺失的692-696角色数据
-- =======================================================================

USE WitchTrialWT;
GO

PRINT '=== 开始补充692-696角色详细信息 ===';

-- 插入692-696的详细信息
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

PRINT '692-696角色详细信息补充完成';

-- 验证导入结果
PRINT '';
PRINT '=== 验证补充结果 ===';

SELECT 
    '692-696详细信息验证' AS 状态,
    wd.DetailID,
    w.PrisonerNo,
    w.Name AS 魔女名,
    wd.PersonalID AS 个人番号,
    wd.Gender AS 性别,
    wd.BirthDate AS 出生日期,
    wd.Ethnicity AS 民族,
    wd.Height AS 身高,
    wd.Weight AS 体重,
    wd.HighestEducation AS 最高学历,
    wd.Education1_School AS 学校
FROM wt.WitchDetail wd
JOIN wt.Witch w ON w.WitchID = wd.WitchID
WHERE w.PrisonerNo BETWEEN '692' AND '696'
ORDER BY w.PrisonerNo;

PRINT '';
PRINT '=== 692-696详细信息补充完成 ===';
PRINT '现在684-696所有角色都应该有完整的详细信息了';
GO