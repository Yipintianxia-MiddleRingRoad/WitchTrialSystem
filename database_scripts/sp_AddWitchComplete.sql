-- ========================================
-- 存储过程：wt.sp_AddWitchComplete
-- 功能：添加魔女的完整详细档案（38个字段）
-- 作者：WitchTrialSystem
-- 日期：2025-12-02
-- ========================================

USE WitchTrialWT;
GO

-- 如果存储过程已存在，先删除
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'wt.sp_AddWitchComplete') AND type = 'P')
BEGIN
    DROP PROCEDURE wt.sp_AddWitchComplete;
    PRINT '已删除旧版本的 wt.sp_AddWitchComplete';
END
GO

CREATE PROCEDURE wt.sp_AddWitchComplete
    -- 基础字段（10个）
    @Name NVARCHAR(50),
    @Magic NVARCHAR(100),
    @PrisonerNo NVARCHAR(20) = NULL,
    @Status NVARCHAR(20) = N'待分配',
    @ExecutionResult NVARCHAR(50) = NULL,
    @AvatarPath NVARCHAR(255) = NULL,
    @IslandID INT = NULL,
    @BatchID INT = NULL,
    @DescriptionPublic NVARCHAR(MAX) = NULL,
    
    -- 扩展字段（28个）
    @PersonalNo NVARCHAR(20) = NULL,
    @FormerName NVARCHAR(100) = NULL,
    @Gender NVARCHAR(10) = NULL,
    @BirthDate DATE = NULL,
    @Ethnicity NVARCHAR(50) = NULL,
    @Birthplace NVARCHAR(100) = NULL,
    @Height DECIMAL(5,2) = NULL,
    @Weight DECIMAL(5,2) = NULL,
    @BloodType NVARCHAR(10) = NULL,
    @Address NVARCHAR(500) = NULL,
    @Phone NVARCHAR(50) = NULL,
    @Email NVARCHAR(100) = NULL,
    @LineAccount NVARCHAR(100) = NULL,
    @HighestEducation NVARCHAR(100) = NULL,
    @EducationHistory NVARCHAR(MAX) = NULL,
    @WorkHistory NVARCHAR(MAX) = NULL,
    @FamilyStructure NVARCHAR(200) = NULL,
    @Father NVARCHAR(200) = NULL,
    @Mother NVARCHAR(200) = NULL,
    @OtherFamily1 NVARCHAR(200) = NULL,
    @OtherFamily2 NVARCHAR(200) = NULL,
    @OtherFamily3 NVARCHAR(200) = NULL,
    @Skills NVARCHAR(500) = NULL,
    @Hobbies NVARCHAR(500) = NULL,
    @Dreams NVARCHAR(500) = NULL,
    @Dislikes NVARCHAR(500) = NULL,
    @Trauma NVARCHAR(MAX) = NULL,
    @WitchTransformMethod NVARCHAR(500) = NULL,
    @Remarks NVARCHAR(MAX) = NULL,
    
    -- 时间戳字段（4个）
    @CaptureTime DATETIME2 = NULL,
    @DepartureTime DATETIME2 = NULL,
    @ArrivalTime DATETIME2 = NULL,
    @DeathTime DATETIME2 = NULL,
    
    -- 输出参数
    @NewWitchID INT OUTPUT,
    @CreateUser BIT = 0 OUTPUT  -- 是否创建了用户账号
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- ========================================
        -- 1. 验证必填字段
        -- ========================================
        IF @Name IS NULL OR LTRIM(RTRIM(@Name)) = ''
        BEGIN
            RAISERROR(N'姓名不能为空', 16, 1);
            RETURN;
        END
        
        IF @Magic IS NULL OR LTRIM(RTRIM(@Magic)) = ''
        BEGIN
            RAISERROR(N'魔法不能为空', 16, 1);
            RETURN;
        END
        
        IF @Status IS NULL OR LTRIM(RTRIM(@Status)) = ''
        BEGIN
            RAISERROR(N'状态不能为空', 16, 1);
            RETURN;
        END
        
        -- ========================================
        -- 2. 验证岛屿-批次关系
        -- ========================================
        IF @IslandID IS NOT NULL AND @BatchID IS NULL
        BEGIN
            RAISERROR(N'选择岛屿后必须选择批次', 16, 1);
            RETURN;
        END
        
        -- ========================================
        -- 3. 检查批次人数限制（如果提供了批次）
        -- ========================================
        IF @BatchID IS NOT NULL
        BEGIN
            DECLARE @CurrentCount INT;
            SELECT @CurrentCount = WitchCount 
            FROM wt.Batch 
            WHERE BatchID = @BatchID;
            
            IF @CurrentCount IS NULL
            BEGIN
                RAISERROR(N'批次不存在', 16, 1);
                RETURN;
            END
            
            IF @CurrentCount >= 13
            BEGIN
                RAISERROR(N'该批次已满（13/13），无法继续添加', 16, 1);
                RETURN;
            END
        END
        
        -- ========================================
        -- 4. 检查囚犯编号唯一性（如果提供了）
        -- ========================================
        IF @PrisonerNo IS NOT NULL AND LTRIM(RTRIM(@PrisonerNo)) != ''
        BEGIN
            IF EXISTS (SELECT 1 FROM wt.Witch WHERE PrisonerNo = @PrisonerNo)
            BEGIN
                RAISERROR(N'囚犯编号已存在', 16, 1);
                RETURN;
            END
        END
        
        -- ========================================
        -- 5. 插入魔女记录
        -- ========================================
        INSERT INTO wt.Witch (
            Name, Magic, PrisonerNo, [Status], ExecutionResult, AvatarPath, 
            IslandID, BatchID, DescriptionPublic,
            PersonalNo, FormerName, Gender, BirthDate, Ethnicity, Birthplace,
            Height, Weight, BloodType, Address, Phone, Email, LineAccount,
            HighestEducation, EducationHistory, WorkHistory,
            FamilyStructure, Father, Mother, OtherFamily1, OtherFamily2, OtherFamily3,
            Skills, Hobbies, Dreams, Dislikes, Trauma,
            WitchTransformMethod, Remarks,
            CaptureTime, DepartureTime, ArrivalTime, DeathTime
        )
        VALUES (
            @Name, @Magic, @PrisonerNo, @Status, @ExecutionResult, @AvatarPath,
            @IslandID, @BatchID, @DescriptionPublic,
            @PersonalNo, @FormerName, @Gender, @BirthDate, @Ethnicity, @Birthplace,
            @Height, @Weight, @BloodType, @Address, @Phone, @Email, @LineAccount,
            @HighestEducation, @EducationHistory, @WorkHistory,
            @FamilyStructure, @Father, @Mother, @OtherFamily1, @OtherFamily2, @OtherFamily3,
            @Skills, @Hobbies, @Dreams, @Dislikes, @Trauma,
            @WitchTransformMethod, @Remarks,
            @CaptureTime, @DepartureTime, @ArrivalTime, @DeathTime
        );
        
        -- 获取新创建的 WitchID
        SET @NewWitchID = SCOPE_IDENTITY();
        
        PRINT N'✓ 魔女记录已插入，WitchID: ' + CAST(@NewWitchID AS NVARCHAR);
        
        -- ========================================
        -- 6. 创建用户账号（条件性）
        -- ========================================
        -- 条件：囚犯编号不为空 且 状态为"分配至岛屿"
        IF @PrisonerNo IS NOT NULL 
           AND LTRIM(RTRIM(@PrisonerNo)) != '' 
           AND @Status = N'分配至岛屿'
           AND @IslandID IS NOT NULL
           AND @BatchID IS NOT NULL
        BEGIN
            -- 检查用户名是否已存在
            IF NOT EXISTS (SELECT 1 FROM wt.[User] WHERE Username = @PrisonerNo)
            BEGIN
                DECLARE @RoleWitch INT;
                SELECT @RoleWitch = RoleID FROM wt.Role WHERE Name = N'Witch';
                
                -- 创建用户账号
                INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore)
                VALUES (@PrisonerNo, N'PENDING', N'PENDING', @RoleWitch, @IslandID, @BatchID, 0);
                
                DECLARE @NewUserID INT = SCOPE_IDENTITY();
                
                -- 创建 UserWitch 关联
                INSERT INTO wt.UserWitch (UserID, WitchID)
                VALUES (@NewUserID, @NewWitchID);
                
                SET @CreateUser = 1;
                PRINT N'✓ 用户账号已创建，Username: ' + @PrisonerNo;
            END
            ELSE
            BEGIN
                PRINT N'⚠ 用户名已存在，跳过账号创建: ' + @PrisonerNo;
            END
        END
        ELSE
        BEGIN
            SET @CreateUser = 0;
            IF @Status = N'待分配'
            BEGIN
                PRINT N'ℹ 状态为"待分配"，未创建用户账号';
            END
            ELSE IF @PrisonerNo IS NULL OR LTRIM(RTRIM(@PrisonerNo)) = ''
            BEGIN
                PRINT N'ℹ 囚犯编号为空，未创建用户账号';
            END
        END
        
        -- ========================================
        -- 7. 更新批次魔女数量（如果提供了批次）
        -- ========================================
        -- 注意：这个更新会被触发器 trg_Witch_BatchCount 自动处理
        -- 但为了确保，我们也可以手动更新
        IF @BatchID IS NOT NULL
        BEGIN
            UPDATE wt.Batch 
            SET WitchCount = (SELECT COUNT(*) FROM wt.Witch WHERE BatchID = @BatchID)
            WHERE BatchID = @BatchID;
            
            PRINT N'✓ 批次魔女数量已更新';
        END
        
        COMMIT TRANSACTION;
        
        -- 返回成功信息
        SELECT 
            @NewWitchID AS WitchID,
            @CreateUser AS UserCreated,
            N'魔女添加成功' AS Message;
            
        PRINT N'========================================';
        PRINT N'✅ 魔女添加成功！';
        PRINT N'   WitchID: ' + CAST(@NewWitchID AS NVARCHAR);
        PRINT N'   姓名: ' + @Name;
        PRINT N'   状态: ' + @Status;
        IF @CreateUser = 1
            PRINT N'   用户账号: 已创建';
        ELSE
            PRINT N'   用户账号: 未创建';
        PRINT N'========================================';
        
    END TRY
    BEGIN CATCH
        -- 回滚事务
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- 返回错误信息
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT N'========================================';
        PRINT N'❌ 魔女添加失败！';
        PRINT N'   错误信息: ' + @ErrorMessage;
        PRINT N'========================================';
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

-- ========================================
-- 测试存储过程
-- ========================================
PRINT N'';
PRINT N'========================================';
PRINT N'存储过程 wt.sp_AddWitchComplete 创建完成';
PRINT N'========================================';
PRINT N'';
PRINT N'使用示例：';
PRINT N'';
PRINT N'-- 示例1：添加待分配状态的魔女（不创建用户账号）';
PRINT N'DECLARE @WitchID INT, @UserCreated BIT;';
PRINT N'EXEC wt.sp_AddWitchComplete';
PRINT N'    @Name = N''测试魔女'',';
PRINT N'    @Magic = N''测试魔法'',';
PRINT N'    @Status = N''待分配'',';
PRINT N'    @NewWitchID = @WitchID OUTPUT,';
PRINT N'    @CreateUser = @UserCreated OUTPUT;';
PRINT N'';
PRINT N'-- 示例2：添加分配至岛屿的魔女（创建用户账号）';
PRINT N'DECLARE @WitchID INT, @UserCreated BIT;';
PRINT N'EXEC wt.sp_AddWitchComplete';
PRINT N'    @Name = N''测试魔女2'',';
PRINT N'    @Magic = N''测试魔法2'',';
PRINT N'    @PrisonerNo = N''999'',';
PRINT N'    @Status = N''分配至岛屿'',';
PRINT N'    @IslandID = 1,';
PRINT N'    @BatchID = 1,';
PRINT N'    @NewWitchID = @WitchID OUTPUT,';
PRINT N'    @CreateUser = @UserCreated OUTPUT;';
PRINT N'';

GO
