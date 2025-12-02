-- 批量重新编号岛屿2魔女（从109-121改为31-43）

BEGIN TRANSACTION;

-- 定义映射关系
DECLARE @WitchMapping TABLE (OldID INT, NewID INT, PrisonerNo VARCHAR(50), Name NVARCHAR(50));
INSERT INTO @WitchMapping VALUES
(109, 31, '684', '柊舞缇娜'),
(110, 32, '685', '阿良河琪舞'),
(111, 33, '686', '杜乃可莉丝'),
(112, 34, '687', '阿古屋真珠'),
(113, 35, '688', '姐母娞摩'),
(114, 36, '689', '花菱春香'),
(115, 37, '690', '水神小夜'),
(116, 38, '691', '天川薰子'),
(117, 39, '692', '相野美都'),
(118, 40, '693', '平良伊纲'),
(119, 41, '694', '江利内美智'),
(120, 42, '695', '椎崎咲良'),
(121, 43, '696', '月出Style');

-- 显示映射关系
SELECT '映射关系：' AS Info, OldID, NewID, PrisonerNo, Name FROM @WitchMapping ORDER BY OldID;

-- 创建临时表保存完整的魔女数据
SELECT * INTO #TempWitches FROM wt.Witch WHERE IslandID = 2;

-- 更新UserWitch表（临时设为负数避免冲突）
UPDATE uw
SET uw.WitchID = -uw.WitchID
FROM wt.UserWitch uw
INNER JOIN @WitchMapping m ON uw.WitchID = m.OldID;

-- 删除旧的岛屿2魔女记录
DELETE FROM wt.Witch WHERE IslandID = 2;

-- 重新插入新的WitchID记录
SET IDENTITY_INSERT wt.Witch ON;

INSERT INTO wt.Witch (
    WitchID, PrisonerNo, PersonalNo, Name, Gender, BirthDate, Height, Weight, BloodType,
    Magic, [Status], HighestEducation, Birthplace, Phone, Email, Skills,
    Hobbies, Dreams, Trauma, IslandID, BatchID, AvatarPath, DescriptionPublic
)
SELECT 
    m.NewID,
    t.PrisonerNo, t.PersonalNo, t.Name, t.Gender, t.BirthDate, t.Height, t.Weight, t.BloodType,
    t.Magic, t.[Status], t.HighestEducation, t.Birthplace, t.Phone, t.Email, t.Skills,
    t.Hobbies, t.Dreams, t.Trauma, t.IslandID, t.BatchID, t.AvatarPath, t.DescriptionPublic
FROM #TempWitches t
INNER JOIN @WitchMapping m ON t.WitchID = m.OldID;

SET IDENTITY_INSERT wt.Witch OFF;

-- 更新UserWitch表
UPDATE uw
SET uw.WitchID = m.NewID
FROM wt.UserWitch uw
INNER JOIN @WitchMapping m ON uw.WitchID = -m.OldID;

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
