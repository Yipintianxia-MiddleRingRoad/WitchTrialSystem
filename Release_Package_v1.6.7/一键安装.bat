@echo off
chcp 65001 >nul
echo ═══════════════════════════════════════════════════════════════
echo   魔女审判系统 v1.6.7 一键安装程序
echo ═══════════════════════════════════════════════════════════════
echo.

set INSTALL_DIR=%~dp0
set DB_NAME=WitchTrialWT
set DB_FILE=%INSTALL_DIR%Data\WitchTrialWT.mdf
set DB_LOG_FILE=%INSTALL_DIR%Data\WitchTrialWT_log.ldf

REM 检查数据库文件是否存在（最重要！）
echo [1/5] 检查数据库文件...
echo 脚本位置: %INSTALL_DIR%
echo 查找数据库文件: %DB_FILE%
echo.

if not exist "%DB_FILE%" (
    echo ❌ 错误：数据库文件不存在！
    echo.
    echo 缺少文件：%DB_FILE%
    echo.
    echo 当前目录内容：
    dir "%INSTALL_DIR%"
    echo.
    echo 请确认：
    echo 1. Data 文件夹中必须包含 WitchTrialWT.mdf 文件
    echo 2. 该文件包含所有系统数据，不能缺失
    echo 3. 请从完整的发布包中重新获取
    echo 4. 如果看到 "Data备份" 文件夹，请重命名为 "Data"
    echo.
    pause
    exit /b 1
)
if not exist "%DB_LOG_FILE%" (
    echo ⚠ 警告：数据库日志文件不存在
    echo 将尝试自动重建日志文件...
)
echo ✓ 数据库文件完整

REM 检查 LocalDB 是否安装
echo.
echo [2/5] 检查 SQL Server LocalDB...
sqllocaldb info MSSQLLocalDB >nul 2>&1
if errorlevel 1 (
    echo ❌ 未检测到 SQL Server LocalDB
    echo.
    echo 请先安装 SQL Server LocalDB：
    echo 1. 运行 SqlLocalDB.msi 安装包
    echo 2. 安装完成后重启电脑
    echo 3. 再次运行本程序
    echo.
    pause
    exit /b 1
)
echo ✓ SQL Server LocalDB 已安装

REM 检查 sqlcmd 是否可用
echo.
echo [2.5/5] 检查 sqlcmd 工具...
where sqlcmd >nul 2>&1
if errorlevel 1 (
    echo ⚠ 警告：未在系统 PATH 中找到 sqlcmd
    echo 尝试从 SQL Server 安装目录查找...
    
    REM 尝试从常见的 SQL Server 安装路径查找 sqlcmd
    if exist "C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\sqlcmd.exe" (
        set "SQLCMD=C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\sqlcmd.exe"
        echo ✓ 在 SQL Server 2019 路径中找到 sqlcmd
    ) else if exist "C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\180\Tools\Binn\sqlcmd.exe" (
        set "SQLCMD=C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\180\Tools\Binn\sqlcmd.exe"
        echo ✓ 在 SQL Server 2022 路径中找到 sqlcmd
    ) else (
        echo ❌ 无法找到 sqlcmd 工具
        echo.
        echo 解决方案：
        echo 1. 重新运行 SQL Server 安装程序
        echo 2. 选择"修改"选项
        echo 3. 勾选"客户端工具"中的"命令行实用工具"
        echo 4. 完成安装后重新运行本程序
        echo.
        pause
        exit /b 1
    )
) else (
    set "SQLCMD=sqlcmd"
    echo ✓ sqlcmd 工具可用
)

REM 启动 LocalDB 实例
echo.
echo [3/5] 启动 LocalDB 实例...
sqllocaldb stop MSSQLLocalDB >nul 2>&1
timeout /t 2 >nul
sqllocaldb start MSSQLLocalDB >nul 2>&1
if errorlevel 1 (
    echo ⚠ LocalDB 实例启动失败，尝试创建新实例...
    sqllocaldb create MSSQLLocalDB >nul 2>&1
    sqllocaldb start MSSQLLocalDB >nul 2>&1
)
timeout /t 3 >nul
echo ✓ LocalDB 实例已启动

REM 附加数据库
echo.
echo [4/5] 附加数据库...
echo 正在检查并附加数据库文件，请稍候...
echo 数据库文件路径: %DB_FILE%
echo.

REM 先尝试删除已存在的数据库（如果存在）
echo 清理旧数据库...
%SQLCMD% -S "(localdb)\MSSQLLocalDB" -E -Q "IF DB_ID('%DB_NAME%') IS NOT NULL BEGIN ALTER DATABASE [%DB_NAME%] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [%DB_NAME%]; END" >nul 2>&1

REM 使用 ATTACH 方式附加数据库
echo 尝试附加数据库...
%SQLCMD% -S "(localdb)\MSSQLLocalDB" -E -Q "CREATE DATABASE [%DB_NAME%] ON (FILENAME = '%DB_FILE%') FOR ATTACH" 2>&1

if errorlevel 1 (
    echo.
    echo ⚠ ATTACH 方式失败，尝试使用 ATTACH_REBUILD_LOG 方式重建日志...
    %SQLCMD% -S "(localdb)\MSSQLLocalDB" -E -Q "CREATE DATABASE [%DB_NAME%] ON (FILENAME = '%DB_FILE%') FOR ATTACH_REBUILD_LOG" 2>&1
    
    if errorlevel 1 (
        echo.
        echo ❌ 数据库附加失败！
        echo.
        echo 请检查以下项目：
        echo 1. 确保文件 "%DB_FILE%" 存在且可访问
        echo 2. 数据库文件是否完整且未被损坏
        echo 3. 当前用户是否有足够的文件访问权限
        echo 4. 尝试以管理员身份重新运行此脚本
        echo 5. 检查 Data 文件夹是否有读写权限
        echo.
        pause
        exit /b 1
    )
)
echo ✓ 数据库附加成功

REM 验证数据库连接并配置权限
echo.
echo [5/5] 验证数据库连接...
echo 尝试连接数据库...
%SQLCMD% -S "(localdb)\MSSQLLocalDB" -d %DB_NAME% -E -Q "SELECT COUNT(*) as TableCount FROM sys.tables" 2>&1

if errorlevel 1 (
    echo.
    echo ⚠ 数据库连接验证失败，尝试修复权限...
    
    REM 将当前 Windows 用户添加到数据库并授予权限
    echo 配置数据库权限...
    %SQLCMD% -S "(localdb)\MSSQLLocalDB" -d %DB_NAME% -E -Q "CREATE USER [%USERDOMAIN%\%USERNAME%] FOR LOGIN [%USERDOMAIN%\%USERNAME%]; EXEC sp_addrolemember 'db_owner', '%USERDOMAIN%\%USERNAME%';" 2>&1
    
    REM 再次验证
    echo 再次尝试连接...
    %SQLCMD% -S "(localdb)\MSSQLLocalDB" -d %DB_NAME% -E -Q "SELECT COUNT(*) as TableCount FROM sys.tables" 2>&1
    if errorlevel 1 (
        echo.
        echo ❌ 数据库连接失败！
        echo 请检查数据库文件是否兼容当前 SQL Server 版本
        echo.
        pause
        exit /b 1
    )
    echo ✓ 数据库权限配置成功
)
echo ✓ 数据库连接验证成功

REM 启动程序
echo.
echo ═══════════════════════════════════════════════════════════════
echo ✅ 环境检查完成，正在启动魔女审判系统...
echo ═══════════════════════════════════════════════════════════════
echo.
start "" "%INSTALL_DIR%WitchTrialSystem.exe"

echo ✅ 程序已启动！
timeout /t 3 /nobreak >nul
exit
