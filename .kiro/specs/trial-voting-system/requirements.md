# 处刑投票流程系统 - 需求文档

## 简介

本文档定义了《魔女审判系统》中处刑投票流程的需求。该功能实现完整的魔女审判投票流程，包括典狱长发起审判、魔女投票、确认处刑对象、执行处刑等环节。由于系统为单机单用户模式，需要通过状态持久化确保用户退出再进入时保持正确状态。

## 术语表

- **System**: 魔女审判系统
- **TrialSession**: 审判会话，一次完整的审判流程
- **Warden**: 典狱长，负责发起审判、查看投票结果、确认处刑对象
- **Witch**: 魔女，参与投票、确认处刑
- **Meruru**: 监管者（岛屿监管者层），如 meruru_regulator, utena_regulator
- **Admin**: 管理员（国家层），如 admin
- **Participant**: 参与者，参加审判的魔女
- **VotingPhase**: 投票阶段，魔女进行投票
- **ExecutionPhase**: 处刑阶段，魔女确认处刑并点击处刑按钮
- **ExecutionTarget**: 处刑对象，被投票选出的魔女
- **StateRecovery**: 状态恢复，用户退出再进入时恢复到正确状态
- **PhoneForm**: 手机界面，魔女的主界面
- **ExecutionForm**: 处刑界面，点击处刑按钮的界面

## 四层权限体系

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

## 核心挑战

1. **单机单用户限制**: 数据库只能在一台电脑打开，不能多端同时登录
2. **状态持久化**: 用户退出再进入需要保持同一状态
3. **流程不可逆**: 投票后不能修改，必须完成整个流程
4. **防止强制退出**: 关键阶段不能退出，强制退出后需恢复状态
5. **角色隔离**: 典狱长和魔女看到完全不同的界面

## 需求

### 需求 1: 审判会话管理

**用户故事**: 作为系统管理员，我希望系统能够管理审判会话的完整生命周期，以便追踪审判的当前状态。

#### 验收标准

1. WHEN 系统初始化时 THEN System SHALL 支持每个岛屿独立的审判会话
2. WHEN 查询审判状态时 THEN System SHALL 返回当前审判会话的状态（Idle/Pending/Voting/Confirmed/Executing/Completed）
3. WHEN 一个岛屿有进行中的审判时 THEN System SHALL 不允许发起新的审判
4. WHEN 审判完成后 THEN System SHALL 将状态重置为 Idle
5. WHEN 查询审判会话时 THEN System SHALL 返回发起人、创建时间、参与魔女列表等信息

### 需求 2: 典狱长发起审判

**用户故事**: 作为典狱长，我希望能够发起一轮审判并选择参与的魔女，以便开始投票流程。

#### 验收标准

1. WHEN Warden发起审判时 THEN System SHALL 显示本岛屿本批次的所有魔女列表
2. WHEN Warden选择参与魔女时 THEN System SHALL 允许勾选多个魔女（至少2人，最多13人）
3. WHEN Warden确认发起审判时 THEN System SHALL 创建审判会话（Status = 'Pending'）
4. WHEN 审判会话创建后 THEN System SHALL 为每个参与魔女创建参与者记录
5. WHEN 审判会话创建后 THEN System SHALL 为每个参与魔女创建通知消息
6. WHEN 审判会话创建后 THEN System SHALL 显示"开始投票"按钮
7. WHEN 当前岛屿已有进行中的审判时 THEN System SHALL 拒绝发起新审判

### 需求 3: 魔女接收审判通知

**用户故事**: 作为魔女，我希望在审判发起后收到通知，以便知道需要参加审判。

#### 验收标准

1. WHEN Witch登录系统时 THEN System SHALL 检查是否有未读审判通知
2. WHEN 有未读通知时 THEN System SHALL 显示通知弹窗（5秒自动消失）
3. WHEN 显示通知时 THEN System SHALL 显示典狱长头像和通知文字
4. WHEN 显示通知时 THEN System SHALL 通知文字为"呀咧呀咧，又死人了，真实的，请速速前往审判庭"
5. WHEN 通知显示后 THEN System SHALL 将通知标记为已读
6. WHEN 有进行中的审判时 THEN System SHALL 使处刑按钮变为可点击状态

### 需求 4: 典狱长开始投票

**用户故事**: 作为典狱长，我希望能够开始投票阶段，以便魔女开始投票。

#### 验收标准

1. WHEN Warden点击"开始投票"按钮时 THEN System SHALL 更新审判会话状态为 'Voting'
2. WHEN 投票开始后 THEN System SHALL 记录投票开始时间
3. WHEN 投票开始后 THEN System SHALL 显示参与魔女列表和投票状态
4. WHEN 投票开始后 THEN System SHALL 实时显示已投票/未投票人数
5. WHEN 所有人投票完成后 THEN System SHALL 显示"查看投票结果"按钮

### 需求 5: 魔女投票

**用户故事**: 作为魔女，我希望能够投票选择处刑对象，以便参与审判流程。

#### 验收标准

1. WHEN Witch点击处刑按钮时 THEN System SHALL 检查当前审判状态
2. WHEN 审判状态为 'Pending' 时 THEN System SHALL 显示"等待投票开始"消息
3. WHEN 审判状态为 'Voting' 且未投票时 THEN System SHALL 显示投票界面
4. WHEN 显示投票界面时 THEN System SHALL 显示所有参与魔女的头像（三个一行）
5. WHEN 显示投票界面时 THEN System SHALL 每个头像下方有一个复选框
6. WHEN Witch选择投票对象时 THEN System SHALL 只允许选择一个魔女（单选）
7. WHEN Witch未选择任何人时 THEN System SHALL 不允许提交投票
8. WHEN Witch点击"确认投票"按钮时 THEN System SHALL 记录投票结果
9. WHEN 投票提交后 THEN System SHALL 更新参与者的 HasVoted 标志为 true
10. WHEN 投票提交后 THEN System SHALL 显示"等待其他人投票"界面
11. WHEN 投票提交后 THEN System SHALL 禁用窗口关闭按钮
12. WHEN Witch已投票时 THEN System SHALL 不允许修改投票
13. WHEN Witch强制退出后再进入时 THEN System SHALL 恢复到"等待其他人投票"状态

### 需求 6: 典狱长查看投票结果

**用户故事**: 作为典狱长，我希望能够查看投票结果并确认处刑对象，以便进入下一阶段。

#### 验收标准

1. WHEN 所有人投票完成后 THEN System SHALL 允许典狱长查看投票结果
2. WHEN 典狱长点击"查看投票结果"时 THEN System SHALL 显示投票统计
3. WHEN 显示投票统计时 THEN System SHALL 显示每个魔女的得票数（柱状图或列表）
4. WHEN 显示投票统计时 THEN System SHALL 显示每个魔女的投票详情（谁投给了谁）
5. WHEN 只有一个最高得票者时 THEN System SHALL 自动选择该魔女为处刑对象
6. WHEN 有多个最高得票者时 THEN System SHALL 弹出对话框让典狱长选择一个
7. WHEN 典狱长确认处刑对象后 THEN System SHALL 更新审判会话状态为 'Confirmed'
8. WHEN 典狱长确认处刑对象后 THEN System SHALL 记录处刑对象的 WitchID
9. WHEN 典狱长确认处刑对象后 THEN System SHALL 显示"宣布处刑对象"按钮

### 需求 7: 典狱长宣布处刑对象

**用户故事**: 作为典狱长，我希望能够宣布处刑对象，以便魔女进入确认处刑阶段。

#### 验收标准

1. WHEN 典狱长点击"宣布处刑对象"按钮时 THEN System SHALL 更新审判会话状态为 'Executing'
2. WHEN 状态更新为 'Executing' 后 THEN System SHALL 记录宣布时间
3. WHEN 状态更新为 'Executing' 后 THEN System SHALL 显示处刑对象信息
4. WHEN 状态更新为 'Executing' 后 THEN System SHALL 显示每个魔女的确认状态
5. WHEN 所有魔女确认后 THEN System SHALL 显示"开始处刑"按钮

### 需求 8: 魔女确认处刑对象

**用户故事**: 作为魔女，我希望能够看到处刑对象并确认，以便进入处刑按钮界面。

#### 验收标准

1. WHEN 审判状态为 'Executing' 时 THEN System SHALL 显示处刑对象头像
2. WHEN 显示处刑对象时 THEN System SHALL 播放滑稽音效
3. WHEN 显示处刑对象时 THEN System SHALL 显示处刑对象姓名
4. WHEN 显示处刑对象时 THEN System SHALL 显示"确认处刑"按钮
5. WHEN Witch点击"确认处刑"按钮时 THEN System SHALL 跳转到 ExecutionForm 界面
6. WHEN 跳转到 ExecutionForm 时 THEN System SHALL 显示 execution_bg.png 背景
7. WHEN 跳转到 ExecutionForm 时 THEN System SHALL 显示灰色处刑按钮
8. WHEN Witch点击处刑按钮时 THEN System SHALL 切换背景为 execution_complete.png
9. WHEN Witch点击处刑按钮时 THEN System SHALL 显示红色处刑按钮（融入背景图）
10. WHEN Witch点击处刑按钮时 THEN System SHALL 弹出"处刑成功"消息框
11. WHEN Witch点击处刑按钮时 THEN System SHALL 更新参与者的 HasConfirmedExecution 标志为 true
12. WHEN Witch未点击处刑按钮时 THEN System SHALL 禁用窗口关闭按钮
13. WHEN Witch强制退出后再进入时 THEN System SHALL 恢复到处刑对象确认界面或处刑按钮界面

### 需求 9: 典狱长完成处刑

**用户故事**: 作为典狱长，我希望在所有魔女确认后能够完成处刑，以便结束审判流程。

#### 验收标准

1. WHEN 所有魔女确认处刑后 THEN System SHALL 允许典狱长点击"开始处刑"按钮
2. WHEN 典狱长点击"开始处刑"按钮时 THEN System SHALL 更新审判会话状态为 'Completed'
3. WHEN 审判完成后 THEN System SHALL 记录完成时间
4. WHEN 审判完成后 THEN System SHALL 更新处刑对象魔女的状态为"已处刑"
5. WHEN 审判完成后 THEN System SHALL 记录处刑结果到魔女档案
6. WHEN 审判完成后 THEN System SHALL 显示"审判完成"消息
7. WHEN 审判完成后 THEN System SHALL 允许发起新的审判

### 需求 10: 状态恢复机制

**用户故事**: 作为用户，我希望在退出再进入时能够恢复到正确的状态，以确保流程不会中断。

#### 验收标准

1. WHEN Witch登录时 THEN System SHALL 检查是否有进行中的审判
2. WHEN 有进行中的审判时 THEN System SHALL 检查该魔女是否参与
3. WHEN 魔女参与且审判状态为 'Pending' 时 THEN System SHALL 显示"等待投票开始"
4. WHEN 魔女参与且审判状态为 'Voting' 且未投票时 THEN System SHALL 显示投票界面
5. WHEN 魔女参与且审判状态为 'Voting' 且已投票时 THEN System SHALL 显示"等待其他人投票"
6. WHEN 魔女参与且审判状态为 'Confirmed' 时 THEN System SHALL 显示"等待宣布处刑对象"
7. WHEN 魔女参与且审判状态为 'Executing' 且未确认时 THEN System SHALL 显示处刑对象确认界面
8. WHEN 魔女参与且审判状态为 'Executing' 且已确认时 THEN System SHALL 显示"等待其他人确认"
9. WHEN 魔女参与且审判状态为 'Completed' 时 THEN System SHALL 显示"审判已完成"
10. WHEN Warden登录时 THEN System SHALL 检查是否有进行中的审判
11. WHEN 有进行中的审判时 THEN System SHALL 根据状态显示对应的典狱长界面

### 需求 11: 防止退出机制

**用户故事**: 作为系统管理员，我希望在关键阶段防止用户退出，以确保流程完整性。

#### 验收标准

1. WHEN 魔女在投票界面且未投票时 THEN System SHALL 禁用窗口关闭按钮
2. WHEN 魔女在投票界面且未投票时 THEN System SHALL 点击关闭按钮显示"请先完成投票"提示
3. WHEN 魔女在处刑按钮界面且未点击时 THEN System SHALL 禁用窗口关闭按钮
4. WHEN 魔女在处刑按钮界面且未点击时 THEN System SHALL 点击关闭按钮显示"请先确认处刑"提示
5. WHEN 魔女完成投票后 THEN System SHALL 允许退出（状态已保存）
6. WHEN 魔女完成处刑确认后 THEN System SHALL 允许退出（状态已保存）

### 需求 12: 实时状态刷新

**用户故事**: 作为用户，我希望界面能够实时反映审判状态的变化，以便及时响应。

#### 验收标准

1. WHEN 典狱长界面打开时 THEN System SHALL 每2秒检查一次审判状态
2. WHEN 审判状态发生变化时 THEN System SHALL 自动刷新界面
3. WHEN 魔女界面打开时 THEN System SHALL 每2秒检查一次审判状态
4. WHEN 审判状态发生变化时 THEN System SHALL 自动切换到对应界面
5. WHEN 投票人数变化时 THEN System SHALL 实时更新典狱长界面的投票进度
6. WHEN 确认人数变化时 THEN System SHALL 实时更新典狱长界面的确认进度

### 需求 13: 权限控制

**用户故事**: 作为系统管理员，我希望系统能够正确控制不同角色的操作权限，以确保只有授权人员可以操作。

#### 验收标准

1. WHEN Warden访问审判管理界面时 THEN System SHALL 允许发起审判、查看投票结果、确认处刑对象
2. WHEN Warden访问审判管理界面时 THEN System SHALL 不允许参与投票
3. WHEN Witch访问手机界面时 THEN System SHALL 允许参与投票和确认处刑
4. WHEN Witch访问手机界面时 THEN System SHALL 不允许查看投票结果或管理审判
5. WHEN Admin访问系统时 THEN System SHALL 允许查看所有岛屿的审判状态（仅监督）
6. WHEN Admin访问系统时 THEN System SHALL 不允许参与任何审判操作
7. WHEN Meruru访问系统时 THEN System SHALL 不参与审判流程

### 需求 14: 岛屿隔离

**用户故事**: 作为系统管理员，我希望每个岛屿的审判数据相互独立，以确保不同岛屿的审判互不干扰。

#### 验收标准

1. WHEN Warden发起审判时 THEN System SHALL 只显示本岛屿的魔女
2. WHEN Witch参与投票时 THEN System SHALL 只显示本岛屿本批次的参与魔女
3. WHEN 查询审判会话时 THEN System SHALL 只返回本岛屿的审判
4. WHEN Admin查看审判时 THEN System SHALL 可以选择查看任意岛屿的审判
5. WHEN 一个岛屿有进行中的审判时 THEN System SHALL 不影响其他岛屿发起审判

### 需求 15: 数据持久化

**用户故事**: 作为系统管理员，我希望所有审判数据能够持久化存储，以确保数据不会丢失。

#### 验收标准

1. WHEN 审判会话创建时 THEN System SHALL 立即写入数据库
2. WHEN 审判状态变化时 THEN System SHALL 立即更新数据库
3. WHEN 魔女投票时 THEN System SHALL 立即记录投票结果到数据库
4. WHEN 魔女确认处刑时 THEN System SHALL 立即更新确认状态到数据库
5. WHEN 系统重启后 THEN System SHALL 从数据库恢复审判的最新状态
6. WHEN 系统重启后 THEN System SHALL 保留所有历史审判记录

### 需求 16: 错误处理

**用户故事**: 作为用户，我希望系统能够妥善处理各种错误情况，以确保操作的可靠性。

#### 验收标准

1. WHEN 尝试发起审判但已有进行中的审判时 THEN System SHALL 显示"当前岛屿已有进行中的审判"错误
2. WHEN 尝试投票但审判状态不正确时 THEN System SHALL 显示"当前无法投票"错误
3. WHEN 数据库连接失败时 THEN System SHALL 显示"数据库连接失败"错误并阻止操作
4. WHEN 权限不足时 THEN System SHALL 显示"权限不足"错误
5. WHEN 发生未预期错误时 THEN System SHALL 记录错误日志并显示友好的错误消息

### 需求 17: 用户界面

**用户故事**: 作为用户，我希望有直观的界面来参与审判流程，以便快速完成操作。

#### 验收标准

1. WHEN Warden打开审判管理界面时 THEN System SHALL 显示当前审判状态
2. WHEN Warden打开审判管理界面时 THEN System SHALL 根据状态显示对应的操作按钮
3. WHEN Witch点击处刑按钮时 THEN System SHALL 根据审判状态显示对应的界面
4. WHEN 显示投票界面时 THEN System SHALL 使用手机界面风格（450x800）
5. WHEN 显示处刑对象确认界面时 THEN System SHALL 使用手机界面风格
6. WHEN 操作成功时 THEN System SHALL 显示成功提示消息
7. WHEN 操作失败时 THEN System SHALL 显示具体的错误原因

### 需求 18: 音效支持

**用户故事**: 作为魔女，我希望在看到处刑对象时能够听到音效，以增强体验。

#### 验收标准

1. WHEN 显示处刑对象确认界面时 THEN System SHALL 播放滑稽音效
2. WHEN 音效文件不存在时 THEN System SHALL 跳过音效播放，不影响流程
3. WHEN 音效播放失败时 THEN System SHALL 记录错误日志，不影响流程

