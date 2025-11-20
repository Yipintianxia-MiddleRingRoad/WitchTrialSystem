# Git 提交指南

## 准备工作

在提交到 GitHub 之前，请确保：

1. ✅ 所有代码已编译通过
2. ✅ 已测试主要功能
3. ✅ 已更新相关文档
4. ✅ 已检查 `.gitignore` 文件

## 提交步骤

### 1. 检查当前状态

```bash
git status
```

查看哪些文件被修改、新增或删除。

### 2. 添加文件到暂存区

```bash
# 添加所有修改的文件
git add .

# 或者选择性添加特定文件
git add README.md CHANGELOG.md
git add UI/GomokuMatchLogForm.cs
git add DAL/GomokuMatchLogDAL.cs
```

### 3. 提交更改

```bash
git commit -m "feat: 完善五子棋系统 - 添加对局日志和排行榜功能 (v1.2.0)"
```

提交信息建议格式：
- `feat:` - 新功能
- `fix:` - 修复bug
- `docs:` - 文档更新
- `style:` - 代码格式调整
- `refactor:` - 代码重构
- `perf:` - 性能优化
- `test:` - 测试相关

### 4. 推送到 GitHub

```bash
# 首次推送（如果还没有设置远程仓库）
git remote add origin https://github.com/Yipintianxia-MiddleRingRoad/WitchTrialSystem.git
git branch -M main
git push -u origin main

# 后续推送
git push
```

## 本次更新内容 (v1.2.0)

### 新增文件
- `UI/GomokuMatchLogForm.cs` - 对局日志界面
- `DAL/GomokuMatchLogDAL.cs` - 对局日志数据访问
- `create_gomoku_match_log_table.sql` - 创建对局日志表
- `GIT提交指南.md` - 本文件

### 修改文件
- `README.md` - 更新项目说明和功能介绍
- `CHANGELOG.md` - 添加 v1.2.0 更新日志
- `数据库初始化说明.md` - 完善数据库初始化步骤
- `Form1.cs` - 管理面板优化（全屏、列宽调整）
- `UI/PhoneForm.cs` - 添加对局日志和排行榜入口
- `UI/GomokuBoardForm.cs` - 计时器优化、字体优化
- `UI/GomokuModeForm.cs` - 返回逻辑修复
- `五子棋功能说明.md` - 更新功能说明
- `五子棋测试指南.md` - 更新测试指南

### 数据库变更
- 新增表 `wt.GomokuMatchLog`
- 新增字段 `wt.[User].GomokuScore`

## 提交建议

### 完整提交命令

```bash
# 1. 查看状态
git status

# 2. 添加所有文件
git add .

# 3. 提交
git commit -m "feat: 完善五子棋系统 (v1.2.0)

新增功能：
- 对局日志系统（记录、查询、筛选）
- 积分排行榜（显示前13名）
- 管理面板全屏优化

改进：
- 五子棋计时器精度优化（13ms）
- 界面字体改为方正小标宋
- 窗口管理逻辑优化
- 列宽和布局优化

数据库变更：
- 新增 wt.GomokuMatchLog 表
- 完善数据库初始化文档"

# 4. 推送
git push
```

### 分步提交（推荐）

如果更改较多，可以分多次提交：

```bash
# 提交数据库相关
git add create_gomoku_match_log_table.sql DAL/GomokuMatchLogDAL.cs
git commit -m "feat: 添加对局日志数据访问层"

# 提交界面相关
git add UI/GomokuMatchLogForm.cs UI/PhoneForm.cs
git commit -m "feat: 添加对局日志界面和排行榜"

# 提交优化相关
git add Form1.cs UI/GomokuBoardForm.cs UI/GomokuModeForm.cs
git commit -m "refactor: 优化管理面板和五子棋界面"

# 提交文档
git add README.md CHANGELOG.md 数据库初始化说明.md
git commit -m "docs: 更新项目文档 (v1.2.0)"

# 推送所有提交
git push
```

## 注意事项

### 不要提交的文件

以下文件已在 `.gitignore` 中，不应提交：

- `appsettings.json` - 包含敏感配置
- `bin/` - 编译输出
- `obj/` - 编译中间文件
- `.vs/` - Visual Studio 配置
- `*.user` - 用户特定配置

### 检查 .gitignore

确保 `.gitignore` 包含以下内容：

```
# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Ww][Ii][Nn]32/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio
.vs/
*.user
*.suo
*.userosscache
*.sln.docstates

# Configuration files with sensitive data
appsettings.json
appsettings.*.json
!appsettings.example.json

# Database files
*.mdf
*.ldf
```

## 创建 Release

在 GitHub 上创建新版本：

1. 进入仓库页面
2. 点击 "Releases" → "Create a new release"
3. 标签版本：`v1.2.0`
4. 发布标题：`v1.2.0 - 五子棋系统完善`
5. 描述：复制 CHANGELOG.md 中的 v1.2.0 内容
6. 点击 "Publish release"

## 常见问题

### Q: 提示"fatal: remote origin already exists"

A: 远程仓库已存在，直接推送即可：
```bash
git push
```

### Q: 提示"rejected - non-fast-forward"

A: 远程仓库有新的提交，需要先拉取：
```bash
git pull --rebase
git push
```

### Q: 如何撤销上一次提交？

A: 
```bash
# 撤销提交但保留更改
git reset --soft HEAD~1

# 撤销提交并丢弃更改（谨慎使用）
git reset --hard HEAD~1
```

### Q: 如何查看提交历史？

A:
```bash
git log --oneline --graph --all
```

## 参考资源

- [Git 官方文档](https://git-scm.com/doc)
- [GitHub 使用指南](https://docs.github.com/)
- [约定式提交](https://www.conventionalcommits.org/zh-hans/)
