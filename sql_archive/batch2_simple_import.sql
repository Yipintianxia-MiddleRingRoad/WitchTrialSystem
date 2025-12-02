USE WitchTrialWT;
GO

DECLARE @i INT = 1;
DECLARE @b INT = 2;

DELETE FROM wt.Witch WHERE BatchID = 2;
DELETE FROM wt.[User] WHERE BatchID = 2;

INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID) VALUES (N'小鸟游六花', N'邪王真眼', '671', N'Normal', 'Images/671.png', @i, @b);
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID) VALUES (N'富樫勇太', N'漆黑烈焰使', '672', N'Normal', 'Images/672.png', @i, @b);
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID) VALUES (N'丹生谷森夏', N'森夏魔女', '673', N'Normal', 'Images/673.png', @i, @b);
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID) VALUES (N'五月七日茴香', N'茴香魔女', '674', N'Normal', 'Images/674.png', @i, @b);
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID) VALUES (N'凸守早苗', N'雷之征服者', '675', N'Normal', 'Images/675.png', @i, @b);
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID) VALUES (N'七宫智音', N'智音魔女', '676', N'Normal', 'Images/676.png', @i, @b);
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID) VALUES (N'伊雷娜', N'灰之魔女', '677', N'Normal', 'Images/677.png', @i, @b);
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID) VALUES (N'维多利加', N'高阶全属性魔法', '678', N'Normal', 'Images/678.png', @i, @b);
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID) VALUES (N'沙耶', N'扫帚魔女', '679', N'Normal', 'Images/679.png', @i, @b);
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID) VALUES (N'芙兰', N'星辰魔女', '680', N'Normal', 'Images/680.png', @i, @b);
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID) VALUES (N'席拉', N'暗夜魔女', '681', N'Normal', 'Images/681.png', @i, @b);
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID) VALUES (N'琪琪', N'飞行魔法', '682', N'Normal', 'Images/682.png', @i, @b);
INSERT INTO wt.Witch (Name, Magic, PrisonerNo, [Status], AvatarPath, IslandID, BatchID) VALUES (N'冰上梅露露', N'治愈再生', '683', N'Normal', 'Images/683.png', @i, @b);

UPDATE wt.Batch SET WitchCount = 13 WHERE BatchID = 2;

DECLARE @roleWitch INT = (SELECT RoleID FROM wt.Role WHERE Name = N'Witch');

INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore) VALUES ('671', N'PENDING', N'PENDING', @roleWitch, @i, @b, 0);
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore) VALUES ('672', N'PENDING', N'PENDING', @roleWitch, @i, @b, 0);
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore) VALUES ('673', N'PENDING', N'PENDING', @roleWitch, @i, @b, 0);
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore) VALUES ('674', N'PENDING', N'PENDING', @roleWitch, @i, @b, 0);
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore) VALUES ('675', N'PENDING', N'PENDING', @roleWitch, @i, @b, 0);
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore) VALUES ('676', N'PENDING', N'PENDING', @roleWitch, @i, @b, 0);
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore) VALUES ('677', N'PENDING', N'PENDING', @roleWitch, @i, @b, 0);
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore) VALUES ('678', N'PENDING', N'PENDING', @roleWitch, @i, @b, 0);
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore) VALUES ('679', N'PENDING', N'PENDING', @roleWitch, @i, @b, 0);
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore) VALUES ('680', N'PENDING', N'PENDING', @roleWitch, @i, @b, 0);
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore) VALUES ('681', N'PENDING', N'PENDING', @roleWitch, @i, @b, 0);
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore) VALUES ('682', N'PENDING', N'PENDING', @roleWitch, @i, @b, 0);
INSERT INTO wt.[User] (Username, PasswordHash, Salt, RoleID, IslandID, BatchID, GomokuScore) VALUES ('683', N'PENDING', N'PENDING', @roleWitch, @i, @b, 0);

INSERT INTO wt.UserWitch (UserID, WitchID)
SELECT u.UserID, w.WitchID FROM wt.[User] u JOIN wt.Witch w ON u.Username = w.PrisonerNo WHERE u.BatchID = 2 AND w.BatchID = 2;

SELECT 'Batch 2 Import Complete' AS Status;
SELECT COUNT(*) AS WitchCount FROM wt.Witch WHERE BatchID = 2;
SELECT COUNT(*) AS UserCount FROM wt.[User] WHERE BatchID = 2;
GO
