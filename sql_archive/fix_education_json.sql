-- 修复教育经历JSON格式
-- 将格式化的JSON转换为紧凑格式

USE WitchTrialWT;
GO

-- 备份当前数据（可选）
-- SELECT PrisonerNo, Name, EducationHistory INTO Witch_Backup_EducationHistory FROM wt.Witch WHERE EducationHistory IS NOT NULL;

-- 修复所有包含换行符的JSON（格式化JSON）
-- 使用JSON_QUERY来验证和重新格式化JSON

DECLARE @PrisonerNo NVARCHAR(20);
DECLARE @Json NVARCHAR(MAX);
DECLARE @CleanJson NVARCHAR(MAX);

DECLARE json_cursor CURSOR FOR
SELECT PrisonerNo, EducationHistory 
FROM wt.Witch 
WHERE EducationHistory IS NOT NULL 
  AND EducationHistory LIKE '%' + CHAR(10) + '%'  -- 包含换行符
  AND PrisonerNo >= '671';  -- 只处理671及以后的魔女

OPEN json_cursor;
FETCH NEXT FROM json_cursor INTO @PrisonerNo, @Json;

WHILE @@FETCH_STATUS = 0
BEGIN
    BEGIN TRY
        -- 尝试解析JSON并重新格式化为紧凑格式
        SET @CleanJson = (SELECT JSON_QUERY(@Json));
        
        IF @CleanJson IS NOT NULL
        BEGIN
            -- 更新为紧凑格式
            UPDATE wt.Witch 
            SET EducationHistory = @CleanJson
            WHERE PrisonerNo = @PrisonerNo;
            
            PRINT '✅ 已修复 ' + @PrisonerNo + ' 的教育经历JSON';
        END
    END TRY
    BEGIN CATCH
        PRINT '❌ 无法修复 ' + @PrisonerNo + ': ' + ERROR_MESSAGE();
    END CATCH;
    
    FETCH NEXT FROM json_cursor INTO @PrisonerNo, @Json;
END;

CLOSE json_cursor;
DEALLOCATE json_cursor;

PRINT '';
PRINT '修复完成！';
GO

-- 验证修复结果
SELECT 
    PrisonerNo,
    Name,
    CASE 
        WHEN EducationHistory LIKE '%' + CHAR(10) + '%' THEN '格式化'
        ELSE '紧凑'
    END AS JSON格式,
    LEN(EducationHistory) AS 长度
FROM wt.Witch
WHERE PrisonerNo >= '671' AND EducationHistory IS NOT NULL
ORDER BY PrisonerNo;
GO
