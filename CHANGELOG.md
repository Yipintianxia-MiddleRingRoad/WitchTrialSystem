# WitchTrialSystem 更新日志

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

### 技术细节
- ✅ 固定盐值：`Yipintianxia_MiddleRingRoad_2025`
- ✅ 固定哈希：`0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976`（对应密码 123456）
- ✅ 资格检查：状态="分配至岛屿"、有囚犯编号、有批次ID、属于监管员岛屿、无现有账号
- ✅ 事务保证：User 和 UserWitch 记录原子性创建

### 文件变更
- 📦 新增 `.kiro/specs/witch-account-creation/requirements.md` - 需求文档
- 📦 新增 `.kiro/specs/witch-account-creation/design.md` - 设计文档
- 📦 新增 `.kiro/specs/witch-account-creation/tasks.md` - 任务清单
- 📝 更新 `DAL/UserDAL.cs`
- 📝 更新 `BLL/UserBLL.cs`
- 📝 更新 `Form1_Regulator.cs`
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
