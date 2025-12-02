非常好！你的需求已经很清晰了。让我重新梳理一下完整的思路，特别是关于状态管理和分配流程的部分。

## 📋 需求确认总结

### ✅ 已确认的设计决策

1. **界面风格**：独立窗口（Form）+ ScrollPanel 滚动
2. **字段显示**：英文字段名 → 中文标签（如 `PrisonerNo` → "囚犯编号"）
3. **囚犯编号**：必须唯一，添加时验证
4. **批次限制**：不检查数量限制
5. **账号创建**：不立即创建，由管理层后续创建，密码默认 123456
6. **操作入口**：工具栏添加"新增魔女"按钮
7. **权限保持**：典狱长只读，双击查看 A4 档案

------

## 🎯 核心创新：魔女分配流程

### 新的状态系统（6种状态）

```
1. 待分配 (Unassigned)        - 新增魔女的初始状态
2. 分配至岛屿 (Assigned)       - 已分配到岛屿，但未分配批次
3. 审判中 (OnTrial)           - 正在接受审判
4. 死亡(正常) (DeadNormal)     - 正常死亡
5. 死亡(魔女化) (DeadWitch)    - 魔女化后死亡
6. 其它 (Other)               - 其他状态
```

### 分配流程设计

```
┌─────────────────────────────────────────────────────────┐
│                    魔女生命周期                          │
└─────────────────────────────────────────────────────────┘

第1阶段：国家层创建
┌──────────────────────────────────────┐
│  admin 在国家端添加新魔女              │
│  ├─ 填写完整个人信息                  │
│  ├─ 状态：待分配                      │
│  ├─ 岛屿：空（或选择岛屿）            │
│  ├─ 批次：空                          │
│  └─ 账号：未创建                      │
└──────────────────────────────────────┘
           ↓
第2阶段：国家层分配岛屿（可选）
┌──────────────────────────────────────┐
│  admin 分配魔女到岛屿                 │
│  ├─ 选择岛屿（岛屿1 或 岛屿2）        │
│  ├─ 状态：分配至岛屿                  │
│  ├─ 批次：仍为空                      │
│  └─ 账号：仍未创建                    │
└──────────────────────────────────────┘
           ↓
第3阶段：管理者层分配批次 + 创建账号
┌──────────────────────────────────────┐
│  meruru_regulator / utena_regulator  │
│  ├─ 看到本岛屿"待分配"的魔女          │
│  ├─ 分配到具体批次（批次1/2/4/5）     │
│  ├─ 创建用户账号（囚犯编号为用户名）  │
│  ├─ 状态：审判中（或其他）            │
│  └─ 魔女可以登录系统                  │
└──────────────────────────────────────┘
           ↓
第4阶段：后续状态变更
┌──────────────────────────────────────┐
│  管理者/典狱长 更改状态               │
│  ├─ 审判中 → 死亡(正常)               │
│  ├─ 审判中 → 死亡(魔女化)             │
│  └─ 审判中 → 其它                     │
└──────────────────────────────────────┘
```

------

## 🗂️ 新增字段设计

### 时间戳字段（追踪魔女流转）

| 英文字段名 | 中文名称 | 数据类型 | 说明 | |-----------|---------|---------|------| | `CaptureTime` | 被抓捕时间 | DateTime? | 魔女被抓捕的时间 | | `DepartureTime` | 离开囚牢时间 | DateTime? | 离开原囚牢前往魔女岛的时间 | | `ArrivalTime` | 抵达魔女岛时间 | DateTime? | 抵达魔女岛的时间 | | `DeathTime` | 死亡时间 | DateTime? | 死亡时间（如果死亡） |

### 状态字段更新

| 英文字段名 | 中文名称 | 数据类型 | 说明 | |-----------|---------|---------|------| | `Status` | 状态 | NVARCHAR(50) | 6种状态之一 | | `IslandID` | 岛屿 | INT NULL | 可为空，表示未分配 | | `BatchID` | 批次 | INT NULL | 可为空，表示未分配批次 |

------

## 🎨 界面设计方案（详细版）

### 1. **Form1 改造方案**

#### 方案1：三个独立的 Form（推荐）

```
Form1_Admin.cs          - 国家层管理面板
├─ 显示所有魔女（包括"待分配"状态）
├─ 工具栏按钮：[新增魔女] [分配岛屿] [编辑信息] [更改状态]
├─ 列显示：ID | 囚犯编号 | 姓名 | 状态 | 岛屿 | 批次 | 被抓捕时间 | ...
└─ 可以筛选"待分配"状态的魔女

Form1_Regulator.cs      - 管理者层管理面板
├─ 显示本岛屿所有魔女（包括"待分配"和"分配至岛屿"）
├─ 工具栏按钮：[分配批次] [创建账号] [编辑公开描述] [更改状态]
├─ 列显示：ID | 囚犯编号 | 姓名 | 状态 | 批次 | 被抓捕时间 | ...
└─ 可以筛选"待分配批次"的魔女

Form1_Warden.cs         - 典狱长层管理面板
├─ 显示本岛屿所有魔女（只读）
├─ 工具栏按钮：[刷新] [查看详情]
├─ 列显示：ID | 囚犯编号 | 姓名 | 状态 | 批次 | ...
└─ 只能查看，不能修改
```

**优点：**

- 权限边界清晰，不会混乱
- 每个角色的界面简洁，只显示需要的功能
- 易于维护和扩展

**缺点：**

- 代码有一定重复（可以通过基类解决）

#### 方案2：一个 Form + 动态控制（备选）

```
Form1.cs
├─ 根据 _roleName 动态显示/隐藏按钮
├─ 根据 _roleName 动态启用/禁用功能
└─ 根据 _roleName 动态调整列显示
```

**优点：**

- 代码集中，易于统一修改

**缺点：**

- 逻辑复杂，容易出错
- 权限判断分散在各处

**我推荐方案1**，因为你的权限层级很清晰，独立的 Form 更易于管理。

------

### 2. **新增魔女窗口（WitchAddForm.cs）**

```
┌────────────────────────────────────────────────────────┐
│  新增魔女 - 完整信息录入                          [×]    │
├────────────────────────────────────────────────────────┤
│  ┌──────────────────────────────────────────────────┐ │
│  │  【基本信息】                                     │ │
│  │  ┌────────────────────────────────────────────┐ │ │
│  │  │ * 囚犯编号：[__________]  (必填，自动检查重复) │ │ │
│  │  │   个人番号：[__________]                    │ │ │
│  │  │ * 姓名：    [__________]  (必填)            │ │ │
│  │  │   曾用名：  [__________]                    │ │ │
│  │  │   性别：    [男▼] [女] [其他]              │ │ │
│  │  │   出生日期：[📅 选择日期]                   │ │ │
│  │  │   民族：    [__________]                    │ │ │
│  │  │   籍贯：    [__________]                    │ │ │
│  │  └────────────────────────────────────────────┘ │ │
│  │                                                   │ │
│  │  【分配信息】                                     │ │
│  │  ┌────────────────────────────────────────────┐ │ │
│  │  │   状态：    [待分配▼] (默认)                │ │ │
│  │  │   岛屿：    [未分配▼] [魔女岛·壹] [魔女岛·贰] │ │ │
│  │  │   批次：    [未分配▼] (岛屿选择后可用)      │ │ │
│  │  └────────────────────────────────────────────┘ │ │
│  │                                                   │ │
│  │  【时间记录】                                     │ │
│  │  ┌────────────────────────────────────────────┐ │ │
│  │  │   被抓捕时间：      [📅 选择日期时间]       │ │ │
│  │  │   离开囚牢时间：    [📅 选择日期时间]       │ │ │
│  │  │   抵达魔女岛时间：  [📅 选择日期时间]       │ │ │
│  │  │   死亡时间：        [📅 选择日期时间]       │ │ │
│  │  └────────────────────────────────────────────┘ │ │
│  │                                                   │ │
│  │  【身体特征】                                     │ │
│  │  ┌────────────────────────────────────────────┐ │ │
│  │  │   身高：[_____] cm   体重：[_____] kg      │ │ │
│  │  │   血型：[A▼] [B] [AB] [O] [其他]          │ │ │
│  │  └────────────────────────────────────────────┘ │ │
│  │                                                   │ │
│  │  【联系方式】                                     │ │
│  │  ┌────────────────────────────────────────────┐ │ │
│  │  │   地址：[_________________________________] │ │ │
│  │  │   电话：[__________]                        │ │ │
│  │  │   邮箱：[__________]                        │ │ │
│  │  │   LINE账号：[__________]                    │ │ │
│  │  └────────────────────────────────────────────┘ │ │
│  │                                                   │ │
│  │  【教育背景】                                     │ │
│  │  ┌────────────────────────────────────────────┐ │ │
│  │  │   最高学历：[__________]                    │ │ │
│  │  │   教育经历：                                │ │ │
│  │  │   ┌──────────────────────────────────────┐ │ │ │
│  │  │   │ [多行文本框，5行高度]                 │ │ │ │
│  │  │   │                                       │ │ │ │
│  │  │   └──────────────────────────────────────┘ │ │ │
│  │  │   工作经历：                                │ │ │
│  │  │   ┌──────────────────────────────────────┐ │ │ │
│  │  │   │ [多行文本框，5行高度]                 │ │ │ │
│  │  │   └──────────────────────────────────────┘ │ │ │
│  │  └────────────────────────────────────────────┘ │ │
│  │                                                   │ │
│  │  【家庭关系】                                     │ │
│  │  ┌────────────────────────────────────────────┐ │ │
│  │  │   家庭结构：[__________]                    │ │ │
│  │  │   父亲：[__________]                        │ │ │
│  │  │   母亲：[__________]                        │ │ │
│  │  │   其他成员1：[__________]                   │ │ │
│  │  │   其他成员2：[__________]                   │ │ │
│  │  │   其他成员3：[__________]                   │ │ │
│  │  └────────────────────────────────────────────┘ │ │
│  │                                                   │ │
│  │  【个性特征】                                     │ │
│  │  ┌────────────────────────────────────────────┐ │ │
│  │  │   技能/特长：                               │ │ │
│  │  │   ┌──────────────────────────────────────┐ │ │ │
│  │  │   │ [多行文本框，3行]                     │ │ │ │
│  │  │   └──────────────────────────────────────┘ │ │ │
│  │  │   兴趣爱好：                                │ │ │
│  │  │   ┌──────────────────────────────────────┐ │ │ │
│  │  │   │ [多行文本框，3行]                     │ │ │ │
│  │  │   └──────────────────────────────────────┘ │ │ │
│  │  │   理想：                                    │ │ │
│  │  │   ┌──────────────────────────────────────┐ │ │ │
│  │  │   │ [多行文本框，3行]                     │ │ │ │
│  │  │   └──────────────────────────────────────┘ │ │ │
│  │  │   讨厌的事物：                              │ │ │
│  │  │   ┌──────────────────────────────────────┐ │ │ │
│  │  │   │ [多行文本框，3行]                     │ │ │ │
│  │  │   └──────────────────────────────────────┘ │ │ │
│  │  │   心理创伤：                                │ │ │
│  │  │   ┌──────────────────────────────────────┐ │ │ │
│  │  │   │ [多行文本框，4行]                     │ │ │ │
│  │  │   └──────────────────────────────────────┘ │ │ │
│  │  └────────────────────────────────────────────┘ │ │
│  │                                                   │ │
│  │  【魔法信息】                                     │ │
│  │  ┌────────────────────────────────────────────┐ │ │
│  │  │   魔法：                                    │ │ │
│  │  │   ┌──────────────────────────────────────┐ │ │ │
│  │  │   │ [多行文本框，4行]                     │ │ │ │
│  │  │   └──────────────────────────────────────┘ │ │ │
│  │  │   魔女化办法：                              │ │ │
│  │  │   ┌──────────────────────────────────────┐ │ │ │
│  │  │   │ [多行文本框，4行]                     │ │ │ │
│  │  │   └──────────────────────────────────────┘ │ │ │
│  │  │   公开描述：                                │ │ │
│  │  │   ┌──────────────────────────────────────┐ │ │ │
│  │  │   │ [多行文本框，5行]                     │ │ │ │
│  │  │   └──────────────────────────────────────┘ │ │ │
│  │  │   备注：                                    │ │ │
│  │  │   ┌──────────────────────────────────────┐ │ │ │
│  │  │   │ [多行文本框，3行]                     │ │ │ │
│  │  │   └──────────────────────────────────────┘ │ │ │
│  │  └────────────────────────────────────────────┘ │ │
│  │                                                   │ │
│  │  【头像设置】                                     │ │
│  │  ┌────────────────────────────────────────────┐ │ │
│  │  │   头像路径：[__________] [浏览...]         │ │ │
│  │  │   (自动生成：Images/囚犯编号.png)          │ │ │
│  │  └────────────────────────────────────────────┘ │ │
│  └──────────────────────────────────────────────────┘ │
│                                                        │
│  [✓ 保存并创建]  [取消]                                │
└────────────────────────────────────────────────────────┘
```

**关键特性：**

1. **必填字段标记**：用 `*` 标记必填项
2. **实时验证**：囚犯编号输入时检查重复
3. **级联选择**：选择岛屿后，批次下拉框才可用
4. **默认值**：状态默认"待分配"，岛屿默认"未分配"
5. **头像路径**：根据囚犯编号自动生成

------

### 3. **编辑公开描述窗口（WitchEditDescriptionForm.cs）**

```
┌────────────────────────────────────────────┐
│  编辑公开描述 - 樱羽艾玛 (658)        [×]  │
├────────────────────────────────────────────┤
│                                            │
│  【基本信息】（只读，灰色背景）             │
│  ┌────────────────────────────────────┐   │
│  │  囚犯编号：658                      │   │
│  │  姓名：樱羽艾玛                     │   │
│  │  岛屿：魔女岛·壹                    │   │
│  │  批次：1                            │   │
│  │  状态：审判中                       │   │
│  └────────────────────────────────────┘   │
│                                            │
│  【公开描述】（可编辑）                     │
│  ┌────────────────────────────────────┐   │
│  │                                    │   │
│  │  [多行文本框，高度约15行]           │   │
│  │                                    │   │
│  │  拥有的魔法是可以瞬间治疗伤痛的魔法。│   │
│  │                                    │   │
│  │                                    │   │
│  └────────────────────────────────────┘   │
│                                            │
│  字数统计：45 / 无限制                      │
│                                            │
│  [✓ 保存修改]  [取消]                      │
└────────────────────────────────────────────┘
```

------

### 4. **分配批次窗口（BatchAssignForm.cs）**

管理者用于给"待分配批次"的魔女分配批次：

```
┌────────────────────────────────────────────┐
│  分配批次 - 樱羽艾玛 (658)            [×]  │
├────────────────────────────────────────────┤
│                                            │
│  【当前信息】                               │
│  ┌────────────────────────────────────┐   │
│  │  囚犯编号：658                      │   │
│  │  姓名：樱羽艾玛                     │   │
│  │  当前状态：分配至岛屿               │   │
│  │  当前岛屿：魔女岛·壹                │   │
│  │  当前批次：未分配                   │   │
│  └────────────────────────────────────┘   │
│                                            │
│  【分配操作】                               │
│  ┌────────────────────────────────────┐   │
│  │  分配到批次：[批次1▼] [批次2]      │   │
│  │                                    │   │
│  │  新状态：[审判中▼] (可选修改)      │   │
│  └────────────────────────────────────┘   │
│                                            │
│  [✓ 确认分配]  [取消]                      │
└────────────────────────────────────────────┘
```

------

### 5. **创建账号窗口（UserCreateForm.cs）**

管理者用于给已分配批次的魔女创建账号：

```
┌────────────────────────────────────────────┐
│  创建用户账号 - 樱羽艾玛 (658)        [×]  │
├────────────────────────────────────────────┤
│                                            │
│  【魔女信息】                               │
│  ┌────────────────────────────────────┐   │
│  │  囚犯编号：658                      │   │
│  │  姓名：樱羽艾玛                     │   │
│  │  岛屿：魔女岛·壹                    │   │
│  │  批次：1                            │   │
│  └────────────────────────────────────┘   │
│                                            │
│  【账号信息】                               │
│  ┌────────────────────────────────────┐   │
│  │  用户名：658 (自动使用囚犯编号)     │   │
│  │  密码：123456 (默认密码)           │   │
│  │  角色：Witch (魔女)                 │   │
│  └────────────────────────────────────┘   │
│                                            │
│  ⚠️ 注意：账号创建后，魔女可以登录系统      │
│                                            │
│  [✓ 创建账号]  [取消]                      │
└────────────────────────────────────────────┘
```

------

## 🗄️ 数据库改造方案

### 1. **修改 Witch 表**

```sql
-- 添加新字段
ALTER TABLE wt.Witch ADD CaptureTime DATETIME2 NULL;
ALTER TABLE wt.Witch ADD DepartureTime DATETIME2 NULL;
ALTER TABLE wt.Witch ADD ArrivalTime DATETIME2 NULL;
ALTER TABLE wt.Witch ADD DeathTime DATETIME2 NULL;

-- 修改现有字段，允许为空
ALTER TABLE wt.Witch ALTER COLUMN IslandID INT NULL;
ALTER TABLE wt.Witch ALTER COLUMN BatchID INT NULL;

-- 更新状态字段长度（如果需要）
ALTER TABLE wt.Witch ALTER COLUMN [Status] NVARCHAR(50) NOT NULL;
```

### 2. **更新存储过程**

```sql
-- 修改 sp_AddWitch，支持更多字段
CREATE OR ALTER PROCEDURE wt.sp_AddWitchComplete
    @PrisonerNo NVARCHAR(20),
    @PersonalNo NVARCHAR(50) = NULL,
    @Name NVARCHAR(50),
    @FormerName NVARCHAR(50) = NULL,
    @Gender NVARCHAR(10) = NULL,
    @BirthDate DATE = NULL,
    @Ethnicity NVARCHAR(50) = NULL,
    @Birthplace NVARCHAR(100) = NULL,
    @Height DECIMAL(5,2) = NULL,
    @Weight DECIMAL(5,2) = NULL,
    @BloodType NVARCHAR(10) = NULL,
    @Address NVARCHAR(MAX) = NULL,
    @Phone NVARCHAR(50) = NULL,
    @Email NVARCHAR(100) = NULL,
    @LineAccount NVARCHAR(100) = NULL,
    @HighestEducation NVARCHAR(100) = NULL,
    @EducationHistory NVARCHAR(MAX) = NULL,
    @WorkHistory NVARCHAR(MAX) = NULL,
    @FamilyStructure NVARCHAR(MAX) = NULL,
    @Father NVARCHAR(MAX) = NULL,
    @Mother NVARCHAR(MAX) = NULL,
    @OtherFamily1 NVARCHAR(MAX) = NULL,
    @OtherFamily2 NVARCHAR(MAX) = NULL,
    @OtherFamily3 NVARCHAR(MAX) = NULL,
    @Skills NVARCHAR(MAX) = NULL,
    @Hobbies NVARCHAR(MAX) = NULL,
    @Dreams NVARCHAR(MAX) = NULL,
    @Dislikes NVARCHAR(MAX) = NULL,
    @Trauma NVARCHAR(MAX) = NULL,
    @Magic NVARCHAR(MAX) = NULL,
    @WitchTransformMethod NVARCHAR(MAX) = NULL,
    @DescriptionPublic NVARCHAR(MAX) = NULL,
    @Remarks NVARCHAR(MAX) = NULL,
    @Status NVARCHAR(50) = N'待分配',
    @IslandID INT = NULL,
    @BatchID INT = NULL,
    @AvatarPath NVARCHAR(255) = NULL,
    @CaptureTime DATETIME2 = NULL,
    @DepartureTime DATETIME2 = NULL,
    @ArrivalTime DATETIME2 = NULL,
    @DeathTime DATETIME2 = NULL
AS
BEGIN
    -- 检查囚犯编号是否重复
    IF EXISTS (SELECT 1 FROM wt.Witch WHERE PrisonerNo = @PrisonerNo)
    BEGIN
        RAISERROR('囚犯编号已存在', 16, 1);
        RETURN;
    END

    -- 插入数据
    INSERT INTO wt.Witch (
        PrisonerNo, PersonalNo, Name, FormerName, Gender, BirthDate,
        Ethnicity, Birthplace, Height, Weight, BloodType,
        Address, Phone, Email, LineAccount,
        HighestEducation, EducationHistory, WorkHistory,
        FamilyStructure, Father, Mother, OtherFamily1, OtherFamily2, OtherFamily3,
        Skills, Hobbies, Dreams, Dislikes, Trauma,
        Magic, WitchTransformMethod, DescriptionPublic, Remarks,
        [Status], IslandID, BatchID, AvatarPath,
        CaptureTime, DepartureTime, ArrivalTime, DeathTime
    )
    VALUES (
        @PrisonerNo, @PersonalNo, @Name, @FormerName, @Gender, @BirthDate,
        @Ethnicity, @Birthplace, @Height, @Weight, @BloodType,
        @Address, @Phone, @Email, @LineAccount,
        @HighestEducation, @EducationHistory, @WorkHistory,
        @FamilyStructure, @Father, @Mother, @OtherFamily1, @OtherFamily2, @OtherFamily3,
        @Skills, @Hobbies, @Dreams, @Dislikes, @Trauma,
        @Magic, @WitchTransformMethod, @DescriptionPublic, @Remarks,
        @Status, @IslandID, @BatchID, @AvatarPath,
        @CaptureTime, @DepartureTime, @ArrivalTime, @DeathTime
    );

    -- 返回新插入的 WitchID
    SELECT SCOPE_IDENTITY() AS NewWitchID;
END
GO
```

------

## 🔧 代码架构方案

### 目录结构

```
WitchTrialSystem/
├── UI/
│   ├── Admin/
│   │   ├── Form1_Admin.cs              # 国家层管理面板
│   │   ├── WitchAddForm.cs             # 新增魔女（完整）
│   │   ├── WitchEditForm.cs            
```

Credits used: 1.46

Elapsed time: 1m 36s

Checkpoint

Restore

你分析好了吗？怎么卡住了



Kiro

抱歉！让我继续完成分析。

## 🔧 代码架构方案（续）

### 目录结构

```
WitchTrialSystem/
├── UI/
│   ├── Admin/
│   │   ├── Form1_Admin.cs              # 国家层管理面板
│   │   ├── WitchAddForm.cs             # 新增魔女（完整）
│   │   └── WitchEditForm.cs            # 编辑魔女（完整）
│   ├── Regulator/
│   │   ├── Form1_Regulator.cs          # 管理者层管理面板
│   │   ├── WitchEditDescriptionForm.cs # 编辑公开描述
│   │   ├── BatchAssignForm.cs          # 分配批次
│   │   └── UserCreateForm.cs           # 创建账号
│   ├── Warden/
│   │   └── Form1_Warden.cs             # 典狱长层管理面板（只读）
│   └── LoginForm.cs                     # 登录（根据角色跳转不同Form）
├── DAL/
│   └── WitchDAL.cs                      # 扩展方法
├── BLL/
│   └── WitchBLL.cs                      # 新增业务逻辑
└── Models/
    └── WitchModel.cs                    # 数据模型
```

------

## 📝 实施步骤（详细版）

### 阶段1：数据库改造（优先级：最高）

**任务清单：**

- [ ] 1.1 添加时间戳字段（CaptureTime, DepartureTime, ArrivalTime, DeathTime）
- [ ] 1.2 修改 IslandID 和 BatchID 为可空
- [ ] 1.3 创建存储过程 `sp_AddWitchComplete`
- [ ] 1.4 创建存储过程 `sp_UpdateWitchComplete`
- [ ] 1.5 创建存储过程 `sp_UpdateDescription`
- [ ] 1.6 创建存储过程 `sp_AssignBatch`
- [ ] 1.7 测试所有存储过程

**预计时间：** 1-2小时

------

### 阶段2：数据访问层扩展（优先级：高）

**任务清单：**

- [ ] 2.1 创建 `Models/WitchModel.cs`（包含所有字段）
- [ ] 2.2 在 `WitchDAL.cs` 添加 `AddWitchComplete(WitchModel witch)`
- [ ] 2.3 在 `WitchDAL.cs` 添加 `UpdateWitchComplete(WitchModel witch)`
- [ ] 2.4 在 `WitchDAL.cs` 添加 `UpdateDescription(int witchId, string description)`
- [ ] 2.5 在 `WitchDAL.cs` 添加 `AssignBatch(int witchId, int batchId, string status)`
- [ ] 2.6 在 `WitchDAL.cs` 添加 `CheckPrisonerNoExists(string prisonerNo)`
- [ ] 2.7 在 `UserDAL.cs` 添加 `CreateWitchUser(string username, int islandId, int batchId)`

**预计时间：** 2-3小时

------

### 阶段3：国家层界面（优先级：高）

**任务清单：**

- [ ] 3.1 备份 `Form1.cs` 为 `Form1_Backup.cs`
- [ ] 3.2 创建 `UI/Admin/Form1_Admin.cs`（复制 Form1，调整列显示）
- [ ] 3.3 在 Form1_Admin 工具栏添加"新增魔女"按钮
- [ ] 3.4 创建 `UI/Admin/WitchAddForm.cs`（完整表单，ScrollPanel）
- [ ] 3.5 实现字段验证（囚犯编号唯一性、必填项）
- [ ] 3.6 实现级联选择（岛屿→批次）
- [ ] 3.7 实现保存逻辑，调用 `WitchDAL.AddWitchComplete`
- [ ] 3.8 测试新增功能

**预计时间：** 4-6小时

------

### 阶段4：管理者层界面（优先级：中）

**任务清单：**

- [ ] 4.1 创建 `UI/Regulator/Form1_Regulator.cs`
- [ ] 4.2 调整列显示（显示"待分配"状态）
- [ ] 4.3 创建 `UI/Regulator/WitchEditDescriptionForm.cs`
- [ ] 4.4 实现编辑公开描述功能
- [ ] 4.5 创建 `UI/Regulator/BatchAssignForm.cs`
- [ ] 4.6 实现分配批次功能
- [ ] 4.7 创建 `UI/Regulator/UserCreateForm.cs`
- [ ] 4.8 实现创建账号功能（默认密码123456）
- [ ] 4.9 测试管理者权限

**预计时间：** 4-5小时

------

### 阶段5：典狱长层界面（优先级：低）

**任务清单：**

- [ ] 5.1 创建 `UI/Warden/Form1_Warden.cs`（只读版本）
- [ ] 5.2 移除所有编辑按钮
- [ ] 5.3 保留"查看详情"功能（双击打开 A4 档案）
- [ ] 5.4 测试典狱长权限

**预计时间：** 1-2小时

------

### 阶段6：登录跳转逻辑（优先级：高）

**任务清单：**

- [ ] 6.1 修改

   

  ```
  LoginForm.cs
  ```

  ，根据角色跳转不同 Form

  ```csharp
  if (roleName == "Admin")
      new Form1_Admin(username).Show();
  else if (roleName == "Meruru")
      new Form1_Regulator(username).Show();
  else if (roleName == "Warden")
      new Form1_Warden(username).Show();
  else
      new PhoneForm(username).Show(); // 魔女
  ```

- [ ] 6.2 测试所有角色登录

**预计时间：** 0.5小时

------

### 阶段7：测试与优化（优先级：高）

**任务清单：**

- [ ] 7.1 测试 admin 添加魔女（待分配状态）
- [ ] 7.2 测试 admin 分配岛屿
- [ ] 7.3 测试管理者分配批次
- [ ] 7.4 测试管理者创建账号
- [ ] 7.5 测试管理者编辑公开描述
- [ ] 7.6 测试典狱长只读权限
- [ ] 7.7 测试魔女登录（新创建的账号）
- [ ] 7.8 优化界面布局和用户体验

**预计时间：** 2-3小时

------

## 🎯 关键技术点

### 1. **囚犯编号唯一性验证**

```csharp
// 在 WitchAddForm 的 TextBox_Leave 事件中
private void txtPrisonerNo_Leave(object sender, EventArgs e)
{
    string prisonerNo = txtPrisonerNo.Text.Trim();
    if (string.IsNullOrEmpty(prisonerNo)) return;
    
    if (_witchDAL.CheckPrisonerNoExists(prisonerNo))
    {
        MessageBox.Show("囚犯编号已存在，请使用其他编号。", "验证失败");
        txtPrisonerNo.BackColor = Color.LightPink;
        txtPrisonerNo.Focus();
    }
    else
    {
        txtPrisonerNo.BackColor = Color.White;
    }
}
```

### 2. **级联选择（岛屿→批次）**

```csharp
private void cbIsland_SelectedIndexChanged(object sender, EventArgs e)
{
    if (cbIsland.SelectedValue == null || (int)cbIsland.SelectedValue == 0)
    {
        cbBatch.Enabled = false;
        cbBatch.DataSource = null;
        return;
    }
    
    int islandId = (int)cbIsland.SelectedValue;
    var batches = _witchDAL.GetBatches(islandId);
    
    // 添加"未分配"选项
    var dt = new DataTable();
    dt.Columns.Add("BatchID", typeof(int));
    dt.Columns.Add("DisplayText", typeof(string));
    dt.Rows.Add(0, "未分配");
    
    foreach (DataRow row in batches.Rows)
    {
        dt.Rows.Add(row["BatchID"], row["BatchID"].ToString());
    }
    
    cbBatch.DisplayMember = "DisplayText";
    cbBatch.ValueMember = "BatchID";
    cbBatch.DataSource = dt;
    cbBatch.Enabled = true;
}
```

### 3. **ScrollPanel 实现**

```csharp
// 在 WitchAddForm 构造函数中
var scrollPanel = new Panel
{
    Dock = DockStyle.Fill,
    AutoScroll = true,
    Padding = new Padding(20)
};

var contentPanel = new Panel
{
    Location = new Point(0, 0),
    Width = 700,
    AutoSize = true,
    AutoSizeMode = AutoSizeMode.GrowAndShrink
};

// 添加所有字段控件到 contentPanel
// ...

scrollPanel.Controls.Add(contentPanel);
this.Controls.Add(scrollPanel);
```

### 4. **字段映射（英文→中文）**

```csharp
private Dictionary<string, string> _fieldLabels = new()
{
    { "PrisonerNo", "囚犯编号" },
    { "PersonalNo", "个人番号" },
    { "Name", "姓名" },
    { "FormerName", "曾用名" },
    { "Gender", "性别" },
    { "BirthDate", "出生日期" },
    { "Ethnicity", "民族" },
    { "Birthplace", "籍贯" },
    { "Height", "身高" },
    { "Weight", "体重" },
    { "BloodType", "血型" },
    // ... 其他字段
};
```

------

## ⚠️ 注意事项

### 1. **状态流转规则**

```
待分配 → 分配至岛屿 → 审判中 → 死亡(正常)/死亡(魔女化)/其它
   ↓
   └→ 可以直接跳到任何状态（admin权限）
```

### 2. **权限边界**

| 操作 | admin | meruru/utena | warden | witch | |------|-------|--------------|--------|-------| | 添加魔女 | ✅ | ❌ | ❌ | ❌ | | 编辑所有字段 | ✅ | ❌ | ❌ | ❌ | | 编辑公开描述 | ✅ | ✅（本岛屿） | ❌ | ❌ | | 分配岛屿 | ✅ | ❌ | ❌ | ❌ | | 分配批次 | ✅ | ✅（本岛屿） | ❌ | ❌ | | 创建账号 | ✅ | ✅（本岛屿） | ❌ | ❌ | | 更改状态 | ✅ | ✅（本岛屿） | ❌ | ❌ | | 查看详情 | ✅ | ✅ | ✅ | ✅（本批次） |

### 3. **数据完整性**

- 囚犯编号必须唯一
- 姓名必填
- 状态默认"待分配"
- IslandID 和 BatchID 可为 NULL
- 时间字段可为 NULL
- 头像路径自动生成：`Images/{囚犯编号}.png`

------

## 📊 总结

### 核心改动点

1. **数据库**：添加4个时间字段，IslandID/BatchID 改为可空
2. **Form1 分离**：3个独立的 Form（Admin/Regulator/Warden）
3. **新增界面**：WitchAddForm（ScrollPanel + 完整字段）
4. **编辑界面**：WitchEditDescriptionForm（管理者专用）
5. **分配界面**：BatchAssignForm + UserCreateForm
6. **登录跳转**：根据角色跳转不同 Form

### 开发优先级

1. **最高优先级**：数据库改造 + WitchAddForm（国家层添加魔女）
2. **高优先级**：WitchEditDescriptionForm（管理者编辑描述）
3. **中优先级**：BatchAssignForm + UserCreateForm（管理者分配批次和创建账号）
4. **低优先级**：Form1_Warden（典狱长只读）

### 预计总工作量

- **数据库改造**：1-2小时
- **DAL/BLL 扩展**：2-3小时
- **国家层界面**：4-6小时
- **管理者层界面**：4-5小时
- **典狱长层界面**：1-2小时
- **测试与优化**：2-3小时

**总计：14-21小时**

------

## 🚀 下一步

你觉得这个方案如何？有什么需要调整的地方吗？

如果方案确定，我建议按以下顺序开始实施：

1. **先做数据库改造**（添加字段、创建存储过程）
2. **再做 WitchAddForm**（国家层添加魔女，最核心的功能）
3. **然后做 WitchEditDescriptionForm**（管理者编辑描述）
4. **最后完善其他功能**

你想从哪一步开始？还是需要我再细化某个部分的设计？