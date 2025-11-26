-- ========================================
-- 魔女详细档案批量导入模板
-- 使用说明：复制下面的模板，填入数据后执行
-- ========================================

USE WitchTrialWT;
GO

-- ========================================
-- 方法 1：单个魔女更新（推荐用于少量数据）
-- ========================================

-- 模板：复制此段，修改数据后执行
UPDATE wt.Witch
SET 
    -- 基本信息
    PersonalNo = '1234-5678-9011',                   -- 个人番号
    FormerName = '无',                                -- 曾用名
    Gender = '女',                                    -- 性别
    BirthDate = '2010-03-05',                        -- 出生日期（格式：YYYY-MM-DD）
    Ethnicity = '大和民族',                           -- 民族
    Birthplace = '东京都',                            -- 籍贯
    
    -- 身体特征
    Height = 156.00,                                 -- 身高(cm)
    Weight = 48.00,                                  -- 体重(kg)
    BloodType = 'A',                                 -- 血型
    
    -- 联系方式
    Address = '东京都涩谷区道玄坂 2 丁目',            -- 住民票地址
    Phone = '03-1234-5678',                          -- 电话
    Email = 'sakuraba_ema@yahoo.co.jp',              -- 邮箱
    LineAccount = 'ema_sakura0305',                  -- LINE账号
    
    -- 教育背景
    HighestEducation = '中学校毕业',                  -- 最高学历
    EducationHistory = N'[
        {
            "school": "东京都立樱丘中学校",
            "degree": "中学校",
            "status": "毕业",
            "specialNote": "备注信息"
        },
        {
            "school": "东京都立樱丘高等学校",
            "degree": "高等学校",
            "status": "未入学",
            "specialNote": "备注信息"
        }
    ]',
    
    -- 工作经历（如果没有工作经历，填 '[]'）
    WorkHistory = N'[
        {
            "period": "2020/04-2022/03",
            "company": "公司名称",
            "position": "职位和职责",
            "salary": "薪资水平",
            "resignReason": "离职原因"
        }
    ]',
    
    -- 家庭关系
    FamilyStructure = '核心成员为父母',               -- 家庭基本情况
    Father = '父亲姓名，年龄，职业，工作单位',         -- 父亲信息
    Mother = '母亲姓名，年龄，职业，工作单位',         -- 母亲信息
    OtherFamily1 = NULL,                             -- 其他家庭成员1
    OtherFamily2 = NULL,                             -- 其他家庭成员2
    OtherFamily3 = NULL,                             -- 其他家庭成员3
    
    -- 个性特征
    Skills = '技能1、技能2、技能3',                   -- 技能/特长
    Hobbies = '爱好1、爱好2',                         -- 兴趣爱好
    Dreams = '理想描述',                              -- 理想
    Dislikes = '讨厌的事物1、讨厌的事物2',            -- 讨厌的事物
    Trauma = '心理创伤描述',                          -- 心理创伤
    
    -- 魔女相关
    WitchTransformMethod = '魔女化办法描述',          -- 魔女化办法
    
    -- 备注
    Remarks = '其他备注信息'                          -- 备注
    
WHERE PrisonerNo = '658';  -- ⚠️ 修改为对应的囚人番号
GO


-- ========================================
-- 方法 2：批量导入（推荐用于大量数据）
-- ========================================

-- 使用临时表批量导入
DECLARE @WitchData TABLE (
    PrisonerNo NVARCHAR(20),
    PersonalNo NVARCHAR(20),
    FormerName NVARCHAR(100),
    Gender NVARCHAR(10),
    BirthDate DATE,
    Ethnicity NVARCHAR(50),
    Birthplace NVARCHAR(100),
    Height DECIMAL(5,2),
    Weight DECIMAL(5,2),
    BloodType NVARCHAR(10),
    Address NVARCHAR(500),
    Phone NVARCHAR(50),
    Email NVARCHAR(100),
    LineAccount NVARCHAR(100),
    HighestEducation NVARCHAR(100),
    FamilyStructure NVARCHAR(200),
    Father NVARCHAR(200),
    Mother NVARCHAR(200),
    Skills NVARCHAR(500),
    Hobbies NVARCHAR(500),
    Dreams NVARCHAR(500),
    Dislikes NVARCHAR(500),
    Trauma NVARCHAR(MAX),
    WitchTransformMethod NVARCHAR(500),
    Remarks NVARCHAR(MAX)
);

-- 插入数据（示例：樱羽艾玛）
INSERT INTO @WitchData VALUES (
    '658',                                           -- 囚人番号
    '1234-5678-9011',                                -- 个人番号
    '无',                                             -- 曾用名
    '女',                                             -- 性别
    '2010-03-05',                                    -- 出生日期
    '大和民族',                                       -- 民族
    '东京都',                                         -- 籍贯
    156.00,                                          -- 身高
    48.00,                                           -- 体重
    'A',                                             -- 血型
    '东京都涩谷区道玄坂 2 丁目',                      -- 地址
    '03-1234-5678',                                  -- 电话
    'sakuraba_ema@yahoo.co.jp',                      -- 邮箱
    'ema_sakura0305',                                -- LINE账号
    '中学校毕业',                                     -- 最高学历
    '核心成员为父母',                                 -- 家庭结构
    '樱羽健一，45 岁，会社社员，东京商事株式会社',   -- 父亲
    '樱羽静香，43 岁，家庭主妇',                      -- 母亲
    '推理能力敏锐、观察力强、吃饭快速',              -- 技能
    '寻找美食店、和朋友相处',                        -- 爱好
    '交 100 个朋友',                                 -- 理想
    '孤独、被排挤',                                   -- 讨厌
    '旁观好友月代雪霸凌致其自杀，篡改记忆掩盖创伤', -- 创伤
    '无',                                             -- 魔女化办法
    '无'                                              -- 备注
);

-- 继续添加其他魔女...
-- INSERT INTO @WitchData VALUES (...);

-- 批量更新
UPDATE w
SET 
    w.PersonalNo = d.PersonalNo,
    w.FormerName = d.FormerName,
    w.Gender = d.Gender,
    w.BirthDate = d.BirthDate,
    w.Ethnicity = d.Ethnicity,
    w.Birthplace = d.Birthplace,
    w.Height = d.Height,
    w.Weight = d.Weight,
    w.BloodType = d.BloodType,
    w.Address = d.Address,
    w.Phone = d.Phone,
    w.Email = d.Email,
    w.LineAccount = d.LineAccount,
    w.HighestEducation = d.HighestEducation,
    w.FamilyStructure = d.FamilyStructure,
    w.Father = d.Father,
    w.Mother = d.Mother,
    w.Skills = d.Skills,
    w.Hobbies = d.Hobbies,
    w.Dreams = d.Dreams,
    w.Dislikes = d.Dislikes,
    w.Trauma = d.Trauma,
    w.WitchTransformMethod = d.WitchTransformMethod,
    w.Remarks = d.Remarks
FROM wt.Witch w
INNER JOIN @WitchData d ON w.PrisonerNo = d.PrisonerNo;

PRINT '✅ 批量更新完成';
GO


-- ========================================
-- 方法 3：快速填充模板（只填必要字段）
-- ========================================

-- 如果某些字段暂时没有数据，可以只填写关键字段
UPDATE wt.Witch
SET 
    PersonalNo = '个人番号',
    Gender = '性别',
    BirthDate = '出生日期',
    Height = 身高,
    Weight = 体重,
    BloodType = '血型',
    HighestEducation = '最高学历',
    Magic = '魔法能力',
    Status = '状态'
WHERE PrisonerNo = '囚人番号';
GO


-- ========================================
-- 验证查询
-- ========================================

-- 查看所有魔女的基本信息
SELECT 
    PrisonerNo AS 囚人番号,
    Name AS 姓名,
    Gender AS 性别,
    BirthDate AS 出生日期,
    Height AS 身高,
    Weight AS 体重,
    BloodType AS 血型,
    HighestEducation AS 最高学历,
    Magic AS 魔法,
    Status AS 状态
FROM wt.Witch
ORDER BY PrisonerNo;
GO

-- 查看特定魔女的完整档案
SELECT * FROM wt.v_WitchFullProfile WHERE 囚人番号 = '658';
GO

-- 查看哪些魔女还没有填写详细信息
SELECT 
    PrisonerNo AS 囚人番号,
    Name AS 姓名,
    CASE WHEN PersonalNo IS NULL THEN '❌' ELSE '✅' END AS 个人番号,
    CASE WHEN Gender IS NULL THEN '❌' ELSE '✅' END AS 性别,
    CASE WHEN BirthDate IS NULL THEN '❌' ELSE '✅' END AS 出生日期,
    CASE WHEN Height IS NULL THEN '❌' ELSE '✅' END AS 身高,
    CASE WHEN HighestEducation IS NULL THEN '❌' ELSE '✅' END AS 最高学历
FROM wt.Witch
ORDER BY PrisonerNo;
GO


-- ========================================
-- 教育经历和工作经历的 JSON 格式说明
-- ========================================

/*
教育经历格式（EducationHistory）：
[
  {
    "school": "学校名称",
    "degree": "学历类型（小学校/中学校/高等学校/大学/大学院等）",
    "status": "状态（在读/毕业/肄业/未入学等）",
    "specialNote": "特殊说明"
  }
]

工作经历格式（WorkHistory）：
[
  {
    "period": "起止时间（如：2020/04-2022/03）",
    "company": "公司名称",
    "position": "职位和职责",
    "salary": "薪资水平",
    "resignReason": "离职原因"
  }
]

如果没有教育或工作经历，填写：'[]'
如果有多条记录，在数组中添加多个对象
*/


-- ========================================
-- 导出为 Excel 的查询（方便在 Excel 中编辑后导入）
-- ========================================

SELECT 
    PrisonerNo AS 囚人番号,
    PersonalNo AS 个人番号,
    Name AS 姓名,
    FormerName AS 曾用名,
    Gender AS 性别,
    CONVERT(VARCHAR(10), BirthDate, 120) AS 出生日期,
    Ethnicity AS 民族,
    Birthplace AS 籍贯,
    Height AS 身高,
    Weight AS 体重,
    BloodType AS 血型,
    Address AS 地址,
    Phone AS 电话,
    Email AS 邮箱,
    LineAccount AS LINE账号,
    HighestEducation AS 最高学历,
    FamilyStructure AS 家庭结构,
    Father AS 父亲,
    Mother AS 母亲,
    Skills AS 技能特长,
    Hobbies AS 兴趣爱好,
    Dreams AS 理想,
    Dislikes AS 讨厌的事物,
    Trauma AS 心理创伤,
    Magic AS 魔法,
    Status AS 状态,
    WitchTransformMethod AS 魔女化办法,
    Remarks AS 备注
FROM wt.Witch
ORDER BY PrisonerNo;
GO

PRINT '========================================';
PRINT '📝 使用说明：';
PRINT '1. 方法1：适合单个魔女更新';
PRINT '2. 方法2：适合批量导入多个魔女';
PRINT '3. 方法3：快速填充关键字段';
PRINT '4. 教育和工作经历使用 JSON 格式存储';
PRINT '5. 可以先导出到 Excel 编辑，再批量导入';
PRINT '========================================';
GO
