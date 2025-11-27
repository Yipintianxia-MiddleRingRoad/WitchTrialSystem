-- 重新编号岛屿2魔女的WitchID（从109-121改为29-41）
-- 需要临时移除自增属性

BEGIN TRANSACTION;

-- 创建临时表存储新的魔女数据
SELECT 
    29 + ROW_NUMBER() OVER (ORDER BY WitchID) - 1 AS NewWitchID,
    PrisonerNo,
    PersonalNo,
    Name,
    Gender,
    BirthDate,
    Height,
    Weight,
    BloodType,
    Magic,
    [Status],
    HighestEducation,
    Birthplace,
    Phone,
    Email,
    Skills,
    Hobbies,
    Dreams,
    Trauma,
    IslandID,
    BatchID,
    AvatarPath,
    DescriptionPublic
INTO #TempWitches
FROM wt.Witch
WHERE IslandID = 2
ORDER BY WitchID;

-- 显示新的WitchID
SELECT '新的岛屿2魔女WitchID：' AS Info;
SELECT NewWitchID, PrisonerNo, Name FROM #TempWitches ORDER BY NewWitchID;

-- 删除原有的岛屿2魔女记录
DELETE FROM wt.Witch WHERE IslandID = 2;

-- 删除相关的UserWitch记录
DELETE uw FROM wt.UserWitch uw
INNER JOIN wt.[User] u ON uw.UserID = u.UserID
WHERE u.IslandID = 2;

-- 临时移除WitchID的自增属性
EXEC sp_dropidentity 'wt.Witch';

-- 插入新的魔女记录
INSERT INTO wt.Witch (
    PrisonerNo, PersonalNo, Name, Gender, BirthDate, Height, Weight, BloodType,
    Magic, [Status], HighestEducation, Birthplace, Phone, Email, Skills,
    Hobbies, Dreams, Trauma, IslandID, BatchID, AvatarPath, DescriptionPublic
)
SELECT 
    PrisonerNo, PersonalNo, Name, Gender, BirthDate, Height, Weight, BloodType,
    Magic, [Status], HighestEducation, Birthplace, Phone, Email, Skills,
    Hobbies, Dreams, Trauma, IslandID, BatchID, AvatarPath, DescriptionPublic
FROM #TempWitches;

-- 重新添加自增属性
EXEC sp_addidentity 'wt.Witch', 'WitchID';

-- 获取新插入的WitchID并更新UserWitch
INSERT INTO wt.UserWitch (UserID, WitchID)
SELECT u.UserID, w.WitchID
FROM wt.[User] u
CROSS JOIN wt.Witch w
WHERE u.IslandID = 2 
AND w.IslandID = 2
AND u.PrisonerNo = w.PrisonerNo;

-- 验证结果
SELECT '更新后的岛屿2魔女：' AS Info;
SELECT WitchID, PrisonerNo, Name, IslandID, BatchID 
FROM wt.Witch 
WHERE IslandID = 2 
ORDER BY WitchID;

SELECT '更新后的UserWitch关联：' AS Info;
SELECT uw.UserID, u.Username, uw.WitchID, w.PrisonerNo, w.Name
FROM wt.UserWitch uw
INNER JOIN wt.[User] u ON uw.UserID = u.UserID
INNER JOIN wt.Witch w ON uw.WitchID = w.WitchID
WHERE w.IslandID = 2
ORDER BY uw.WitchID;

-- 清理
DROP TABLE #TempWitches;

COMMIT TRANSACTION;
