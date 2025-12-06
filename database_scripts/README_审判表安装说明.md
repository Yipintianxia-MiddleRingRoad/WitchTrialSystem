# 审判投票流程系统 - 数据库表安装说明

## 问题
点击"审判管理"按钮时出现错误：
```
SQL执行失败: 对象名 'wt.TrialSession' 无效
```

## 原因
数据库中还没有创建审判相关的表。

## 解决方案

### 方法1：使用 SQL Server Management Studio (推荐)

1. 打开 SQL Server Management Studio (SSMS)
2. 连接到你的数据库服务器
3. 打开文件：`database_scripts/create_trial_tables_all.sql`
4. 确认数据库名称为 `WitchTrialWT`（脚本第7行）
5. 点击"执行"按钮（或按 F5）
6. 等待执行完成，应该看到成功消息

### 方法2：使用 sqlcmd 命令行工具

在命令行中执行：
```cmd
sqlcmd -S localhost -d WitchTrialWT -i database_scripts\create_trial_tables_all.sql
```

如果需要指定用户名和密码：
```cmd
sqlcmd -S localhost -U sa -P your_password -d WitchTrialWT -i database_scripts\create_trial_tables_all.sql
```

### 方法3：使用 Visual Studio 的 SQL Server 对象资源管理器

1. 在 Visual Studio 中打开"视图" > "SQL Server 对象资源管理器"
2. 连接到你的数据库服务器
3. 展开服务器 > 数据库 > WitchTrialWT
4. 右键点击数据库，选择"新建查询"
5. 复制 `create_trial_tables_all.sql` 的内容到查询窗口
6. 点击"执行"按钮

## 创建的表

执行脚本后会创建以下3个表：

1. **wt.TrialSession** - 审判会话表
   - 存储每次审判的基本信息
   - 包含状态、创建时间、投票时间、处刑对象等

2. **wt.TrialParticipant** - 审判参与者表
   - 存储每次审判的参与魔女信息
   - 包含投票记录、确认处刑记录等

3. **wt.TrialNotification** - 审判通知表
   - 存储发送给魔女的通知消息
   - 包含通知内容、是否已读等

## 验证安装

执行以下 SQL 查询验证表是否创建成功：

```sql
-- 查看所有审判相关的表
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'wt' 
  AND TABLE_NAME LIKE 'Trial%'
ORDER BY TABLE_NAME;

-- 应该返回3行：
-- TrialNotification
-- TrialParticipant
-- TrialSession
```

## 注意事项

1. **数据库名称**：确保你的数据库名称是 `WitchTrialWT`，如果不是，请修改脚本第7行
2. **Schema名称**：所有表都在 `wt` schema 下，确保该 schema 已存在
3. **外键依赖**：这些表依赖以下现有表：
   - `wt.Island` - 岛屿表
   - `wt.Batch` - 批次表
   - `wt.[User]` - 用户表
   - `wt.Witch` - 魔女表
4. **删除重建**：如果表已存在，脚本会先删除再重建（会丢失数据）

## 故障排除

### 错误：数据库 'WitchTrialWT' 不存在
- 检查数据库名称是否正确
- 修改脚本第7行的数据库名称

### 错误：Schema 'wt' 不存在
- 先创建 schema：`CREATE SCHEMA wt;`

### 错误：外键引用的表不存在
- 确保 `wt.Island`, `wt.Batch`, `wt.[User]`, `wt.Witch` 表已存在
- 这些是系统的基础表，应该在初始化时就已创建

## 完成后

表创建成功后，重新启动应用程序，点击"审判管理"按钮应该就能正常打开了。
