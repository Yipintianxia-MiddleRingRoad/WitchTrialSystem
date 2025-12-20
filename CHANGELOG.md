# WitchTrialSystem 更新日志

## [1.6.7] - 2025-12-20 🎉

### ✨ 新增功能

- **录音功能（RecordingForm / RecordingService）** ⭐NEW
  - 魔女手机界面新增录音功能
  - 支持录音的开始、暂停、继续、停止操作
  - 支持"暂停状态下结束录音"仍然视为有效录音
  - 录音完成后自动弹出标题输入对话框
  - 录音文件保存路径：`UI/records/{编号}/`，按用户编号隔离
  - 录音完成后自动刷新列表并选中当前录音
  - 支持录音列表查看和播放
  - 支持删除已保存的录音文件

- **对话功能（ChatForm / ChatService）** ⭐NEW
  - 魔女手机界面新增对话功能
  - 支持角色间互相聊天
  - 左右分栏布局：左侧联系人、右侧对话框，支持 1:2 自适应宽度
  - 支持本地文件聊天记录：按用户分别保存在 `Data/ChatLogs/{用户名}/{对方用户名}.txt`
  - 双方各有一份独立的聊天记录
  - 添加当前联系人聊天记录一键清空按钮
  - 左侧联系人支持"未读高亮"，对方有新消息时以金色显示
  - 打开对话后自动恢复未读状态
  - 支持实时消息发送和接收
  - 支持消息时间戳记录

### 🎨 界面优化

- 魔女手机界面新增录音和对话功能入口
- 优化手机界面布局，新增功能按钮
- 完善用户交互体验

### 📊 项目统计（已验证）

#### 代码规模
| 类别 | 数量 | 说明 |
|------|:---:|------|
| **界面文件** | 51个 | 根目录4个 + UI文件夹47个 |
| **业务逻辑层** | 13个 | BLL文件夹（新增RecordingService, ChatService） |
| **数据访问层** | 15个 | DAL文件夹 |
| **数据模型层** | 5个 | Models文件夹 |
| **总代码文件** | 88个 | 包含所有.cs文件 |

#### 数据库规模
| 类别 | 数量 | 说明 |
|------|:---:|------|
| **数据库表** | 18个 | 保持不变 |
| **Witch表字段** | 43个 | 保持不变 |

#### 功能模块
| 模块 | 状态 | 说明 |
|------|:---:|------|
| 魔女档案管理 | ✅ | 43字段完整档案 |
| 四层权限体系 | ✅ | Admin/Meruru/Warden/Witch |
| 图鉴系统 | ✅ | 人物/证物/记录/地图/规定 |
| 审判投票系统 | ✅ | 7个状态流转 |
| 处刑平台管理 | ✅ | 49个处刑台管理 |
| 可视化大屏 | ✅ | LiveCharts图表 |
| 五子棋对局 | ✅ | 积分系统 |
| 照相功能 | ✅ | 魔女手机界面 |
| 录音功能 | ✅ NEW | 魔女手机界面 |
| 对话功能 | ✅ NEW | 角色间互相聊天 |
| 操作日志 | ✅ | 审计追踪 |

### 🎯 版本对比

| 功能 | v1.6.3 | v1.6.7 |
|------|:---:|:---:|
| 数据库表 | 18个 | 18个 ✅ |
| 界面文件 | 41个 | 51个 ⬆️ |
| 代码文件 | 82个 | 88个 ⬆️ |
| 图鉴系统 | ✅ | ✅ |
| 审判投票系统 | ✅ | ✅ |
| 处刑平台管理 | ✅ | ✅ |
| 可视化大屏 | ✅ | ✅ |
| 照相功能 | ✅ | ✅ |
| 录音功能 | 🔄 开发中 | ✅ ⬆️ |
| 对话功能 | ❌ | ✅ ⬆️ |
| 文档完整度 | 📗📗📗📗📗 | 📗📗📗📗📗 ✅ |

### 📝 文件变更

#### 新增文件（6个）
- 📦 UI层：2个新界面
  - UI/RecordingForm.cs - 录音界面
  - UI/ChatForm.cs - 对话界面
- 📦 BLL层：2个新业务逻辑类
  - BLL/RecordingService.cs - 录音业务逻辑
  - BLL/ChatService.cs - 对话业务逻辑
- 📦 数据文件夹：2个新目录
  - UI/records/ - 录音文件存储目录
  - Data/ChatLogs/ - 聊天记录存储目录

#### 更新文件（3个）
- 📝 UI/PhoneForm.cs - 新增录音和对话功能入口
- 📝 README.md - 更新版本号和功能说明
- 📝 CHANGELOG.md - 添加v1.6.7版本记录

### 🔮 下一步计划

- 🚀 优化录音和对话界面美化
- 🚀 添加更多聊天功能（表情、文件分享等）
- 🚀 完善录音播放功能
- 🚀 增加聊天记录搜索功能

---

## [1.6.4] - 2025-12-18 📝

### ✨ 新增 / 改进

- **聊天系统（ChatForm）**
  - 增加左右分栏布局：左侧联系人、右侧对话框，支持 1:2 自适应宽度。
  - 支持本地文件聊天记录：按用户分别保存在 `Data/ChatLogs/{用户名}/{对方用户名}.txt`，双方各有一份。
  - 添加当前联系人聊天记录一键清空按钮，仅清除本地当前用户侧记录。
  - 左侧联系人支持“未读高亮”，对方有新消息时在列表中以金色显示，打开对话后自动恢复。
- **录音系统（RecordingForm / RecordingService）**
  - 支持“暂停状态下结束录音”仍然视为有效录音，并正常弹出标题输入与保存流程。
  - 录音文件保存路径调整为 `UI/records/{编号}/`，按用户编号隔离。
  - 录音完成后自动刷新列表并选中当前录音，体验更流畅。

### 🐛 Bug 修复

- 修复聊天窗口首次消息可能被标题栏遮挡的问题。
- 修复 `SplitContainer` 在极小宽度时调整 `SplitterDistance` 可能抛出异常的问题。
- 修复录音在某些情况下结束但未生成有效文件时的状态异常。

---

## [1.6.3] - 2024-12-12 🎨

### 🌟 图鉴系统完善：新增证物、记录、地图、规定四大图鉴

本版本完善了魔女图鉴系统，从单一人物图鉴扩展到五大图鉴模块！

### ✨ 新增功能

#### 📚 图鉴系统扩展（4个新界面）
- **证物图鉴（EvidenceForm.cs）**
  - 浏览所有证物信息（编号、名称、描述、图片）
  - 底部热键切换证物
  - 左侧显示证物编号和名称
  - 右侧显示证物图片和描述
  - 数据库表：wt.Evidence（5个字段）
  
- **记录图鉴（RecordsForm.cs）**
  - 浏览历史记录文档
  - 底部热键切换记录
  - 左侧显示记录标题（22号字体，居中对齐）
  - 右侧显示Markdown文档内容（可滚动）
  - 数据库表：wt.Record（3个字段）
  - 支持8个字的长标题完整显示
  
- **地图图鉴（MapForm.cs）** ⭐队友开发
  - 查看4张地图（map_bg.png, 地图2-4.png）
  - 左下角4个透明按钮切换地图
  - 无需数据库支持
  - 继承自BasePokedexForm
  
- **规定图鉴（RulesForm.cs）** ⭐队友开发
  - 查看5个规定（规定1-5.md）
  - 左下角热键切换规定（规定Ⅰ-Ⅴ）
  - 左侧显示罗马数字和规定名称
  - 右侧显示Markdown文档内容（可滚动）
  - 使用自定义字体"方正小标宋简.ttf"
  - 删除Word文档支持，仅保留Markdown

### 🗄️ 数据库扩展

#### 新增表（2个）
- **wt.Evidence** - 证物表（5个字段）
  - EvidenceID, EvidenceNo, Name, Description, ImagePath
- **wt.Record** - 记录表（3个字段）
  - RecordID, Title, Content（Markdown文件路径）

#### 数据库规模
- 表数量：16个 → 18个
- 新增图鉴系统分类

### 🎨 界面优化

#### 界面总数
- 37个 → 41个界面文件
- 新增4个图鉴界面

#### 技术特色
- Markdown文档渲染
- 自定义字体支持（方正小标宋简）
- 透明热键交互
- 响应式布局
- 滚动内容支持

### 📚 文档完善

#### 更新核心文档（3个）
- **README.md** - 更新版本号v2.3.0
  - 新增图鉴系统介绍
  - 更新项目统计数据（41个界面，18个表）
  - 更新核心特色说明
  
- **项目架构说明.md** - 更新架构设计
  - 新增图鉴系统模块详细说明
  - 更新文件统计（41个界面）
  - 更新版本历史V2.3.0
  
- **系统界面跳转与权限层级说明.md** - 更新界面关系
  - 新增5个图鉴界面的跳转关系
  - 更新界面总数（41个）
  - 更新功能模块（9个）
  
- **数据库结构文档.md** - 更新数据库结构
  - 新增2个表的详细说明
  - 更新表数量统计（18个表）
  - 新增图鉴系统分类

### 🎯 关键优化

#### 代码简化
- ✅ RulesForm删除DocumentFormat.OpenXml依赖
- ✅ 简化LoadRuleContent()方法，仅支持Markdown
- ✅ 优化字体加载和释放机制

#### 界面美化
- ✅ RecordsForm标题字体大小优化（48→22号）
- ✅ RecordsForm标题宽度优化（300→500px）
- ✅ RecordsForm标题位置调整，确保完整显示
- ✅ 底部热键颜色统一（RGB 196,177,169）

### 📊 项目统计（已验证）

#### 代码规模
- 界面文件：37个 → 41个
- 数据库表：16个 → 18个
- 功能模块：8个 → 9个

#### 图鉴系统
- 人物图鉴：PokedexForm.cs（原有）
- 证物图鉴：EvidenceForm.cs（新增）
- 记录图鉴：RecordsForm.cs（新增）
- 地图图鉴：MapForm.cs（新增，队友开发）
- 规定图鉴：RulesForm.cs（新增，队友开发）

### 🎉 版本对比

| 功能 | v1.6.2 | v1.6.3 |
|------|:---:|:---:|
| 数据库表 | 16个 | 18个 ⬆️ |
| 界面文件 | 37个 | 41个 ⬆️ |
| 图鉴系统 | 人物 | 人物/证物/记录/地图/规定 ⬆️ |
| 审判投票系统 | ✅ | ✅ |
| 处刑平台管理 | ✅ | ✅ |
| 可视化大屏 | ✅ | ✅ |
| 文档完整度 | 📗📗📗📗📗 | 📗📗📗📗📗 ✅ |

### 📝 文件变更

#### 新增文件（8个）
- 📦 UI层：4个新图鉴界面
  - UI/EvidenceForm.cs
  - UI/RecordsForm.cs
  - UI/MapForm.cs
  - UI/RulesForm.cs
- 📦 数据库脚本：4个
  - database_scripts/create_evidence_table.sql
  - database_scripts/insert_evidence_data.sql
  - database_scripts/create_record_table.sql
  - database_scripts/insert_record_data.sql

#### 更新文件（5个）
- 📝 核心文档：4个
  - README.md（版本号v2.3.0）
  - 项目架构说明.md（版本号2.3）
  - 系统界面跳转与权限层级说明.md（版本号2.2）
  - 数据库结构文档.md（版本号2.2）
- 📝 更新日志：1个
  - CHANGELOG.md

### 🎯 团队协作

- **证物图鉴** - 自主开发
- **记录图鉴** - 自主开发
- **地图图鉴** - 队友开发，已集成
- **规定图鉴** - 队友开发，已集成并优化

### 🔮 下一步计划

- 🚀 优化图鉴界面美化
- 🚀 添加更多证物和记录
- 🚀 完善Markdown渲染效果
- 🚀 增加图鉴搜索功能

---

## [1.6.2] - 2024-12-07

### 📦 发布包准备

本版本主要进行发布包的准备和打包工作。

### 改进
- 📦 创建Self-Contained发布包
- 📦 配置数据库连接字符串（LocalDB）
- 📦 创建启动程序.bat（整合权限处理和数据库附加）
- 📦 创建安装说明.txt
- 📦 数据库文件打包（WitchTrialWT.mdf + WitchTrialWT_log.ldf）
- 📦 删除危险的"数据库初始化工具.bat"

### 文件变更
- 📦 新增 Release_Package_v1.6.2/ 文件夹
- 📦 新增 启动程序.bat
- 📦 新增 安装说明.txt
- 📦 新增 Data/WitchTrialWT.mdf（64 MB）
- 📦 新增 Data/WitchTrialWT_log.ldf（32 MB）

---

## [1.6.0] - 2024-12-07 🎉🎉🎉

### 🌟 重大更新：审判投票系统与处刑平台管理系统全面上线

本版本实现了两个重大功能模块，标志着魔女审判系统进入2.0时代！

### ✨ 新增功能

#### ⚖️ 审判投票系统（完整实现）
- **典狱长功能**
  - 创建审判会话（选择2-13名参与魔女）
  - 开始投票（将状态从Pending改为Voting）
  - 查看投票结果（实时统计，可视化展示）
  - 宣布处刑对象（根据投票结果选择）
  - 完成审判（标记为Completed状态）
  
- **魔女功能**
  - 接收审判通知（登录时自动弹窗）
  - 参与投票（选择投票对象）
  - 确认处刑对象（查看并确认）
  - 点击处刑按钮（执行处刑）
  - 单机多账号支持（投票后可切换账号）
  
- **技术特色**
  - 7个状态流转：Pending → Voting → Confirmed → Executing → Completed
  - 状态持久化到数据库
  - 实时通知系统
  - 完整的权限控制和岛屿隔离

#### 🏛️ 处刑平台管理系统（完整实现）
- **平台管理**
  - 49个处刑台管理（每个岛屿）
  - 升起操作（从原位1-49升起到审判庭50号位）
  - 返回操作（从审判庭返回到原位）
  - 状态管理（空闲/使用中）
  
- **刑具管理**
  - 刑具信息管理（名称、类型、描述）
  - 批量编辑功能
  - 刑具查询功能
  
- **移动日志**
  - 历史记录查询
  - 时间管理（当前时间+手动输入）
  - 日志筛选（按日期、处刑台编号）
  - 匿名记录（不记录操作人）

#### 📊 可视化大屏系统（完整实现）
- **数据展示**
  - 魔女总数统计（数字卡片）
  - 岛屿分布统计
  - 状态分布饼图（LiveCharts）
  - 批次分布柱状图
  - 五子棋积分排行榜（TOP 10）
  - 最近对局记录（最近10场）
  - 审判会话统计
  - 处刑台状态统计
  
- **技术实现**
  - LiveCharts.WinForms图表库
  - 实时数据刷新（定时器）
  - 响应式布局设计
  - 权限控制（按岛屿过滤）

### 🗄️ 数据库扩展

#### 新增表（5个）
- **wt.TrialSession** - 审判会话表（11个字段）
- **wt.TrialParticipant** - 审判参与者表（9个字段）
- **wt.TrialNotification** - 审判通知表（6个字段）
- **wt.ExecutionPlatform** - 处刑台表（11个字段）
- **wt.PlatformMovementLog** - 处刑台移动记录表（10个字段）

#### 数据库规模
- 表数量：11个 → 16个
- 完整的索引和约束设计
- 支持事务和并发控制

### 🎨 界面优化

#### 新增界面（13个）
- **审判系统界面**
  - TrialManagementForm.cs - 审判管理主界面
  - CreateTrialDialog.cs - 创建审判对话框
  - TrialVotingForm.cs - 投票界面
  - VotingResultDialog.cs - 投票结果对话框
  - TrialExecutionConfirmForm.cs - 处刑确认界面
  - NotificationPopupForm.cs - 通知弹窗
  
- **处刑平台界面**
  - ExecutionPlatformManagementForm.cs - 处刑平台管理主界面
  - PlatformMoveDialog.cs - 移动对话框
  - ToolManagementDialog.cs - 刑具管理对话框
  - MovementLogViewForm.cs - 移动记录查看界面
  
- **可视化界面**
  - DashboardForm.cs - 可视化大屏主界面

#### 界面总数
- 20+个 → 37个界面文件

### 🔧 业务逻辑层扩展

#### 新增BLL类（6个）
- TrialSessionService.cs - 审判会话业务逻辑
- TrialVotingService.cs - 投票业务逻辑
- TrialNotificationService.cs - 通知业务逻辑
- ExecutionPlatformService.cs - 处刑平台业务逻辑
- MovementLogService.cs - 移动日志业务逻辑
- DashboardService.cs - 可视化大屏业务逻辑

### 📦 数据访问层扩展

#### 新增DAL类（6个）
- TrialSessionDAL.cs - 审判会话数据访问
- TrialParticipantDAL.cs - 参与者数据访问
- TrialNotificationDAL.cs - 通知数据访问
- ExecutionPlatformDAL.cs - 处刑平台数据访问
- MovementLogDAL.cs - 移动日志数据访问
- DashboardDAL.cs - 可视化大屏数据访问

### 📚 文档完善

#### 更新核心文档（4个）
- **README.md** - 添加v2.0重大更新说明
  - 新增审判投票系统介绍
  - 新增处刑平台管理系统介绍
  - 新增可视化大屏系统介绍
  - 更新项目统计数据
  - 更新使用场景
  
- **项目架构说明.md** - 更新架构设计
  - 新增审判系统模块
  - 新增处刑平台模块
  - 新增可视化模块
  - 更新文件统计
  
- **系统界面跳转与权限层级说明.md** - 更新界面关系
  - 新增37个界面的跳转关系
  - 更新权限控制说明
  - 新增界面截图
  
- **数据库结构文档.md** - 更新数据库结构
  - 新增5个表的详细说明
  - 更新表数量统计（16个表）
  - 新增ER关系图

#### 新增专题文档（2个）
- **四层权限体系说明.md** - 权限体系详解
  - 详细说明RoleID 1-4
  - 账号与权限对应关系
  - 权限对比矩阵
  
- **文档更新说明_20241207.md** - 数据验证记录
  - 所有数据通过SQL查询验证
  - 界面文件统计验证
  - 登录流程验证

### 🎯 关键修正

#### 数据准确性
- ✅ Witch表字段：42个 → 43个（已验证）
- ✅ 数据库表数量：11个 → 16个（已验证）
- ✅ 界面文件数量：20+个 → 37个（已验证）
- ✅ Form1.cs状态：已废弃，魔女登录直接跳转PhoneForm.cs

#### 权限体系
- ✅ 统一标注RoleID（1-4）
- ✅ 明确账号列表
  - Admin(1): admin
  - Meruru(2): meruru_regulator, utena_regulator
  - Warden(3): warden, warden2
  - Witch(4): 多个魔女用户
- ✅ 权限递减：Admin(1) > Meruru(2) > Warden(3) > Witch(4)

### 🚀 技术改进

#### 架构优化
- 完善三层架构设计（UI → BLL → DAL）
- 优化数据访问层性能
- 改进错误处理机制
- 完善日志记录系统

#### 代码质量
- 统一代码风格
- 完善注释文档
- 优化SQL查询性能
- 改进事务处理

### 📊 项目统计（已验证）

#### 代码规模
- 界面文件：37个
- 业务逻辑层：9个
- 数据访问层：13个
- 数据模型层：5个
- 总代码文件：100+

#### 数据库规模
- 数据库表：16个
- Witch表字段：43个
- 存储过程：多个
- 视图：2个

### 🎉 版本对比

| 功能 | v1.5 | v1.6 |
|------|:---:|:---:|
| 数据库表 | 11个 | 16个 ⬆️ |
| 界面文件 | 20+个 | 37个 ⬆️ |
| 审判投票系统 | ❌ | ✅ NEW |
| 处刑平台管理 | ❌ | ✅ NEW |
| 可视化大屏 | ❌ | ✅ NEW |
| 文档完整度 | 📗📗📗 | 📗📗📗📗📗 ⬆️ |

### 📝 文件变更

#### 新增文件（30+个）
- 📦 UI层：13个新界面
- 📦 BLL层：6个新业务逻辑类
- 📦 DAL层：6个新数据访问类
- 📦 Models层：3个新模型类
- 📦 数据库脚本：5个新建表脚本
- 📦 文档：2个新文档

#### 更新文件（20+个）
- 📝 核心文档：4个
- 📝 Form文件：3个（Admin, Regulator, Warden）
- 📝 配置文件：1个

### 🎯 使用场景

#### 场景1：典狱长创建审判
1. 使用warden账号登录
2. 点击"审判管理"按钮
3. 创建新审判，选择参与魔女
4. 开始投票

#### 场景2：魔女参与投票
1. 使用魔女账号登录
2. 接收审判通知
3. 参与投票
4. 确认处刑对象

#### 场景3：管理处刑平台
1. 使用warden账号登录
2. 点击"处刑平台管理"
3. 升起/返回处刑台
4. 查看移动日志

#### 场景4：查看可视化大屏
1. 使用admin/meruru/warden账号登录
2. 点击"可视化大屏"
3. 查看统计数据和图表

### 🔮 下一步计划

- 🚀 优化前端界面美化
- 🚀 添加更多图表类型
- 🚀 完善移动端适配
- 🚀 增加数据导出功能
- 🚀 优化性能和响应速度

---

## [1.5.1] - 2024-12-06

### 设计亮点
- 🎯 **状态机设计**
  - 6个状态：Idle → Pending → Voting → Confirmed → Executing → Completed
  - 9个魔女端状态：Idle, NotParticipating, WaitingToStart, Voting, WaitingForOthersToVote, WaitingForExecutionAnnouncement, ConfirmingExecution, WaitingForOthersToConfirm, Completed
  - 完整的状态转换逻辑和恢复机制
- 🗄️ **数据库设计**
  - 3张新表：TrialSession（审判会话）、TrialParticipant（参与者）、TrialNotification（通知）
  - 完整的索引和约束设计
  - 支持状态持久化和历史记录
- 🏗️ **架构设计**
  - 三层架构：UI → BLL → DAL
  - 清晰的接口定义和职责划分
  - 完整的错误处理和日志记录

### 核心功能
- 👨‍⚖️ **典狱长功能**
  - 发起审判（选择2-13名参与魔女）
  - 开始投票
  - 查看投票结果（可视化统计）
  - 确认处刑对象（多人同票时手动选择）
  - 宣布处刑对象
  - 完成处刑（更新魔女状态）
- 🧙‍♀️ **魔女功能**
  - 接收审判通知（5秒弹窗）
  - 投票选择处刑对象（三个一行布局）
  - 确认处刑对象（显示头像+音效）
  - 点击处刑按钮（复用现有ExecutionForm）
  - 状态恢复（退出再进入保持状态）
  - 防止退出（关键阶段不能关闭）

### 技术特性
- ✅ 单机单用户模式适配
- ✅ 状态持久化到数据库
- ✅ 实时状态刷新（Timer 2秒间隔）
- ✅ 防止退出机制（OnFormClosing拦截）
- ✅ 音效支持（滑稽音效）
- ✅ 手机风格界面（450x800）
- ✅ 完整的权限控制
- ✅ 岛屿数据隔离

### 文件变更
- 📦 新增 `.kiro/specs/trial-voting-system/requirements.md` - 需求文档
- 📦 新增 `.kiro/specs/trial-voting-system/design.md` - 设计文档
- 📦 新增 `.kiro/specs/trial-voting-system/tasks.md` - 任务清单

### 下一步计划
- 🚀 开始实现数据库表创建（阶段1）
- 🚀 实现数据模型类（阶段2）
- 🚀 实现DAL层（阶段3）
- 🚀 实现BLL层（阶段4）
- 🚀 实现UI层（阶段5-6）

---

## [1.5.1] - 2024-12-06

### 新增
- ✨ **魔女账号创建功能**
  - 监管员可为已分配到本岛屿但无账号的魔女创建账号
  - 右键菜单"创建账号"选项
  - 用户名自动使用囚犯编号（PrisonerNo）
  - 默认密码统一为 123456
  - 完整的权限控制和资格验证
  - 事务性操作保证数据一致性
- ✨ **智慧可视化大屏优化**
  - 修复饼图颜色显示问题，现在正确使用状态对应的颜色
  - 优化热力图图例位置
- ✨ **处刑台管理系统UI层实现**
  - 处刑台管理主界面（ExecutionPlatformManagementForm）
  - 移动对话框（PlatformMoveDialog）
  - 刑具管理对话框（ToolManagementDialog）
  - 移动记录查看界面（MovementLogViewForm）
  - 在 Form1_Warden、Form1_Regulator、Form1_Admin 中添加入口按钮

### 改进
- 🔧 **DAL层优化**
  - 新增 `UserDAL.UserExists()` - 检查用户名是否存在
  - 新增 `UserDAL.CreateWitchAccountWithAssociation()` - 事务性创建账号和关联
  - 修复数据库连接管理问题
- 🔧 **BLL层优化**
  - 新增 `UserBLL.IsAccountEligible()` - 验证魔女创建账号资格
  - 新增 `UserBLL.CreateWitchAccount()` - 账号创建业务逻辑
- 🔧 **UI层优化**
  - Form1_Regulator 新增"创建账号"右键菜单项
  - 完善的用户提示和错误处理
  - 创建成功后自动刷新数据列表
  - 修复 MovementLogViewForm 的 DataGridView 显示问题
    - 确保 DataGridView 正确填充整个窗体
    - 修复列宽设置，确保所有列都可见
    - 移除时间筛选功能（按用户要求）
    - 参考 Form1.cs 的布局方式优化界面

### 权限修正
- 🔐 **处刑台管理权限层级**
  - Admin（国家端）：只能查看，不能操作（仅监督）
  - Regulator（监管者）：可以移动处刑台 + 管理刑具（唯一可以管理刑具的角色）
  - Warden（典狱长）：只能移动处刑台

### 技术细节
- ✅ 固定盐值：`Yipintianxia_MiddleRingRoad_2025`
- ✅ 固定哈希：`0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976`（对应密码 123456）
- ✅ 资格检查：状态="分配至岛屿"、有囚犯编号、有批次ID、属于监管员岛屿、无现有账号
- ✅ 事务保证：User 和 UserWitch 记录原子性创建

### 文件变更
- 📦 新增 `.kiro/specs/witch-account-creation/requirements.md` - 需求文档
- 📦 新增 `.kiro/specs/witch-account-creation/design.md` - 设计文档
- 📦 新增 `.kiro/specs/witch-account-creation/tasks.md` - 任务清单
- 📦 新增 `.kiro/specs/execution-platform-management/requirements.md` - 处刑台管理需求文档
- 📦 新增 `.kiro/specs/execution-platform-management/design.md` - 处刑台管理设计文档
- 📦 新增 `.kiro/specs/execution-platform-management/tasks.md` - 处刑台管理任务清单
- 📦 新增 `UI/ExecutionPlatformManagementForm.cs` - 处刑台管理主界面
- 📦 新增 `UI/PlatformMoveDialog.cs` - 移动对话框
- 📦 新增 `UI/ToolManagementDialog.cs` - 刑具管理对话框
- 📦 新增 `UI/MovementLogViewForm.cs` - 移动记录查看界面
- 📝 更新 `DAL/UserDAL.cs`
- 📝 更新 `BLL/UserBLL.cs`
- 📝 更新 `Form1_Regulator.cs`
- 📝 更新 `Form1_Warden.cs`
- 📝 更新 `Form1_Admin.cs`
- 📝 更新 `UI/DashboardForm.cs`

---

## [1.5.0] - 2024-12-03 🎉

### 🚀 重大突破：前后端联动完整实现

本版本实现了完整的前后端数据联动，攻克了课设开发中最难的技术难关！

### 新增
- ✨ **存储过程完整实现**
  - 创建 `wt.sp_UpdateWitchComplete` 存储过程
  - 支持42字段完整更新
  - 完善的事务处理和错误处理机制
- ✨ **时间字段扩展**
  - 新增 CaptureTime（抓捕时间）
  - 新增 DepartureTime（离开时间）
  - 新增 ArrivalTime（到达时间）
  - 新增 DeathTime（死亡时间）
- ✨ **完整文档体系**
  - 新增 `数据库结构文档.md` - 11张表86个字段的详细说明
  - 新增 `系统界面跳转与权限层级说明.md` - 完整的界面关系和权限说明
  - 新增 `数据库部署说明.md` - 详细的数据库部署指南
  - 新增 `文件整理说明.md` - 项目文件整理记录
- ✨ **数据库部署优化**
  - 提供完整的 .mdf 和 .ldf 数据库文件
  - 创建 attach_database.sql 附加脚本
  - 创建 detach_database.sql 分离脚本
  - 支持一键部署完整数据库

### 改进
- 🔧 **DAL层优化**
  - 修正 WitchDAL.UpdateWitchComplete 参数映射
  - 优化参数顺序与存储过程对应
  - 完善数据类型转换
- 🔧 **数据库修正**
  - 修正表名：wt.Witch（而非 wt.Witches）
  - 修正数据库名：WitchTrialWT（而非 WitchTrialDB）
  - 统一命名规范
- 📁 **项目文件整理**
  - 创建 sql_archive/ 归档历史SQL脚本（60+个文件）
  - 创建 docs_archive/ 归档历史文档（40+个文件）
  - 创建 scripts_archive/ 归档PowerShell脚本
  - 根目录只保留核心文档和代码
- 📝 **README 重写**
  - 详细的项目介绍和功能说明
  - 完整的技术架构图
  - 清晰的快速开始指南
  - 美观的排版和徽章

### 技术突破
- ✅ UI层 → BLL层 → DAL层 → 存储过程 完整打通
- ✅ 42字段数据完整验证和更新
- ✅ 教育经历和工作经历 JSON 序列化
- ✅ 数据一致性和完整性保障

### 文件变更
- 📦 新增 `database_scripts/sp_UpdateWitchComplete.sql`
- 📦 新增 `database_scripts/add_witch_time_fields.sql`
- 📦 新增 `database_scripts/attach_database.sql`
- 📦 新增 `database_scripts/detach_database.sql`
- 📦 新增 `database_scripts/数据库部署说明.md`
- 📦 新增 `数据库结构文档.md`
- 📦 新增 `系统界面跳转与权限层级说明.md`
- 📦 新增 `文件整理说明.md`
- 📦 新增 `Data/WitchTrialWT.mdf` 和 `Data/WitchTrialWT_log.ldf`
- 📁 新增 `sql_archive/` 文件夹（归档60+个SQL文件）
- 📁 新增 `docs_archive/` 文件夹（归档40+个文档）
- 📁 新增 `scripts_archive/` 文件夹（归档脚本文件）
- 📝 重写 `README.md` 为详细版本
- 📝 更新 `CHANGELOG.md`

---

## [1.3.0] - 2024-11-26

### 新增
- ✨ **五子棋认输功能**
  - 在"魔法"和"伪证"按钮中间新增"疑问"按钮
  - 点击后弹出确认对话框："魔法：疑问\n[当前玩家]请求认输，[对手名字]是否同意？"
  - 对手同意则认输成功，记录对局结果并更新积分
  - 对手不同意则继续游戏
- ✨ **项目文档优化**
  - 新增 `项目架构说明.md` - 详细的项目架构和技术说明
  - 新增 `快速开始指南.md` - 5分钟快速部署指南
  - 更新 `README.md` 版本信息和功能描述
  - 优化文档结构，提升可读性

### 改进
- ✨ **代码清理** 
  - 删除重复的数据库脚本文件
  - 删除过时的PowerShell脚本
  - 删除无用的日志文件和备份文件
  - 整理项目文件结构
- ✨ **文档整理**
  - 合并重复的说明文档
  - 统一文档格式和命名规范
  - 添加版本历史追踪

### 文件变更
- 📦 新增 `项目架构说明.md`
- 📦 新增 `快速开始指南.md`
- 🗑️ 删除过时文件（见下方删除列表）
- 📝 更新 `README.md` 和 `CHANGELOG.md`

### 删除的文件
```
- import_13_witches_full_data.sql (空文件)
- import_log1.txt, import_log2.txt, import_log3.txt (空日志)
- 数据库完整初始化脚本_步骤1_基础结构.sql (重复)
- 数据库完整初始化脚本_步骤2_扩展字段.sql (重复)  
- 数据库完整初始化脚本_步骤3_导入详细数据.sql (重复)
- 数据库完整初始化脚本_步骤4_更新状态.sql (重复)
- batch2_import_utf8.ps1, batch2_powershell_import.ps1 (过时)
- GomokuBoardForm.cs.bak_20251126_173216 (备份文件)
- 编译错误修复说明.md, 代码整理完成报告.md (过时文档)
```

---

## [1.2.0] - 2024-11-20

### 新增
- ✨ **对局日志系统**
  - 完整的对局记录功能
  - 记录玩家囚人番号、对局时间、结果、分数变化
  - 支持筛选（全部对局/单个玩家/两个玩家）
  - 2200x1300 大窗口，优化列宽显示
  - 新增数据库表 `wt.GomokuMatchLog`
- ✨ **积分排行榜**
  - 显示所有魔女的五子棋积分排名
  - 前三名特殊标记（🥇🥈🥉）
  - 手机界面底部第二个按钮访问
- ✨ **管理面板优化**
  - 全屏显示模式
  - 工具条高度增加，按钮完整显示
  - 列宽优化（ID、囚犯编号、姓名等）
  - 囚犯编号列前置（第二列）
  - 描述列自动填充剩余空间
  - 隐藏图像路径列

### 改进
- ✨ 五子棋计时器精度优化（13ms间隔，显示毫秒）
- ✨ 五子棋界面字体改为方正小标宋
- ✨ 手机界面点击右上角X返回登录界面（而非退出程序）
- ✨ 五子棋模式选择界面返回按钮正确返回手机界面
- ✨ 选择对手时点击取消正确返回模式选择界面
- 🐛 修复窗口管理问题（使用 Hide/Show 模式）
- 🐛 修复对局日志窗口初始化错误

### 数据库变更
- 🗄️ 新增表 `wt.GomokuMatchLog`（对局日志）
  - MatchID（对局序号）
  - Player1Username, Player1Name（玩家1信息）
  - Player2Username, Player2Name（玩家2信息）
  - StartTime, EndTime（对局时间）
  - Player1Result, Player1ScoreChange（玩家1结果）
  - Player2Result, Player2ScoreChange（玩家2结果）
  - TotalMoves, Duration（对局统计）

### 文件变更
- 📦 新增 `UI/GomokuMatchLogForm.cs` - 对局日志界面
- 📦 新增 `DAL/GomokuMatchLogDAL.cs` - 对局日志数据访问
- 📦 新增 `create_gomoku_match_log_table.sql` - 创建对局日志表
- 📝 更新所有 Markdown 文档

---

## [1.1.0] - 2024-11-20

### 新增
- ✨ **五子棋对弈系统**
  - 单设备对弈模式（本地双人）
  - 对手选择与密码验证
  - 15x15 标准棋盘绘制
  - 落子逻辑与交互
  - 五子连珠自动判定（横、竖、斜四个方向）
  - 实时计时系统（步时 + 局时）
  - 积分系统（胜 +10，负 -5）
  - 悔棋功能（魔法按钮 - 时间回溯）
  - 和棋功能（伪证按钮 - 宣称均势）
- 📦 新增文件：
  - `UI/GomokuBoardForm.cs` - 五子棋棋盘界面
  - `UI/GomokuModeForm.cs` - 五子棋模式选择界面
  - `DAL/Models/UserProfile.cs` - 用户档案模型
  - `add_gomoku_score_column.sql` - 数据库更新脚本
  - `五子棋功能说明.md` - 功能详细说明
  - `五子棋测试指南.md` - 测试指南

### 改进
- ✨ 手机界面新增五子棋入口（底部左下角红色图标）
- ✨ UserProfileDAL 新增获取和更新用户档案方法
- 📚 更新 README.md，添加五子棋功能说明

### 数据库变更
- 🗄️ User 表新增 `GomokuScore` 字段（INT，默认值 0）

---

## [1.0.1] - 2024-11-12

### 修复
- 🐛 修复管理员页面"退出登录"按钮错误跳转到修改密码页面的问题
- 🐛 修复手机界面右上角退出按钮直接关闭程序的问题，现在会返回登录界面

### 改进
- ✨ 统一所有退出按钮的行为：返回登录界面而不是关闭程序

---

## [1.0.0] - 2024-11-12

### 新增
- ✨ 初始版本发布
- ✨ 登录系统（支持不同角色）
- ✨ 魔女手机界面
- ✨ 图鉴系统（人物、证物、地图、规定、记录）
- ✨ 管理员面板
- ✨ 数据库配置化
- ✨ Git 版本控制
- 📚 完善的项目文档
