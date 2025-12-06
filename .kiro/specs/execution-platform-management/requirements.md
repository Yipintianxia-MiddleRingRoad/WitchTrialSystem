# 处刑台管理系统 - 需求文档

## 简介

本文档定义了《魔女审判系统》中处刑台管理功能的需求。该功能允许典狱长和管理者管理处刑台的位置、刑具配置，并记录所有移动历史。

## 术语表

- **System**: 魔女审判系统
- **ExecutionPlatform**: 处刑台，用于执行魔女处刑的平台设备
- **ExecutionTool**: 刑具，安装在处刑台上的处刑工具（如断头台、火刑架等）
- **Location**: 位置，处刑台可以放置的固定位置点
- **Basement**: 地下室（-1F），包含位置1-49
- **TrialHall**: 审判庭（1F），位置50
- **Warden**: 典狱长，负责岛屿日常管理和处刑执行
- **Regulator**: 管理者（监管员），负责岛屿监管和刑具管理
- **MovementLog**: 移动记录，记录处刑台移动的历史日志
- **HomePosition**: 原位，处刑台绑定的初始位置

## 需求

### 需求 1: 处刑台位置管理

**用户故事**: 作为系统管理员，我希望系统能够管理每个岛屿的处刑台位置，以便追踪处刑台的当前状态和位置。

#### 验收标准

1. WHEN 系统初始化时 THEN System SHALL 为每个岛屿创建50个固定位置
2. WHEN 系统初始化时 THEN System SHALL 为每个岛屿创建49个处刑台
3. WHEN 系统初始化时 THEN System SHALL 将处刑台1-49分别放置在位置1-49
4. WHEN 查询处刑台位置时 THEN System SHALL 返回处刑台的当前位置编号
5. WHEN 查询位置状态时 THEN System SHALL 返回该位置是否有处刑台占用

### 需求 2: 处刑台移动到审判庭

**用户故事**: 作为典狱长或管理者，我希望能够将处刑台移动到审判庭，以便执行魔女处刑。

#### 验收标准

1. WHEN Warden或Regulator选择一个处刑台并请求移动到审判庭 THEN System SHALL 检查审判庭位置是否为空
2. WHEN 审判庭位置为空 THEN System SHALL 允许处刑台移动到位置50
3. WHEN 审判庭位置已被占用 THEN System SHALL 拒绝移动请求并显示错误消息
4. WHEN 处刑台成功移动到审判庭 THEN System SHALL 更新处刑台的当前位置为50
5. WHEN 处刑台成功移动到审判庭 THEN System SHALL 记录移动日志

### 需求 3: 处刑台返回原位

**用户故事**: 作为典狱长或管理者，我希望处刑完成后能够将处刑台返回原位，以便恢复地下室的正常状态。

#### 验收标准

1. WHEN Warden或Regulator请求处刑台返回原位 THEN System SHALL 获取该处刑台的HomePosition
2. WHEN 处刑台的HomePosition为空 THEN System SHALL 允许处刑台返回原位
3. WHEN 处刑台的HomePosition已被占用 THEN System SHALL 拒绝返回请求并显示错误消息
4. WHEN 处刑台成功返回原位 THEN System SHALL 更新处刑台的当前位置为HomePosition
5. WHEN 处刑台成功返回原位 THEN System SHALL 记录移动日志

### 需求 4: 刑具管理

**用户故事**: 作为管理者，我希望能够为处刑台添加、更换或移除刑具，以便根据处刑需要配置合适的刑具。

#### 验收标准

1. WHEN Regulator为处刑台添加刑具 THEN System SHALL 记录刑具名称和类型
2. WHEN Regulator为处刑台添加刑具 THEN System SHALL 更新处刑台的刑具信息
3. WHEN Regulator更换处刑台的刑具 THEN System SHALL 替换原有刑具信息
4. WHEN Regulator移除处刑台的刑具 THEN System SHALL 将处刑台的刑具信息设置为空
5. WHEN 处刑台移动时 THEN System SHALL 保持刑具与处刑台的绑定关系

### 需求 5: 移动记录查询

**用户故事**: 作为典狱长或管理者，我希望能够查看处刑台的移动历史记录，以便了解处刑台的使用情况和审计操作。

#### 验收标准

1. WHEN Warden或Regulator查询移动记录 THEN System SHALL 显示所有移动记录列表
2. WHEN 显示移动记录时 THEN System SHALL 包含移动时间（北京时间）
3. WHEN 显示移动记录时 THEN System SHALL 包含处刑台编号
4. WHEN 显示移动记录时 THEN System SHALL 包含刑具名称（如果有）
5. WHEN 显示移动记录时 THEN System SHALL 包含起始位置和目标位置
6. WHEN 显示移动记录时 THEN System SHALL 不包含操作人信息
7. WHEN Warden或Regulator筛选移动记录 THEN System SHALL 支持按时间范围筛选
8. WHEN Warden或Regulator筛选移动记录 THEN System SHALL 支持按处刑台编号筛选
9. WHEN Warden或Regulator筛选移动记录 THEN System SHALL 支持按位置筛选

### 需求 6: 权限控制

**用户故事**: 作为系统管理员，我希望系统能够正确控制不同角色的操作权限，以确保只有授权人员可以操作处刑台。

#### 验收标准

1. WHEN Warden访问处刑台管理界面 THEN System SHALL 允许查看和移动处刑台
2. WHEN Warden访问处刑台管理界面 THEN System SHALL 不允许管理刑具
3. WHEN Regulator访问处刑台管理界面 THEN System SHALL 允许查看和移动处刑台
4. WHEN Regulator访问处刑台管理界面 THEN System SHALL 允许管理刑具（仅Regulator可以管理刑具）
5. WHEN Admin访问处刑台管理界面 THEN System SHALL 允许查看所有岛屿的处刑台和移动记录，但不允许操作（国家端仅监督）
6. WHEN Witch访问系统 THEN System SHALL 不显示处刑台管理功能

### 需求 7: 岛屿隔离

**用户故事**: 作为系统管理员，我希望每个岛屿的处刑台数据相互独立，以确保不同岛屿的处刑台管理互不干扰。

#### 验收标准

1. WHEN Warden或Regulator查看处刑台列表 THEN System SHALL 只显示所属岛屿的处刑台
2. WHEN Warden或Regulator操作处刑台 THEN System SHALL 只允许操作所属岛屿的处刑台
3. WHEN Admin查看处刑台列表 THEN System SHALL 可以选择查看任意岛屿的处刑台
4. WHEN 系统初始化时 THEN System SHALL 为每个岛屿独立创建处刑台数据
5. WHEN 查询移动记录时 THEN System SHALL 只返回当前岛屿的移动记录

### 需求 8: 用户界面

**用户故事**: 作为典狱长或管理者，我希望有一个直观的界面来管理处刑台，以便快速完成操作。

#### 验收标准

1. WHEN Warden或Regulator打开处刑台管理界面 THEN System SHALL 显示地下室布局图（1-49位置）
2. WHEN Warden或Regulator打开处刑台管理界面 THEN System SHALL 显示审判庭状态（位置50）
3. WHEN 显示处刑台时 THEN System SHALL 标识处刑台编号、当前位置、刑具信息
4. WHEN Warden或Regulator点击处刑台 THEN System SHALL 显示操作菜单（移动到审判庭/返回原位）
5. WHEN Regulator点击处刑台 THEN System SHALL 显示刑具管理选项（添加/更换/移除刑具）
6. WHEN 操作成功时 THEN System SHALL 显示成功提示消息
7. WHEN 操作失败时 THEN System SHALL 显示具体的错误原因

### 需求 9: 数据持久化

**用户故事**: 作为系统管理员，我希望所有处刑台数据和移动记录能够持久化存储，以确保数据不会丢失。

#### 验收标准

1. WHEN 处刑台位置发生变化 THEN System SHALL 立即更新数据库
2. WHEN 刑具信息发生变化 THEN System SHALL 立即更新数据库
3. WHEN 处刑台移动时 THEN System SHALL 立即记录移动日志到数据库
4. WHEN 系统重启后 THEN System SHALL 从数据库恢复处刑台的最新状态
5. WHEN 系统重启后 THEN System SHALL 保留所有历史移动记录

### 需求 10: 错误处理

**用户故事**: 作为用户，我希望系统能够妥善处理各种错误情况，以确保操作的可靠性。

#### 验收标准

1. WHEN 尝试移动不存在的处刑台 THEN System SHALL 显示"处刑台不存在"错误
2. WHEN 尝试移动到已占用的位置 THEN System SHALL 显示"目标位置已被占用"错误
3. WHEN 数据库连接失败 THEN System SHALL 显示"数据库连接失败"错误并阻止操作
4. WHEN 权限不足时 THEN System SHALL 显示"权限不足"错误
5. WHEN 发生未预期错误时 THEN System SHALL 记录错误日志并显示友好的错误消息
