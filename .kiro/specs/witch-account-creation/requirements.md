# Requirements Document

## Introduction

本文档定义了"监管员为魔女创建账号"功能的需求。该功能允许监管员（Regulator）为已分配到本岛屿但尚未创建用户账号的魔女批量或单独创建登录账号，账号名为囚犯编号，默认密码为123456。

## Glossary

- **System**: 魔女审判系统（WitchTrialSystem）
- **Regulator**: 监管员角色，负责管理特定岛屿的魔女
- **Witch**: 魔女，系统中被管理的对象
- **User Account**: 用户账号，用于登录系统的凭证
- **PrisonerNo**: 囚犯编号，魔女的唯一标识符，也用作账号名
- **PENDING**: 待初始化状态，表示账号已创建但密码尚未设置
- **UserWitch Table**: 用户与魔女的关联表
- **Context Menu**: 右键菜单，用户在数据网格中右键点击时显示的操作菜单

## Requirements

### Requirement 1

**User Story:** As a Regulator, I want to create user accounts for witches assigned to my island who don't have accounts yet, so that they can log into the system.

#### Acceptance Criteria

1. WHEN a Regulator right-clicks on a witch in the data grid THEN the System SHALL display a context menu with account management options
2. WHEN the witch has status "分配至岛屿" AND has a PrisonerNo AND has a BatchID AND belongs to the Regulator's island AND does not have a User Account THEN the System SHALL enable the "创建账号" menu item
3. WHEN the witch does not meet the conditions in criterion 2 THEN the System SHALL disable or hide the "创建账号" menu item
4. WHEN a Regulator clicks "创建账号" for an eligible witch THEN the System SHALL create a User Account with Username equal to PrisonerNo
5. WHEN creating a User Account THEN the System SHALL set the password to "123456" using the fixed salt "Yipintianxia_MiddleRingRoad_2025" and hash "0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976"

### Requirement 2

**User Story:** As a Regulator, I want the system to validate witch eligibility before creating accounts, so that only appropriate witches receive accounts.

#### Acceptance Criteria

1. WHEN checking witch eligibility THEN the System SHALL verify the witch status is "分配至岛屿"
2. WHEN checking witch eligibility THEN the System SHALL verify the witch has a non-null and non-empty PrisonerNo
3. WHEN checking witch eligibility THEN the System SHALL verify the witch has a non-null BatchID
4. WHEN checking witch eligibility THEN the System SHALL verify the witch belongs to the Regulator's assigned island
5. WHEN checking witch eligibility THEN the System SHALL verify no User Account exists with Username matching the witch's PrisonerNo

### Requirement 3

**User Story:** As a Regulator, I want the system to create the complete account structure, so that witches can immediately log in after account creation.

#### Acceptance Criteria

1. WHEN creating a User Account THEN the System SHALL insert a record into the wt.[User] table with Username, PasswordHash, Salt, RoleID, IslandID, BatchID, and GomokuScore fields
2. WHEN creating a User Account THEN the System SHALL set RoleID to the Witch role identifier
3. WHEN creating a User Account THEN the System SHALL set IslandID to the witch's IslandID
4. WHEN creating a User Account THEN the System SHALL set BatchID to the witch's BatchID
5. WHEN creating a User Account THEN the System SHALL set GomokuScore to 0
6. WHEN a User Account is created THEN the System SHALL create a corresponding record in the wt.UserWitch table linking the UserID to the WitchID

### Requirement 4

**User Story:** As a Regulator, I want to receive clear feedback after attempting to create an account, so that I know whether the operation succeeded or failed.

#### Acceptance Criteria

1. WHEN account creation succeeds THEN the System SHALL display a success message showing the created username and default password
2. WHEN account creation fails due to duplicate username THEN the System SHALL display an error message indicating the username already exists
3. WHEN account creation fails due to database error THEN the System SHALL display an error message with the failure reason
4. WHEN account creation succeeds THEN the System SHALL refresh the data grid to reflect the updated state
5. WHEN the Regulator attempts to create an account for an ineligible witch THEN the System SHALL display a message explaining why the witch is ineligible

### Requirement 5

**User Story:** As a Regulator, I want the account creation to be transactional, so that partial failures don't leave the database in an inconsistent state.

#### Acceptance Criteria

1. WHEN creating a User Account THEN the System SHALL execute the User table insert and UserWitch table insert within a single database transaction
2. WHEN any step of account creation fails THEN the System SHALL roll back all changes made during that transaction
3. WHEN the transaction rolls back THEN the System SHALL ensure no User record or UserWitch record is created
4. WHEN the transaction commits THEN the System SHALL ensure both User and UserWitch records are persisted
5. WHEN a database constraint violation occurs THEN the System SHALL handle the exception gracefully and display an appropriate error message

### Requirement 6

**User Story:** As a Regulator, I want to only create accounts for witches in my assigned island, so that I don't accidentally create accounts for witches I don't manage.

#### Acceptance Criteria

1. WHEN a Regulator views the witch list THEN the System SHALL filter witches to show only those belonging to the Regulator's assigned island
2. WHEN a Regulator attempts to create an account THEN the System SHALL verify the witch's IslandID matches the Regulator's assigned IslandID
3. WHEN the IslandID verification fails THEN the System SHALL reject the account creation and display a permission error message
4. WHEN the Regulator's assigned island changes THEN the System SHALL update the available witches accordingly
5. WHEN displaying the context menu THEN the System SHALL only show the "创建账号" option for witches belonging to the Regulator's island

### Requirement 7

**User Story:** As a system administrator, I want the default password to use a secure hashing mechanism, so that witch accounts are protected even with a simple default password.

#### Acceptance Criteria

1. WHEN hashing the default password THEN the System SHALL use SHA-256 algorithm
2. WHEN hashing the default password THEN the System SHALL concatenate the password with the salt before hashing
3. WHEN storing the password THEN the System SHALL store only the salt and hash, never the plaintext password
4. WHEN the fixed salt and hash are used THEN the System SHALL ensure they correspond to the password "123456"
5. WHEN a witch logs in with the default password THEN the System SHALL successfully verify the credentials using the stored salt and hash
