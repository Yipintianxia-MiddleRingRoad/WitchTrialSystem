@echo off
chcp 65001 >nul
echo ═══════════════════════════════════════════════════════════════
echo   魔女审判系统 v1.6.2 启动程序
echo ═══════════════════════════════════════════════════════════════
echo.

set INSTALL_DIR=%~dp0
set DB_NAME=WitchTrialWT
set DB_FILE=%INSTALL_DIR%Data\WitchTrialWT.mdf
set DB_LOG_FILE=%INSTALL_DIR%Data\WitchTrialWT_log.ldf

REM 检查数据库文件是否存在（最重要！）
echo [1/5] 检查数据库文件...
if not exist "%DB_FILE%" (
    echo ❌ 错误：数据库文件不存在！
    echo.
    echo 缺少文件：%DB_FILE%
    echo.
    echo 请确认：
    echo 1. Data 文件夹中必须包含 WitchTrialWT.mdf 文件
    echo 2. 该文件包含所有系统数据，不能缺失
    echo 3. 请从完整的发布包中重新获取
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

REM 先尝试删除已存在的数据库（如果存在）
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -Q "IF DB_ID('%DB_NAME%') IS NOT NULL BEGIN ALTER DATABASE [%DB_NAME%] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [%DB_NAME%]; END" >nul 2>&1

REM 使用 ATTACH 方式附加数据库
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -Q "CREATE DATABASE [%DB_NAME%] ON (FILENAME = '%DB_FILE%') FOR ATTACH" >nul 2>&1

if errorlevel 1 (
    echo ⚠ ATTACH 方式失败，尝试使用 ATTACH_REBUILD_LOG 方式重建日志...
    sqlcmd -S "(localdb)\MSSQLLocalDB" -E -Q "CREATE DATABASE [%DB_NAME%] ON (FILENAME = '%DB_FILE%') FOR ATTACH_REBUILD_LOG" >nul 2>&1
    
    if errorlevel 1 (
        echo ❌ 数据库附加失败！
        echo.
        echo 请检查以下项目：
        echo 1. 确保文件 "%DB_FILE%" 存在且可访问
        echo 2. 数据库文件是否完整且未被损坏
        echo 3. 当前用户是否有足够的文件访问权限
        echo 4. 尝试以管理员身份重新运行此脚本
        echo.
        pause
        exit /b 1
    )
)
echo ✓ 数据库附加成功

REM 验证数据库连接并配置权限
echo.
echo [5/5] 验证数据库连接...
sqlcmd -S "(localdb)\MSSQLLocalDB" -d %DB_NAME% -E -Q "SELECT COUNT(*) as TableCount FROM sys.tables" >nul 2>&1
if errorlevel 1 (
    echo ⚠ 数据库连接验证失败，尝试修复权限...
    
    REM 将当前 Windows 用户添加到数据库并授予权限
    sqlcmd -S "(localdb)\MSSQLLocalDB" -d %DB_NAME% -E -Q "CREATE USER [%USERDOMAIN%\%USERNAME%] FOR LOGIN [%USERDOMAIN%\%USERNAME%]; EXEC sp_addrolemember 'db_owner', '%USERDOMAIN%\%USERNAME%';" >nul 2>&1
    
    REM 再次验证
    sqlcmd -S "(localdb)\MSSQLLocalDB" -d %DB_NAME% -E -Q "SELECT COUNT(*) as TableCount FROM sys.tables" >nul 2>&1
    if errorlevel 1 (
        echo ❌ 数据库连接失败！
        echo 请检查数据库文件是否兼容当前 SQL Server 版本
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
