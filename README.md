# 魔女审判资料管理系统

[![Version](https://img.shields.io/badge/version-1.0.1-blue.svg)](https://github.com/Yipintianxia-MiddleRingRoad/WitchTrialSystem/releases)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-Educational-green.svg)](LICENSE)

数据库课程设计项目 - 基于 C# WinForms 和 SQL Server 的魔女审判管理系统

**当前版本：V1.0.1**

## 项目简介

这是一个魔女审判主题的资料管理系统，包含：
- 用户登录与权限管理
- 魔女图鉴系统（人物、证物、地图、规定、记录）
- 手机界面（普通魔女用户）
- 管理面板（管理员/典狱长/梅露露）

## 环境要求

- .NET 8.0 或更高版本
- SQL Server 2019 或更高版本（支持 LocalDB、Express、完整版）
- Windows 操作系统

## 安装步骤

### 1. 克隆项目

```bash
git clone <你的仓库地址>
cd WitchTrialSystem
```

### 2. 配置数据库连接

1. 复制配置文件模板：
   ```bash
   copy appsettings.example.json appsettings.json
   ```

2. 编辑 `appsettings.json`，修改数据库连接字符串：

   **方式一：Windows 身份验证（推荐）**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=WitchTrialWT;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

   **方式二：SQL Server 身份验证**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=WitchTrialWT;User ID=sa;Password=你的密码;TrustServerCertificate=True;"
     }
   }
   ```

   **说明：**
   - `Server=.` 表示本地默认实例
   - 如果使用命名实例（如 SQLEXPRESS），改为 `Server=.\\SQLEXPRESS`
   - 如果连接远程服务器，改为 `Server=服务器IP或名称`

### 3. 创建数据库

在 SQL Server Management Studio (SSMS) 中执行以下步骤：

1. 连接到你的 SQL Server 实例
2. 打开并执行项目中的数据库脚本（如果有 `database.sql` 或类似文件）
3. 确保数据库名为 `WitchTrialWT`

### 4. 准备资源文件

确保以下文件夹存在并包含必要的资源：

```
Images/
  ├── ui/
  │   ├── login_bg.png          # 登录界面背景
  │   ├── phone_bg.png          # 手机界面背景
  │   ├── pokedex_bg.png        # 图鉴背景
  │   ├── evidence_bg.png       # 证物背景
  │   ├── map_bg.png            # 地图背景
  │   ├── rules_bg.png          # 规定背景
  │   └── records_bg.png        # 记录背景
  ├── characters/               # 角色姓名图片
  └── _placeholder.png          # 占位图

Fonts/
  └── 方正小标宋简.ttf          # 自定义字体
```

### 5. 运行项目

```bash
dotnet run
```

或在 Visual Studio 中按 F5 运行。

## 默认账号

系统会自动初始化默认密码为 `123456`，具体账号请查看数据库中的 `wt.[User]` 表。

## 项目结构

```
WitchTrialSystem/
├── BLL/                    # 业务逻辑层
├── DAL/                    # 数据访问层
│   └── DBHelper.cs        # 数据库帮助类
├── UI/                     # 用户界面层
│   ├── LoginForm.cs       # 登录界面
│   ├── PhoneForm.cs       # 手机界面
│   ├── PokedexForm.cs     # 图鉴·人物
│   ├── BasePokedexForm.cs # 图鉴基类
│   ├── EvidenceForm.cs    # 图鉴·证物
│   ├── MapForm.cs         # 图鉴·地图
│   ├── RulesForm.cs       # 图鉴·规定
│   └── RecordsForm.cs     # 图鉴·记录
├── Images/                 # 图片资源
├── Fonts/                  # 字体资源
├── Form1.cs               # 管理面板
├── Program.cs             # 程序入口
└── appsettings.json       # 配置文件（不上传到 Git）
```

## 功能说明

### 用户角色

- **Witch（魔女）**：普通用户，登录后进入手机界面，可查看图鉴
- **Admin/Warden/Meruru**：管理员，登录后进入管理面板，可管理数据

### 图鉴系统

- **人物**：查看魔女信息（姓名、囚犯编号、魔法、描述）
- **证物**：查看证物信息
- **地图**：查看地图信息
- **规定**：查看规定信息
- **记录**：查看记录信息

### 导航

- 图鉴页面右侧有导航按钮，可在五个页面间切换
- 右上角退出按钮返回手机界面（魔女）或登录界面（管理员）

## 常见问题

### Q: 提示"未找到配置文件 appsettings.json"

A: 请按照"安装步骤"第2步创建配置文件。

### Q: 提示"SQL连接失败"

A: 检查以下几点：
1. SQL Server 服务是否启动
2. 数据库名称是否正确（WitchTrialWT）
3. 连接字符串中的服务器名是否正确
4. 如果使用 SQL 登录，用户名密码是否正确

### Q: 图片不显示

A: 确保 `Images/` 文件夹中有对应的图片文件，并且路径正确。

### Q: 字体显示异常

A: 确保 `Fonts/方正小标宋简.ttf` 文件存在。

## 开发团队

- 数据库课程设计小组

## 许可证

本项目仅用于教学目的。
