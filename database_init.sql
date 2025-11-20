下面给你整理出一份**干净、精简、可直接发给别人**的版本。
它只保留“功能性、有意义、对项目必要”的部分，把你调试、验证、核对、测试删除、二次检查的全部删掉了。
也整理成**结构化、易读、易复制粘贴执行**的格式。

---

# 《魔女审判数据库系统》SQL Server 2022 精简版脚本

（可直接交付给队友 / 同学 / 老师）

说明：
本脚本包含
① 建库
② 建 Schema
③ 建核心数据表
④ 建种子数据（角色、岛、批次、用户、13 位魔女）
⑤ 建账号绑定表
⑥ 上限控制、触发器、索引
⑦ 魔女公开描述
⑧ 操作日志系统

所有内容均可直接在 SSMS 中一键执行。

---1. 创建数据库与 Schema

USE master;
GO

CREATE DATABASE WitchTrialWT
ON PRIMARY
(
  NAME = N'WitchTrialWT',
  FILENAME = N'E:\WitchTrialSystem\Data\WitchTrialWT.mdf',
  SIZE = 64MB, FILEGROWTH = 16MB
)
LOG ON
(
  NAME = N'WitchTrialWT_log',
  FILENAME = N'E:\WitchTrialSystem\Data\WitchTrialWT_log.ldf',
  SIZE = 32MB, FILEGROWTH = 16MB
);
GO

USE WitchTrialWT;
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name='wt')
    EXEC('CREATE SCHEMA wt AUTHORIZATION dbo;');
GO


---2. 创建基础表：Role / Island / Batch / User / Witch

-- 角色
CREATE TABLE wt.Role(
    RoleID INT IDENTITY PRIMARY KEY,
    Name   NVARCHAR(20) UNIQUE NOT NULL
);

-- 岛屿
CREATE TABLE wt.Island(
    IslandID INT IDENTITY PRIMARY KEY,
    Name     NVARCHAR(50) NOT NULL
);

-- 批次（最多 13 人）
CREATE TABLE wt.Batch(
    BatchID    INT IDENTITY PRIMARY KEY,
    IslandID   INT NOT NULL,
    WitchCount INT NOT NULL DEFAULT(0),
    CONSTRAINT FK_wt_Batch_Island FOREIGN KEY(IslandID) REFERENCES wt.Island(IslandID)
);

-- 用户（含 Salt + Hash）
CREATE TABLE wt.[User](
    UserID       INT IDENTITY PRIMARY KEY,
    Username     NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(64) NOT NULL,
    Salt         NVARCHAR(64) NOT NULL,
    RoleID       INT NOT NULL,
    IslandID     INT NULL,
    BatchID      INT NULL,
    CONSTRAINT FK_wt_User_Role   FOREIGN KEY(RoleID)   REFERENCES wt.Role(RoleID),
    CONSTRAINT FK_wt_User_Island FOREIGN KEY(IslandID) REFERENCES wt.Island(IslandID),
    CONSTRAINT FK_wt_User_Batch  FOREIGN KEY(BatchID)  REFERENCES wt.Batch(BatchID)
);

-- 魔女
CREATE TABLE wt.Witch(
    WitchID         INT IDENTITY PRIMARY KEY,
    Name            NVARCHAR(50) NOT NULL,
    Magic           NVARCHAR(100) NULL,
    PrisonerNo      NVARCHAR(20) NULL,
    [Status]        NVARCHAR(20) NOT NULL DEFAULT(N'Normal'),
    ExecutionResult NVARCHAR(50) NULL,
    AvatarPath      NVARCHAR(255) NULL,
    IslandID        INT NOT NULL,
    BatchID         INT NOT NULL,
    DescriptionPublic NVARCHAR(MAX) NULL,
    CONSTRAINT FK_wt_Witch_Island FOREIGN KEY(IslandID) REFERENCES wt.Island(IslandID),
    CONSTRAINT FK_wt_Witch_Batch  FOREIGN KEY(BatchID)  REFERENCES wt.Batch(BatchID)
);


---3. 种子数据（角色 / 岛 / 批次 / 管理员）

INSERT wt.Role(Name)
VALUES (N'Admin'), (N'Meruru'), (N'Warden'), (N'Witch');

INSERT wt.Island(Name) VALUES (N'魔女岛·壹');

INSERT wt.Batch(IslandID, WitchCount)
SELECT TOP 1 IslandID, 0 FROM wt.Island;

INSERT wt.[User](Username, PasswordHash, Salt, RoleID)
SELECT N'admin', N'PENDING', N'PENDING', RoleID
FROM wt.Role WHERE Name=N'Admin';


---4. 批量导入英文账号（Witch / Meruru / Warden）

DECLARE @island INT = (SELECT TOP 1 IslandID FROM wt.Island);
DECLARE @batch  INT = (SELECT TOP 1 BatchID  FROM wt.Batch);
DECLARE @roleWitch   INT = (SELECT RoleID FROM wt.Role WHERE Name=N'Witch');
DECLARE @roleMeruru  INT = (SELECT RoleID FROM wt.Role WHERE Name=N'Meruru');
DECLARE @roleWarden  INT = (SELECT RoleID FROM wt.Role WHERE Name=N'Warden');

DECLARE @Users TABLE(Username NVARCHAR(50), RoleID INT);
INSERT @Users VALUES
(N'ema',@roleWitch),(N'hiro',@roleWitch),(N'anan',@roleWitch),
(N'noah',@roleWitch),(N'leia',@roleWitch),(N'miria',@roleWitch),
(N'margo',@roleWitch),(N'nanoka',@roleWitch),(N'alisa',@roleWitch),
(N'sherry',@roleWitch),(N'hanna',@roleWitch),(N'coco',@roleWitch),
(N'meruru',@roleWitch),
(N'meruru_regulator',@roleMeruru),
(N'warden',@roleWarden);

INSERT wt.[User](Username,PasswordHash,Salt,RoleID,IslandID,BatchID)
SELECT Username, N'PENDING',N'PENDING',RoleID,@island,@batch
FROM @Users;


---5. 导入 13 位魔女（官方编号 658–670）


DECLARE @Witches TABLE(Name NVARCHAR(50), Magic NVARCHAR(100), PrisonerNo NVARCHAR(20));
INSERT @Witches VALUES
(N'樱羽艾玛',N'魔女杀手',N'658'),
(N'二阶堂希罗',N'死而复返',N'659'),
(N'夏目安安',N'洗脑',N'660'),
(N'城崎诺亚',N'液体操作',N'661'),
(N'莲见蕾娅',N'视线诱导',N'662'),
(N'佐伯米莉亚',N'身体互换',N'663'),
(N'宝生玛格',N'声带模仿',N'664'),
(N'黑部奈叶香',N'幻视',N'665'),
(N'紫藤爱丽莎',N'发火',N'666'),
(N'橘雪莉',N'怪力',N'667'),
(N'远野汉娜',N'浮游',N'668'),
(N'泽渡可可',N'千里眼',N'669'),
(N'冰上梅露露',N'治疗',N'670');

INSERT wt.Witch(Name,Magic,PrisonerNo,[Status],AvatarPath,IslandID,BatchID)
SELECT Name,Magic,PrisonerNo,N'Normal',NULL,@island,@batch
FROM @Witches;


---6. 账号 ↔ 魔女档案映射表

CREATE TABLE wt.UserWitch(
    UserID  INT PRIMARY KEY,
    WitchID INT NULL,
    FOREIGN KEY(UserID)  REFERENCES wt.[User](UserID) ON DELETE CASCADE,
    FOREIGN KEY(WitchID) REFERENCES wt.Witch(WitchID)
);

DECLARE @Map TABLE(Username NVARCHAR(50), PrisonerNo NVARCHAR(20) NULL);
INSERT @Map VALUES
(N'ema','658'),(N'hiro','659'),(N'anan','660'),(N'noah','661'),
(N'leia','662'),(N'miria','663'),(N'margo','664'),(N'nanoka','665'),
(N'alisa','666'),(N'sherry','667'),(N'hanna','668'),(N'coco','669'),
(N'meruru','670'),(N'meruru_regulator','670'),(N'warden',NULL);

INSERT wt.UserWitch(UserID,WitchID)
SELECT u.UserID, w.WitchID
FROM @Map m
JOIN wt.[User] u ON u.Username = m.Username
LEFT JOIN wt.Witch w ON w.PrisonerNo = m.PrisonerNo;


---7. 完善：AvatarPath 批量自动填充

UPDATE w SET AvatarPath =
 CASE PrisonerNo
  WHEN '658' THEN 'Images/ema.png'
  WHEN '659' THEN 'Images/hiro.png'
  WHEN '660' THEN 'Images/anan.png'
  WHEN '661' THEN 'Images/noah.png'
  WHEN '662' THEN 'Images/leia.png'
  WHEN '663' THEN 'Images/miria.png'
  WHEN '664' THEN 'Images/margo.png'
  WHEN '665' THEN 'Images/nanoka.png'
  WHEN '666' THEN 'Images/alisa.png'
  WHEN '667' THEN 'Images/sherry.png'
  WHEN '668' THEN 'Images/hanna.png'
  WHEN '669' THEN 'Images/coco.png'
  WHEN '670' THEN 'Images/meruru.png'
 END
FROM wt.Witch w;


--- 8. 存储过程：合法状态更新

CREATE OR ALTER PROCEDURE wt.sp_UpdateWitchStatus
  @WitchID INT,
  @NewStatus NVARCHAR(20),
  @ExecutionResult NVARCHAR(50) = NULL
AS
BEGIN
  IF @NewStatus NOT IN (N'Normal',N'OnTrial',N'Executed',N'Acquitted')
      THROW 50020, N'非法状态值', 1;

  UPDATE wt.Witch
  SET [Status] = @NewStatus,
      ExecutionResult = @ExecutionResult
  WHERE WitchID = @WitchID;
END;
GO


---9. 日志系统（表 + 存储过程）


CREATE TABLE wt.OperationLog(
    LogID     INT IDENTITY PRIMARY KEY,
    UserID    INT NULL,
    Username  NVARCHAR(50) NOT NULL,
    Action    NVARCHAR(50) NOT NULL,
    Target    NVARCHAR(100) NULL,
    Detail    NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE OR ALTER PROCEDURE wt.sp_LogOperation
  @UserID INT=NULL,
  @Username NVARCHAR(50),
  @Action NVARCHAR(50),
  @Target NVARCHAR(100)=NULL,
  @Detail NVARCHAR(MAX)=NULL
AS
BEGIN
  INSERT wt.OperationLog(UserID,Username,Action,Target,Detail)
  VALUES(@UserID,@Username,@Action,@Target,@Detail);
END;
GO


---10. 控制批次人数 ≤ 13：存储过程 + 触发器

CREATE OR ALTER PROCEDURE wt.sp_AddWitch
  @Name NVARCHAR(50),
  @Magic NVARCHAR(100)=NULL,
  @PrisonerNo NVARCHAR(20)=NULL,
  @IslandID INT,
  @BatchID INT,
  @Status NVARCHAR(20)=N'Normal',
  @AvatarPath NVARCHAR(255)=NULL
AS
BEGIN
  BEGIN TRAN;
  DECLARE @cnt INT;
  SELECT @cnt = WitchCount
  FROM wt.Batch WITH(UPDLOCK, HOLDLOCK)
  WHERE BatchID=@BatchID;

  IF @cnt >= 13 THROW 50010, N'批次人数≥13', 1;

  INSERT wt.Witch(Name,Magic,PrisonerNo,[Status],AvatarPath,IslandID,BatchID)
  VALUES(@Name,@Magic,@PrisonerNo,@Status,@AvatarPath,@IslandID,@BatchID);

  COMMIT;
END;
GO

CREATE OR ALTER TRIGGER wt.trg_Witch_BatchCount
ON wt.Witch
AFTER INSERT, DELETE, UPDATE
AS
BEGIN
  UPDATE b
  SET WitchCount = (SELECT COUNT(*) FROM wt.Witch w WHERE w.BatchID=b.BatchID)
  FROM wt.Batch b
  WHERE b.BatchID IN (
        SELECT BatchID FROM inserted UNION SELECT BatchID FROM deleted
  );
END;
GO


---11. 公共查询视图（用于普通魔女权限）

CREATE VIEW wt.v_WitchPublic AS
SELECT WitchID, Name, Magic, PrisonerNo, [Status],
       IslandID, BatchID, AvatarPath, DescriptionPublic
FROM wt.Witch;
GO


---12. 魔女 DescriptionPublic（根据 JSON 导入）

DECLARE @json NVARCHAR(MAX)=N'[
  {"no":"662","desc":"艺能事务所所属的舞台剧演员，在电视上也经常露面。"},
  {"no":"664","desc":"擅长占卜，一直在图书室研究魔女之书。"},
  {"no":"670","desc":"拥有瞬间治疗伤痛的魔法。"},
  {"no":"663","desc":"在惩罚室遭夏目安安杀害。"},
  {"no":"665","desc":"魔法枪的持有者，在处刑台遭杀害。"},
  {"no":"661","desc":"世界著名街头艺术家【气球】，被杀害。"},
  {"no":"667","desc":"孤儿出身，杀害远野汉娜后被处刑。"},
  {"no":"666","desc":"在处刑台遭某人杀害。"},
  {"no":"660","desc":"杀害佐伯米莉亚，被处刑并证实不死。"},
  {"no":"669","desc":"日常进行直播。"},
  {"no":"658","desc":"15 岁，被检测为魔女因子携带者。"},
  {"no":"668","desc":"在招待所中遭橘雪莉杀害。"},
  {"no":"659","desc":"艾玛的儿时玩伴，被杀害。"}
]';

;WITH J AS (
  SELECT no,[desc]
  FROM OPENJSON(@json)
  WITH (no NVARCHAR(8) '$.no', [desc] NVARCHAR(MAX) '$.desc')
)
UPDATE W
SET DescriptionPublic = J.[desc]
FROM wt.Witch W
JOIN J ON W.PrisonerNo = J.no;

