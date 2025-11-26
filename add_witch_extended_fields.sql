-- ========================================
-- 扩展魔女表字段 - 添加详细档案信息
-- ========================================

USE WitchTrialWT;
GO

-- ========================================
-- 1. 添加基本信息字段
-- ========================================

ALTER TABLE wt.Witch ADD
    -- 基本身份信息
    PersonalNo NVARCHAR(20) NULL,                    -- 个人番号
    FormerName NVARCHAR(100) NULL,                   -- 曾用名
    Gender NVARCHAR(10) NULL,                        -- 性别
    BirthDate DATE NULL,                             -- 出生日期
    Ethnicity NVARCHAR(50) NULL,                     -- 民族
    Birthplace NVARCHAR(100) NULL,                   -- 籍贯
    
    -- 身体特征
    Height DECIMAL(5,2) NULL,                        -- 身高(cm)
    Weight DECIMAL(5,2) NULL,                        -- 体重(kg)
    BloodType NVARCHAR(10) NULL,                     -- 血型
    
    -- 联系方式
    Address NVARCHAR(500) NULL,                      -- 住民票地址
    Phone NVARCHAR(50) NULL,                         -- 电话
    Email NVARCHAR(100) NULL,                        -- 邮箱
    LineAccount NVARCHAR(100) NULL,                  -- LINE账号
    
    -- 教育背景（最高学历）
    HighestEducation NVARCHAR(100) NULL,             -- 最高学历
    
    -- 家庭关系（简要）
    FamilyStructure NVARCHAR(200) NULL,              -- 家庭基本情况
    Father NVARCHAR(200) NULL,                       -- 父亲信息
    Mother NVARCHAR(200) NULL,                       -- 母亲信息
    OtherFamily1 NVARCHAR(200) NULL,                 -- 其他家庭成员1
    OtherFamily2 NVARCHAR(200) NULL,                 -- 其他家庭成员2
    OtherFamily3 NVARCHAR(200) NULL,                 -- 其他家庭成员3
    
    -- 个性特征
    Skills NVARCHAR(500) NULL,                       -- 技能/特长
    Hobbies NVARCHAR(500) NULL,                      -- 兴趣爱好
    Dreams NVARCHAR(500) NULL,                       -- 理想
    Dislikes NVARCHAR(500) NULL,                     -- 讨厌的事物
    Trauma NVARCHAR(MAX) NULL,                       -- 心理创伤
    
    -- 魔女相关
    WitchTransformMethod NVARCHAR(500) NULL,         -- 魔女化办法
    
    -- 备注
    Remarks NVARCHAR(MAX) NULL;                      -- 备注
GO

PRINT '✅ 基本字段添加完成';
GO

-- ========================================
-- 2. 添加教育经历字段（JSON 格式）
-- ========================================

ALTER TABLE wt.Witch ADD
    EducationHistory NVARCHAR(MAX) NULL;             -- 教育经历（JSON格式）
GO

PRINT '✅ 教育经历字段添加完成';
GO

-- 教育经历 JSON 格式示例：
/*
[
  {
    "school": "东京都立樱丘中学校",
    "degree": "中学校",
    "status": "毕业",
    "specialNote": "初中时旁观好友月代雪霸凌致其自杀，篡改记忆掩盖创伤"
  },
  {
    "school": "东京都立樱丘高等学校",
    "degree": "高等学校",
    "status": "未入学",
    "specialNote": "高中开学前一日被抓至魔女岛"
  }
]
*/

-- ========================================
-- 3. 添加工作经历字段（JSON 格式）
-- ========================================

ALTER TABLE wt.Witch ADD
    WorkHistory NVARCHAR(MAX) NULL;                  -- 工作经历（JSON格式）
GO

PRINT '✅ 工作经历字段添加完成';
GO

-- 工作经历 JSON 格式示例：
/*
[
  {
    "period": "2020/04-2022/03",
    "company": "东京商事株式会社",
    "position": "营业部助理",
    "salary": "月薪 25 万日元",
    "resignReason": "被发现魔女身份"
  }
]
*/

-- ========================================
-- 4. 验证字段是否添加成功
-- ========================================

SELECT 
    COLUMN_NAME AS 字段名,
    DATA_TYPE AS 数据类型,
    CHARACTER_MAXIMUM_LENGTH AS 最大长度,
    IS_NULLABLE AS 可为空
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'wt' 
    AND TABLE_NAME = 'Witch'
    AND COLUMN_NAME IN (
        'PersonalNo', 'FormerName', 'Gender', 'BirthDate', 'Ethnicity', 'Birthplace',
        'Height', 'Weight', 'BloodType', 'Address', 'Phone', 'Email', 'LineAccount',
        'HighestEducation', 'FamilyStructure', 'Father', 'Mother', 
        'OtherFamily1', 'OtherFamily2', 'OtherFamily3',
        'Skills', 'Hobbies', 'Dreams', 'Dislikes', 'Trauma',
        'WitchTransformMethod', 'Remarks', 'EducationHistory', 'WorkHistory'
    )
ORDER BY ORDINAL_POSITION;
GO

PRINT '✅ 所有字段添加完成！';
PRINT '📊 请运行上面的查询验证字段是否正确添加';
GO

-- ========================================
-- 5. 插入樱羽艾玛的示例数据
-- ========================================

UPDATE wt.Witch
SET 
    -- 基本信息
    PersonalNo = '1234-5678-9011',
    FormerName = '无',
    Gender = '女',
    BirthDate = '2010-03-05',
    Ethnicity = '大和民族',
    Birthplace = '东京都',
    
    -- 身体特征
    Height = 156.00,
    Weight = 48.00,
    BloodType = 'A',
    
    -- 联系方式
    Address = '东京都涩谷区道玄坂 2 丁目',
    Phone = '03-1234-5678',
    Email = 'sakuraba_ema@yahoo.co.jp',
    LineAccount = 'ema_sakura0305',
    
    -- 教育背景
    HighestEducation = '中学校毕业',
    EducationHistory = N'[
        {
            "school": "东京都立樱丘中学校",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "初中时旁观好友月代雪霸凌致其自杀，篡改记忆掩盖创伤"
        },
        {
            "school": "东京都立樱丘高等学校",
            "degree": "高等学校",
            "status": "未入学",
            "specialNote": "高中开学前一日被抓至魔女岛"
        }
    ]',
    
    -- 工作经历
    WorkHistory = N'[]',  -- 无工作经历
    
    -- 家庭关系
    FamilyStructure = '核心成员为父母',
    Father = '樱羽健一，45 岁，会社社员，东京商事株式会社',
    Mother = '樱羽静香，43 岁，家庭主妇',
    OtherFamily1 = NULL,
    OtherFamily2 = NULL,
    OtherFamily3 = NULL,
    
    -- 个性特征
    Skills = '推理能力敏锐、观察力强、吃饭快速',
    Hobbies = '寻找美食店、和朋友相处',
    Dreams = '交 100 个朋友',
    Dislikes = '孤独、被排挤',
    Trauma = '旁观好友月代雪霸凌致其自杀，篡改记忆掩盖创伤',
    
    -- 魔女相关
    WitchTransformMethod = '无',
    
    -- 备注
    Remarks = '无'
    
WHERE PrisonerNo = '658';  -- 樱羽艾玛
GO

PRINT '✅ 樱羽艾玛的详细档案已更新';
GO

-- ========================================
-- 6. 查询验证樱羽艾玛的完整档案
-- ========================================

SELECT 
    WitchID,
    Name AS 姓名,
    PrisonerNo AS 囚人番号,
    PersonalNo AS 个人番号,
    Gender AS 性别,
    BirthDate AS 出生日期,
    Height AS 身高,
    Weight AS 体重,
    BloodType AS 血型,
    Phone AS 电话,
    Email AS 邮箱,
    HighestEducation AS 最高学历,
    Father AS 父亲,
    Mother AS 母亲,
    Skills AS 技能特长,
    Dreams AS 理想,
    Trauma AS 心理创伤,
    Magic AS 魔法,
    Status AS 状态
FROM wt.Witch
WHERE PrisonerNo = '658';
GO

-- ========================================
-- 7. 创建视图：魔女完整档案
-- ========================================

CREATE OR ALTER VIEW wt.v_WitchFullProfile AS
SELECT 
    w.WitchID,
    w.Name AS 姓名,
    w.PrisonerNo AS 囚人番号,
    w.PersonalNo AS 个人番号,
    w.FormerName AS 曾用名,
    w.Gender AS 性别,
    w.BirthDate AS 出生日期,
    DATEDIFF(YEAR, w.BirthDate, GETDATE()) AS 年龄,
    w.Ethnicity AS 民族,
    w.Birthplace AS 籍贯,
    w.Height AS 身高,
    w.Weight AS 体重,
    w.BloodType AS 血型,
    w.Address AS 地址,
    w.Phone AS 电话,
    w.Email AS 邮箱,
    w.LineAccount AS LINE账号,
    w.HighestEducation AS 最高学历,
    w.EducationHistory AS 教育经历,
    w.WorkHistory AS 工作经历,
    w.FamilyStructure AS 家庭结构,
    w.Father AS 父亲,
    w.Mother AS 母亲,
    w.Skills AS 技能特长,
    w.Hobbies AS 兴趣爱好,
    w.Dreams AS 理想,
    w.Dislikes AS 讨厌的事物,
    w.Trauma AS 心理创伤,
    w.Magic AS 魔法,
    w.Status AS 状态,
    w.WitchTransformMethod AS 魔女化办法,
    w.Remarks AS 备注,
    i.Name AS 岛屿,
    b.BatchID AS 批次
FROM wt.Witch w
LEFT JOIN wt.Island i ON w.IslandID = i.IslandID
LEFT JOIN wt.Batch b ON w.BatchID = b.BatchID;
GO

PRINT '✅ 视图 wt.v_WitchFullProfile 创建完成';
GO

-- ========================================
-- 8. 查询使用示例
-- ========================================

-- 查询樱羽艾玛的完整档案
SELECT * FROM wt.v_WitchFullProfile WHERE 囚人番号 = '658';
GO

-- 查询所有魔女的基本信息
SELECT 
    姓名, 囚人番号, 性别, 年龄, 身高, 体重, 血型, 
    最高学历, 魔法, 状态
FROM wt.v_WitchFullProfile
ORDER BY 囚人番号;
GO

PRINT '========================================';
PRINT '✅ 魔女表扩展完成！';
PRINT '📝 新增字段数：28 个';
PRINT '📊 支持 JSON 格式存储教育和工作经历';
PRINT '🔍 已创建视图 wt.v_WitchFullProfile 方便查询';
PRINT '========================================';
GO
