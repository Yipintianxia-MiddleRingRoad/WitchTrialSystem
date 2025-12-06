# Design Document

## Overview

This document describes the design for the "Witch Account Creation" feature, which allows Regulators to create user accounts for witches assigned to their island who don't yet have login credentials. The feature integrates into the existing Form1_Regulator interface through a context menu option.

## Architecture

The feature follows the existing three-tier architecture:

```
UI Layer (Form1_Regulator)
    ↓
BLL Layer (UserBLL)
    ↓
DAL Layer (UserDAL)
    ↓
Database (wt.[User], wt.UserWitch)
```

**Key Components:**
- **Form1_Regulator**: Handles UI interactions, displays context menu, shows feedback messages
- **UserBLL**: Contains business logic for account creation, eligibility validation, and transaction management
- **UserDAL**: Executes database operations for checking account existence and creating User/UserWitch records
- **Security**: Provides password hashing utilities (already exists)

## Components and Interfaces

### UI Layer: Form1_Regulator

**New Methods:**
```csharp
private void CreateAccount_Click(object? sender, EventArgs e)
```
- Handles the "创建账号" context menu click event
- Validates witch selection
- Calls BLL to create account
- Displays success/error messages
- Refreshes the data grid

**Modified Methods:**
```csharp
private void InitializeContextMenu()
```
- Add new menu item "创建账号"
- Wire up CreateAccount_Click event handler

**Context Menu Item:**
- Text: "创建账号"
- Enabled: Only when witch is eligible (status="分配至岛屿", has PrisonerNo, has BatchID, no existing account)
- Position: After "分配批次" and before "修改状态"

### BLL Layer: UserBLL

**New Methods:**
```csharp
public (bool Success, string Message) CreateWitchAccount(
    string prisonerNo, 
    int islandId, 
    int batchId, 
    int regulatorIslandId)
```
- Validates witch eligibility
- Checks for existing account
- Creates User and UserWitch records in a transaction
- Returns success status and message

```csharp
public bool IsAccountEligible(
    string status, 
    string? prisonerNo, 
    int? batchId, 
    int witchIslandId, 
    int regulatorIslandId)
```
- Checks if witch meets all criteria for account creation
- Returns true if eligible, false otherwise

```csharp
private bool AccountExists(string username)
```
- Checks if a user account already exists with the given username
- Returns true if exists, false otherwise

### DAL Layer: UserDAL

**New Methods:**
```csharp
public bool UserExists(string username)
```
- Queries wt.[User] table to check if username exists
- Returns true if found, false otherwise

```csharp
public int CreateWitchAccountWithAssociation(
    string username, 
    int roleId, 
    int islandId, 
    int batchId, 
    string salt, 
    string hash, 
    int witchId)
```
- Inserts User record
- Inserts UserWitch association record
- Executes within a transaction
- Returns the new UserID on success, throws exception on failure

**Modified Methods:**
```csharp
public (int UserID, string Username, int RoleID, string Salt, string Hash)? GetByUsername(string username)
```
- No changes needed, already exists and works correctly

## Data Models

### User Table (wt.[User])
```sql
UserID INT PRIMARY KEY IDENTITY
Username NVARCHAR(50) UNIQUE NOT NULL
PasswordHash NVARCHAR(64) NOT NULL
Salt NVARCHAR(64) NOT NULL
RoleID INT NOT NULL
IslandID INT NULL
BatchID INT NULL
GomokuScore INT DEFAULT 0
```

### UserWitch Table (wt.UserWitch)
```sql
UserID INT PRIMARY KEY
WitchID INT NULL
FOREIGN KEY (UserID) REFERENCES wt.[User](UserID)
FOREIGN KEY (WitchID) REFERENCES wt.Witch(WitchID)
```

### Witch Table (wt.Witch) - Reference Only
```sql
WitchID INT PRIMARY KEY
PrisonerNo NVARCHAR(20) UNIQUE
Status NVARCHAR(20)
IslandID INT
BatchID INT
... (other fields)
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Eligibility determines menu item state
*For any* witch in the data grid, the "创建账号" menu item should be enabled if and only if the witch has status="分配至岛屿", non-empty PrisonerNo, non-null BatchID, belongs to Regulator's island, and has no existing account
**Validates: Requirements 1.2, 1.3, 2.1, 2.2, 2.3, 2.4, 2.5**

### Property 2: Created username matches PrisonerNo
*For any* eligible witch, when an account is created, the Username field in wt.[User] should equal the witch's PrisonerNo
**Validates: Requirements 1.4**

### Property 3: Fixed password credentials
*For any* created account, the Salt should be "Yipintianxia_MiddleRingRoad_2025" and PasswordHash should be "0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976"
**Validates: Requirements 1.5, 7.4**

### Property 4: Account inherits witch properties
*For any* created account, the IslandID, BatchID, and RoleID fields should match the witch's IslandID, BatchID, and the Witch role identifier respectively
**Validates: Requirements 3.2, 3.3, 3.4**

### Property 5: UserWitch association exists
*For any* created User account, a corresponding UserWitch record should exist linking the UserID to the WitchID
**Validates: Requirements 3.6**

### Property 6: Transaction atomicity
*For any* account creation attempt, either both User and UserWitch records are created, or neither is created (no partial state)
**Validates: Requirements 5.1, 5.2, 5.3, 5.4**

### Property 7: Island authorization
*For any* account creation attempt, if the witch's IslandID does not match the Regulator's assigned IslandID, the operation should be rejected
**Validates: Requirements 6.2, 6.3**

### Property 8: Duplicate prevention
*For any* account creation attempt, if a User with the same Username already exists, the operation should fail with an appropriate error message
**Validates: Requirements 2.5, 4.2**

### Property 9: Password verification round-trip
*For any* created account, logging in with username=PrisonerNo and password="123456" should succeed
**Validates: Requirements 7.5**

## Error Handling

### Validation Errors
- **Ineligible Witch**: Display message explaining which criteria are not met
  - Missing PrisonerNo: "该魔女没有囚犯编号，无法创建账号"
  - Wrong Status: "只能为状态为'分配至岛屿'的魔女创建账号"
  - Missing BatchID: "该魔女未分配批次，无法创建账号"
  - Wrong Island: "您只能为本岛屿的魔女创建账号"
  - Account Exists: "该魔女已有账号（用户名：{PrisonerNo}）"

### Database Errors
- **Duplicate Username**: "账号创建失败：用户名已存在"
- **Transaction Failure**: "账号创建失败：{exception.Message}"
- **Connection Error**: "数据库连接失败，请稍后重试"

### Success Messages
- **Account Created**: "账号创建成功！\n用户名：{PrisonerNo}\n默认密码：123456\n\n请提醒魔女首次登录后修改密码。"

## Testing Strategy

### Unit Tests
- Test `UserDAL.UserExists()` with existing and non-existing usernames
- Test `UserDAL.CreateWitchAccountWithAssociation()` with valid parameters
- Test `UserBLL.IsAccountEligible()` with various witch states
- Test `UserBLL.CreateWitchAccount()` success and failure scenarios
- Test transaction rollback on database errors

### Property-Based Tests
The following properties should be tested using a property-based testing framework (e.g., FsCheck for C#):

**Property Test 1: Eligibility check consistency**
- Generate random witch data with various combinations of status, PrisonerNo, BatchID, IslandID
- Verify `IsAccountEligible()` returns consistent results based on the criteria
- **Validates: Property 1**

**Property Test 2: Username equals PrisonerNo**
- Generate random eligible witches with various PrisonerNo values
- Create accounts and verify Username matches PrisonerNo
- **Validates: Property 2**

**Property Test 3: Fixed credentials**
- Create multiple accounts for different witches
- Verify all have the same Salt and PasswordHash
- **Validates: Property 3**

**Property Test 4: Property inheritance**
- Generate witches with various IslandID and BatchID values
- Create accounts and verify fields are correctly inherited
- **Validates: Property 4**

**Property Test 5: Association creation**
- Create accounts for random witches
- Verify UserWitch records exist for all created accounts
- **Validates: Property 5**

**Property Test 6: Transaction atomicity**
- Simulate database failures at various points
- Verify no partial records exist after failures
- **Validates: Property 6**

**Property Test 7: Authorization enforcement**
- Generate witches from different islands
- Attempt account creation with mismatched Regulator islands
- Verify all unauthorized attempts are rejected
- **Validates: Property 7**

**Property Test 8: Duplicate prevention**
- Create an account, then attempt to create again
- Verify second attempt fails appropriately
- **Validates: Property 8**

**Property Test 9: Login verification**
- Create accounts for random witches
- Verify all can log in with default password
- **Validates: Property 9**

### Integration Tests
- Test complete workflow: right-click → create account → verify login
- Test with real database connection
- Test UI feedback messages display correctly
- Test data grid refresh after account creation

### Edge Cases
- Witch with whitespace-only PrisonerNo
- Witch with very long PrisonerNo (boundary testing)
- Multiple Regulators creating accounts simultaneously (concurrency)
- Database connection loss during transaction
- Witch deleted after menu shown but before account created

## Security Considerations

### Password Security
- Default password "123456" is intentionally simple for initial setup
- Users should be prompted to change password on first login (future enhancement)
- Password is never stored in plaintext
- SHA-256 hashing with salt provides adequate security for default credentials

### Authorization
- Regulators can only create accounts for witches in their assigned island
- IslandID verification happens at both UI and BLL layers
- Database constraints prevent unauthorized data manipulation

### Audit Trail
- Consider logging account creation events (future enhancement)
- Log should include: timestamp, Regulator username, witch PrisonerNo, success/failure

## Performance Considerations

- Account creation is a low-frequency operation (typically done once per witch)
- Database queries are simple and indexed (Username, IslandID)
- Transaction overhead is minimal (2 inserts)
- UI should remain responsive during operation (< 1 second expected)

## Future Enhancements

1. **Batch Account Creation**: Allow creating accounts for all eligible witches at once
2. **Custom Password**: Allow Regulator to set a custom default password
3. **Password Reset**: Add ability to reset forgotten passwords
4. **Audit Logging**: Track all account creation activities
5. **Email Notification**: Send credentials to witch via email (if email available)
6. **Force Password Change**: Require password change on first login
