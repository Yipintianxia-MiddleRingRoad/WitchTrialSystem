# 处刑投票流程系统 - 设计文档

## 概述

处刑投票流程系统是《魔女审判系统》的核心功能模块，实现完整的魔女审判投票流程。由于系统为单机单用户模式，通过数据库状态持久化和状态机设计，确保用户退出再进入时能够恢复到正确状态。

### 设计目标

1. **状态持久化**: 所有状态存储在数据库，用户退出再进入保持一致
2. **流程不可逆**: 通过数据库标志位控制，投票后不能修改
3. **防止退出**: 在关键阶段禁用关闭按钮，强制完成流程
4. **实时同步**: 通过定时器检查状态变化，模拟多端同步
5. **角色隔离**: 典狱长和魔女看到完全不同的界面
6. **异常恢复**: 崩溃后可以恢复到正确状态
7. **岛屿隔离**: 每个岛屿独立管理，数据互不干扰

### 四层权限体系

**极其重要**：系统有四层权限体系，时刻记住：

1. **Admin（国家层）** - 如 admin
   - 最高权限，可查看所有岛屿数据
   - 仅监督，不参与审判流程

2. **Meruru（岛屿监管者层）** - 如 meruru_regulator, utena_regulator
   - 监管岛屿运营
   - 不参与审判流程

3. **Warden（岛屿典狱长层）** - 如 warden, warden2
   - 发起审判、查看投票结果、确认处刑对象
   - 不参与投票

4. **Witch（普通魔女层）** - 普通魔女用户
   - 参与投票、确认处刑
   - 通过手机界面操作

## 架构设计

### 系统架构图

```
┌─────────────────────────────────────────────────────────────┐
│                        UI Layer                              │
│  ┌──────────────────┐  ┌──────────────────┐                │
│  │ TrialManagement  │  │ TrialVoting      │                │
│  │ Form (Warden)    │  │ Form (Witch)     │                │
│  └────────┬─────────┘  └────────┬─────────┘                │
└───────────┼──────────────────────┼──────────────────────────┘
            │                      │
            ↓                      ↓
┌─────────────────────────────────────────────────────────────┐
│                        BLL Layer                             │
│  ┌──────────────────┐  ┌──────────────────┐                │
│  │ TrialSession     │  │ TrialVoting      │                │
│  │ Service          │  │ Service          │                │
│  └────────┬─────────┘  └────────┬─────────┘                │
└───────────┼──────────────────────┼──────────────────────────┘
            │                      │
            ↓                      ↓
┌─────────────────────────────────────────────────────────────┐
│                        DAL Layer                             │
│  ┌──────────────────┐  ┌──────────────────┐                │
│  │ TrialSession     │  │ TrialParticipant │                │
│  │ DAL              │  │ DAL              │                │
│  └────────┬─────────┘  └────────┬─────────┘                │
└───────────┼──────────────────────┼──────────────────────────┘
            │                      │
            ↓                      ↓
┌─────────────────────────────────────────────────────────────┐
│                      Database Layer                          │
│  ┌──────────────────┐  ┌──────────────────┐                │
│  │ TrialSession     │  │ TrialParticipant │                │
│  │ Table            │  │ Table            │                │
│  └──────────────────┘  └──────────────────┘                │
│  ┌──────────────────┐                                       │
│  │ TrialNotification│                                       │
│  │ Table            │                                       │
│  └──────────────────┘                                       │
└─────────────────────────────────────────────────────────────┘
```


### 三层架构说明

1. **UI Layer (用户界面层)**
   - `TrialManagementForm`: 典狱长审判管理界面
   - `TrialVotingForm`: 魔女投票界面（手机风格）
   - `TrialExecutionConfirmForm`: 魔女处刑对象确认界面（手机风格）
   - `ExecutionForm`: 现有的处刑按钮界面（复用）

2. **BLL Layer (业务逻辑层)**
   - `TrialSessionService`: 审判会话业务逻辑
   - `TrialVotingService`: 投票业务逻辑
   - `TrialNotificationService`: 通知业务逻辑

3. **DAL Layer (数据访问层)**
   - `TrialSessionDAL`: 审判会话数据访问
   - `TrialParticipantDAL`: 参与者数据访问
   - `TrialNotificationDAL`: 通知数据访问

## 数据模型

### 数据库表设计

#### 1. TrialSession（审判会话表）

```sql
CREATE TABLE wt.TrialSession (
    SessionID INT IDENTITY PRIMARY KEY,
    IslandID INT NOT NULL,
    BatchID INT NOT NULL,
    Status NVARCHAR(20) NOT NULL,  -- 状态：Pending, Voting, Confirmed, Executing, Completed, Cancelled
    CreatedBy INT NOT NULL,  -- 发起人UserID（典狱长）
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    VotingStartTime DATETIME2 NULL,  -- 投票开始时间
    VotingEndTime DATETIME2 NULL,    -- 投票结束时间
    ExecutionTargetWitchID INT NULL,  -- 处刑对象WitchID
    ExecutionConfirmedAt DATETIME2 NULL,  -- 确认处刑时间
    CompletedAt DATETIME2 NULL,  -- 完成时间
    
    CONSTRAINT FK_TrialSession_Island FOREIGN KEY (IslandID) 
        REFERENCES wt.Island(IslandID),
    CONSTRAINT FK_TrialSession_Batch FOREIGN KEY (BatchID) 
        REFERENCES wt.Batch(BatchID),
    CONSTRAINT FK_TrialSession_CreatedBy FOREIGN KEY (CreatedBy) 
        REFERENCES wt.[User](UserID),
    CONSTRAINT FK_TrialSession_ExecutionTarget FOREIGN KEY (ExecutionTargetWitchID) 
        REFERENCES wt.Witch(WitchID),
    CONSTRAINT CK_TrialSession_Status CHECK (Status IN (
        'Pending', 'Voting', 'Confirmed', 'Executing', 'Completed', 'Cancelled'
    ))
);

CREATE INDEX IX_TrialSession_Island ON wt.TrialSession(IslandID);
CREATE INDEX IX_TrialSession_Status ON wt.TrialSession(IslandID, Status);
CREATE INDEX IX_TrialSession_CreatedAt ON wt.TrialSession(CreatedAt DESC);
```

**字段说明**:
- `SessionID`: 主键，自增ID
- `IslandID`: 所属岛屿ID
- `BatchID`: 批次ID（参与魔女的批次）
- `Status`: 审判状态
  - `Pending`: 待开始（已创建，通知已发送）
  - `Voting`: 投票进行中
  - `Confirmed`: 投票完成，典狱长已确认处刑对象
  - `Executing`: 等待魔女确认处刑
  - `Completed`: 处刑完成
  - `Cancelled`: 已取消
- `CreatedBy`: 发起人UserID（典狱长）
- `CreatedAt`: 创建时间
- `VotingStartTime`: 投票开始时间
- `VotingEndTime`: 投票结束时间
- `ExecutionTargetWitchID`: 处刑对象WitchID
- `ExecutionConfirmedAt`: 确认处刑时间
- `CompletedAt`: 完成时间


#### 2. TrialParticipant（审判参与者表）

```sql
CREATE TABLE wt.TrialParticipant (
    ParticipantID INT IDENTITY PRIMARY KEY,
    SessionID INT NOT NULL,
    WitchID INT NOT NULL,
    UserID INT NOT NULL,
    HasVoted BIT NOT NULL DEFAULT 0,  -- 是否已投票
    VotedForWitchID INT NULL,  -- 投给谁
    VotedAt DATETIME2 NULL,  -- 投票时间
    HasConfirmedExecution BIT NOT NULL DEFAULT 0,  -- 是否已确认处刑（点击处刑按钮）
    ExecutionConfirmedAt DATETIME2 NULL,  -- 确认处刑时间
    
    CONSTRAINT FK_TrialParticipant_Session FOREIGN KEY (SessionID) 
        REFERENCES wt.TrialSession(SessionID),
    CONSTRAINT FK_TrialParticipant_Witch FOREIGN KEY (WitchID) 
        REFERENCES wt.Witch(WitchID),
    CONSTRAINT FK_TrialParticipant_User FOREIGN KEY (UserID) 
        REFERENCES wt.[User](UserID),
    CONSTRAINT FK_TrialParticipant_VotedFor FOREIGN KEY (VotedForWitchID) 
        REFERENCES wt.Witch(WitchID),
    CONSTRAINT UQ_TrialParticipant_Session_Witch UNIQUE (SessionID, WitchID)
);

CREATE INDEX IX_TrialParticipant_Session ON wt.TrialParticipant(SessionID);
CREATE INDEX IX_TrialParticipant_User ON wt.TrialParticipant(UserID);
CREATE INDEX IX_TrialParticipant_Witch ON wt.TrialParticipant(WitchID);
```

**字段说明**:
- `ParticipantID`: 主键，自增ID
- `SessionID`: 审判会话ID
- `WitchID`: 参与魔女的WitchID
- `UserID`: 参与魔女的UserID
- `HasVoted`: 是否已投票
- `VotedForWitchID`: 投给谁（WitchID）
- `VotedAt`: 投票时间
- `HasConfirmedExecution`: 是否已确认处刑（点击处刑按钮）
- `ExecutionConfirmedAt`: 确认处刑时间

#### 3. TrialNotification（审判通知表）

```sql
CREATE TABLE wt.TrialNotification (
    NotificationID INT IDENTITY PRIMARY KEY,
    SessionID INT NOT NULL,
    UserID INT NOT NULL,
    Message NVARCHAR(500) NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_TrialNotification_Session FOREIGN KEY (SessionID) 
        REFERENCES wt.TrialSession(SessionID),
    CONSTRAINT FK_TrialNotification_User FOREIGN KEY (UserID) 
        REFERENCES wt.[User](UserID)
);

CREATE INDEX IX_TrialNotification_User ON wt.TrialNotification(UserID, IsRead);
CREATE INDEX IX_TrialNotification_Session ON wt.TrialNotification(SessionID);
```

**字段说明**:
- `NotificationID`: 主键，自增ID
- `SessionID`: 审判会话ID
- `UserID`: 接收通知的UserID
- `Message`: 通知消息内容
- `IsRead`: 是否已读
- `CreatedAt`: 创建时间


### C# 数据模型

#### TrialSessionModel

```csharp
namespace WitchTrialSystem.Models
{
    public class TrialSessionModel
    {
        public int SessionID { get; set; }
        public int IslandID { get; set; }
        public int BatchID { get; set; }
        public string Status { get; set; } = "";
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? VotingStartTime { get; set; }
        public DateTime? VotingEndTime { get; set; }
        public int? ExecutionTargetWitchID { get; set; }
        public DateTime? ExecutionConfirmedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        
        // 计算属性
        public bool IsPending => Status == "Pending";
        public bool IsVoting => Status == "Voting";
        public bool IsConfirmed => Status == "Confirmed";
        public bool IsExecuting => Status == "Executing";
        public bool IsCompleted => Status == "Completed";
        public bool IsActive => Status != "Completed" && Status != "Cancelled";
    }
}
```

#### TrialParticipantModel

```csharp
namespace WitchTrialSystem.Models
{
    public class TrialParticipantModel
    {
        public int ParticipantID { get; set; }
        public int SessionID { get; set; }
        public int WitchID { get; set; }
        public int UserID { get; set; }
        public bool HasVoted { get; set; }
        public int? VotedForWitchID { get; set; }
        public DateTime? VotedAt { get; set; }
        public bool HasConfirmedExecution { get; set; }
        public DateTime? ExecutionConfirmedAt { get; set; }
        
        // 扩展属性（从其他表JOIN获取）
        public string WitchName { get; set; } = "";
        public string Username { get; set; } = "";
        public string AvatarPath { get; set; } = "";
        public string VotedForWitchName { get; set; } = "";
    }
}
```

#### TrialNotificationModel

```csharp
namespace WitchTrialSystem.Models
{
    public class TrialNotificationModel
    {
        public int NotificationID { get; set; }
        public int SessionID { get; set; }
        public int UserID { get; set; }
        public string Message { get; set; } = "";
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
```

#### TrialStateEnum

```csharp
namespace WitchTrialSystem.Models
{
    public enum TrialState
    {
        Idle = 0,                           // 无审判
        NotParticipating = 1,               // 不参与此审判
        WaitingToStart = 2,                 // 等待投票开始（Pending状态）
        Voting = 3,                         // 投票中（未投票）
        WaitingForOthersToVote = 4,         // 等待其他人投票（已投票）
        WaitingForExecutionAnnouncement = 5,// 等待宣布处刑对象（Confirmed状态）
        ConfirmingExecution = 6,            // 确认处刑中（未确认）
        WaitingForOthersToConfirm = 7,      // 等待其他人确认（已确认）
        Completed = 8                       // 审判完成
    }
}
```


## 状态机设计

### 状态转换流程图

```
状态转换流程：

┌─────────────┐
│   Idle      │ 无审判状态（初始状态）
│  (状态0)    │
└──────┬──────┘
       │ 典狱长发起审判
       ↓
┌─────────────┐
│  Pending    │ 审判待开始（已创建，通知已发送）
│  (状态1)    │ - 魔女收到通知
└──────┬──────┘ - 典狱长显示"开始投票"按钮
       │ 典狱长点击"开始投票"
       ↓
┌─────────────┐
│  Voting     │ 投票进行中
│  (状态2)    │ - 魔女看到投票界面
└──────┬──────┘ - 不能退出，必须投票
       │ 所有人投票完成
       ↓
┌─────────────┐
│ Confirmed   │ 投票完成，典狱长确认处刑对象
│  (状态3)    │ - 典狱长看到投票结果
└──────┬──────┘ - 选择处刑对象
       │ 典狱长点击"宣布处刑对象"
       ↓
┌─────────────┐
│ Executing   │ 等待魔女确认处刑
│  (状态4)    │ - 魔女看到处刑对象头像
└──────┬──────┘ - 点击确认后进入处刑按钮界面
       │ 所有魔女点击处刑按钮
       ↓
┌─────────────┐
│ Completed   │ 处刑完成
│  (状态5)    │ - 典狱长可以开始实际处刑
└─────────────┘ - 更新魔女状态为"已处刑"
       │
       ↓
    返回Idle
```

### 魔女端状态检测逻辑

```csharp
public TrialState GetCurrentTrialState(int userId, int islandID)
{
    // 1. 查询是否有进行中的审判
    var session = GetActiveTrialSession(islandID);
    if (session == null)
        return TrialState.Idle;
    
    // 2. 检查用户是否参与此审判
    var participant = GetParticipant(session.SessionID, userId);
    if (participant == null)
        return TrialState.NotParticipating;
    
    // 3. 根据会话状态和参与者状态返回
    return session.Status switch
    {
        "Pending" => TrialState.WaitingToStart,
        "Voting" => participant.HasVoted 
            ? TrialState.WaitingForOthersToVote 
            : TrialState.Voting,
        "Confirmed" => TrialState.WaitingForExecutionAnnouncement,
        "Executing" => participant.HasConfirmedExecution 
            ? TrialState.WaitingForOthersToConfirm 
            : TrialState.ConfirmingExecution,
        "Completed" => TrialState.Completed,
        _ => TrialState.Idle
    };
}
```


## 详细流程设计

### 阶段0：无审判状态（Idle）

**典狱长端：**
- 显示"发起审判"按钮
- 可以选择本岛屿本批次的魔女参加审判

**魔女端：**
- 处刑按钮显示为灰色/禁用状态
- 点击后提示"当前无审判进行中"

---

### 阶段1：审判待开始（Pending）

**典狱长操作：**
1. 点击"发起审判"按钮
2. 弹出对话框，勾选参加审判的魔女（本岛屿本批次，至少2人，最多13人）
3. 确认后：
   - 创建 `TrialSession` 记录（Status = 'Pending'）
   - 为每个参与魔女创建 `TrialParticipant` 记录
   - 为每个参与魔女创建 `TrialNotification` 通知
4. 典狱长界面显示"开始投票"按钮

**魔女端：**
- 登录后自动检测是否有未读通知
- 如果有，弹出通知消息框（5秒自动消失）：
  - 左侧：典狱长头像
  - 右侧：文字"呀咧呀咧，又死人了，真实的，请速速前往审判庭"
- 处刑按钮变为可点击状态（但点击后显示"等待投票开始"）

---

### 阶段2：投票进行中（Voting）

**典狱长操作：**
1. 点击"开始投票"按钮
2. 更新 `TrialSession.Status = 'Voting'`，记录 `VotingStartTime`
3. 典狱长界面显示：
   - 参与魔女列表
   - 每个魔女的投票状态（已投票/未投票）
   - 实时更新投票进度（如：5/10 已投票）
4. 当所有人投票完成后，显示"查看投票结果"按钮

**魔女端：**
- 点击处刑按钮后，**强制进入投票界面**
- 投票界面设计（手机风格 450x800）：
  ```
  ┌─────────────────────────────┐
  │     魔女审判 - 投票         │
  ├─────────────────────────────┤
  │  请选择您认为应该被处刑的魔女  │
  ├─────────────────────────────┤
  │  ┌───┐  ┌───┐  ┌───┐       │
  │  │头像│  │头像│  │头像│       │
  │  │ ○ │  │ ○ │  │ ○ │       │
  │  │名字│  │名字│  │名字│       │
  │  └───┘  └───┘  └───┘       │
  │  ┌───┐  ┌───┐  ┌───┐       │
  │  │头像│  │头像│  │头像│       │
  │  │ ○ │  │ ○ │  │ ○ │       │
  │  │名字│  │名字│  │名字│       │
  │  └───┘  └───┘  └───┘       │
  │         ...                 │
  ├─────────────────────────────┤
  │        [确认投票]           │
  └─────────────────────────────┘
  ```
- 关键约束：
  - 必须选择一个人（不能不选）
  - 只能选择一个人（单选，使用 RadioButton）
  - 点击"确认投票"后：
    - 更新 `TrialParticipant.HasVoted = 1`
    - 记录 `VotedForWitchID` 和 `VotedAt`
    - **界面变为"等待其他人投票"状态**
    - **不能返回，不能修改**
    - **不能退出程序（禁用关闭按钮）**
  - 如果强制退出程序：
    - 下次登录自动检测状态
    - 如果已投票，显示"等待其他人投票"
    - 如果未投票，继续显示投票界面

---

### 阶段3：投票完成，确认处刑对象（Confirmed）

**典狱长操作：**
1. 所有人投票完成后，点击"查看投票结果"
2. 显示投票结果可视化：
   ```
   ┌─────────────────────────────────┐
   │      投票结果统计               │
   ├─────────────────────────────────┤
   │ 魔女A  │ ████████ 8票          │
   │ 魔女B  │ ████ 4票              │
   │ 魔女C  │ ██ 2票                │
   ├─────────────────────────────────┤
   │ 投票详情：                      │
   │ - 魔女X 投给了 魔女A            │
   │ - 魔女Y 投给了 魔女A            │
   │ ...                             │
   ├─────────────────────────────────┤
   │   [确定处刑对象：魔女A]         │
   └─────────────────────────────────┘
   ```
3. 如果有多人得票相同且最高：
   - 弹出对话框让典狱长选择一个
4. 确认后：
   - 更新 `TrialSession.Status = 'Confirmed'`
   - 更新 `ExecutionTargetWitchID`
   - 记录 `VotingEndTime`
5. 点击"宣布处刑对象"按钮

**魔女端：**
- 仍然显示"等待投票结果"界面
- 不能退出


---

### 阶段4：等待魔女确认处刑（Executing）

**典狱长操作：**
1. 点击"宣布处刑对象"
2. 更新 `TrialSession.Status = 'Executing'`，记录 `ExecutionConfirmedAt`
3. 典狱长界面显示：
   - 处刑对象信息（头像、姓名）
   - 每个魔女的确认状态（已确认/未确认）
   - 实时更新确认进度（如：5/10 已确认）
4. 当所有人确认后，显示"开始处刑"按钮

**魔女端：**
- 自动刷新界面，显示处刑对象头像
- 界面设计（手机风格 450x800）：
  ```
  ┌─────────────────────────────┐
  │     处刑对象确认            │
  ├─────────────────────────────┤
  │                             │
  │      ┌─────────┐            │
  │      │         │            │
  │      │  头像   │            │
  │      │         │            │
  │      └─────────┘            │
  │                             │
  │      魔女A 将被处刑         │
  │                             │
  ├─────────────────────────────┤
  │      [确认处刑]             │
  └─────────────────────────────┘
  ```
- 播放滑稽音效（音频文件路径：`Images/sounds/execution_notice.wav`）
- 点击"确认处刑"后：
  - 跳转到现有的 `ExecutionForm` 界面
  - 显示 `execution_bg.png` 背景
  - 处刑按钮为灰色（未点击状态）
  - 点击处刑按钮后：
    - 背景切换为 `execution_complete.png`
    - 处刑按钮变红色（融入背景图）
    - 弹出"处刑成功"消息框
    - 更新 `TrialParticipant.HasConfirmedExecution = 1`
    - 记录 `ExecutionConfirmedAt`
  - **不能退出，必须完成点击**

---

### 阶段5：处刑完成（Completed）

**典狱长操作：**
1. 所有魔女确认后，点击"开始处刑"
2. 弹出确认对话框："确定要处刑 [魔女名字] 吗？"
3. 确认后：
   - 更新 `TrialSession.Status = 'Completed'`
   - 记录 `CompletedAt`
   - 更新魔女状态：`UPDATE wt.Witch SET Status = N'已处刑', ExecutionResult = N'投票处刑' WHERE WitchID = @WitchID`
   - 记录操作日志到 `OperationLog` 表
4. 显示"审判完成"消息
5. 流程结束，返回 Idle 状态

**魔女端：**
- 自动返回手机主界面
- 处刑按钮恢复为灰色/禁用状态


## 组件和接口

### BLL层接口设计

#### ITrialSessionService

```csharp
public interface ITrialSessionService
{
    // 查询操作
    TrialSessionModel? GetActiveSession(int islandID);
    TrialSessionModel? GetSessionByID(int sessionID);
    List<TrialSessionModel> GetSessionHistory(int islandID, int limit = 10);
    bool HasActiveSession(int islandID);
    
    // 典狱长操作
    (bool Success, string Message, int SessionID) CreateSession(int islandID, int batchID, int createdBy, List<int> participantWitchIDs);
    (bool Success, string Message) StartVoting(int sessionID, int wardenUserID);
    (bool Success, string Message) ConfirmExecutionTarget(int sessionID, int targetWitchID, int wardenUserID);
    (bool Success, string Message) AnnounceExecutionTarget(int sessionID, int wardenUserID);
    (bool Success, string Message) CompleteExecution(int sessionID, int wardenUserID);
    (bool Success, string Message) CancelSession(int sessionID, int wardenUserID);
    
    // 状态检测
    TrialState GetCurrentState(int userId, int islandID);
    
    // 统计信息
    Dictionary<int, int> GetVotingStatistics(int sessionID);  // WitchID -> 得票数
    List<TrialParticipantModel> GetVotingDetails(int sessionID);
    (int Voted, int Total) GetVotingProgress(int sessionID);
    (int Confirmed, int Total) GetConfirmationProgress(int sessionID);
}
```

#### ITrialVotingService

```csharp
public interface ITrialVotingService
{
    // 投票操作
    (bool Success, string Message) SubmitVote(int sessionID, int voterWitchID, int votedForWitchID);
    (bool Success, string Message) ConfirmExecution(int sessionID, int witchID);
    
    // 查询操作
    List<TrialParticipantModel> GetParticipants(int sessionID);
    TrialParticipantModel? GetParticipant(int sessionID, int userID);
    bool HasVoted(int sessionID, int userID);
    bool HasConfirmedExecution(int sessionID, int userID);
    
    // 验证操作
    bool CanVote(int sessionID, int userID);
    bool CanConfirmExecution(int sessionID, int userID);
}
```

#### ITrialNotificationService

```csharp
public interface ITrialNotificationService
{
    // 通知操作
    void CreateNotifications(int sessionID, List<int> userIDs, string message);
    List<TrialNotificationModel> GetUnreadNotifications(int userID);
    void MarkAsRead(int notificationID);
    void MarkAllAsRead(int userID);
    
    // 查询操作
    List<TrialNotificationModel> GetNotificationsBySession(int sessionID);
    int GetUnreadCount(int userID);
}
```

### DAL层接口设计

#### ITrialSessionDAL

```csharp
public interface ITrialSessionDAL
{
    // CRUD操作
    int Insert(TrialSessionModel session);
    int Update(TrialSessionModel session);
    TrialSessionModel? GetByID(int sessionID);
    TrialSessionModel? GetActiveByIsland(int islandID);
    List<TrialSessionModel> GetByIsland(int islandID, int limit = 10);
    
    // 状态更新
    int UpdateStatus(int sessionID, string status);
    int UpdateExecutionTarget(int sessionID, int targetWitchID);
}
```

#### ITrialParticipantDAL

```csharp
public interface ITrialParticipantDAL
{
    // CRUD操作
    int Insert(TrialParticipantModel participant);
    int Update(TrialParticipantModel participant);
    int InsertBatch(List<TrialParticipantModel> participants);
    
    // 查询操作
    List<TrialParticipantModel> GetBySession(int sessionID);
    TrialParticipantModel? GetBySessionAndUser(int sessionID, int userID);
    TrialParticipantModel? GetBySessionAndWitch(int sessionID, int witchID);
    
    // 投票操作
    int UpdateVote(int participantID, int votedForWitchID);
    int UpdateExecutionConfirmation(int participantID);
    
    // 统计操作
    int GetVotedCount(int sessionID);
    int GetConfirmedCount(int sessionID);
    Dictionary<int, int> GetVoteStatistics(int sessionID);
}
```

#### ITrialNotificationDAL

```csharp
public interface ITrialNotificationDAL
{
    // CRUD操作
    int Insert(TrialNotificationModel notification);
    int InsertBatch(List<TrialNotificationModel> notifications);
    
    // 查询操作
    List<TrialNotificationModel> GetByUser(int userID, bool unreadOnly = false);
    List<TrialNotificationModel> GetBySession(int sessionID);
    
    // 更新操作
    int MarkAsRead(int notificationID);
    int MarkAllAsRead(int userID);
}
```


## 关键技术实现要点

### 1. 状态检测机制

每次用户登录或打开处刑界面时，都要检测当前状态：

```csharp
public TrialState GetCurrentTrialState(int userId, int islandID)
{
    // 1. 查询是否有进行中的审判
    var session = GetActiveTrialSession(islandID);
    if (session == null)
        return TrialState.Idle;
    
    // 2. 检查用户是否参与此审判
    var participant = GetParticipant(session.SessionID, userId);
    if (participant == null)
        return TrialState.NotParticipating;
    
    // 3. 根据会话状态和参与者状态返回
    return session.Status switch
    {
        "Pending" => TrialState.WaitingToStart,
        "Voting" => participant.HasVoted 
            ? TrialState.WaitingForOthersToVote 
            : TrialState.Voting,
        "Confirmed" => TrialState.WaitingForExecutionAnnouncement,
        "Executing" => participant.HasConfirmedExecution 
            ? TrialState.WaitingForOthersToConfirm 
            : TrialState.ConfirmingExecution,
        "Completed" => TrialState.Completed,
        _ => TrialState.Idle
    };
}
```

### 2. 防止退出机制

```csharp
// 在投票和确认处刑阶段，禁用关闭按钮
protected override void OnFormClosing(FormClosingEventArgs e)
{
    if (_currentState == TrialState.Voting && !_hasVoted)
    {
        e.Cancel = true;
        MessageBox.Show("请先完成投票！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }
    
    if (_currentState == TrialState.ConfirmingExecution && !_hasConfirmed)
    {
        e.Cancel = true;
        MessageBox.Show("请先确认处刑！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }
    
    base.OnFormClosing(e);
}
```

### 3. 实时状态刷新

```csharp
// 使用Timer定期检查状态变化
private Timer _stateCheckTimer = new Timer { Interval = 2000 }; // 每2秒检查一次

private void OnStateCheckTimerTick(object sender, EventArgs e)
{
    var newState = GetCurrentTrialState(_userId, _islandID);
    if (newState != _currentState)
    {
        _currentState = newState;
        RefreshUI();
    }
}

private void RefreshUI()
{
    switch (_currentState)
    {
        case TrialState.Idle:
            ShowIdleUI();
            break;
        case TrialState.WaitingToStart:
            ShowWaitingToStartUI();
            break;
        case TrialState.Voting:
            ShowVotingUI();
            break;
        case TrialState.WaitingForOthersToVote:
            ShowWaitingForOthersUI();
            break;
        case TrialState.ConfirmingExecution:
            ShowExecutionConfirmUI();
            break;
        case TrialState.WaitingForOthersToConfirm:
            ShowWaitingForConfirmUI();
            break;
        case TrialState.Completed:
            ShowCompletedUI();
            break;
    }
}
```

### 4. 通知系统

```csharp
// 登录后检查未读通知
private void CheckNotifications()
{
    var notifications = GetUnreadNotifications(_userId);
    foreach (var notification in notifications)
    {
        ShowNotificationPopup(notification);
        MarkAsRead(notification.NotificationID);
    }
}

// 显示通知弹窗（5秒自动消失）
private void ShowNotificationPopup(TrialNotificationModel notification)
{
    var popup = new NotificationPopupForm(notification);
    popup.Show();
    
    var timer = new Timer { Interval = 5000 };
    timer.Tick += (s, e) => 
    { 
        popup.Close(); 
        timer.Stop(); 
        timer.Dispose();
    };
    timer.Start();
}
```

### 5. 音效播放

```csharp
// 播放滑稽音效
private void PlayExecutionNoticeSound()
{
    try
    {
        string soundPath = Path.Combine(AppContext.BaseDirectory, "Images", "sounds", "execution_notice.wav");
        if (File.Exists(soundPath))
        {
            using (var player = new System.Media.SoundPlayer(soundPath))
            {
                player.Play();
            }
        }
    }
    catch (Exception ex)
    {
        // 音效播放失败不影响流程，只记录日志
        Console.WriteLine($"音效播放失败：{ex.Message}");
    }
}
```

### 6. 投票界面布局

```csharp
// 动态生成投票界面（三个一行）
private void GenerateVotingUI(List<TrialParticipantModel> participants)
{
    var flowPanel = new FlowLayoutPanel
    {
        Dock = DockStyle.Fill,
        AutoScroll = true,
        FlowDirection = FlowDirection.LeftToRight,
        WrapContents = true,
        Padding = new Padding(20)
    };
    
    foreach (var participant in participants)
    {
        var voteCard = CreateVoteCard(participant);
        flowPanel.Controls.Add(voteCard);
    }
    
    this.Controls.Add(flowPanel);
}

private Panel CreateVoteCard(TrialParticipantModel participant)
{
    var card = new Panel
    {
        Width = 120,
        Height = 160,
        Margin = new Padding(10)
    };
    
    // 头像
    var avatar = new PictureBox
    {
        Width = 100,
        Height = 100,
        Left = 10,
        Top = 10,
        SizeMode = PictureBoxSizeMode.StretchImage,
        ImageLocation = participant.AvatarPath
    };
    
    // 单选按钮
    var radioButton = new RadioButton
    {
        Width = 100,
        Left = 10,
        Top = 115,
        Text = participant.WitchName,
        Tag = participant.WitchID
    };
    
    card.Controls.Add(avatar);
    card.Controls.Add(radioButton);
    
    return card;
}
```


## 数据流图

```
典狱长端                    数据库                     魔女端
    │                        │                         │
    │ 1. 发起审判             │                         │
    ├──────────────────────>│                         │
    │   创建Session          │                         │
    │   创建Participant      │                         │
    │   创建Notification     │                         │
    │                        │<────────────────────────┤
    │                        │  2. 登录检查通知         │
    │                        │────────────────────────>│
    │                        │  返回通知消息            │
    │                        │                         │
    │ 3. 开始投票             │                         │
    ├──────────────────────>│                         │
    │   更新Status=Voting    │                         │
    │                        │<────────────────────────┤
    │                        │  4. 提交投票             │
    │                        │────────────────────────>│
    │                        │  更新HasVoted=1          │
    │<───────────────────────┤                         │
    │  5. 查询投票进度        │                         │
    │                        │                         │
    │ 6. 确认处刑对象         │                         │
    ├──────────────────────>│                         │
    │   更新Status=Confirmed │                         │
    │   更新ExecutionTarget  │                         │
    │                        │                         │
    │ 7. 宣布处刑对象         │                         │
    ├──────────────────────>│                         │
    │   更新Status=Executing │                         │
    │                        │<────────────────────────┤
    │                        │  8. 确认处刑             │
    │                        │────────────────────────>│
    │                        │  更新HasConfirmedExecution=1│
    │<───────────────────────┤                         │
    │  9. 查询确认进度        │                         │
    │                        │                         │
    │ 10. 开始处刑            │                         │
    ├──────────────────────>│                         │
    │   更新Status=Completed │                         │
    │   更新Witch.Status     │                         │
    └────────────────────────┴─────────────────────────┘
```

## 错误处理

### 错误类型定义

```csharp
public enum TrialErrorCode
{
    Success = 0,
    SessionNotFound = 2001,
    SessionAlreadyExists = 2002,
    InvalidStatus = 2003,
    NotParticipant = 2004,
    AlreadyVoted = 2005,
    AlreadyConfirmed = 2006,
    InsufficientParticipants = 2007,
    TooManyParticipants = 2008,
    PermissionDenied = 2009,
    IslandMismatch = 2010,
    DatabaseError = 2011,
    UnexpectedError = 2012
}
```

### 错误处理策略

1. **业务逻辑错误**: 返回 `(false, errorMessage)` 元组
2. **数据库错误**: 捕获 `SqlException`，记录日志，返回友好错误消息
3. **权限错误**: 在BLL层检查，拒绝无权限操作
4. **未预期错误**: 捕获所有异常，记录详细日志，返回通用错误消息

## 性能考虑

### 数据库优化

1. **索引策略**:
   - `IslandID` 上的索引（频繁按岛屿查询）
   - `(IslandID, Status)` 上的复合索引（查询进行中的审判）
   - `UserID` 上的索引（查询参与者）
   - `SessionID` 上的索引（关联查询）

2. **查询优化**:
   - 使用参数化查询防止SQL注入
   - 合理使用JOIN减少数据库往返
   - 避免N+1查询问题

### 并发控制

1. **乐观锁**: 使用版本号或时间戳检测并发修改
2. **事务隔离**: 使用 `READ COMMITTED` 隔离级别
3. **状态锁**: 状态更新使用数据库行锁防止冲突

## 安全考虑

### 权限验证

1. **岛屿隔离**: 所有操作验证用户岛屿ID
2. **角色检查**: 典狱长和魔女权限分离
3. **双重验证**: UI层和BLL层都进行权限检查

### 数据验证

1. **输入验证**: 验证参与人数（2-13人）
2. **状态验证**: 验证状态转换合法性
3. **外键验证**: 确保IslandID、WitchID、UserID有效

### 审计日志

1. **操作记录**: 记录所有关键操作到 `OperationLog` 表
2. **时间戳**: 使用北京时间（UTC+8）
3. **详细信息**: 记录操作人、操作类型、操作目标

## 部署说明

### 数据库迁移

1. 执行 `create_trial_tables.sql` 创建表结构
2. 验证数据完整性
3. 测试状态转换流程

### 配置项

```json
{
  "TrialVoting": {
    "MinParticipants": 2,
    "MaxParticipants": 13,
    "StateCheckInterval": 2000,
    "NotificationDisplayDuration": 5000,
    "EnableSound": true,
    "SoundPath": "Images/sounds/execution_notice.wav"
  }
}
```

### 音频文件

1. 创建目录：`Images/sounds/`
2. 放置音效文件：`execution_notice.wav`
3. 确保文件格式为 WAV（Windows支持）

## 未来扩展

### 可能的功能增强

1. **投票时限**: 设置投票超时时间
2. **匿名投票**: 隐藏投票详情，只显示统计
3. **投票理由**: 允许魔女输入投票理由
4. **审判回放**: 查看历史审判的完整流程
5. **统计报表**: 魔女被投票次数、处刑率等统计

### 技术债务

1. 考虑使用SignalR实现真正的实时通信（如果未来支持多端）
2. 考虑使用消息队列处理通知（异步化）
3. 考虑使用缓存减少数据库查询

---

**设计文档版本**: 1.0  
**最后更新**: 2024年12月6日

