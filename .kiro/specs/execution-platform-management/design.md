# 处刑台管理系统 - 设计文档

## 概述

处刑台管理系统是《魔女审判系统》的核心功能模块，用于管理每个岛屿的处刑台位置、刑具配置和移动历史。系统支持典狱长和管理者通过界面操作处刑台，并自动记录所有移动历史以供审计。

### 设计目标

1. **位置管理**: 管理50个固定位置（1-49地下室，50审判庭）
2. **处刑台管理**: 管理49个处刑台及其当前位置
3. **刑具管理**: 支持管理者为处刑台配置刑具
4. **移动控制**: 确保审判庭同时只有一个处刑台
5. **历史追踪**: 记录所有移动历史（不记录操作人）
6. **岛屿隔离**: 每个岛屿独立管理，数据互不干扰
7. **权限控制**: 典狱长可移动，管理者可移动+管理刑具

## 架构设计

### 系统架构图

```
┌─────────────────────────────────────────────────────────────┐
│                        UI Layer                              │
│  ┌──────────────────┐  ┌──────────────────┐                │
│  │ ExecutionPlatform│  │ MovementLog      │                │
│  │ ManagementForm   │  │ ViewForm         │                │
│  └────────┬─────────┘  └────────┬─────────┘                │
└───────────┼──────────────────────┼──────────────────────────┘
            │                      │
            ↓                      ↓
┌─────────────────────────────────────────────────────────────┐
│                        BLL Layer                             │
│  ┌──────────────────┐  ┌──────────────────┐                │
│  │ ExecutionPlatform│  │ MovementLog      │                │
│  │ Service          │  │ Service          │                │
│  └────────┬─────────┘  └────────┬─────────┘                │
└───────────┼──────────────────────┼──────────────────────────┘
            │                      │
            ↓                      ↓
┌─────────────────────────────────────────────────────────────┐
│                        DAL Layer                             │
│  ┌──────────────────┐  ┌──────────────────┐                │
│  │ ExecutionPlatform│  │ MovementLog      │                │
│  │ DAL              │  │ DAL              │                │
│  └────────┬─────────┘  └────────┬─────────┘                │
└───────────┼──────────────────────┼──────────────────────────┘
            │                      │
            ↓                      ↓
┌─────────────────────────────────────────────────────────────┐
│                      Database Layer                          │
│  ┌──────────────────┐  ┌──────────────────┐                │
│  │ ExecutionPlatform│  │ PlatformMovement │                │
│  │ Table            │  │ Log Table        │                │
│  └──────────────────┘  └──────────────────┘                │
└─────────────────────────────────────────────────────────────┘
```

### 三层架构说明

1. **UI Layer (用户界面层)**
   - `ExecutionPlatformManagementForm`: 处刑台管理主界面
   - `MovementLogViewForm`: 移动记录查看界面

2. **BLL Layer (业务逻辑层)**
   - `ExecutionPlatformService`: 处刑台业务逻辑
   - `MovementLogService`: 移动记录业务逻辑

3. **DAL Layer (数据访问层)**
   - `ExecutionPlatformDAL`: 处刑台数据访问
   - `MovementLogDAL`: 移动记录数据访问

## 数据模型

### 数据库表设计

#### 1. ExecutionPlatform（处刑台表）

```sql
CREATE TABLE wt.ExecutionPlatform (
    PlatformID INT PRIMARY KEY IDENTITY(1,1),
    IslandID INT NOT NULL,
    PlatformNumber INT NOT NULL,           -- 处刑台编号 (1-49)
    HomePosition INT NOT NULL,             -- 原位位置 (1-49)
    CurrentPosition INT NOT NULL,          -- 当前位置 (1-50)
    ToolName NVARCHAR(100) NULL,          -- 刑具名称
    ToolType NVARCHAR(50) NULL,           -- 刑具类型
    ToolDescription NVARCHAR(500) NULL,   -- 刑具描述
    Status NVARCHAR(20) NOT NULL DEFAULT '空闲',  -- 状态：空闲/使用中
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_ExecutionPlatform_Island FOREIGN KEY (IslandID) 
        REFERENCES wt.Island(IslandID),
    CONSTRAINT UQ_ExecutionPlatform_Island_Number UNIQUE (IslandID, PlatformNumber),
    CONSTRAINT CK_ExecutionPlatform_Number CHECK (PlatformNumber BETWEEN 1 AND 49),
    CONSTRAINT CK_ExecutionPlatform_HomePosition CHECK (HomePosition BETWEEN 1 AND 49),
    CONSTRAINT CK_ExecutionPlatform_CurrentPosition CHECK (CurrentPosition BETWEEN 1 AND 50),
    CONSTRAINT CK_ExecutionPlatform_Status CHECK (Status IN ('空闲', '使用中'))
);

CREATE INDEX IX_ExecutionPlatform_Island ON wt.ExecutionPlatform(IslandID);
CREATE INDEX IX_ExecutionPlatform_CurrentPosition ON wt.ExecutionPlatform(IslandID, CurrentPosition);
```

**字段说明**:
- `PlatformID`: 主键，自增ID
- `IslandID`: 所属岛屿ID
- `PlatformNumber`: 处刑台编号（1-49）
- `HomePosition`: 原位位置（1-49），处刑台的固定归属位置
- `CurrentPosition`: 当前位置（1-50），50表示在审判庭
- `ToolName`: 刑具名称（可为空）
- `ToolType`: 刑具类型（可为空）
- `ToolDescription`: 刑具描述（可为空）
- `Status`: 状态（空闲/使用中）
- `CreatedAt`: 创建时间
- `UpdatedAt`: 更新时间

#### 2. PlatformMovementLog（处刑台移动记录表）

```sql
CREATE TABLE wt.PlatformMovementLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    IslandID INT NOT NULL,
    PlatformID INT NOT NULL,
    PlatformNumber INT NOT NULL,           -- 处刑台编号
    FromPosition INT NOT NULL,             -- 起始位置
    ToPosition INT NOT NULL,               -- 目标位置
    ToolName NVARCHAR(100) NULL,          -- 移动时的刑具名称
    MovementTime DATETIME2 NOT NULL,      -- 移动时间（北京时间，可手动输入）
    IsManualTime BIT NOT NULL DEFAULT 0,  -- 是否手动输入时间
    MovementType NVARCHAR(20) NOT NULL,   -- 移动类型：升起/返回
    
    CONSTRAINT FK_PlatformMovementLog_Island FOREIGN KEY (IslandID) 
        REFERENCES wt.Island(IslandID),
    CONSTRAINT FK_PlatformMovementLog_Platform FOREIGN KEY (PlatformID) 
        REFERENCES wt.ExecutionPlatform(PlatformID),
    CONSTRAINT CK_PlatformMovementLog_Position CHECK (
        FromPosition BETWEEN 1 AND 50 AND ToPosition BETWEEN 1 AND 50
    ),
    CONSTRAINT CK_PlatformMovementLog_Type CHECK (MovementType IN ('升起', '返回'))
);

CREATE INDEX IX_PlatformMovementLog_Island ON wt.PlatformMovementLog(IslandID);
CREATE INDEX IX_PlatformMovementLog_Platform ON wt.PlatformMovementLog(PlatformID);
CREATE INDEX IX_PlatformMovementLog_Time ON wt.PlatformMovementLog(MovementTime DESC);
```

**字段说明**:
- `LogID`: 主键，自增ID
- `IslandID`: 所属岛屿ID
- `PlatformID`: 处刑台ID
- `PlatformNumber`: 处刑台编号（冗余字段，便于查询）
- `FromPosition`: 起始位置
- `ToPosition`: 目标位置
- `ToolName`: 移动时的刑具名称（冗余字段）
- `MovementTime`: 移动时间（北京时间，可手动输入精确到秒）
- `IsManualTime`: 是否手动输入时间（true=手动输入，false=系统当前时间）
- `MovementType`: 移动类型（升起/返回）
- **注意**: 不记录操作人信息

### C# 数据模型

#### ExecutionPlatformModel

```csharp
namespace WitchTrialSystem.Models
{
    public class ExecutionPlatformModel
    {
        public int PlatformID { get; set; }
        public int IslandID { get; set; }
        public int PlatformNumber { get; set; }
        public int HomePosition { get; set; }
        public int CurrentPosition { get; set; }
        public string? ToolName { get; set; }
        public string? ToolType { get; set; }
        public string? ToolDescription { get; set; }
        public string Status { get; set; } = "空闲";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // 计算属性
        public bool IsInTrialHall => CurrentPosition == 50;
        public bool IsAtHome => CurrentPosition == HomePosition;
        public bool HasTool => !string.IsNullOrEmpty(ToolName);
        public string LocationDescription => CurrentPosition == 50 ? "审判庭" : $"地下室-{CurrentPosition}号位";
    }
}
```

#### PlatformMovementLogModel

```csharp
namespace WitchTrialSystem.Models
{
    public class PlatformMovementLogModel
    {
        public int LogID { get; set; }
        public int IslandID { get; set; }
        public int PlatformID { get; set; }
        public int PlatformNumber { get; set; }
        public int FromPosition { get; set; }
        public int ToPosition { get; set; }
        public string? ToolName { get; set; }
        public DateTime MovementTime { get; set; }
        public bool IsManualTime { get; set; }
        public string MovementType { get; set; } = "";
        
        // 计算属性
        public string FromLocationDescription => FromPosition == 50 ? "审判庭" : $"地下室-{FromPosition}号位";
        public string ToLocationDescription => ToPosition == 50 ? "审判庭" : $"地下室-{ToPosition}号位";
        public string MovementDescription => $"{PlatformNumber}号处刑台从{FromLocationDescription}移动到{ToLocationDescription}";
        public string TimeSourceDescription => IsManualTime ? "手动输入" : "系统记录";
    }
}
```

## 组件和接口

### BLL层接口设计

#### IExecutionPlatformService

```csharp
public interface IExecutionPlatformService
{
    // 查询操作
    List<ExecutionPlatformModel> GetPlatformsByIsland(int islandID);
    ExecutionPlatformModel? GetPlatformByID(int platformID);
    ExecutionPlatformModel? GetPlatformAtPosition(int islandID, int position);
    bool IsPositionOccupied(int islandID, int position);
    bool IsTrialHallOccupied(int islandID);
    
    // 移动操作
    (bool Success, string Message) MoveToTrialHall(int platformID, int userIslandID, DateTime? customTime = null);
    (bool Success, string Message) ReturnToHome(int platformID, int userIslandID, DateTime? customTime = null);
    
    // 刑具管理
    (bool Success, string Message) AddTool(int platformID, string toolName, string toolType, string? description, int userIslandID);
    (bool Success, string Message) UpdateTool(int platformID, string toolName, string toolType, string? description, int userIslandID);
    (bool Success, string Message) RemoveTool(int platformID, int userIslandID);
    
    // 初始化
    void InitializePlatforms(int islandID);
}
```

#### IMovementLogService

```csharp
public interface IMovementLogService
{
    // 查询操作
    List<PlatformMovementLogModel> GetLogsByIsland(int islandID);
    List<PlatformMovementLogModel> GetLogsByPlatform(int platformID);
    List<PlatformMovementLogModel> GetLogsByTimeRange(int islandID, DateTime startTime, DateTime endTime);
    List<PlatformMovementLogModel> GetLogsByPosition(int islandID, int position);
    
    // 记录操作
    void LogMovement(int islandID, int platformID, int platformNumber, int fromPosition, int toPosition, string? toolName, string movementType, DateTime? customTime = null);
}
```

### DAL层接口设计

#### IExecutionPlatformDAL

```csharp
public interface IExecutionPlatformDAL
{
    // CRUD操作
    List<ExecutionPlatformModel> GetByIsland(int islandID);
    ExecutionPlatformModel? GetByID(int platformID);
    ExecutionPlatformModel? GetByPosition(int islandID, int position);
    int Insert(ExecutionPlatformModel platform);
    int Update(ExecutionPlatformModel platform);
    int Delete(int platformID);
    
    // 批量操作
    int InsertBatch(List<ExecutionPlatformModel> platforms);
    
    // 位置查询
    bool IsPositionOccupied(int islandID, int position);
    int GetPlatformCountAtPosition(int islandID, int position);
}
```

#### IMovementLogDAL

```csharp
public interface IMovementLogDAL
{
    // 查询操作
    List<PlatformMovementLogModel> GetByIsland(int islandID);
    List<PlatformMovementLogModel> GetByPlatform(int platformID);
    List<PlatformMovementLogModel> GetByTimeRange(int islandID, DateTime startTime, DateTime endTime);
    List<PlatformMovementLogModel> GetByPosition(int islandID, int position);
    
    // 插入操作
    int Insert(PlatformMovementLogModel log);
}
```

## 正确性属性

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: 位置查询一致性
*For any* 处刑台，查询其位置应该返回其当前存储的位置值
**Validates: Requirements 1.4**

### Property 2: 位置占用状态正确性
*For any* 位置，查询其占用状态应该与实际是否有处刑台在该位置一致
**Validates: Requirements 1.5**

### Property 3: 审判庭移动前置检查
*For any* 处刑台移动到审判庭的请求，系统应该先检查审判庭是否为空
**Validates: Requirements 2.1**

### Property 4: 审判庭为空时允许移动
*For any* 处刑台，当审判庭为空时，移动到审判庭的请求应该成功
**Validates: Requirements 2.2**

### Property 5: 审判庭被占用时拒绝移动
*For any* 处刑台，当审判庭已被占用时，移动到审判庭的请求应该失败并返回错误消息
**Validates: Requirements 2.3**

### Property 6: 移动到审判庭后位置更新
*For any* 处刑台，成功移动到审判庭后，其当前位置应该是50
**Validates: Requirements 2.4**

### Property 7: 移动到审判庭记录日志
*For any* 处刑台，成功移动到审判庭后，应该在移动日志表中有新的记录
**Validates: Requirements 2.5**

### Property 8: 返回原位使用HomePosition
*For any* 处刑台返回原位的操作，目标位置应该等于其HomePosition字段
**Validates: Requirements 3.1**

### Property 9: 原位为空时允许返回
*For any* 处刑台，当其原位为空时，返回原位的请求应该成功
**Validates: Requirements 3.2**

### Property 10: 原位被占用时拒绝返回
*For any* 处刑台，当其原位已被占用时，返回原位的请求应该失败并返回错误消息
**Validates: Requirements 3.3**

### Property 11: 返回原位后位置更新
*For any* 处刑台，成功返回原位后，其当前位置应该等于HomePosition
**Validates: Requirements 3.4**

### Property 12: 返回原位记录日志
*For any* 处刑台，成功返回原位后，应该在移动日志表中有新的记录
**Validates: Requirements 3.5**

### Property 13: 添加刑具后信息存储
*For any* 处刑台，添加刑具后，其刑具名称和类型字段应该被正确设置
**Validates: Requirements 4.1, 4.2**

### Property 14: 更换刑具替换原信息
*For any* 处刑台，更换刑具后，新刑具信息应该替换旧刑具信息
**Validates: Requirements 4.3**

### Property 15: 移除刑具清空信息
*For any* 处刑台，移除刑具后，其刑具信息字段应该为null
**Validates: Requirements 4.4**

### Property 16: 移动时刑具绑定不变
*For any* 处刑台，移动前后其刑具信息应该保持一致
**Validates: Requirements 4.5**

### Property 17: 移动记录包含必要字段
*For any* 移动记录，应该包含时间、处刑台编号、刑具名称、起始位置和目标位置
**Validates: Requirements 5.2, 5.3, 5.4, 5.5**

### Property 18: 移动记录不包含操作人
*For any* 移动记录，不应该包含操作人信息字段
**Validates: Requirements 5.6**

### Property 19: 时间范围筛选正确性
*For any* 时间范围筛选，返回的记录的移动时间应该都在指定范围内
**Validates: Requirements 5.7**

### Property 20: 处刑台编号筛选正确性
*For any* 处刑台编号筛选，返回的记录应该都属于指定的处刑台
**Validates: Requirements 5.8**

### Property 21: 位置筛选正确性
*For any* 位置筛选，返回的记录的起始或目标位置应该匹配指定位置
**Validates: Requirements 5.9**

### Property 22: 岛屿数据隔离
*For any* 用户查询处刑台列表，返回的处刑台应该都属于用户所属的岛屿
**Validates: Requirements 7.1**

### Property 23: 跨岛屿操作拒绝
*For any* 用户尝试操作其他岛屿的处刑台，操作应该被拒绝
**Validates: Requirements 7.2**

### Property 24: 移动记录岛屿隔离
*For any* 用户查询移动记录，返回的记录应该都属于用户所属的岛屿
**Validates: Requirements 7.5**

### Property 25: 位置变化立即持久化
*For any* 处刑台位置变化，数据库中的记录应该立即更新
**Validates: Requirements 9.1**

### Property 26: 刑具变化立即持久化
*For any* 刑具信息变化，数据库中的记录应该立即更新
**Validates: Requirements 9.2**

### Property 27: 移动日志立即记录
*For any* 处刑台移动，移动日志应该立即写入数据库
**Validates: Requirements 9.3**

### Property 28: 系统重启状态恢复
*For any* 系统重启，处刑台的状态应该与重启前一致
**Validates: Requirements 9.4**

### Property 29: 历史记录持久保留
*For any* 系统重启，所有历史移动记录应该保持完整
**Validates: Requirements 9.5**

## 错误处理

### 错误类型定义

```csharp
public enum PlatformErrorCode
{
    Success = 0,
    PlatformNotFound = 1001,
    PositionOccupied = 1002,
    TrialHallOccupied = 1003,
    HomePositionOccupied = 1004,
    InvalidPosition = 1005,
    PermissionDenied = 1006,
    IslandMismatch = 1007,
    DatabaseError = 1008,
    UnexpectedError = 1009
}
```

### 错误处理策略

1. **业务逻辑错误**: 返回 `(false, errorMessage)` 元组
2. **数据库错误**: 捕获 `SqlException`，记录日志，返回友好错误消息
3. **权限错误**: 在BLL层检查，拒绝无权限操作
4. **未预期错误**: 捕获所有异常，记录详细日志，返回通用错误消息

## 测试策略

### 单元测试

**测试范围**:
- DAL层：数据库CRUD操作
- BLL层：业务逻辑验证
- 权限检查逻辑
- 错误处理逻辑

**测试工具**: xUnit + Moq

### 属性测试

**测试框架**: FsCheck 或 CsCheck

**测试策略**:
- 为每个正确性属性编写对应的属性测试
- 使用随机生成的测试数据
- 每个属性测试运行至少100次迭代

**示例属性测试**:

```csharp
[Property]
public Property MoveToTrialHall_WhenTrialHallEmpty_ShouldSucceed()
{
    return Prop.ForAll(
        Arb.Generate<int>().Where(id => id > 0),
        platformID =>
        {
            // Arrange: 确保审判庭为空
            var service = new ExecutionPlatformService();
            
            // Act: 移动到审判庭
            var (success, _) = service.MoveToTrialHall(platformID, islandID);
            
            // Assert: 应该成功
            return success;
        }
    );
}
```

### 集成测试

**测试范围**:
- UI → BLL → DAL 完整流程
- 数据库事务完整性
- 并发操作处理

### 边缘案例测试

**测试场景**:
- 不存在的处刑台ID
- 已占用的位置
- 数据库连接失败
- 权限不足
- 并发移动同一处刑台

## 性能考虑

### 数据库优化

1. **索引策略**:
   - `IslandID` 上的索引（频繁按岛屿查询）
   - `CurrentPosition` 上的复合索引（位置占用查询）
   - `MovementTime` 上的降序索引（时间范围查询）

2. **查询优化**:
   - 使用参数化查询防止SQL注入
   - 避免N+1查询问题
   - 合理使用JOIN减少数据库往返

### 并发控制

1. **乐观锁**: 使用 `UpdatedAt` 字段检测并发修改
2. **事务隔离**: 使用 `READ COMMITTED` 隔离级别
3. **位置锁**: 移动操作使用数据库行锁防止冲突

## 安全考虑

### 权限验证

1. **岛屿隔离**: 所有操作验证用户岛屿ID
2. **角色检查**: 刑具管理仅限Regulator和Admin
3. **双重验证**: UI层和BLL层都进行权限检查

### 数据验证

1. **输入验证**: 验证位置范围（1-50）
2. **状态验证**: 验证处刑台状态转换合法性
3. **外键验证**: 确保IslandID和PlatformID有效

### 审计日志

1. **移动记录**: 自动记录所有移动操作
2. **不记录操作人**: 按需求不记录操作人信息
3. **时间戳**: 使用北京时间（UTC+8）

## 部署说明

### 数据库迁移

1. 执行 `create_execution_platform_tables.sql` 创建表结构
2. 执行 `initialize_execution_platforms.sql` 初始化每个岛屿的处刑台数据
3. 验证数据完整性

### 配置项

```json
{
  "ExecutionPlatform": {
    "MaxPlatforms": 49,
    "MaxPositions": 50,
    "TrialHallPosition": 50,
    "EnableConcurrencyCheck": true,
    "LogRetentionDays": 365
  }
}
```

## 未来扩展

### 可能的功能增强

1. **处刑预约系统**: 预约处刑台使用时间
2. **刑具维护记录**: 记录刑具维护历史
3. **处刑台状态监控**: 实时监控处刑台状态
4. **统计报表**: 处刑台使用频率统计
5. **3D可视化**: 地下室和审判庭的3D布局展示

### 技术债务

1. 考虑使用消息队列处理移动日志记录（异步化）
2. 考虑使用缓存减少数据库查询
3. 考虑使用分布式锁处理高并发场景

---

**设计文档版本**: 1.0  
**最后更新**: 2024年12月6日
