-- ========================================
-- 数据库结构查询命令集合
-- 在 SQL Server Management Studio (SSMS) 中运行
-- ========================================

USE WitchTrialWT;
GO

-- ========================================
-- 1. 查看所有表
-- ========================================
SELECT 
    TABLE_NAME AS 表名,
    TABLE_TYPE AS 类型
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'wt'
ORDER BY TABLE_NAME;
GO

-- ========================================
-- 2. 查看所有表及其记录数
-- ========================================
SELECT 
    t.name AS 表名,
    p.rows AS 记录数
FROM sys.tables t
INNER JOIN sys.partitions p ON t.object_id = p.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = 'wt' 
    AND p.index_id IN (0, 1)  -- 0=堆表, 1=聚集索引
ORDER BY t.name;
GO

-- ========================================
-- 3. 查看指定表的详细结构（以 User 表为例）
-- ========================================
SELECT 
    COLUMN_NAME AS 字段名,
    DATA_TYPE AS 数据类型,
    CHARACTER_MAXIMUM_LENGTH AS 最大长度,
    IS_NULLABLE AS 可为空,
    COLUMN_DEFAULT AS 默认值,
    COLUMNPROPERTY(OBJECT_ID(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity') AS 是否自增
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'wt' AND TABLE_NAME = 'User'
ORDER BY ORDINAL_POSITION;
GO

-- ========================================
-- 4. 查看所有表的主键
-- ========================================
SELECT 
    tc.TABLE_NAME AS 表名,
    kcu.COLUMN_NAME AS 主键字段,
    tc.CONSTRAINT_NAME AS 约束名称
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu 
    ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
WHERE tc.TABLE_SCHEMA = 'wt' 
    AND tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
ORDER BY tc.TABLE_NAME;
GO

-- ========================================
-- 5. 查看所有外键关系
-- ========================================
SELECT 
    fk.name AS 外键名称,
    OBJECT_SCHEMA_NAME(fk.parent_object_id) + '.' + OBJECT_NAME(fk.parent_object_id) AS 从表,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS 从表字段,
    OBJECT_SCHEMA_NAME(fk.referenced_object_id) + '.' + OBJECT_NAME(fk.referenced_object_id) AS 到表,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS 到表字段
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc 
    ON fk.object_id = fkc.constraint_object_id
WHERE OBJECT_SCHEMA_NAME(fk.parent_object_id) = 'wt'
ORDER BY OBJECT_NAME(fk.parent_object_id);
GO

-- ========================================
-- 6. 查看所有存储过程
-- ========================================
SELECT 
    ROUTINE_NAME AS 存储过程名称,
    CREATED AS 创建时间,
    LAST_ALTERED AS 最后修改时间
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = 'wt' 
    AND ROUTINE_TYPE = 'PROCEDURE'
ORDER BY ROUTINE_NAME;
GO

-- ========================================
-- 7. 查看所有触发器
-- ========================================
SELECT 
    t.name AS 触发器名称,
    OBJECT_NAME(t.parent_id) AS 所属表,
    t.create_date AS 创建时间,
    t.modify_date AS 修改时间,
    CASE WHEN t.is_disabled = 0 THEN '启用' ELSE '禁用' END AS 状态
FROM sys.triggers t
JOIN sys.tables tb ON t.parent_id = tb.object_id
JOIN sys.schemas s ON tb.schema_id = s.schema_id
WHERE s.name = 'wt'
ORDER BY OBJECT_NAME(t.parent_id), t.name;
GO

-- ========================================
-- 8. 查看所有视图
-- ========================================
SELECT 
    TABLE_NAME AS 视图名称,
    VIEW_DEFINITION AS 视图定义
FROM INFORMATION_SCHEMA.VIEWS
WHERE TABLE_SCHEMA = 'wt'
ORDER BY TABLE_NAME;
GO

-- ========================================
-- 9. 查看所有索引
-- ========================================
SELECT 
    OBJECT_SCHEMA_NAME(i.object_id) + '.' + OBJECT_NAME(i.object_id) AS 表名,
    i.name AS 索引名称,
    i.type_desc AS 索引类型,
    COL_NAME(ic.object_id, ic.column_id) AS 列名,
    CASE WHEN i.is_primary_key = 1 THEN '是' ELSE '否' END AS 是否主键,
    CASE WHEN i.is_unique = 1 THEN '是' ELSE '否' END AS 是否唯一
FROM sys.indexes i
JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE OBJECT_SCHEMA_NAME(i.object_id) = 'wt'
    AND i.name IS NOT NULL
ORDER BY OBJECT_NAME(i.object_id), i.name;
GO

-- ========================================
-- 10. 生成所有表的 CREATE TABLE 脚本（简化版）
-- ========================================
-- 注意：这只是一个简化版本，完整的脚本需要使用 SSMS 的"生成脚本"功能

SELECT 
    'CREATE TABLE ' + TABLE_SCHEMA + '.' + TABLE_NAME + ' (' + CHAR(13) + CHAR(10) +
    STUFF((
        SELECT 
            '    ' + COLUMN_NAME + ' ' + 
            DATA_TYPE + 
            CASE 
                WHEN CHARACTER_MAXIMUM_LENGTH IS NOT NULL 
                THEN '(' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
                ELSE ''
            END + 
            CASE WHEN IS_NULLABLE = 'NO' THEN ' NOT NULL' ELSE '' END + ',' + CHAR(13) + CHAR(10)
        FROM INFORMATION_SCHEMA.COLUMNS c2
        WHERE c2.TABLE_SCHEMA = c1.TABLE_SCHEMA 
            AND c2.TABLE_NAME = c1.TABLE_NAME
        ORDER BY ORDINAL_POSITION
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 0, '') + 
    ');' + CHAR(13) + CHAR(10) + 'GO' AS 建表脚本
FROM INFORMATION_SCHEMA.COLUMNS c1
WHERE TABLE_SCHEMA = 'wt'
GROUP BY TABLE_SCHEMA, TABLE_NAME
ORDER BY TABLE_NAME;
GO

-- ========================================
-- 11. 查看数据库大小
-- ========================================
SELECT 
    DB_NAME() AS 数据库名称,
    SUM(CASE WHEN type = 0 THEN size * 8 / 1024 END) AS 数据文件大小MB,
    SUM(CASE WHEN type = 1 THEN size * 8 / 1024 END) AS 日志文件大小MB,
    SUM(size * 8 / 1024) AS 总大小MB
FROM sys.database_files
GROUP BY DB_NAME();
GO

-- ========================================
-- 12. 查看每个表的大小
-- ========================================
SELECT 
    s.name AS 架构,
    t.name AS 表名,
    p.rows AS 行数,
    SUM(a.total_pages) * 8 / 1024 AS 总大小MB,
    SUM(a.used_pages) * 8 / 1024 AS 已用空间MB,
    (SUM(a.total_pages) - SUM(a.used_pages)) * 8 / 1024 AS 未用空间MB
FROM sys.tables t
JOIN sys.schemas s ON t.schema_id = s.schema_id
JOIN sys.indexes i ON t.object_id = i.object_id
JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
JOIN sys.allocation_units a ON p.partition_id = a.container_id
WHERE s.name = 'wt'
    AND i.index_id <= 1  -- 0=堆表, 1=聚集索引
GROUP BY s.name, t.name, p.rows
ORDER BY SUM(a.total_pages) DESC;
GO

-- ========================================
-- 13. 快速查看所有表的数据（前5行）
-- ========================================
-- Role 表
SELECT TOP 5 '=== wt.Role ===' AS 表名, * FROM wt.Role;

-- Island 表
SELECT TOP 5 '=== wt.Island ===' AS 表名, * FROM wt.Island;

-- Batch 表
SELECT TOP 5 '=== wt.Batch ===' AS 表名, * FROM wt.Batch;

-- User 表（隐藏密码）
SELECT TOP 5 
    '=== wt.User ===' AS 表名,
    UserID, Username, '***' AS PasswordHash, '***' AS Salt, 
    RoleID, IslandID, BatchID, GomokuScore 
FROM wt.[User];

-- Witch 表
SELECT TOP 5 '=== wt.Witch ===' AS 表名, * FROM wt.Witch;

-- UserWitch 表
SELECT TOP 5 '=== wt.UserWitch ===' AS 表名, * FROM wt.UserWitch;

-- GomokuMatchLog 表
SELECT TOP 5 '=== wt.GomokuMatchLog ===' AS 表名, * FROM wt.GomokuMatchLog;

-- OperationLog 表
SELECT TOP 5 '=== wt.OperationLog ===' AS 表名, * FROM wt.OperationLog;
GO

-- ========================================
-- 14. 查看用户与魔女的完整关联信息
-- ========================================
SELECT 
    u.UserID,
    u.Username,
    r.Name AS 角色,
    w.WitchID,
    w.Name AS 魔女姓名,
    w.PrisonerNo AS 囚犯编号,
    w.Magic AS 魔法,
    u.GomokuScore AS 五子棋积分
FROM wt.[User] u
JOIN wt.Role r ON u.RoleID = r.RoleID
LEFT JOIN wt.UserWitch uw ON u.UserID = uw.UserID
LEFT JOIN wt.Witch w ON uw.WitchID = w.WitchID
ORDER BY u.UserID;
GO

-- ========================================
-- 15. 查看数据库完整性（检查外键约束）
-- ========================================
-- 检查是否有孤立记录（外键指向不存在的记录）
EXEC sp_MSforeachtable @command1="DBCC CHECKCONSTRAINTS ('?') WITH ALL_CONSTRAINTS";
GO

-- ========================================
-- 16. 导出表结构为 Markdown 格式（User 表示例）
-- ========================================
SELECT 
    '| ' + COLUMN_NAME + 
    ' | ' + DATA_TYPE + 
    CASE 
        WHEN CHARACTER_MAXIMUM_LENGTH IS NOT NULL 
        THEN '(' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
        ELSE ''
    END + 
    ' | ' + CASE WHEN IS_NULLABLE = 'NO' THEN '✅' ELSE '❌' END + 
    ' | ' + ISNULL(COLUMN_DEFAULT, '') + 
    ' |' AS Markdown行
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'wt' AND TABLE_NAME = 'User'
ORDER BY ORDINAL_POSITION;
GO
