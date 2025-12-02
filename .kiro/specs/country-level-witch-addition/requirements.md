# Requirements Document

## Introduction

本需求文档定义了魔女审判系统的"国家层添加魔女"功能。当前系统只支持在 Form1 中添加魔女的基本信息（姓名、魔法、囚犯编号），无法录入完整的详细档案。本需求旨在为 Admin 角色提供一个完整的魔女档案录入界面，支持录入所有 38 个字段的详细信息。

## Glossary

- **System**: 魔女审判系统（WitchTrialSystem）
- **Admin**: 系统管理员角色，拥有最高权限
- **国家层添加**: 指添加魔女的完整详细档案，包含所有 38 个字段
- **岛屿层添加**: 指当前 Form1 中的简单添加功能，只包含基本信息
- **Witch 表**: 数据库中存储魔女信息的表，包含 38 个字段
- **WitchAddForm**: 新创建的完整魔女添加表单
- **JSON 字段**: EducationHistory 和 WorkHistory 字段，以 JSON 格式存储数组数据
- **批次限制**: 每个批次最多 13 位魔女的业务规则

## Requirements

### Requirement 1

**User Story:** 作为 Admin，我希望能够添加魔女的完整详细档案，以便建立完整的魔女信息数据库。

#### Acceptance Criteria

1. WHEN Admin 点击"国家层添加魔女"按钮 THEN the system SHALL 打开 WitchAddForm 表单
2. WHEN WitchAddForm 打开 THEN the system SHALL 显示包含所有 38 个字段的输入界面
3. WHEN Admin 填写完所有必填字段并点击保存 THEN the system SHALL 验证数据完整性
4. WHEN 数据验证通过 THEN the system SHALL 将魔女信息保存到数据库
5. WHEN 保存成功 THEN the system SHALL 显示成功消息并关闭表单

### Requirement 2

**User Story:** 作为 Admin，我希望输入界面按照逻辑分组，以便清晰地录入不同类别的信息。

#### Acceptance Criteria

1. WHEN WitchAddForm 显示 THEN the system SHALL 使用 TabControl 将字段分为 9 个标签页
2. WHEN 显示"基本信息"标签页 THEN the system SHALL 包含姓名、囚犯编号、个人番号、性别、出生日期、民族、籍贯、曾用名字段
3. WHEN 显示"身体特征"标签页 THEN the system SHALL 包含身高、体重、血型字段
4. WHEN 显示"联系方式"标签页 THEN the system SHALL 包含地址、电话、邮箱、LINE账号字段
5. WHEN 显示"教育背景"标签页 THEN the system SHALL 包含最高学历和教育经历列表
6. WHEN 显示"工作经历"标签页 THEN the system SHALL 包含工作经历列表
7. WHEN 显示"家庭关系"标签页 THEN the system SHALL 包含家庭结构、父亲、母亲、其他家庭成员字段
8. WHEN 显示"个性特征"标签页 THEN the system SHALL 包含技能特长、兴趣爱好、理想、讨厌的事物、心理创伤字段
9. WHEN 显示"魔女信息"标签页 THEN the system SHALL 包含魔法、状态（待分配/分配至岛屿/审判中/死亡(正常)/死亡(魔女化)/其它）、魔女化办法、备注、公开描述字段
10. WHEN 显示"分配信息"标签页 THEN the system SHALL 包含岛屿、批次、头像路径字段

### Requirement 3

**User Story:** 作为 Admin，我希望能够添加多条教育经历和工作经历，以便记录魔女的完整履历。

#### Acceptance Criteria

1. WHEN Admin 在"教育背景"标签页点击"添加"按钮 THEN the system SHALL 打开教育经历编辑对话框
2. WHEN Admin 填写教育经历信息并保存 THEN the system SHALL 将该条记录添加到教育经历列表
3. WHEN Admin 选中教育经历列表中的某条记录并点击"编辑" THEN the system SHALL 打开编辑对话框并显示该条记录
4. WHEN Admin 选中教育经历列表中的某条记录并点击"删除" THEN the system SHALL 从列表中移除该条记录
5. WHEN Admin 保存魔女信息 THEN the system SHALL 将教育经历列表序列化为 JSON 格式存储到 EducationHistory 字段
6. WHEN Admin 在"工作经历"标签页点击"添加"按钮 THEN the system SHALL 打开工作经历编辑对话框
7. WHEN Admin 填写工作经历信息并保存 THEN the system SHALL 将该条记录添加到工作经历列表
8. WHEN Admin 选中工作经历列表中的某条记录并点击"编辑" THEN the system SHALL 打开编辑对话框并显示该条记录
9. WHEN Admin 选中工作经历列表中的某条记录并点击"删除" THEN the system SHALL 从列表中移除该条记录
10. WHEN Admin 保存魔女信息 THEN the system SHALL 将工作经历列表序列化为 JSON 格式存储到 WorkHistory 字段

### Requirement 4

**User Story:** 作为 Admin，我希望系统验证必填字段和数据格式，以便确保数据的完整性和正确性。

#### Acceptance Criteria

1. WHEN Admin 点击保存按钮 THEN the system SHALL 验证姓名字段不为空
2. WHEN Admin 点击保存按钮 THEN the system SHALL 验证魔法字段不为空
3. WHEN Admin 点击保存按钮 THEN the system SHALL 验证状态字段已选择
4. WHEN Admin 未选择岛屿和批次 THEN the system SHALL 允许保存并将状态设置为"待分配"
5. WHEN Admin 选择了岛屿但未选择批次 THEN the system SHALL 显示错误消息"选择岛屿后必须选择批次"
6. WHEN Admin 输入身高 THEN the system SHALL 验证输入值为有效的数字格式
7. WHEN Admin 输入体重 THEN the system SHALL 验证输入值为有效的数字格式
8. WHEN Admin 输入出生日期 THEN the system SHALL 验证日期格式正确且不晚于当前日期
9. WHEN Admin 输入邮箱 THEN the system SHALL 验证邮箱格式正确
10. WHEN 任何验证失败 THEN the system SHALL 显示具体的错误消息并阻止保存

### Requirement 5

**User Story:** 作为 Admin，我希望系统检查批次人数限制，以便遵守每个批次最多 13 人的业务规则。

#### Acceptance Criteria

1. WHEN Admin 选择批次 THEN the system SHALL 查询该批次当前的魔女数量
2. WHEN 批次当前魔女数量小于 13 THEN the system SHALL 允许继续添加
3. WHEN 批次当前魔女数量等于 13 THEN the system SHALL 显示警告消息"该批次已满（13/13），无法继续添加"
4. WHEN 批次已满 THEN the system SHALL 禁用保存按钮
5. WHEN Admin 切换到未满的批次 THEN the system SHALL 重新启用保存按钮
6. WHEN Admin 未选择岛屿和批次（状态为"待分配"） THEN the system SHALL 跳过批次人数检查

### Requirement 6

**User Story:** 作为开发者，我希望创建存储过程来处理完整魔女信息的插入，以便在数据库层统一管理业务逻辑。

#### Acceptance Criteria

1. WHEN 存储过程 sp_AddWitchComplete 被调用 THEN the system SHALL 接受所有 38 个字段作为参数
2. WHEN 存储过程执行 THEN the system SHALL 验证批次人数限制
3. WHEN 批次已满 THEN the system SHALL 抛出错误并回滚事务
4. WHEN 批次未满 THEN the system SHALL 插入魔女记录到 wt.Witch 表
5. WHEN 插入成功 THEN the system SHALL 返回新创建的 WitchID
6. WHEN 插入成功 THEN the system SHALL 自动更新批次的 WitchCount 字段
7. WHEN 任何步骤失败 THEN the system SHALL 回滚事务并返回错误信息

### Requirement 7

**User Story:** 作为 Admin，我希望在 Form1_Admin 中有明确的入口访问国家层添加功能，以便与简单添加功能区分开。

#### Acceptance Criteria

1. WHEN Form1_Admin 加载 THEN the system SHALL 在工具栏显示"国家层添加魔女"按钮
2. WHEN Admin 点击"国家层添加魔女"按钮 THEN the system SHALL 打开 WitchAddForm
3. WHEN WitchAddForm 关闭且保存成功 THEN the system SHALL 刷新 Form1_Admin 的数据网格
4. WHEN WitchAddForm 关闭且未保存 THEN the system SHALL 不刷新数据网格
5. WHEN "国家层添加魔女"按钮显示 THEN the system SHALL 与原有的"新增魔女"按钮并列显示

### Requirement 8

**User Story:** 作为 Admin，我希望系统自动创建对应的用户账号，以便新添加的魔女可以登录系统。

#### Acceptance Criteria

1. WHEN 魔女信息保存成功 THEN the system SHALL 检查是否需要创建用户账号
2. WHEN 囚犯编号不为空且状态为"分配至岛屿" THEN the system SHALL 使用囚犯编号作为用户名创建账号
3. WHEN 创建用户账号 THEN the system SHALL 设置密码为 PENDING 状态
4. WHEN 创建用户账号 THEN the system SHALL 设置角色为 Witch
5. WHEN 创建用户账号 THEN the system SHALL 设置岛屿和批次与魔女信息一致
6. WHEN 创建用户账号 THEN the system SHALL 在 UserWitch 表中建立关联关系
7. WHEN 用户名已存在 THEN the system SHALL 跳过账号创建并显示提示信息
8. WHEN 状态为"待分配" THEN the system SHALL 不创建用户账号
