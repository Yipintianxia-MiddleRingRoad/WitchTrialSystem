-- ========================================
-- 测试脚本：wt.sp_UpdateWitchComplete
-- 功能：测试更新魔女完整信息的存储过程
-- 日期：2024-12-02
-- ========================================

USE WitchTrialWT;
GO

PRINT N'========================================';
PRINT N'开始测试 wt.sp_UpdateWitchComplete';
PRINT N'========================================';
PRINT N'';

-- ========================================
-- 测试1：更新现有魔女的基本信息
-- ========================================
PRINT N'测试1：更新现有魔女的基本信息';
PRINT N'----------------------------------------';

-- 假设更新 WitchID = 1 的魔女
DECLARE @TestWitchID INT = 1;

-- 检查魔女是否存在
IF EXISTS (SELECT 1 FROM wt.Witch WHERE WitchID = @TestWitchID)
BEGIN
    PRINT N'✓ 找到测试魔女，WitchID: ' + CAST(@TestWitchID AS NVARCHAR);
    
    -- 显示更新前的信息
    SELECT 
        WitchID,
        Name,
        Magic,
        [Status],
        IslandID,
        BatchID
    FROM wt.Witch 
    WHERE WitchID = @TestWitchID;
    
    -- 执行更新（只更新部分字段作为测试）
    EXEC wt.sp_UpdateWitchComplete
        @WitchID = @TestWitchID,
        @Name = N'测试更新姓名',
        @Magic = N'测试更新魔法',
        @Status = N'审判中',
        @Remarks = N'这是一条测试更新的备注';
    
    PRINT N'';
    PRINT N'更新后的信息：';
    
    -- 显示更新后的信息
    SELECT 
        WitchID,
        Name,
        Magic,
        [Status],
        Remarks
    FROM wt.Witch 
    WHERE WitchID = @TestWitchID;
    
    PRINT N'';
    PRINT N'✅ 测试1完成';
END
ELSE
BEGIN
    PRINT N'⚠ 未找到 WitchID = ' + CAST(@TestWitchID AS NVARCHAR) + ' 的魔女，跳过测试';
END

PRINT N'';
PRINT N'========================================';
PRINT N'测试完成';
PRINT N'========================================';
PRINT N'';
PRINT N'注意：此测试会修改数据库中的数据';
PRINT N'如果不想保留测试数据，请手动回滚或恢复';
PRINT N'';

GO
