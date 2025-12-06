# Implementation Plan

- [x] 1. Implement DAL layer for account management


  - Add methods to check account existence and create accounts with associations
  - _Requirements: 2.5, 3.1, 3.6, 5.1_

- [x] 1.1 Add UserExists method to UserDAL


  - Write method to query wt.[User] table by username
  - Return boolean indicating if account exists
  - _Requirements: 2.5_

- [x] 1.2 Add CreateWitchAccountWithAssociation method to UserDAL

  - Write method to insert User record with all required fields
  - Insert UserWitch association record
  - Wrap both inserts in a database transaction
  - Return new UserID on success
  - _Requirements: 3.1, 3.6, 5.1_

- [ ]* 1.3 Write property test for UserExists
  - **Property 8: Duplicate prevention**
  - **Validates: Requirements 2.5**

- [ ]* 1.4 Write unit tests for CreateWitchAccountWithAssociation
  - Test successful account creation
  - Test transaction rollback on failure
  - _Requirements: 3.1, 3.6, 5.1_

- [x] 2. Implement BLL layer for account creation logic


  - Add eligibility validation and account creation business logic
  - _Requirements: 1.4, 1.5, 2.1, 2.2, 2.3, 2.4, 2.5, 3.2, 3.3, 3.4, 3.5, 5.2, 5.3, 5.4, 6.2, 6.3_

- [x] 2.1 Add IsAccountEligible method to UserBLL


  - Validate witch status is "分配至岛屿"
  - Validate PrisonerNo is non-null and non-empty
  - Validate BatchID is non-null
  - Validate witch IslandID matches Regulator IslandID
  - Check if account already exists
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 6.2_

- [ ]* 2.2 Write property test for IsAccountEligible
  - **Property 1: Eligibility determines menu item state**
  - **Validates: Requirements 1.2, 1.3, 2.1, 2.2, 2.3, 2.4, 2.5**

- [x] 2.3 Add CreateWitchAccount method to UserBLL

  - Call IsAccountEligible to validate
  - Get Witch role ID from database
  - Use fixed salt and hash for password "123456"
  - Call DAL to create User and UserWitch records
  - Handle exceptions and return appropriate messages
  - _Requirements: 1.4, 1.5, 3.2, 3.3, 3.4, 3.5, 5.2, 5.3, 5.4, 6.3_

- [ ]* 2.4 Write property test for CreateWitchAccount username
  - **Property 2: Created username matches PrisonerNo**
  - **Validates: Requirements 1.4**

- [ ]* 2.5 Write property test for CreateWitchAccount credentials
  - **Property 3: Fixed password credentials**
  - **Validates: Requirements 1.5, 7.4**

- [ ]* 2.6 Write property test for CreateWitchAccount property inheritance
  - **Property 4: Account inherits witch properties**
  - **Validates: Requirements 3.2, 3.3, 3.4**

- [ ]* 2.7 Write property test for UserWitch association
  - **Property 5: UserWitch association exists**
  - **Validates: Requirements 3.6**

- [ ]* 2.8 Write property test for transaction atomicity
  - **Property 6: Transaction atomicity**
  - **Validates: Requirements 5.1, 5.2, 5.3, 5.4**

- [ ]* 2.9 Write property test for island authorization
  - **Property 7: Island authorization**
  - **Validates: Requirements 6.2, 6.3**

- [ ]* 2.10 Write property test for login verification
  - **Property 9: Password verification round-trip**
  - **Validates: Requirements 7.5**

- [x] 3. Implement UI layer in Form1_Regulator


  - Add context menu item and event handler for account creation
  - _Requirements: 1.1, 1.2, 1.3, 4.1, 4.2, 4.3, 4.4, 4.5, 6.5_

- [x] 3.1 Modify InitializeContextMenu method


  - Add new ToolStripMenuItem "创建账号"
  - Position after "分配批次" menu item
  - Wire up CreateAccount_Click event handler
  - _Requirements: 1.1_

- [x] 3.2 Implement CreateAccount_Click event handler

  - Validate witch selection in data grid
  - Extract witch data from selected row
  - Check eligibility using BLL method
  - Display appropriate message if ineligible
  - Call BLL to create account
  - Display success or error message
  - Refresh data grid on success
  - _Requirements: 1.2, 1.3, 1.4, 4.1, 4.2, 4.3, 4.4, 4.5_

- [x] 3.3 Add menu item enable/disable logic

  - Check witch eligibility when context menu opens
  - Enable "创建账号" only for eligible witches
  - Show tooltip explaining why disabled if ineligible
  - _Requirements: 1.2, 1.3, 6.5_

- [ ]* 3.4 Write integration tests for UI workflow
  - Test right-click displays context menu
  - Test menu item enabled/disabled states
  - Test success message display
  - Test error message display
  - Test data grid refresh
  - _Requirements: 1.1, 1.2, 1.3, 4.1, 4.2, 4.3, 4.4_

- [x] 4. Checkpoint - Ensure all tests pass

  - Ensure all tests pass, ask the user if questions arise.

- [x] 5. Handle edge cases and error scenarios


  - Add robust error handling for various failure modes
  - _Requirements: 4.2, 4.3, 4.5, 5.5_

- [x] 5.1 Add validation for whitespace-only PrisonerNo

  - Trim and check PrisonerNo before validation
  - Display appropriate error message
  - _Requirements: 2.2, 4.5_

- [x] 5.2 Add error handling for database connection failures

  - Catch SqlException and display user-friendly message
  - Ensure transaction is rolled back
  - _Requirements: 4.3, 5.2_

- [x] 5.3 Add error handling for duplicate username constraint

  - Catch unique constraint violation
  - Display specific error message about existing account
  - _Requirements: 4.2, 5.5_

- [x] 5.4 Add error handling for concurrent modifications

  - Handle case where witch is deleted after menu shown
  - Handle case where account is created by another user simultaneously
  - Display appropriate error messages
  - _Requirements: 4.3_

- [ ]* 5.5 Write unit tests for error handling
  - Test whitespace PrisonerNo rejection
  - Test database error handling
  - Test duplicate username handling
  - Test concurrent modification handling
  - _Requirements: 4.2, 4.3, 4.5, 5.5_

- [x] 6. Final checkpoint - Ensure all tests pass


  - Ensure all tests pass, ask the user if questions arise.
