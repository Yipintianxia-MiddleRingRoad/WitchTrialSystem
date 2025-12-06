# 处刑台管理系统 - 实现计划

## 任务清单

- [x] 1. 创建数据库表结构


  - 创建 ExecutionPlatform 表和 PlatformMovementLog 表
  - 添加必要的索引和约束
  - _Requirements: 1.1, 1.2, 1.3, 9.1, 9.2, 9.3_

- [x] 1.1 创建 ExecutionPlatform 表


  - 编写 SQL 脚本创建处刑台表
  - 包含所有字段：PlatformID, IslandID, PlatformNumber, HomePosition, CurrentPosition, ToolName, ToolType, ToolDescription, Status, CreatedAt, UpdatedAt
  - 添加外键约束、唯一约束和检查约束
  - 创建索引：IslandID, (IslandID, CurrentPosition)
  - _Requirements: 1.1, 1.2, 1.3_

- [x] 1.2 创建 PlatformMovementLog 表


  - 编写 SQL 脚本创建移动记录表
  - 包含所有字段：LogID, IslandID, PlatformID, PlatformNumber, FromPosition, ToPosition, ToolName, MovementTime, IsManualTime, MovementType
  - 添加外键约束和检查约束
  - 创建索引：IslandID, PlatformID, MovementTime DESC
  - _Requirements: 5.2, 5.3, 5.4, 5.5, 5.6, 9.3_

- [x] 1.3 创建初始化脚本


  - 编写 SQL 脚本为每个岛屿初始化49个处刑台
  - 处刑台编号1-49，HomePosition和CurrentPosition初始值相同
  - 状态初始为"空闲"，刑具信息为空
  - _Requirements: 1.1, 1.2, 1.3_

- [x] 2. 实现数据模型类



  - 创建 ExecutionPlatformModel 和 PlatformMovementLogModel
  - _Requirements: All_

- [x] 2.1 创建 ExecutionPlatformModel


  - 定义所有属性字段
  - 添加计算属性：IsInTrialHall, IsAtHome, HasTool, LocationDescription
  - _Requirements: 1.4, 1.5, 2.4, 3.4_

- [x] 2.2 创建 PlatformMovementLogModel

  - 定义所有属性字段
  - 添加计算属性：FromLocationDescription, ToLocationDescription, MovementDescription, TimeSourceDescription
  - _Requirements: 5.2, 5.3, 5.4, 5.5_

- [x] 3. 实现 DAL 层



  - 创建 ExecutionPlatformDAL 和 MovementLogDAL
  - _Requirements: All_

- [x] 3.1 实现 ExecutionPlatformDAL 基础查询


  - 实现 GetByIsland() - 按岛屿查询所有处刑台
  - 实现 GetByID() - 按ID查询单个处刑台
  - 实现 GetByPosition() - 按位置查询处刑台
  - 实现 IsPositionOccupied() - 检查位置是否被占用
  - _Requirements: 1.4, 1.5, 7.1_

- [x] 3.2 实现 ExecutionPlatformDAL 修改操作

  - 实现 Insert() - 插入新处刑台
  - 实现 Update() - 更新处刑台信息
  - 实现 Delete() - 删除处刑台
  - 实现 InsertBatch() - 批量插入处刑台
  - _Requirements: 2.4, 3.4, 4.2, 9.1, 9.2_

- [x] 3.3 实现 MovementLogDAL


  - 实现 GetByIsland() - 按岛屿查询移动记录
  - 实现 GetByPlatform() - 按处刑台查询移动记录
  - 实现 GetByTimeRange() - 按时间范围查询
  - 实现 GetByPosition() - 按位置查询
  - 实现 Insert() - 插入移动记录
  - _Requirements: 5.1, 5.7, 5.8, 5.9, 9.3_

- [ ]* 3.4 编写 DAL 层单元测试
  - 测试所有查询方法
  - 测试所有修改方法
  - 测试边界条件和错误情况
  - _Requirements: All_

- [x] 4. 实现 BLL 层



  - 创建 ExecutionPlatformService 和 MovementLogService
  - _Requirements: All_

- [x] 4.1 实现 ExecutionPlatformService 查询方法


  - 实现 GetPlatformsByIsland() - 获取岛屿的所有处刑台
  - 实现 GetPlatformByID() - 获取单个处刑台
  - 实现 GetPlatformAtPosition() - 获取指定位置的处刑台
  - 实现 IsPositionOccupied() - 检查位置占用
  - 实现 IsTrialHallOccupied() - 检查审判庭占用
  - 添加岛屿权限验证
  - _Requirements: 1.4, 1.5, 6.1, 6.3, 7.1, 7.2_

- [x] 4.2 实现 ExecutionPlatformService 移动方法

  - 实现 MoveToTrialHall() - 移动到审判庭
    - 验证用户岛屿权限
    - 检查审判庭是否为空
    - 更新处刑台位置为50
    - 更新状态为"使用中"
    - 调用 LogMovement 记录日志
    - 支持自定义时间参数
  - 实现 ReturnToHome() - 返回原位
    - 验证用户岛屿权限
    - 检查原位是否为空
    - 更新处刑台位置为HomePosition
    - 更新状态为"空闲"
    - 调用 LogMovement 记录日志
    - 支持自定义时间参数
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 3.1, 3.2, 3.3, 3.4, 3.5, 6.1, 6.3, 7.2_

- [x] 4.3 实现 ExecutionPlatformService 刑具管理方法

  - 实现 AddTool() - 添加刑具
    - 验证用户岛屿权限
    - 验证用户角色（仅Regulator和Admin）
    - 更新处刑台刑具信息
  - 实现 UpdateTool() - 更换刑具
    - 验证用户岛屿权限
    - 验证用户角色
    - 替换刑具信息
  - 实现 RemoveTool() - 移除刑具
    - 验证用户岛屿权限
    - 验证用户角色
    - 清空刑具信息
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 6.2, 6.4_

- [x] 4.4 实现 ExecutionPlatformService 初始化方法

  - 实现 InitializePlatforms() - 初始化岛屿处刑台
    - 为指定岛屿创建49个处刑台
    - 设置编号、原位和当前位置
    - 批量插入数据库
  - _Requirements: 1.1, 1.2, 1.3, 7.4_

- [x] 4.5 实现 MovementLogService


  - 实现 GetLogsByIsland() - 获取岛屿移动记录
  - 实现 GetLogsByPlatform() - 获取处刑台移动记录
  - 实现 GetLogsByTimeRange() - 按时间范围查询
  - 实现 GetLogsByPosition() - 按位置查询
  - 实现 LogMovement() - 记录移动日志
    - 支持自定义时间参数
    - 自动设置IsManualTime标志
  - 添加岛屿权限验证
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7, 5.8, 5.9, 7.5_

- [ ]* 4.6 编写 BLL 层单元测试
  - 测试所有业务逻辑方法
  - 测试权限验证逻辑
  - 测试错误处理逻辑
  - _Requirements: All_

- [ ]* 4.7 编写属性测试 - 移动到审判庭
  - **Property 4: 审判庭为空时允许移动**
  - **Validates: Requirements 2.2**

- [ ]* 4.8 编写属性测试 - 审判庭占用拒绝
  - **Property 5: 审判庭被占用时拒绝移动**
  - **Validates: Requirements 2.3**

- [ ]* 4.9 编写属性测试 - 移动后位置更新
  - **Property 6: 移动到审判庭后位置更新**
  - **Validates: Requirements 2.4**

- [ ]* 4.10 编写属性测试 - 移动记录日志
  - **Property 7: 移动到审判庭记录日志**
  - **Validates: Requirements 2.5**

- [ ]* 4.11 编写属性测试 - 返回原位
  - **Property 11: 返回原位后位置更新**
  - **Validates: Requirements 3.4**

- [ ]* 4.12 编写属性测试 - 刑具绑定不变
  - **Property 16: 移动时刑具绑定不变**
  - **Validates: Requirements 4.5**

- [ ]* 4.13 编写属性测试 - 岛屿数据隔离
  - **Property 22: 岛屿数据隔离**
  - **Validates: Requirements 7.1**

- [x] 5. 实现 UI 层 - 处刑台管理界面


  - 创建 ExecutionPlatformManagementForm
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.6, 8.7_

- [x] 5.1 创建处刑台管理主界面


  - 创建 ExecutionPlatformManagementForm.cs
  - 设计界面布局：地下室区域 + 审判庭区域
  - 添加岛屿选择下拉框（Admin可选择，其他角色自动）
  - 添加刷新按钮
  - _Requirements: 8.1, 8.2_

- [x] 5.2 实现地下室布局显示

  - 使用 FlowLayoutPanel 或 TableLayoutPanel 显示49个位置
  - 每个位置显示：位置编号、处刑台编号（如果有）、刑具信息（如果有）
  - 空位置显示为灰色，有处刑台的显示为绿色
  - 点击处刑台显示详细信息
  - _Requirements: 8.1, 8.3_

- [x] 5.3 实现审判庭状态显示

  - 单独区域显示位置50（审判庭）
  - 显示当前是否有处刑台
  - 如果有，显示处刑台编号和刑具信息
  - 高亮显示（红色边框）
  - _Requirements: 8.2, 8.3_

- [x] 5.4 实现处刑台操作菜单

  - 右键点击处刑台显示上下文菜单
  - 菜单项：
    - "移动到审判庭"（仅当处刑台在地下室且审判庭为空时可用）
    - "返回原位"（仅当处刑台在审判庭时可用）
    - "添加刑具"（仅Regulator和Admin，处刑台无刑具时）
    - "更换刑具"（仅Regulator和Admin，处刑台有刑具时）
    - "移除刑具"（仅Regulator和Admin，处刑台有刑具时）
    - "查看详情"
  - _Requirements: 8.4, 8.5, 6.1, 6.2, 6.3, 6.4_

- [x] 5.5 实现移动操作对话框

  - 创建移动确认对话框
  - 显示：处刑台编号、当前位置、目标位置、刑具信息
  - 添加时间选择选项：
    - 单选按钮："使用当前时间" / "自定义时间"
    - DateTimePicker 控件（精确到秒）
  - 确认按钮调用 BLL 层移动方法
  - 显示成功或失败消息
  - _Requirements: 2.1, 2.2, 2.3, 3.1, 3.2, 3.3, 8.6, 8.7_

- [x] 5.6 实现刑具管理对话框

  - 创建刑具管理对话框
  - 输入字段：刑具名称、刑具类型、刑具描述
  - 支持添加、更换、移除操作
  - 调用 BLL 层刑具管理方法
  - 显示成功或失败消息
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 8.6, 8.7_

- [x] 5.7 实现权限控制

  - 根据用户角色显示/隐藏刑具管理菜单项
  - Warden: 只能移动处刑台
  - Regulator: 可以移动处刑台 + 管理刑具（仅Regulator可管理刑具）
  - Admin: 只能查看（国家端仅监督，不可操作）+ 可选择岛屿
  - Witch: 不显示处刑台管理功能
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6_

- [x] 5.8 实现数据刷新

  - 操作成功后自动刷新界面
  - 刷新按钮手动刷新
  - 使用定时器定期刷新（可选）
  - _Requirements: 8.6, 8.7_

- [x] 6. 实现 UI 层 - 移动记录查看界面


  - 创建 MovementLogViewForm
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7, 5.8, 5.9_

- [x] 6.1 创建移动记录查看界面


  - 创建 MovementLogViewForm.cs
  - 使用 DataGridView 显示移动记录
  - 列：时间、处刑台编号、刑具、起始位置、目标位置、移动类型、时间来源
  - 按时间降序排列
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 5.6_

- [x] 6.2 实现筛选功能


  - 添加筛选面板
  - 时间范围筛选：开始时间 + 结束时间（DateTimePicker）
  - 处刑台编号筛选：下拉框（1-49 + "全部"）
  - 位置筛选：下拉框（1-50 + "全部"）
  - 应用筛选按钮
  - 重置筛选按钮
  - _Requirements: 5.7, 5.8, 5.9_

- [ ]* 6.3 实现导出功能（可选）
  - 添加导出按钮
  - 支持导出为 CSV 或 Excel
  - 导出当前筛选结果
  - _Requirements: 5.1_

- [x] 6.4 实现权限控制


  - 只显示用户所属岛屿的移动记录
  - Admin 可选择查看任意岛屿
  - _Requirements: 7.5_

- [x] 7. 集成到主界面


  - 在 Form1_Warden 和 Form1_Regulator 中添加入口
  - _Requirements: 6.1, 6.3_

- [x] 7.1 在 Form1_Warden 中添加入口


  - 添加"处刑台管理"按钮
  - 点击打开 ExecutionPlatformManagementForm
  - 添加"移动记录"按钮
  - 点击打开 MovementLogViewForm
  - _Requirements: 6.1_

- [x] 7.2 在 Form1_Regulator 中添加入口


  - 添加"处刑台管理"按钮
  - 点击打开 ExecutionPlatformManagementForm
  - 添加"移动记录"按钮
  - 点击打开 MovementLogViewForm
  - _Requirements: 6.3_

- [x] 7.3 在 Form1_Admin 中添加入口


  - 添加"处刑台管理"按钮（仅查看，不可操作）
  - 点击打开 ExecutionPlatformManagementForm（可选择岛屿，但所有操作按钮禁用）
  - 添加"移动记录"按钮
  - 点击打开 MovementLogViewForm（可选择岛屿）
  - _Requirements: 6.5_
  - **注意**：Admin作为国家端，只有查看权限，不能移动处刑台或管理刑具

- [ ] 8. 错误处理和日志
  - 实现完整的错误处理机制
  - _Requirements: 10.1, 10.2, 10.3, 10.4, 10.5_

- [ ] 8.1 实现错误处理
  - 在 BLL 层捕获所有异常
  - 返回友好的错误消息
  - 记录详细错误日志到 OperationLog 表
  - _Requirements: 10.1, 10.2, 10.3, 10.4, 10.5_

- [ ] 8.2 实现边缘案例处理
  - 处理不存在的处刑台ID
  - 处理已占用的位置
  - 处理数据库连接失败
  - 处理权限不足
  - 处理并发操作冲突
  - _Requirements: 10.1, 10.2, 10.3, 10.4_

- [ ]* 8.3 编写边缘案例测试
  - 测试所有错误处理场景
  - 验证错误消息正确性
  - 验证日志记录完整性
  - _Requirements: 10.1, 10.2, 10.3, 10.4, 10.5_

- [ ] 9. 数据库初始化和部署
  - 执行数据库脚本
  - _Requirements: 1.1, 1.2, 1.3, 7.4_

- [ ] 9.1 执行数据库脚本
  - 在测试环境执行表创建脚本
  - 执行初始化脚本为现有岛屿创建处刑台
  - 验证数据完整性
  - _Requirements: 1.1, 1.2, 1.3, 7.4_

- [ ] 9.2 更新数据库结构文档
  - 在 数据库结构文档.md 中添加新表说明
  - 更新 ER 图
  - _Requirements: All_

- [ ] 10. 最终测试和验收
  - 进行完整的功能测试
  - _Requirements: All_

- [ ] 10.1 功能测试
  - 测试所有移动操作
  - 测试所有刑具管理操作
  - 测试所有查询和筛选功能
  - 测试权限控制
  - 测试岛屿隔离
  - _Requirements: All_

- [ ] 10.2 性能测试
  - 测试大量移动记录的查询性能
  - 测试并发移动操作
  - 验证索引效果
  - _Requirements: 9.1, 9.2, 9.3_

- [ ] 10.3 用户验收测试
  - 邀请用户测试界面易用性
  - 收集反馈并优化
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.6, 8.7_

- [ ] 10.4 文档完善
  - 编写用户操作手册
  - 更新系统架构文档
  - 更新 CHANGELOG.md
  - _Requirements: All_
