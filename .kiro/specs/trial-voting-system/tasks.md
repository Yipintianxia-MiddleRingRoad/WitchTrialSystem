# 处刑投票流程系统 - 实现计划

## 任务清单

### 阶段 1: 数据库设计和实现

- [ ] 1.1 创建 TrialSession 表
  - 编写 SQL 脚本创建审判会话表
  - 包含所有字段：SessionID, IslandID, BatchID, Status, CreatedBy, CreatedAt, VotingStartTime, VotingEndTime, ExecutionTargetWitchID, ExecutionConfirmedAt, CompletedAt
  - 添加外键约束和检查约束
  - 创建索引：IslandID, (IslandID, Status), CreatedAt DESC
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5_

- [ ] 1.2 创建 TrialParticipant 表
  - 编写 SQL 脚本创建参与者表
  - 包含所有字段：ParticipantID, SessionID, WitchID, UserID, HasVoted, VotedForWitchID, VotedAt, HasConfirmedExecution, ExecutionConfirmedAt
  - 添加外键约束和唯一约束
  - 创建索引：SessionID, UserID, WitchID
  - _Requirements: 5.9, 5.12, 8.11_

- [ ] 1.3 创建 TrialNotification 表
  - 编写 SQL 脚本创建通知表
  - 包含所有字段：NotificationID, SessionID, UserID, Message, IsRead, CreatedAt
  - 添加外键约束
  - 创建索引：(UserID, IsRead), SessionID
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

- [ ] 1.4 测试数据库表创建
  - 在测试环境执行表创建脚本
  - 验证表结构和约束
  - 测试插入和查询操作
  - _Requirements: All_

### 阶段 2: 数据模型类实现

- [ ] 2.1 创建 TrialSessionModel
  - 定义所有属性字段
  - 添加计算属性：IsPending, IsVoting, IsConfirmed, IsExecuting, IsCompleted, IsActive
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5_

- [ ] 2.2 创建 TrialParticipantModel
  - 定义所有属性字段
  - 添加扩展属性：WitchName, Username, AvatarPath, VotedForWitchName
  - _Requirements: 5.1, 5.9, 8.11_

- [ ] 2.3 创建 TrialNotificationModel
  - 定义所有属性字段
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

- [ ] 2.4 创建 TrialState 枚举
  - 定义所有状态：Idle, NotParticipating, WaitingToStart, Voting, WaitingForOthersToVote, WaitingForExecutionAnnouncement, ConfirmingExecution, WaitingForOthersToConfirm, Completed
  - _Requirements: 10.1, 10.2, 10.3, 10.4, 10.5, 10.6, 10.7, 10.8, 10.9_


### 阶段 3: DAL 层实现

- [ ] 3.1 实现 TrialSessionDAL 基础查询
  - 实现 GetByID() - 按ID查询审判会话
  - 实现 GetActiveByIsland() - 查询岛屿的进行中审判
  - 实现 GetByIsland() - 查询岛屿的历史审判
  - _Requirements: 1.2, 1.3, 14.3_

- [ ] 3.2 实现 TrialSessionDAL 修改操作
  - 实现 Insert() - 插入新审判会话
  - 实现 Update() - 更新审判会话
  - 实现 UpdateStatus() - 更新状态
  - 实现 UpdateExecutionTarget() - 更新处刑对象
  - _Requirements: 2.3, 4.1, 6.8, 7.1, 9.2_

- [ ] 3.3 实现 TrialParticipantDAL 基础查询
  - 实现 GetBySession() - 查询会话的所有参与者
  - 实现 GetBySessionAndUser() - 查询特定用户的参与记录
  - 实现 GetBySessionAndWitch() - 查询特定魔女的参与记录
  - _Requirements: 5.1, 10.2_

- [ ] 3.4 实现 TrialParticipantDAL 修改操作
  - 实现 Insert() - 插入参与者记录
  - 实现 InsertBatch() - 批量插入参与者
  - 实现 Update() - 更新参与者记录
  - 实现 UpdateVote() - 更新投票
  - 实现 UpdateExecutionConfirmation() - 更新确认状态
  - _Requirements: 2.4, 5.9, 8.11_

- [ ] 3.5 实现 TrialParticipantDAL 统计操作
  - 实现 GetVotedCount() - 获取已投票人数
  - 实现 GetConfirmedCount() - 获取已确认人数
  - 实现 GetVoteStatistics() - 获取投票统计（每个魔女的得票数）
  - _Requirements: 4.4, 6.3, 7.4_

- [ ] 3.6 实现 TrialNotificationDAL
  - 实现 Insert() - 插入通知
  - 实现 InsertBatch() - 批量插入通知
  - 实现 GetByUser() - 查询用户的通知
  - 实现 GetBySession() - 查询会话的通知
  - 实现 MarkAsRead() - 标记为已读
  - 实现 MarkAllAsRead() - 标记所有为已读
  - _Requirements: 3.1, 3.2, 3.5_

- [ ] 3.7 编写 DAL 层单元测试
  - 测试所有查询方法
  - 测试所有修改方法
  - 测试边界条件和错误情况
  - _Requirements: All_

### 阶段 4: BLL 层实现

- [ ] 4.1 实现 TrialSessionService 查询方法
  - 实现 GetActiveSession() - 获取进行中的审判
  - 实现 GetSessionByID() - 获取审判会话
  - 实现 GetSessionHistory() - 获取历史审判
  - 实现 HasActiveSession() - 检查是否有进行中的审判
  - 添加岛屿权限验证
  - _Requirements: 1.2, 1.3, 1.5, 14.1, 14.3_

- [ ] 4.2 实现 TrialSessionService 典狱长操作 - 发起审判
  - 实现 CreateSession() - 创建审判会话
    - 验证用户是典狱长
    - 验证岛屿没有进行中的审判
    - 验证参与人数（2-13人）
    - 验证参与魔女属于本岛屿本批次
    - 创建审判会话（Status = 'Pending'）
    - 批量创建参与者记录
    - 批量创建通知记录
    - 返回 SessionID
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 2.7_

- [ ] 4.3 实现 TrialSessionService 典狱长操作 - 开始投票
  - 实现 StartVoting() - 开始投票
    - 验证用户是典狱长
    - 验证审判状态为 'Pending'
    - 更新状态为 'Voting'
    - 记录 VotingStartTime
  - _Requirements: 4.1, 4.2, 4.3_

- [ ] 4.4 实现 TrialSessionService 典狱长操作 - 确认处刑对象
  - 实现 ConfirmExecutionTarget() - 确认处刑对象
    - 验证用户是典狱长
    - 验证审判状态为 'Voting'
    - 验证所有人已投票
    - 更新状态为 'Confirmed'
    - 更新 ExecutionTargetWitchID
    - 记录 VotingEndTime
  - _Requirements: 6.1, 6.2, 6.7, 6.8_

- [ ] 4.5 实现 TrialSessionService 典狱长操作 - 宣布处刑对象
  - 实现 AnnounceExecutionTarget() - 宣布处刑对象
    - 验证用户是典狱长
    - 验证审判状态为 'Confirmed'
    - 更新状态为 'Executing'
    - 记录 ExecutionConfirmedAt
  - _Requirements: 7.1, 7.2, 7.3_

- [ ] 4.6 实现 TrialSessionService 典狱长操作 - 完成处刑
  - 实现 CompleteExecution() - 完成处刑
    - 验证用户是典狱长
    - 验证审判状态为 'Executing'
    - 验证所有人已确认处刑
    - 更新状态为 'Completed'
    - 记录 CompletedAt
    - 更新魔女状态为"已处刑"
    - 记录操作日志
  - _Requirements: 9.1, 9.2, 9.3, 9.4, 9.5, 9.6, 9.7_

- [ ] 4.7 实现 TrialSessionService 状态检测
  - 实现 GetCurrentState() - 获取当前状态
    - 查询进行中的审判
    - 检查用户是否参与
    - 根据会话状态和参与者状态返回 TrialState
  - _Requirements: 10.1, 10.2, 10.3, 10.4, 10.5, 10.6, 10.7, 10.8, 10.9_

- [ ] 4.8 实现 TrialSessionService 统计信息
  - 实现 GetVotingStatistics() - 获取投票统计
  - 实现 GetVotingDetails() - 获取投票详情
  - 实现 GetVotingProgress() - 获取投票进度
  - 实现 GetConfirmationProgress() - 获取确认进度
  - _Requirements: 4.4, 4.5, 6.3, 6.4, 7.4, 7.5_

- [ ] 4.9 实现 TrialVotingService 投票操作
  - 实现 SubmitVote() - 提交投票
    - 验证用户是参与魔女
    - 验证审判状态为 'Voting'
    - 验证未投票
    - 验证投票对象是参与魔女
    - 更新 HasVoted = 1
    - 记录 VotedForWitchID 和 VotedAt
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7, 5.8, 5.9, 5.12_

- [ ] 4.10 实现 TrialVotingService 确认处刑操作
  - 实现 ConfirmExecution() - 确认处刑
    - 验证用户是参与魔女
    - 验证审判状态为 'Executing'
    - 验证未确认
    - 更新 HasConfirmedExecution = 1
    - 记录 ExecutionConfirmedAt
  - _Requirements: 8.5, 8.6, 8.7, 8.8, 8.9, 8.10, 8.11_

- [ ] 4.11 实现 TrialVotingService 查询和验证
  - 实现 GetParticipants() - 获取参与者列表
  - 实现 GetParticipant() - 获取参与者记录
  - 实现 HasVoted() - 检查是否已投票
  - 实现 HasConfirmedExecution() - 检查是否已确认
  - 实现 CanVote() - 检查是否可以投票
  - 实现 CanConfirmExecution() - 检查是否可以确认
  - _Requirements: 5.1, 5.12, 8.11, 10.4, 10.5, 10.7, 10.8_

- [ ] 4.12 实现 TrialNotificationService
  - 实现 CreateNotifications() - 创建通知
  - 实现 GetUnreadNotifications() - 获取未读通知
  - 实现 MarkAsRead() - 标记为已读
  - 实现 MarkAllAsRead() - 标记所有为已读
  - 实现 GetNotificationsBySession() - 获取会话通知
  - 实现 GetUnreadCount() - 获取未读数量
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

- [ ] 4.13 编写 BLL 层单元测试
  - 测试所有业务逻辑方法
  - 测试权限验证逻辑
  - 测试错误处理逻辑
  - _Requirements: All_


### 阶段 5: UI 层实现 - 典狱长界面 ✅

- [x] 5.1 创建 TrialManagementForm 主界面
  - 创建 TrialManagementForm.cs
  - 设计界面布局：当前审判状态 + 操作按钮区域
  - 添加状态显示标签
  - 添加定时器（每2秒刷新状态）
  - _Requirements: 17.1, 17.2, 12.1, 12.2_

- [x] 5.2 实现发起审判对话框
  - 创建 CreateTrialDialog.cs
  - 显示本岛屿本批次的魔女列表（CheckedListBox）
  - 验证选择人数（2-13人）
  - 调用 BLL 层 CreateSession 方法
  - 显示成功或失败消息
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 2.7_

- [x] 5.3 实现投票进度显示
  - 显示参与魔女列表（DataGridView）
  - 列：头像、姓名、投票状态（已投票/未投票）
  - 实时更新投票进度（如：5/10 已投票）
  - 所有人投票完成后显示"查看投票结果"按钮
  - _Requirements: 4.3, 4.4, 4.5, 12.5_

- [x] 5.4 实现投票结果显示对话框
  - 创建 VotingResultDialog.cs
  - 显示投票统计（柱状图或列表）
  - 显示投票详情（谁投给了谁）
  - 如果有多个最高得票者，弹出选择对话框
  - 确认处刑对象后调用 BLL 层 ConfirmExecutionTarget 方法
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 6.7, 6.8, 6.9_

- [x] 5.5 实现处刑确认进度显示
  - 显示参与魔女列表（DataGridView）
  - 列：头像、姓名、确认状态（已确认/未确认）
  - 实时更新确认进度（如：5/10 已确认）
  - 所有人确认后显示"开始处刑"按钮
  - _Requirements: 7.3, 7.4, 7.5, 12.6_

- [x] 5.6 实现完成处刑确认对话框
  - 弹出确认对话框："确定要处刑 [魔女名字] 吗？"
  - 确认后调用 BLL 层 CompleteExecution 方法
  - 显示"审判完成"消息
  - _Requirements: 9.1, 9.2, 9.3, 9.4, 9.5, 9.6, 9.7_

- [x] 5.7 实现权限控制
  - 验证用户是典狱长
  - 只显示本岛屿的审判
  - _Requirements: 13.1, 13.2, 14.1_

- [x] 5.8 实现状态刷新机制
  - 使用 Timer 每2秒检查状态
  - 状态变化时自动刷新界面
  - _Requirements: 12.1, 12.2_

### 阶段 6: UI 层实现 - 魔女界面 ✅

- [x] 6.1 修改 PhoneForm 处刑按钮逻辑
  - 检查当前审判状态
  - 根据状态跳转到不同界面
  - 如果无审判，显示"当前无审判进行中"
  - 如果有审判，根据状态跳转
  - _Requirements: 10.1, 10.2, 10.3, 10.4, 10.5, 10.6, 10.7, 10.8, 10.9_

- [x] 6.2 实现通知弹窗
  - 创建 NotificationPopupForm.cs
  - 显示典狱长头像和通知文字
  - 5秒后自动关闭
  - 关闭后标记为已读
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

- [x] 6.3 创建投票界面
  - 创建 TrialVotingForm.cs
  - 手机风格（469x777）
  - 使用 FlowLayoutPanel 显示参与魔女（三个一行）
  - 每个魔女显示：头像、姓名、单选按钮（RadioButton）
  - 添加"确认投票"按钮
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 17.3, 17.4_

- [x] 6.4 实现投票提交逻辑
  - 验证必须选择一个魔女
  - 调用 BLL 层 SubmitVote 方法
  - 提交成功后切换到"等待其他人投票"界面
  - 禁用窗口关闭按钮
  - _Requirements: 5.7, 5.8, 5.9, 5.10, 5.11, 11.1, 11.2_

- [x] 6.5 实现等待投票界面
  - 显示"等待其他人投票"消息
  - 显示投票进度（如：5/10 已投票）
  - 使用 Timer 每2秒检查状态
  - 所有人投票完成后自动切换到等待宣布界面
  - _Requirements: 5.10, 5.13, 12.3, 12.4_

- [x] 6.6 创建处刑对象确认界面
  - 创建 TrialExecutionConfirmForm.cs
  - 手机风格（469x777）
  - 显示处刑对象头像和姓名
  - 播放滑稽音效
  - 添加"确认处刑"按钮
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 17.5, 18.1, 18.2, 18.3_

- [x] 6.7 实现确认处刑逻辑
  - 点击"确认处刑"按钮后跳转到 ExecutionForm
  - 传递审判会话信息
  - _Requirements: 8.5, 8.6, 8.7_

- [x] 6.8 修改 ExecutionForm 支持审判流程
  - 添加构造函数参数：sessionID, witchID
  - 点击处刑按钮后调用 BLL 层 ConfirmExecution 方法
  - 更新参与者的 HasConfirmedExecution 标志
  - 禁用窗口关闭按钮（未点击处刑按钮时）
  - _Requirements: 8.8, 8.9, 8.10, 8.11, 11.3, 11.4_

- [x] 6.9 实现等待确认界面
  - 显示"等待其他人确认"消息
  - 显示确认进度（如：5/10 已确认）
  - 使用 Timer 每2秒检查状态
  - 所有人确认后显示"审判完成"消息
  - _Requirements: 8.13, 12.3, 12.4_

- [x] 6.10 实现防止退出机制
  - 在投票界面未投票时禁用关闭按钮
  - 在处刑按钮界面未点击时禁用关闭按钮
  - 点击关闭按钮显示提示消息
  - _Requirements: 11.1, 11.2, 11.3, 11.4, 11.5, 11.6_

- [x] 6.11 实现状态恢复机制
  - 登录时检查当前审判状态
  - 根据状态自动跳转到对应界面
  - 强制退出后再进入恢复到正确状态
  - _Requirements: 10.1, 10.2, 10.3, 10.4, 10.5, 10.6, 10.7, 10.8, 10.9, 10.10, 10.11_

### 阶段 7: 集成到主界面

- [ ] 7.1 在 Form1_Warden 中添加入口
  - 添加"审判管理"按钮
  - 点击打开 TrialManagementForm
  - _Requirements: 13.1_

- [ ] 7.2 在 PhoneForm 中集成审判流程
  - 修改处刑按钮点击逻辑
  - 登录时检查未读通知
  - 显示通知弹窗
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 13.3, 13.4_

- [ ] 7.3 在 Form1_Admin 中添加查看入口（可选）
  - 添加"审判监督"按钮（仅查看）
  - 可以选择岛屿查看审判状态
  - 所有操作按钮禁用
  - _Requirements: 13.5, 13.6_

### 阶段 8: 音效和资源

- [ ] 8.1 准备音效文件
  - 创建目录：Images/sounds/
  - 准备滑稽音效文件：execution_notice.wav
  - 确保文件格式为 WAV
  - _Requirements: 18.1, 18.2, 18.3_

- [ ] 8.2 实现音效播放功能
  - 使用 System.Media.SoundPlayer 播放音效
  - 捕获播放失败异常，不影响流程
  - 记录错误日志
  - _Requirements: 18.1, 18.2, 18.3_

### 阶段 9: 错误处理和日志

- [ ] 9.1 实现错误处理
  - 在 BLL 层捕获所有异常
  - 返回友好的错误消息
  - 记录详细错误日志到 OperationLog 表
  - _Requirements: 16.1, 16.2, 16.3, 16.4, 16.5_

- [ ] 9.2 实现边缘案例处理
  - 处理审判已存在的情况
  - 处理参与人数不足或过多
  - 处理数据库连接失败
  - 处理权限不足
  - 处理并发操作冲突
  - _Requirements: 16.1, 16.2, 16.3, 16.4, 16.5_

- [ ] 9.3 编写边缘案例测试
  - 测试所有错误处理场景
  - 验证错误消息正确性
  - 验证日志记录完整性
  - _Requirements: 16.1, 16.2, 16.3, 16.4, 16.5_

### 阶段 10: 数据库初始化和部署

- [ ] 10.1 执行数据库脚本
  - 在测试环境执行表创建脚本
  - 验证表结构和约束
  - 测试插入和查询操作
  - _Requirements: 15.1, 15.2, 15.3, 15.4, 15.5, 15.6_

- [ ] 10.2 更新数据库结构文档
  - 在 数据库结构文档.md 中添加新表说明
  - 更新 ER 图
  - _Requirements: All_

### 阶段 11: 最终测试和验收

- [ ] 11.1 功能测试 - 完整流程
  - 测试典狱长发起审判
  - 测试魔女接收通知
  - 测试魔女投票
  - 测试典狱长查看投票结果
  - 测试典狱长确认处刑对象
  - 测试魔女确认处刑
  - 测试典狱长完成处刑
  - _Requirements: All_

- [ ] 11.2 功能测试 - 状态恢复
  - 测试投票中途退出再进入
  - 测试确认处刑中途退出再进入
  - 测试强制退出后状态恢复
  - _Requirements: 10.1, 10.2, 10.3, 10.4, 10.5, 10.6, 10.7, 10.8, 10.9, 10.10, 10.11_

- [ ] 11.3 功能测试 - 防止退出
  - 测试投票界面未投票时不能退出
  - 测试处刑按钮界面未点击时不能退出
  - 测试完成后可以正常退出
  - _Requirements: 11.1, 11.2, 11.3, 11.4, 11.5, 11.6_

- [ ] 11.4 功能测试 - 权限控制
  - 测试典狱长权限
  - 测试魔女权限
  - 测试Admin查看权限
  - 测试岛屿隔离
  - _Requirements: 13.1, 13.2, 13.3, 13.4, 13.5, 13.6, 13.7, 14.1, 14.2, 14.3, 14.4, 14.5_

- [ ] 11.5 性能测试
  - 测试大量参与者（13人）的投票流程
  - 测试状态刷新性能
  - 验证索引效果
  - _Requirements: 15.1, 15.2, 15.3, 15.4, 15.5, 15.6_

- [ ] 11.6 用户验收测试
  - 邀请用户测试界面易用性
  - 收集反馈并优化
  - _Requirements: 17.1, 17.2, 17.3, 17.4, 17.5, 17.6, 17.7_

- [ ] 11.7 文档完善
  - 编写用户操作手册
  - 更新系统架构文档
  - 更新 CHANGELOG.md
  - _Requirements: All_

## 优先级说明

**高优先级（必须完成）**:
- 阶段 1: 数据库设计和实现
- 阶段 2: 数据模型类实现
- 阶段 3: DAL 层实现
- 阶段 4: BLL 层实现
- 阶段 5: UI 层实现 - 典狱长界面
- 阶段 6: UI 层实现 - 魔女界面
- 阶段 7: 集成到主界面

**中优先级（建议完成）**:
- 阶段 8: 音效和资源
- 阶段 9: 错误处理和日志
- 阶段 10: 数据库初始化和部署

**低优先级（可选）**:
- 阶段 11: 最终测试和验收（部分测试可以在开发过程中完成）

## 预估工作量

- 阶段 1-2: 1-2天
- 阶段 3: 2-3天
- 阶段 4: 3-4天
- 阶段 5: 2-3天
- 阶段 6: 3-4天
- 阶段 7: 1天
- 阶段 8-9: 1-2天
- 阶段 10-11: 2-3天

**总计**: 约 15-24天（根据开发经验和时间投入）

