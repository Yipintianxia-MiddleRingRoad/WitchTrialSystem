-- =============================================
-- 初始化处刑台数据
-- 用途：为每个岛屿创建49个处刑台
-- 作者：WitchTrialSystem
-- 日期：2024-12-06
-- =============================================

USE WitchTrialWT;
GO

PRINT '开始初始化处刑台数据...';
GO

-- 声明变量
DECLARE @IslandID INT;
DECLARE @PlatformNumber INT;
DECLARE @ExistingCount INT;

-- 获取所有岛屿
DECLARE island_cursor CURSOR FOR
SELECT IslandID FROM wt.Island;

OPEN island_cursor;
FETCH NEXT FROM island_cursor INTO @IslandID;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- 检查该岛屿是否已有处刑台数据
    SELECT @ExistingCount = COUNT(*) 
    FROM wt.ExecutionPlatform 
    WHERE IslandID = @IslandID;
    
    IF @ExistingCount > 0
    BEGIN
        PRINT '岛屿 ' + CAST(@IslandID AS NVARCHAR(10)) + ' 已有 ' + CAST(@ExistingCount AS NVARCHAR(10)) + ' 个处刑台，跳过初始化';
    END
    ELSE
    BEGIN
        PRINT '正在为岛屿 ' + CAST(@IslandID AS NVARCHAR(10)) + ' 创建49个处刑台...';
        
        -- 为该岛屿创建49个处刑台
        SET @PlatformNumber = 1;
        
        WHILE @PlatformNumber <= 49
        BEGIN
            INSERT INTO wt.ExecutionPlatform (
                IslandID,
                PlatformNumber,
                HomePosition,
                CurrentPosition,
                ToolName,
                ToolType,
                ToolDescription,
                Status,
                CreatedAt,
                UpdatedAt
            )
            VALUES (
                @IslandID,
                @PlatformNumber,
                @PlatformNumber,  -- HomePosition = PlatformNumber
                @PlatformNumber,  -- CurrentPosition = PlatformNumber (初始在原位)
                NULL,             -- 初始无刑具
                NULL,
                NULL,
                N'空闲',          -- 初始状态为空闲
                GETDATE(),
                GETDATE()
            );
            
            SET @PlatformNumber = @PlatformNumber + 1;
        END
        
        PRINT '✓ 岛屿 ' + CAST(@IslandID AS NVARCHAR(10)) + ' 的49个处刑台创建完成';
    END
    
    FETCH NEXT FROM island_cursor INTO @IslandID;
END

CLOSE island_cursor;
DEALLOCATE island_cursor;

-- 显示初始化结果
PRINT '';
PRINT '========== 初始化结果 ==========';
SELECT 
    i.IslandID,
    i.Name AS IslandName,
    COUNT(ep.PlatformID) AS PlatformCount
FROM wt.Island i
LEFT JOIN wt.ExecutionPlatform ep ON i.IslandID = ep.IslandID
GROUP BY i.IslandID, i.Name
ORDER BY i.IslandID;

PRINT '';
PRINT '✓ 处刑台数据初始化完成！';
GO
