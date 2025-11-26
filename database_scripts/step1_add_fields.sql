-- ========================================
-- 步骤 1：添加扩展字段到 Witch 表
-- ========================================

USE WitchTrialWT;
GO

ALTER TABLE wt.Witch ADD
    PersonalNo NVARCHAR(20) NULL,
    FormerName NVARCHAR(100) NULL,
    Gender NVARCHAR(10) NULL,
    BirthDate DATE NULL,
    Ethnicity NVARCHAR(50) NULL,
    Birthplace NVARCHAR(100) NULL,
    Height DECIMAL(5,2) NULL,
    Weight DECIMAL(5,2) NULL,
    BloodType NVARCHAR(10) NULL,
    Address NVARCHAR(500) NULL,
    Phone NVARCHAR(50) NULL,
    Email NVARCHAR(100) NULL,
    LineAccount NVARCHAR(100) NULL,
    HighestEducation NVARCHAR(100) NULL,
    EducationHistory NVARCHAR(MAX) NULL,
    WorkHistory NVARCHAR(MAX) NULL,
    FamilyStructure NVARCHAR(200) NULL,
    Father NVARCHAR(200) NULL,
    Mother NVARCHAR(200) NULL,
    OtherFamily1 NVARCHAR(200) NULL,
    OtherFamily2 NVARCHAR(200) NULL,
    OtherFamily3 NVARCHAR(200) NULL,
    Skills NVARCHAR(500) NULL,
    Hobbies NVARCHAR(500) NULL,
    Dreams NVARCHAR(500) NULL,
    Dislikes NVARCHAR(500) NULL,
    Trauma NVARCHAR(MAX) NULL,
    WitchTransformMethod NVARCHAR(500) NULL,
    Remarks NVARCHAR(MAX) NULL;
GO

PRINT '✅ 字段添加完成';
GO
