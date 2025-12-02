# Requirements Document

## Introduction

本需求文档定义了魔女审判系统的角色界面分离功能。当前系统使用单一的 Form1 界面服务所有管理角色（Admin、Regulator、Warden），通过运行时权限检查来控制功能访问。本需求旨在将 Form1 分离为三个独立的角色专用界面，并为 Regulator 角色添加编辑魔女公开描述的功能。

## Glossary

- **System**: 魔女审判系统（WitchTrialSystem）
- **Form1**: 当前的管理面板主界面
- **Admin**: 系统管理员角色，拥有最高权限
- **Regulator**: 监管员角色（Meruru 和 Utena），负责管理特定岛屿的魔女信息
- **Warden**: 典狱长角色，负责监狱管理
- **LoginForm**: 用户登录界面
- **DescriptionPublic**: 魔女的公开描述字段，存储在数据库 wt.Witch 表中
- **Island**: 岛屿，魔女被分配到不同的岛屿
- **WitchDAL**: 魔女数据访问层类
- **Role-based UI**: 基于角色的用户界面，不同角色使用不同的界面

## Requirements

### Requirement 1

**User Story:** 作为系统架构师，我希望将 Form1 分离为三个独立的角色界面，以便每个角色拥有专属的界面实现，提高代码可维护性。

#### Acceptance Criteria

1. WHEN the system starts THEN the system SHALL preserve the original Form1 files as backup with suffix "_Backup"
2. WHEN creating role-specific forms THEN the system SHALL create Form1_Admin.cs, Form1_Regulator.cs, and Form1_Warden.cs with their corresponding Designer and resx files
3. WHEN a role-specific form is instantiated THEN the form SHALL accept username as constructor parameter and set the role name internally
4. WHEN Form1_Admin is instantiated THEN the form SHALL set _roleName to "Admin"
5. WHEN Form1_Regulator is instantiated THEN the form SHALL set _roleName to "Regulator"
6. WHEN Form1_Warden is instantiated THEN the form SHALL set _roleName to "Warden"

### Requirement 2

**User Story:** 作为用户，我希望登录后根据我的角色自动跳转到对应的界面，以便获得符合我权限的操作体验。

#### Acceptance Criteria

1. WHEN a user with role "Admin" logs in successfully THEN the system SHALL navigate to Form1_Admin
2. WHEN a user with role "Meruru" logs in successfully THEN the system SHALL navigate to Form1_Regulator
3. WHEN a user with role "Utena" logs in successfully THEN the system SHALL navigate to Form1_Regulator
4. WHEN a user with role "Warden" logs in successfully THEN the system SHALL navigate to Form1_Warden
5. WHEN a user with role "Witch" logs in successfully THEN the system SHALL navigate to PhoneForm

### Requirement 3

**User Story:** 作为 Regulator，我希望能够编辑本岛屿魔女的公开描述，以便维护准确的魔女档案信息。

#### Acceptance Criteria

1. WHEN a Regulator right-clicks on a witch row in the data grid THEN the system SHALL display a context menu with "编辑公开描述" and "查看详情" options
2. WHEN a Regulator selects "编辑公开描述" for a witch from their assigned island THEN the system SHALL open WitchEditDescriptionForm with the current description
3. WHEN a Regulator selects "编辑公开描述" for a witch from a different island THEN the system SHALL display an error message "您只能编辑本岛屿的魔女信息"
4. WHEN a Regulator saves changes in WitchEditDescriptionForm THEN the system SHALL update the DescriptionPublic field in the database
5. WHEN the description is successfully updated THEN the system SHALL refresh the data grid to show the new description

### Requirement 4

**User Story:** 作为 Regulator，我希望使用一个专门的编辑窗口来修改公开描述，以便清晰地查看和编辑长文本内容。

#### Acceptance Criteria

1. WHEN WitchEditDescriptionForm is opened THEN the system SHALL display the witch name, prisoner number, and current description
2. WHEN the user types in the description text box THEN the system SHALL display a character count
3. WHEN the user clicks "保存修改" button THEN the system SHALL validate and save the description to the database
4. WHEN the save operation succeeds THEN the system SHALL display a success message and close the form with DialogResult.OK
5. WHEN the save operation fails THEN the system SHALL display an error message with the exception details

### Requirement 5

**User Story:** 作为开发者，我希望 WitchDAL 提供更新描述的方法，以便在数据访问层统一管理数据库操作。

#### Acceptance Criteria

1. WHEN WitchDAL.UpdateDescription is called with valid witchId and description THEN the system SHALL execute an UPDATE statement on wt.Witch table
2. WHEN the description parameter is null THEN the system SHALL store DBNull.Value in the database
3. WHEN the description parameter is not null THEN the system SHALL store the description string in the DescriptionPublic column
4. WHEN the update operation completes THEN the system SHALL commit the transaction
5. WHEN the update operation fails THEN the system SHALL throw an exception with error details

### Requirement 6

**User Story:** 作为系统管理员，我希望确保三个角色界面的核心功能保持一致，以便用户体验统一。

#### Acceptance Criteria

1. WHEN any role-specific form loads THEN the system SHALL display the user card with avatar, username, role, and Chinese name
2. WHEN any role-specific form loads THEN the system SHALL load islands, batches, and witch data according to user permissions
3. WHEN a user performs search or filter operations THEN the system SHALL apply the same logic across all role-specific forms
4. WHEN a user double-clicks a witch row THEN the system SHALL open WitchDetailForm with the selected witch's information
5. WHEN a user clicks refresh button THEN the system SHALL reload the data grid with current filter settings
