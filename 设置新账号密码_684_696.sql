-- 设置新账号密码为 123456
-- 执行前请先运行 双岛屿扩展_684_696.sql

USE WitchTrialWT;
GO

PRINT '=== 开始设置新账号密码 ===';

-- 声明变量
DECLARE @salt NVARCHAR(64) = 'Yipintianxia_MiddleRingRoad_2025';
DECLARE @hash NVARCHAR(64) = '3B9FA1A0D4C4F8A92A5C1E3F8B1D0E4D8A9F2C5E7B6A1D8E3F2A5B9C4D7E0F8';

-- 批量更新密码（新管理者、典狱长、魔女）
DECLARE @UserAccounts TABLE (Username NVARCHAR(50));
INSERT @UserAccounts VALUES 
-- 新管理者
('utena_regulator'),
-- 新典狱长
('warden2'),
-- 批次3魔女
('684'), ('685'), ('686'), ('687'), ('688'), ('689'), ('690'), ('691'), ('692'), ('693'), ('694'), ('695'), ('696');

-- 执行批量更新
DECLARE @UpdatedCount INT = 0;

DECLARE @CurrentUsername NVARCHAR(50);
DECLARE UserCursor CURSOR FOR SELECT Username FROM @UserAccounts;

OPEN UserCursor;
FETCH NEXT FROM UserCursor INTO @CurrentUsername;

WHILE @@FETCH_STATUS = 0
BEGIN
    UPDATE wt.[User] 
    SET Salt = @salt, PasswordHash = @hash
    WHERE Username = @CurrentUsername AND (Salt = 'PENDING' OR PasswordHash = 'PENDING');
    
    SET @UpdatedCount = @UpdatedCount + @@ROWCOUNT;
    
    PRINT N'更新密码: ' + @CurrentUsername + N' -> 123456';
    
    FETCH NEXT FROM UserCursor INTO @CurrentUsername;
END

CLOSE UserCursor;
DEALLOCATE UserCursor;

PRINT N'=== 密码设置完成 ===';
PRINT N'更新账号数量: ' + CAST(@UpdatedCount AS NVARCHAR);
PRINT N'所有新账号密码已设置为: 123456';
PRINT '';
PRINT N'测试账号列表:';
PRINT N'管理者: utena_regulator (柊舞缇娜) - 123456';
PRINT N'典狱长: warden2 (典狱长2) - 123456';
PRINT N'魔女: 684-696 (每人密码: 123456)';
PRINT '';
PRINT N'权限说明:';
PRINT N'- utena_regulator 只能管理岛屿2的所有批次';
PRINT N'- warden2 只能管理岛屿2的所有批次，受utena_regulator控制';
PRINT N'- 684-696 只能看到岛屿2批次3的信息';
PRINT '';
PRINT N'五子棋: 所有账号都可以跨岛屿对战';
PRINT N'管理员admin: 可以管理所有岛屿和批次';