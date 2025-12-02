-- 完成岛屿2魔女重新编号（剩余的12个）
-- 将110-121改为32-43

BEGIN TRANSACTION;

-- 定义剩余的映射关系（除了已经完成的31）
DECLARE @Mapping TABLE (OldID INT, NewID INT, Username VARCHAR(50));
INSERT INTO @Mapping VALUES
(110, 32, '685'),
(111, 33, '686'),
(112, 34, '687'),
(113, 35, '688'),
(114, 36, '689'),
(115, 37, '690'),
(116, 38, '691'),
(117, 39, '692'),
(118, 40, '693'),
(119, 41, '694'),
(120, 42, '695'),
(121, 43, '696');

-- 逐个处理
DECLARE @OldID INT, @NewID INT, @Username VARCHAR(50);
DECLARE cursor CURSOR FOR SELECT OldID, NewID, Username FROM @Mapping;

OPEN cursor;
FETCH NEXT FROM cursor INTO @OldID, @NewID, @Username;
WHILE @@FETCH_STATUS = 0
BEGIN
    -- 插入新记录
    SET IDENTITY_INSERT wt.Witch ON;
    EXEC('INSERT INTO wt.Witch (WitchID, PrisonerNo, PersonalNo, Name, Gender, BirthDate, Height, Weight, BloodType, Magic, [Status], HighestEducation, Birthplace, Phone, Email, Skills, Hobbies, Dreams, Trauma, IslandID, BatchID, AvatarPath, DescriptionPublic) SELECT ' + @NewID + ', PrisonerNo, PersonalNo, Name, Gender, BirthDate, Height, Weight, BloodType, Magic, [Status], HighestEducation, Birthplace, Phone, Email, Skills, Hobbies, Dreams, Trauma, IslandID, BatchID, AvatarPath, DescriptionPublic FROM wt.Witch WHERE WitchID = ' + @OldID);
    SET IDENTITY_INSERT wt.Witch OFF;
    
    -- 删除旧记录
    EXEC('DELETE FROM wt.Witch WHERE WitchID = ' + @OldID);
    
    -- 更新UserWitch
    EXEC('UPDATE uw SET uw.WitchID = ' + @NewID + ' FROM wt.UserWitch uw INNER JOIN wt.[User] u ON uw.UserID = u.UserID WHERE u.Username = ''' + @Username + '''');
    
    FETCH NEXT FROM cursor INTO @OldID, @NewID, @Username;
END
CLOSE cursor;
DEALLOCATE cursor;

-- 验证结果
SELECT '最终的岛屿2魔女：' AS Info;
SELECT WitchID, PrisonerNo, Name, IslandID, BatchID 
FROM wt.Witch 
WHERE IslandID = 2 
ORDER BY WitchID;

COMMIT TRANSACTION;
