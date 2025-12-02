-- 创建或更新魔女完整信息的存储过程
-- 用于更新魔女的所有42个字段

USE WitchTrialWT;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'wt.sp_UpdateWitchComplete') AND type in (N'P', N'PC'))
    DROP PROCEDURE wt.sp_UpdateWitchComplete;
GO

CREATE PROCEDURE wt.sp_UpdateWitchComplete
    @WitchID INT,
    @Name NVARCHAR(100),
    @Magic NVARCHAR(500),
    @PrisonerNo NVARCHAR(50) = NULL,
    @Status NVARCHAR(50),
    @AvatarPath NVARCHAR(500) = NULL,
    @IslandID INT = NULL,
    @BatchID INT = NULL,
    @DescriptionPublic NVARCHAR(MAX) = NULL,
    @PersonalNo NVARCHAR(50) = NULL,
    @FormerName NVARCHAR(100) = NULL,
    @Gender NVARCHAR(10) = NULL,
    @BirthDate DATE = NULL,
    @Ethnicity NVARCHAR(50) = NULL,
    @Birthplace NVARCHAR(200) = NULL,
    @Height DECIMAL(5,2) = NULL,
    @Weight DECIMAL(5,2) = NULL,
    @BloodType NVARCHAR(10) = NULL,
    @Address NVARCHAR(500) = NULL,
    @Phone NVARCHAR(50) = NULL,
    @Email NVARCHAR(100) = NULL,
    @LineAccount NVARCHAR(100) = NULL,
    @HighestEducation NVARCHAR(200) = NULL,
    @EducationHistory NVARCHAR(MAX) = NULL,
    @WorkHistory NVARCHAR(MAX) = NULL,
    @FamilyStructure NVARCHAR(MAX) = NULL,
    @Father NVARCHAR(200) = NULL,
    @Mother NVARCHAR(200) = NULL,
    @OtherFamily1 NVARCHAR(200) = NULL,
    @OtherFamily2 NVARCHAR(200) = NULL,
    @OtherFamily3 NVARCHAR(200) = NULL,
    @Skills NVARCHAR(MAX) = NULL,
    @Hobbies NVARCHAR(MAX) = NULL,
    @Dreams NVARCHAR(MAX) = NULL,
    @Dislikes NVARCHAR(MAX) = NULL,
    @Trauma NVARCHAR(MAX) = NULL,
    @WitchTransformMethod NVARCHAR(MAX) = NULL,
    @Remarks NVARCHAR(MAX) = NULL,
    @CaptureTime DATETIME2 = NULL,
    @DepartureTime DATETIME2 = NULL,
    @ArrivalTime DATETIME2 = NULL,
    @DeathTime DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- 更新魔女所有信息（所有字段都在 Witch 表中）
        UPDATE wt.Witch
        SET 
            Name = @Name,
            Magic = @Magic,
            PrisonerNo = @PrisonerNo,
            Status = @Status,
            AvatarPath = @AvatarPath,
            IslandID = @IslandID,
            BatchID = @BatchID,
            DescriptionPublic = @DescriptionPublic,
            PersonalNo = @PersonalNo,
            FormerName = @FormerName,
            Gender = @Gender,
            BirthDate = @BirthDate,
            Ethnicity = @Ethnicity,
            Birthplace = @Birthplace,
            Height = @Height,
            Weight = @Weight,
            BloodType = @BloodType,
            Address = @Address,
            Phone = @Phone,
            Email = @Email,
            LineAccount = @LineAccount,
            HighestEducation = @HighestEducation,
            EducationHistory = @EducationHistory,
            WorkHistory = @WorkHistory,
            FamilyStructure = @FamilyStructure,
            Father = @Father,
            Mother = @Mother,
            OtherFamily1 = @OtherFamily1,
            OtherFamily2 = @OtherFamily2,
            OtherFamily3 = @OtherFamily3,
            Skills = @Skills,
            Hobbies = @Hobbies,
            Dreams = @Dreams,
            Dislikes = @Dislikes,
            Trauma = @Trauma,
            WitchTransformMethod = @WitchTransformMethod,
            Remarks = @Remarks,
            CaptureTime = @CaptureTime,
            DepartureTime = @DepartureTime,
            ArrivalTime = @ArrivalTime,
            DeathTime = @DeathTime
        WHERE WitchID = @WitchID;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

PRINT '存储过程 wt.sp_UpdateWitchComplete 创建成功';
GO
