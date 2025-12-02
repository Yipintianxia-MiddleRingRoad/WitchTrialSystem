-- ========================================
-- 测试脚本：wt.sp_AddWitchComplete
-- 功能：测试存储过程的各种场景
-- ========================================

USE WitchTrialWT;
GO

PRINT N'========================================';
PRINT N'开始测试 wt.sp_AddWitchComplete';
PRINT N'========================================';
PRINT N'';

-- ========================================
-- 测试 1: 添加待分配状态的魔女（不创建用户账号）
-- ========================================
PRINT N'【测试 1】添加待分配状态的魔女';
PRINT N'----------------------------------------';

DECLARE @WitchID1 INT, @UserCreated1 BIT;

BEGIN TRY
    EXEC wt.sp_AddWitchComplete
        @Name = N'测试魔女_待分配',
        @Magic = N'测试魔法_时间停止',
        @Status = N'待分配',
        @Gender = N'女',
        @BirthDate = '2005-01-01',
        @Height = 160.00,
        @Weight = 45.00,
        @Skills = N'测试技能',
        @NewWitchID = @WitchID1 OUTPUT,
        @CreateUser = @UserCreated1 OUTPUT;
    
    PRINT N'✅ 测试 1 通过';
    PRINT N'   WitchID: ' + CAST(@WitchID1 AS NVARCHAR);
    PRINT N'   用户账号创建: ' + CASE WHEN @UserCreated1 = 1 THEN N'是' ELSE N'否' END;
    
    -- 验证数据
    IF EXISTS (SELECT 1 FROM wt.Witch WHERE WitchID = @WitchID1 AND [Status] = N'待分配')
        PRINT N'   ✓ 魔女记录已正确插入';
    ELSE
        PRINT N'   ✗ 魔女记录插入失败';
        
    IF NOT EXISTS (SELECT 1 FROM wt.[User] WHERE Username = N'测试魔女_待分配')
        PRINT N'   ✓ 未创建用户账号（符合预期）';
    ELSE
        PRINT N'   ✗ 错误：创建了用户账号';
END TRY
BEGIN CATCH
    PRINT N'❌ 测试 1 失败: ' + ERROR_MESSAGE();
END CATCH

PRINT N'';

-- ========================================
-- 测试 2: 添加分配至岛屿的魔女（创建用户账号）
-- ========================================
PRINT N'【测试 2】添加分配至岛屿的魔女（创建用户账号）';
PRINT N'----------------------------------------';

DECLARE @WitchID2 INT, @UserCreated2 BIT;
DECLARE @TestPrisonerNo NVARCHAR(20) = N'TEST001';
DECLARE @TestIslandID INT = 2;  -- 使用岛屿2
DECLARE @TestBatchID INT = 5;   -- 使用批次5（应该未满）

BEGIN TRY
    EXEC wt.sp_AddWitchComplete
        @Name = N'测试魔女_分配',
        @Magic = N'测试魔法_瞬间移动',
        @PrisonerNo = @TestPrisonerNo,
        @Status = N'分配至岛屿',
        @IslandID = @TestIslandID,
        @BatchID = @TestBatchID,
        @Gender = N'女',
        @BirthDate = '2004-06-15',
        @Height = 165.00,
        @Weight = 48.00,
        @BloodType = N'A',
        @Phone = N'03-1234-5678',
        @Email = N'test@example.com',
        @Skills = N'战斗、魔法控制',
        @Hobbies = N'阅读、音乐',
        @Dreams = N'成为最强魔女',
        @DescriptionPublic = N'这是一个测试魔女的公开描述',
        @NewWitchID = @WitchID2 OUTPUT,
        @CreateUser = @UserCreated2 OUTPUT;
    
    PRINT N'✅ 测试 2 通过';
    PRINT N'   WitchID: ' + CAST(@WitchID2 AS NVARCHAR);
    PRINT N'   用户账号创建: ' + CASE WHEN @UserCreated2 = 1 THEN N'是' ELSE N'否' END;
    
    -- 验证数据
    IF EXISTS (SELECT 1 FROM wt.Witch WHERE WitchID = @WitchID2 AND [Status] = N'分配至岛屿')
        PRINT N'   ✓ 魔女记录已正确插入';
    ELSE
        PRINT N'   ✗ 魔女记录插入失败';
        
    IF EXISTS (SELECT 1 FROM wt.[User] WHERE Username = @TestPrisonerNo)
        PRINT N'   ✓ 用户账号已创建';
    ELSE
        PRINT N'   ✗ 用户账号创建失败';
        
    IF EXISTS (SELECT 1 FROM wt.UserWitch WHERE WitchID = @WitchID2)
        PRINT N'   ✓ UserWitch 关联已创建';
    ELSE
        PRINT N'   ✗ UserWitch 关联创建失败';
        
    -- 检查批次数量是否更新
    DECLARE @BatchCount INT;
    SELECT @BatchCount = WitchCount FROM wt.Batch WHERE BatchID = @TestBatchID;
    PRINT N'   ℹ 批次' + CAST(@TestBatchID AS NVARCHAR) + N'当前魔女数量: ' + CAST(@BatchCount AS NVARCHAR);
END TRY
BEGIN CATCH
    PRINT N'❌ 测试 2 失败: ' + ERROR_MESSAGE();
END CATCH

PRINT N'';

-- ========================================
-- 测试 3: 测试必填字段验证（姓名为空）
-- ========================================
PRINT N'【测试 3】测试必填字段验证（姓名为空）';
PRINT N'----------------------------------------';

DECLARE @WitchID3 INT, @UserCreated3 BIT;

BEGIN TRY
    EXEC wt.sp_AddWitchComplete
        @Name = N'',  -- 空姓名
        @Magic = N'测试魔法',
        @Status = N'待分配',
        @NewWitchID = @WitchID3 OUTPUT,
        @CreateUser = @UserCreated3 OUTPUT;
    
    PRINT N'❌ 测试 3 失败: 应该抛出错误但没有';
END TRY
BEGIN CATCH
    IF ERROR_MESSAGE() LIKE N'%姓名不能为空%'
        PRINT N'✅ 测试 3 通过: 正确拒绝空姓名';
    ELSE
        PRINT N'❌ 测试 3 失败: 错误消息不正确 - ' + ERROR_MESSAGE();
END CATCH

PRINT N'';

-- ========================================
-- 测试 4: 测试岛屿-批次关系验证
-- ========================================
PRINT N'【测试 4】测试岛屿-批次关系验证（选择岛屿但未选择批次）';
PRINT N'----------------------------------------';

DECLARE @WitchID4 INT, @UserCreated4 BIT;

BEGIN TRY
    EXEC wt.sp_AddWitchComplete
        @Name = N'测试魔女4',
        @Magic = N'测试魔法',
        @Status = N'分配至岛屿',
        @IslandID = 1,  -- 选择了岛屿
        @BatchID = NULL,  -- 但未选择批次
        @NewWitchID = @WitchID4 OUTPUT,
        @CreateUser = @UserCreated4 OUTPUT;
    
    PRINT N'❌ 测试 4 失败: 应该抛出错误但没有';
END TRY
BEGIN CATCH
    IF ERROR_MESSAGE() LIKE N'%选择岛屿后必须选择批次%'
        PRINT N'✅ 测试 4 通过: 正确验证岛屿-批次关系';
    ELSE
        PRINT N'❌ 测试 4 失败: 错误消息不正确 - ' + ERROR_MESSAGE();
END CATCH

PRINT N'';

-- ========================================
-- 测试 5: 测试囚犯编号唯一性
-- ========================================
PRINT N'【测试 5】测试囚犯编号唯一性（重复的囚犯编号）';
PRINT N'----------------------------------------';

DECLARE @WitchID5 INT, @UserCreated5 BIT;

BEGIN TRY
    -- 尝试使用已存在的囚犯编号
    EXEC wt.sp_AddWitchComplete
        @Name = N'测试魔女5',
        @Magic = N'测试魔法',
        @PrisonerNo = @TestPrisonerNo,  -- 使用测试2中已创建的编号
        @Status = N'分配至岛屿',
        @IslandID = 2,
        @BatchID = 5,
        @NewWitchID = @WitchID5 OUTPUT,
        @CreateUser = @UserCreated5 OUTPUT;
    
    PRINT N'❌ 测试 5 失败: 应该抛出错误但没有';
END TRY
BEGIN CATCH
    IF ERROR_MESSAGE() LIKE N'%囚犯编号已存在%'
        PRINT N'✅ 测试 5 通过: 正确检测重复的囚犯编号';
    ELSE
        PRINT N'❌ 测试 5 失败: 错误消息不正确 - ' + ERROR_MESSAGE();
END CATCH

PRINT N'';

-- ========================================
-- 测试 6: 测试 JSON 字段
-- ========================================
PRINT N'【测试 6】测试 JSON 字段（教育经历和工作经历）';
PRINT N'----------------------------------------';

DECLARE @WitchID6 INT, @UserCreated6 BIT;
DECLARE @EducationJSON NVARCHAR(MAX) = N'[
    {
        "school": "东京都立测试中学",
        "degree": "中学校",
        "status": "毕业",
        "specialNote": "成绩优异"
    }
]';
DECLARE @WorkJSON NVARCHAR(MAX) = N'[
    {
        "period": "2020/04-2022/03",
        "company": "测试株式会社",
        "position": "测试职位",
        "salary": "月薪 25 万日元",
        "resignReason": "测试原因"
    }
]';

BEGIN TRY
    EXEC wt.sp_AddWitchComplete
        @Name = N'测试魔女_JSON',
        @Magic = N'测试魔法',
        @Status = N'待分配',
        @EducationHistory = @EducationJSON,
        @WorkHistory = @WorkJSON,
        @NewWitchID = @WitchID6 OUTPUT,
        @CreateUser = @UserCreated6 OUTPUT;
    
    PRINT N'✅ 测试 6 通过';
    PRINT N'   WitchID: ' + CAST(@WitchID6 AS NVARCHAR);
    
    -- 验证 JSON 数据
    IF EXISTS (SELECT 1 FROM wt.Witch WHERE WitchID = @WitchID6 AND EducationHistory IS NOT NULL)
        PRINT N'   ✓ 教育经历 JSON 已保存';
    ELSE
        PRINT N'   ✗ 教育经历 JSON 保存失败';
        
    IF EXISTS (SELECT 1 FROM wt.Witch WHERE WitchID = @WitchID6 AND WorkHistory IS NOT NULL)
        PRINT N'   ✓ 工作经历 JSON 已保存';
    ELSE
        PRINT N'   ✗ 工作经历 JSON 保存失败';
END TRY
BEGIN CATCH
    PRINT N'❌ 测试 6 失败: ' + ERROR_MESSAGE();
END CATCH

PRINT N'';

-- ========================================
-- 清理测试数据
-- ========================================
PRINT N'========================================';
PRINT N'清理测试数据';
PRINT N'========================================';

BEGIN TRY
    -- 删除测试创建的魔女和用户
    DELETE FROM wt.UserWitch WHERE WitchID IN (SELECT WitchID FROM wt.Witch WHERE Name LIKE N'测试魔女%');
    DELETE FROM wt.[User] WHERE Username LIKE N'TEST%';
    DELETE FROM wt.Witch WHERE Name LIKE N'测试魔女%';
    
    PRINT N'✓ 测试数据已清理';
END TRY
BEGIN CATCH
    PRINT N'⚠ 清理测试数据时出错: ' + ERROR_MESSAGE();
END CATCH

PRINT N'';
PRINT N'========================================';
PRINT N'测试完成';
PRINT N'========================================';
