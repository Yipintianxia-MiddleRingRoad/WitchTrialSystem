# Design Document

## Overview

本设计文档描述了魔女审判系统"国家层添加魔女"功能的技术实现方案。该功能为 Admin 角色提供一个完整的魔女档案录入界面，支持录入所有 38 个字段的详细信息，包括基本信息、身体特征、联系方式、教育背景、工作经历、家庭关系、个性特征、魔女信息和分配信息。

设计采用"分层架构"策略：
1. **数据库层**：创建存储过程处理完整插入和验证
2. **DAL 层**：扩展 WitchDAL 支持完整魔女信息操作
3. **UI 层**：创建 WitchAddForm 提供友好的分组输入界面

## Architecture

### 系统架构层次

```
UI Layer (表示层)
├── Form1_Admin.cs                      # 管理员界面（添加入口）
├── WitchAddForm.cs                     # 完整魔女添加表单（新增）
├── EducationEditDialog.cs              # 教育经历编辑对话框（新增）
└── WorkEditDialog.cs                   # 工作经历编辑对话框（新增）

BLL Layer (业务逻辑层)
└── （暂无新增，直接调用 DAL）

DAL Layer (数据访问层)
├── WitchDAL.cs                         # 魔女数据访问（扩展）
│   └── AddWitchComplete()              # 新增方法
└── UserDAL.cs                          # 用户数据访问（可能需要）

Database Layer (数据库层)
└── wt.sp_AddWitchComplete              # 新增存储过程
```

### 数据流程

```mermaid
graph TD
    A[Admin 点击"国家层添加魔女"] --> B[打开 WitchAddForm]
    B --> C[填写 9 个标签页的信息]
    C --> D{点击保存}
    D --> E[验证必填字段]
    E -->|验证失败| F[显示错误消息]
    E -->|验证通过| G[检查批次人数]
    G -->|批次已满| F
    G -->|批次未满或待分配| H[调用 WitchDAL.AddWitchComplete]
    H --> I[调用 sp_AddWitchComplete 存储过程]
    I --> J[插入 Witch 表]
    J --> K{需要创建用户?}
    K -->|是| L[创建 User 和 UserWitch]
    K -->|否| M[返回 WitchID]
    L --> M
    M --> N[显示成功消息]
    N --> O[关闭表单并刷新列表]
```

## Components and Interfaces

### 1. WitchAddForm

**职责**: 完整的魔女档案录入界面

**构造函数**:
```csharp
public WitchAddForm()
```

**UI 组件**:
- `TabControl _tabControl`: 主标签控件，包含 9 个标签页
- `Button _btnSave`: 保存按钮
- `Button _btnCancel`: 取消按钮

**标签页 1: 基本信息**
```csharp
private TextBox _txtName;              // 姓名*
private TextBox _txtPrisonerNo;        // 囚犯编号
private TextBox _txtPersonalNo;        // 个人番号
private ComboBox _cbGender;            // 性别
private DateTimePicker _dtpBirthDate;  // 出生日期
private TextBox _txtEthnicity;         // 民族
private TextBox _txtBirthplace;        // 籍贯
private TextBox _txtFormerName;        // 曾用名
```

**标签页 2: 身体特征**
```csharp
private NumericUpDown _nudHeight;      // 身高
private NumericUpDown _nudWeight;      // 体重
private ComboBox _cbBloodType;         // 血型
```

**标签页 3: 联系方式**
```csharp
private TextBox _txtAddress;           // 地址
private TextBox _txtPhone;             // 电话
private TextBox _txtEmail;             // 邮箱
private TextBox _txtLineAccount;       // LINE账号
```

**标签页 4: 教育背景**
```csharp
private TextBox _txtHighestEducation;  // 最高学历
private DataGridView _dgvEducation;    // 教育经历列表
private Button _btnAddEducation;       // 添加教育经历
private Button _btnEditEducation;      // 编辑教育经历
private Button _btnDeleteEducation;    // 删除教育经历
private List<EducationRecord> _educationList;  // 教育经历数据
```

**标签页 5: 工作经历**
```csharp
private DataGridView _dgvWork;         // 工作经历列表
private Button _btnAddWork;            // 添加工作经历
private Button _btnEditWork;           // 编辑工作经历
private Button _btnDeleteWork;         // 删除工作经历
private List<WorkRecord> _workList;    // 工作经历数据
```

**标签页 6: 家庭关系**
```csharp
private TextBox _txtFamilyStructure;   // 家庭结构
private TextBox _txtFather;            // 父亲
private TextBox _txtMother;            // 母亲
private TextBox _txtOtherFamily1;      // 其他家庭成员1
private TextBox _txtOtherFamily2;      // 其他家庭成员2
private TextBox _txtOtherFamily3;      // 其他家庭成员3
```

**标签页 7: 个性特征**
```csharp
private TextBox _txtSkills;            // 技能特长
private TextBox _txtHobbies;           // 兴趣爱好
private TextBox _txtDreams;            // 理想
private TextBox _txtDislikes;          // 讨厌的事物
private TextBox _txtTrauma;            // 心理创伤
```

**标签页 8: 魔女信息**
```csharp
private TextBox _txtMagic;             // 魔法*
private ComboBox _cbStatus;            // 状态*
private TextBox _txtWitchTransformMethod;  // 魔女化办法
private TextBox _txtRemarks;           // 备注
private TextBox _txtDescriptionPublic; // 公开描述
```

**标签页 9: 分配信息**
```csharp
private ComboBox _cbIsland;            // 岛屿
private ComboBox _cbBatch;             // 批次
private TextBox _txtAvatarPath;        // 头像路径
private Button _btnBrowseAvatar;       // 浏览头像
private Label _lblBatchInfo;           // 批次信息（显示当前人数）
```

**关键方法**:
```csharp
private void InitializeComponent()                    // 初始化UI组件
private void LoadIslands()                            // 加载岛屿列表
private void LoadBatches(int islandId)                // 加载批次列表
private void CheckBatchCapacity(int batchId)          // 检查批次容量
private bool ValidateInput()                          // 验证输入
private void BtnSave_Click(object sender, EventArgs e)  // 保存按钮点击
private string SerializeEducationHistory()            // 序列化教育经历为JSON
private string SerializeWorkHistory()                 // 序列化工作经历为JSON
```

### 2. EducationRecord (数据模型)

```csharp
public class EducationRecord
{
    public string School { get; set; }          // 学校名称
    public string Degree { get; set; }          // 学历
    public string Status { get; set; }          // 状态（毕业/在读/未入学）
    public string SpecialNote { get; set; }     // 特殊说明
}
```

### 3. WorkRecord (数据模型)

```csharp
public class WorkRecord
{
    public string Period { get; set; }          // 时间段
    public string Company { get; set; }         // 公司名称
    public string Position { get; set; }        // 职位
    public string Salary { get; set; }          // 薪资
    public string ResignReason { get; set; }    // 离职原因
}
```

### 4. EducationEditDialog

**职责**: 编辑单条教育经历

**构造函数**:
```csharp
public EducationEditDialog(EducationRecord record = null)
```

**UI 组件**:
```csharp
private TextBox _txtSchool;            // 学校
private TextBox _txtDegree;            // 学历
private ComboBox _cbStatus;            // 状态
private TextBox _txtSpecialNote;       // 特殊说明
private Button _btnOK;                 // 确定
private Button _btnCancel;             // 取消
```

### 5. WorkEditDialog

**职责**: 编辑单条工作经历

**构造函数**:
```csharp
public WorkEditDialog(WorkRecord record = null)
```

**UI 组件**:
```csharp
private TextBox _txtPeriod;            // 时间段
private TextBox _txtCompany;           // 公司
private TextBox _txtPosition;          // 职位
private TextBox _txtSalary;            // 薪资
private TextBox _txtResignReason;      // 离职原因
private Button _btnOK;                 // 确定
private Button _btnCancel;             // 取消
```

### 6. WitchDAL (扩展)

**新增方法**:
```csharp
public int AddWitchComplete(
    string name,
    string magic,
    string prisonerNo,
    string personalNo,
    string formerName,
    string gender,
    DateTime? birthDate,
    string ethnicity,
    string birthplace,
    decimal? height,
    decimal? weight,
    string bloodType,
    string address,
    string phone,
    string email,
    string lineAccount,
    string highestEducation,
    string educationHistory,
    string workHistory,
    string familyStructure,
    string father,
    string mother,
    string otherFamily1,
    string otherFamily2,
    string otherFamily3,
    string skills,
    string hobbies,
    string dreams,
    string dislikes,
    string trauma,
    string witchTransformMethod,
    string remarks,
    string status,
    string descriptionPublic,
    int? islandId,
    int? batchId,
    string avatarPath
)
```

**返回值**: 新创建的 WitchID

### 7. 存储过程 wt.sp_AddWitchComplete

**参数**: 所有 38 个字段

**逻辑**:
1. 开始事务
2. 检查批次人数限制（如果提供了 batchId）
3. 插入 Witch 表
4. 获取新的 WitchID
5. 如果需要，创建 User 账号
6. 如果需要，创建 UserWitch 关联
7. 更新批次 WitchCount
8. 提交事务
9. 返回 WitchID

**SQL 示例**:
```sql
CREATE PROCEDURE wt.sp_AddWitchComplete
    @Name NVARCHAR(50),
    @Magic NVARCHAR(100),
    @PrisonerNo NVARCHAR(20) = NULL,
    -- ... 其他 35 个参数
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- 1. 检查批次人数
        IF @BatchID IS NOT NULL
        BEGIN
            DECLARE @CurrentCount INT;
            SELECT @CurrentCount = WitchCount FROM wt.Batch WHERE BatchID = @BatchID;
            IF @CurrentCount >= 13
            BEGIN
                RAISERROR('批次已满，无法添加新魔女', 16, 1);
                RETURN;
            END
        END
        
        -- 2. 插入魔女记录
        INSERT INTO wt.Witch (Name, Magic, PrisonerNo, ...)
        VALUES (@Name, @Magic, @PrisonerNo, ...);
        
        DECLARE @NewWitchID INT = SCOPE_IDENTITY();
        
        -- 3. 创建用户账号（如果需要）
        IF @PrisonerNo IS NOT NULL AND @Status = N'分配至岛屿'
        BEGIN
            -- 创建 User 和 UserWitch
        END
        
        COMMIT TRANSACTION;
        SELECT @NewWitchID AS WitchID;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
```

## Data Models

### Witch 表字段（38个）

**基础字段**（10个）:
- WitchID (INT, PK, IDENTITY)
- Name (NVARCHAR(50), NOT NULL)
- Magic (NVARCHAR(100))
- PrisonerNo (NVARCHAR(20))
- Status (NVARCHAR(20), NOT NULL, DEFAULT 'Normal')
- ExecutionResult (NVARCHAR(50))
- AvatarPath (NVARCHAR(255))
- IslandID (INT)
- BatchID (INT)
- DescriptionPublic (NVARCHAR(MAX))

**扩展字段**（28个）:
- PersonalNo (NVARCHAR(20))
- FormerName (NVARCHAR(100))
- Gender (NVARCHAR(10))
- BirthDate (DATE)
- Ethnicity (NVARCHAR(50))
- Birthplace (NVARCHAR(100))
- Height (DECIMAL(5,2))
- Weight (DECIMAL(5,2))
- BloodType (NVARCHAR(10))
- Address (NVARCHAR(500))
- Phone (NVARCHAR(50))
- Email (NVARCHAR(100))
- LineAccount (NVARCHAR(100))
- HighestEducation (NVARCHAR(100))
- EducationHistory (NVARCHAR(MAX))  -- JSON
- WorkHistory (NVARCHAR(MAX))       -- JSON
- FamilyStructure (NVARCHAR(200))
- Father (NVARCHAR(200))
- Mother (NVARCHAR(200))
- OtherFamily1 (NVARCHAR(200))
- OtherFamily2 (NVARCHAR(200))
- OtherFamily3 (NVARCHAR(200))
- Skills (NVARCHAR(500))
- Hobbies (NVARCHAR(500))
- Dreams (NVARCHAR(500))
- Dislikes (NVARCHAR(500))
- Trauma (NVARCHAR(MAX))
- WitchTransformMethod (NVARCHAR(500))
- Remarks (NVARCHAR(MAX))

### 状态枚举

```csharp
public enum WitchStatus
{
    待分配,
    分配至岛屿,
    审判中,
    死亡_正常,
    死亡_魔女化,
    其它
}
```

## Correctne
ss Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Data validation completeness

*For any* input data, when the save button is clicked, the system should validate all required fields (name, magic, status) are not empty.
**Validates: Requirements 4.1, 4.2, 4.3**

### Property 2: Pending assignment logic

*For any* witch record where island and batch are not selected, the system should allow saving and automatically set status to "待分配".
**Validates: Requirements 4.4**

### Property 3: Island-batch relationship validation

*For any* input where island is selected but batch is not, the system should display error message and prevent saving.
**Validates: Requirements 4.5**

### Property 4: Numeric field validation

*For any* height or weight input, the system should validate that the value is a valid numeric format.
**Validates: Requirements 4.6, 4.7**

### Property 5: Birth date validation

*For any* birth date input, the system should validate that the date is in correct format and not later than current date.
**Validates: Requirements 4.8**

### Property 6: Email format validation

*For any* email input, the system should validate that it matches standard email format pattern.
**Validates: Requirements 4.9**

### Property 7: Validation failure handling

*For any* validation failure, the system should display specific error message and block the save operation.
**Validates: Requirements 4.10**

### Property 8: Education history management

*For any* education record added to the list, the record should appear in the education history DataGridView.
**Validates: Requirements 3.2**

### Property 9: Education record editing

*For any* selected education record, clicking edit should open the dialog with that record's data pre-filled.
**Validates: Requirements 3.3**

### Property 10: Education record deletion

*For any* selected education record, clicking delete should remove it from the list.
**Validates: Requirements 3.4**

### Property 11: Education history JSON serialization

*For any* education history list, when saving witch information, the list should be correctly serialized to JSON format.
**Validates: Requirements 3.5**

### Property 12: Work history JSON serialization

*For any* work history list, when saving witch information, the list should be correctly serialized to JSON format.
**Validates: Requirements 3.10**

### Property 13: Batch capacity query

*For any* selected batch, the system should query and display the current witch count.
**Validates: Requirements 5.1**

### Property 14: Batch capacity check

*For any* batch with current count less than 13, the system should allow adding new witch.
**Validates: Requirements 5.2**

### Property 15: Full batch prevention

*For any* batch with current count equal to 13, the system should display warning message and disable save button.
**Validates: Requirements 5.3, 5.4**

### Property 16: Batch capacity UI update

*For any* batch selection change, the system should update the save button enabled state based on batch capacity.
**Validates: Requirements 5.5**

### Property 17: Pending assignment batch check bypass

*For any* witch with status "待分配" and no island/batch selected, the system should skip batch capacity check.
**Validates: Requirements 5.6**

### Property 18: Database insert operation

*For any* valid witch data with non-full batch, calling sp_AddWitchComplete should insert a record into wt.Witch table.
**Validates: Requirements 6.4**

### Property 19: WitchID return

*For any* successful insert operation, sp_AddWitchComplete should return the newly created WitchID.
**Validates: Requirements 6.5**

### Property 20: Batch count update

*For any* successful witch insert, the system should automatically increment the batch's WitchCount field.
**Validates: Requirements 6.6**

### Property 21: Transaction rollback on failure

*For any* failure during the insert process, the system should rollback all changes and return error information.
**Validates: Requirements 6.7**

### Property 22: Grid refresh on success

*For any* successful save operation, when WitchAddForm closes, Form1_Admin should refresh its data grid.
**Validates: Requirements 7.3**

### Property 23: No refresh on cancel

*For any* cancelled operation, when WitchAddForm closes without saving, Form1_Admin should not refresh its data grid.
**Validates: Requirements 7.4**

### Property 24: User account creation condition

*For any* witch with non-empty prisoner number and status "分配至岛屿", the system should create a corresponding user account.
**Validates: Requirements 8.2**

### Property 25: User account initialization

*For any* created user account, the password should be set to PENDING, role should be Witch, and island/batch should match witch record.
**Validates: Requirements 8.3, 8.4, 8.5**

### Property 26: UserWitch relationship creation

*For any* created user account, a corresponding record should be created in UserWitch table linking the user to the witch.
**Validates: Requirements 8.6**

### Property 27: Duplicate username handling

*For any* prisoner number that already exists as a username, the system should skip account creation and display informational message.
**Validates: Requirements 8.7**

### Property 28: No account for pending assignment

*For any* witch with status "待分配", the system should not create a user account.
**Validates: Requirements 8.8**

## Error Handling

### 1. Validation Errors

**场景**: 用户输入不完整或格式错误

**处理策略**:
- 在客户端（WitchAddForm）进行第一层验证
- 显示具体的错误消息，指出哪个字段有问题
- 使用 ErrorProvider 控件在字段旁显示错误图标
- 阻止保存操作，直到所有错误修正

### 2. Batch Capacity Errors

**场景**: 批次已满，无法添加新魔女

**处理策略**:
- 在选择批次时实时检查容量
- 显示批次信息："当前人数：13/13（已满）"
- 禁用保存按钮
- 提示用户选择其他批次或创建新批次

### 3. Database Errors

**场景**: 数据库操作失败（连接断开、约束违反等）

**处理策略**:
- 在存储过程中使用 TRY-CATCH 块
- 回滚事务，确保数据一致性
- 向上抛出异常，包含详细错误信息
- 在 UI 层显示友好的错误消息

### 4. JSON Serialization Errors

**场景**: 教育/工作经历序列化失败

**处理策略**:
- 使用 try-catch 包裹序列化代码
- 如果序列化失败，显示错误消息
- 允许用户修正数据后重试
- 记录错误日志以便调试

### 5. Duplicate Prisoner Number

**场景**: 囚犯编号已存在

**处理策略**:
- 在保存前检查囚犯编号唯一性
- 如果重复，显示错误消息："囚犯编号 XXX 已存在"
- 阻止保存操作
- 建议用户检查输入或使用其他编号

## Testing Strategy

### Unit Testing

本项目将使用 **NUnit** 作为单元测试框架。

#### 测试范围

1. **WitchDAL.AddWitchComplete 方法**
   - 测试正常插入流程
   - 测试 null 值处理
   - 测试批次容量检查

2. **WitchAddForm 验证逻辑**
   - 测试必填字段验证
   - 测试数字格式验证
   - 测试日期验证
   - 测试邮箱格式验证

3. **JSON 序列化/反序列化**
   - 测试教育经历序列化
   - 测试工作经历序列化
   - 测试空列表处理

4. **批次容量检查**
   - 测试未满批次允许添加
   - 测试已满批次拒绝添加
   - 测试待分配状态跳过检查

### Property-Based Testing

本项目将使用 **FsCheck** (C# 版本) 作为属性测试框架。每个属性测试将运行至少 **100 次迭代**。

#### 测试配置

```csharp
[Property(MaxTest = 100)]
public Property PropertyName()
{
    // Property implementation
}
```

#### 属性测试标注格式

每个属性测试必须使用以下格式标注：

```csharp
// **Feature: country-level-witch-addition, Property 1: Data validation completeness**
```

#### 测试范围

1. **Property 1: Data validation completeness**
   - 生成随机输入数据
   - 验证必填字段检查逻辑
   - 确保所有必填字段都被验证

2. **Property 11: Education history JSON serialization**
   - 生成随机教育经历列表
   - 序列化为 JSON
   - 反序列化并验证数据一致性

3. **Property 12: Work history JSON serialization**
   - 生成随机工作经历列表
   - 序列化为 JSON
   - 反序列化并验证数据一致性

4. **Property 14: Batch capacity check**
   - 生成随机批次数据
   - 测试容量检查逻辑
   - 验证未满批次允许添加

5. **Property 24: User account creation condition**
   - 生成随机魔女数据
   - 测试账号创建条件
   - 验证只有符合条件的魔女创建账号

### Integration Testing

集成测试将验证完整的用户工作流：

1. **完整添加流程**
   - 打开表单 → 填写所有字段 → 保存 → 验证数据库

2. **批次容量限制流程**
   - 选择已满批次 → 验证保存被阻止

3. **用户账号创建流程**
   - 添加魔女（分配至岛屿） → 验证账号创建 → 验证关联关系

4. **待分配状态流程**
   - 不选择岛屿/批次 → 保存 → 验证状态为"待分配"

## Implementation Notes

### JSON 格式规范

**教育经历 JSON 格式**:
```json
[
  {
    "school": "东京都立樱丘中学校",
    "degree": "中学校",
    "status": "毕业",
    "specialNote": "成绩优异"
  }
]
```

**工作经历 JSON 格式**:
```json
[
  {
    "period": "2020/04-2022/03",
    "company": "东京商事株式会社",
    "position": "营业部助理",
    "salary": "月薪 25 万日元",
    "resignReason": "被发现魔女身份"
  }
]
```

### UI 布局建议

**窗口大小**: 900x700
**TabControl**: Dock = Fill
**按钮区域**: Dock = Bottom, Height = 50

**标签页顺序**:
1. 基本信息（最重要，默认显示）
2. 身体特征
3. 联系方式
4. 教育背景
5. 工作经历
6. 家庭关系
7. 个性特征
8. 魔女信息
9. 分配信息

### 性能考虑

1. **延迟加载批次列表**
   - 只在选择岛屿后加载对应批次
   - 避免一次性加载所有批次

2. **异步保存**
   - 使用 async/await 进行数据库操作
   - 显示进度指示器

3. **数据缓存**
   - 缓存岛屿列表（不常变化）
   - 批次容量信息实时查询

## Security Considerations

### 1. 输入验证

- 在客户端和服务器端都进行验证
- 防止 SQL 注入（使用参数化查询）
- 限制字符串长度，防止缓冲区溢出

### 2. 权限控制

- 只有 Admin 角色可以访问国家层添加功能
- 在 Form1_Admin 中检查用户角色
- 在存储过程中也可以添加权限检查

### 3. 数据完整性

- 使用事务确保原子性
- 外键约束确保引用完整性
- 触发器自动更新批次计数

### 4. 审计日志

- 记录所有魔女添加操作
- 包含操作者、时间、添加的魔女信息
- 使用 AuditDAL.Log 方法

## Deployment Considerations

### 1. 数据库迁移

需要创建存储过程 `wt.sp_AddWitchComplete`

### 2. 向后兼容性

- 保留原有的简单添加功能（Form1 中的"新增魔女"）
- 国家层添加作为新增功能，不影响现有功能

### 3. 数据迁移

无需数据迁移，Witch 表字段已存在

### 4. 用户培训

- 为 Admin 提供国家层添加功能的使用说明
- 强调必填字段和数据格式要求
- 提供示例数据和操作演示

## Future Enhancements

### 1. 批量导入

- 支持从 Excel 或 CSV 文件批量导入魔女信息
- 数据验证和错误报告
- 导入预览功能

### 2. 模板功能

- 保存常用的输入模板
- 快速填充相似魔女的信息
- 模板管理界面

### 3. 图片上传

- 直接在表单中上传头像图片
- 自动重命名为囚犯编号
- 图片预览功能

### 4. 数据导出

- 导出魔女完整档案为 PDF
- 支持批量导出
- 自定义导出字段
