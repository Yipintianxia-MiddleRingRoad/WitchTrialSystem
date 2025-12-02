# SQL 脚本归档说明

本文件夹包含项目开发过程中使用的所有 SQL 脚本文件。

## 脚本分类

### 数据库初始化
- database_init.sql - 数据库初始化主脚本

### 表结构修改
- add_witch_extended_fields.sql - 添加魔女扩展字段
- add_gomoku_score_column.sql - 添加五子棋积分字段
- 添加用户头像列.sql - 添加用户头像字段
- 创建角色详细信息表.sql - 创建详细信息表
- create_gomoku_match_log_table.sql - 创建五子棋对局日志表

### 批次数据导入
- batch2_step1_create_batch.sql - 批次2步骤1：创建批次
- batch2_step2_import_witches_basic.sql - 批次2步骤2：导入基本信息
- batch2_step3_import_details_part1.sql - 批次2步骤3：导入详细信息（第1部分）
- batch2_step3_import_details_part2.sql - 批次2步骤3：导入详细信息（第2部分）
- batch2_step3_import_details_part3.sql - 批次2步骤3：导入详细信息（第3部分）
- batch2_step4_create_users.sql - 批次2步骤4：创建用户账号
- batch2_quick_import.sql - 批次2快速导入
- batch2_simple_import.sql - 批次2简单导入
- batch2_detail_complete.sql - 批次2详细信息完整导入
- batch2_update_public_descriptions.sql - 批次2更新公开描述

### 魔女数据导入
- 完整导入13位魔女数据.sql - 导入13位魔女完整数据
- 批次5魔女完整导入.sql - 批次5魔女导入
- 导入684-696完整信息.sql - 导入684-696号魔女
- 补充692-696详细信息.sql - 补充692-696号详细信息
- 岛屿2魔女详细档案补充.sql - 岛屿2魔女档案补充
- 魔女详细档案导入模板.sql - 魔女档案导入模板

### 双岛屿扩展
- 双岛屿扩展_684_696.sql - 双岛屿系统扩展

### 数据修复脚本
- fix_batch2_duplicates.sql - 修复批次2重复数据
- 修复批次编号.sql - 修复批次编号
- 正确修复批次编号.sql - 正确修复批次编号
- 安全修复批次编号.sql - 安全修复批次编号
- 简单修复批次.sql - 简单修复批次
- 重置批次ID修复.sql - 重置批次ID修复
- 数据库问题修复脚本.sql - 数据库问题修复
- 清理重复魔女数据.sql - 清理重复魔女
- 删除重复的批次3魔女.sql - 删除批次3重复数据

### 岛屿2重新编号
- 重新编号岛屿2魔女.sql - 岛屿2魔女重新编号
- 重新编号岛屿2魔女_修正版.sql - 岛屿2魔女重新编号（修正版）
- 重新编号岛屿2魔女_批量.sql - 岛屿2魔女批量重新编号
- 完成岛屿2重新编号.sql - 完成岛屿2重新编号

### 状态更新
- update_witch_status.sql - 更新魔女状态
- 修改岛屿1批次2状态.sql - 修改岛屿1批次2状态
- 修改岛屿2批次4状态.sql - 修改岛屿2批次4状态

### 头像设置
- 更新管理员头像.sql - 更新管理员头像
- 更新管理员头像_新版.sql - 更新管理员头像（新版）
- 设置所有管理员头像.sql - 设置所有管理员头像
- update_avatar_path_to_prisoner_no.sql - 更新头像路径为囚犯编号

### 密码设置
- 设置新账号密码_684_696.sql - 设置684-696号账号密码
- 设置批次5密码_697_709.sql - 设置批次5密码（697-709号）

### 描述更新
- update_descriptions_with_linebreaks.sql - 更新描述（带换行）

### 查询和检查
- check_batch_status.sql - 检查批次状态
- final_status_check.sql - 最终状态检查
- 检查重复魔女.sql - 检查重复魔女
- 数据库结构查询命令.sql - 数据库结构查询

---

**注意**: 
- 这些脚本大多数是历史脚本，已经执行过
- 执行前请务必备份数据库
- 新的数据库脚本请放在 `database_scripts` 文件夹
