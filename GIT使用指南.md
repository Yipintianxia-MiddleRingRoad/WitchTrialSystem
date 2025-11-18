# Git 使用指南（给小组成员）

## 第一次使用 Git

### 1. 安装 Git

下载并安装 Git：https://git-scm.com/downloads

### 2. 配置 Git（只需要做一次）

打开命令行（CMD 或 PowerShell），输入：

```bash
git config --global user.name "你的名字"
git config --global user.email "你的邮箱"
```

## 项目负责人：上传项目到 GitHub

### 1. 在 GitHub 创建仓库

1. 登录 GitHub
2. 点击右上角 "+" → "New repository"
3. 填写仓库名（如 `WitchTrialSystem`）
4. 选择 Private（私有）或 Public（公开）
5. **不要**勾选 "Initialize this repository with a README"
6. 点击 "Create repository"

### 2. 上传现有项目

在项目文件夹中打开命令行，执行：

```bash
# 初始化 Git 仓库
git init

# 添加所有文件
git add .

# 提交
git commit -m "初始提交：魔女审判系统"

# 关联远程仓库（替换成你的仓库地址）
git remote add origin https://github.com/你的用户名/WitchTrialSystem.git

# 推送到 GitHub
git push -u origin main
```

如果提示分支名是 `master` 而不是 `main`，执行：
```bash
git branch -M main
git push -u origin main
```

### 3. 邀请小组成员

1. 在 GitHub 仓库页面，点击 "Settings"
2. 点击左侧 "Collaborators"
3. 点击 "Add people"
4. 输入小组成员的 GitHub 用户名或邮箱
5. 发送邀请

## 小组成员：克隆项目

### 1. 克隆仓库

```bash
# 克隆项目到本地（替换成实际的仓库地址）
git clone https://github.com/你的用户名/WitchTrialSystem.git

# 进入项目文件夹
cd WitchTrialSystem
```

### 2. 配置数据库

1. 复制配置文件：
   ```bash
   copy appsettings.example.json appsettings.json
   ```

2. 编辑 `appsettings.json`，修改为你自己的数据库连接

3. **重要**：`appsettings.json` 已经在 `.gitignore` 中，不会被上传到 Git，所以每个人的数据库配置互不影响

### 3. 运行项目

```bash
dotnet run
```

## 日常协作流程

### 开始工作前：拉取最新代码

```bash
# 拉取最新代码
git pull
```

### 完成工作后：提交代码

```bash
# 查看修改了哪些文件
git status

# 添加修改的文件
git add .

# 提交（写清楚你做了什么）
git commit -m "描述你的修改，例如：添加了证物管理功能"

# 推送到 GitHub
git push
```

### 提交信息示例

- `git commit -m "修复：登录界面输入框位置问题"`
- `git commit -m "新增：手机界面和图鉴导航功能"`
- `git commit -m "优化：数据库连接配置改为读取配置文件"`
- `git commit -m "文档：更新 README 安装说明"`

## 常见问题

### Q: 提示 "fatal: not a git repository"

A: 你不在 Git 仓库文件夹中，请先 `cd` 到项目文件夹。

### Q: 提示 "Permission denied"

A: 你没有权限推送代码，请联系项目负责人邀请你成为协作者。

### Q: 提示 "conflict"（冲突）

A: 多人同时修改了同一个文件，需要手动解决冲突：

1. 打开冲突的文件，会看到类似这样的标记：
   ```
   <<<<<<< HEAD
   你的代码
   =======
   别人的代码
   >>>>>>> branch-name
   ```

2. 手动选择保留哪部分代码，删除标记

3. 重新提交：
   ```bash
   git add .
   git commit -m "解决冲突"
   git push
   ```

### Q: 不小心提交了 appsettings.json 怎么办？

A: 从 Git 中删除（但保留本地文件）：

```bash
git rm --cached appsettings.json
git commit -m "删除配置文件"
git push
```

## 分支管理（可选，适合大项目）

### 创建功能分支

```bash
# 创建并切换到新分支
git checkout -b feature/新功能名称

# 在新分支上工作...

# 提交到新分支
git push -u origin feature/新功能名称
```

### 合并分支

```bash
# 切换回主分支
git checkout main

# 合并功能分支
git merge feature/新功能名称

# 推送
git push
```

## 推荐工具

- **GitHub Desktop**：图形化 Git 工具，适合不熟悉命令行的同学
  - 下载：https://desktop.github.com/
  
- **Visual Studio 内置 Git**：VS 2022 自带 Git 功能，可以在 IDE 中直接操作

## 小组协作建议

1. **每天开始工作前先 `git pull`**，避免代码冲突
2. **经常提交**，不要攒一大堆修改再提交
3. **写清楚提交信息**，方便其他人了解你的修改
4. **不要提交个人配置文件**（如 `appsettings.json`、`*.user` 等）
5. **遇到问题及时沟通**，不要自己瞎改导致代码混乱

## 紧急情况：撤销操作

### 撤销最后一次提交（还没 push）

```bash
git reset --soft HEAD~1
```

### 放弃本地所有修改

```bash
git reset --hard HEAD
git pull
```

**警告**：这会丢失所有未提交的修改！

## 需要帮助？

- Git 官方文档：https://git-scm.com/doc
- GitHub 帮助：https://docs.github.com/
- 或者问小组其他成员 😊
