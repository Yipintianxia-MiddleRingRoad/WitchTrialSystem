# 导出完整数据库（结构+数据）到 SQL 脚本
# 使用方法：在 PowerShell 中运行此脚本

$ServerInstance = "localhost"  # 你的 SQL Server 实例
$Database = "WitchTrialWT"
$OutputFile = "database_scripts\WitchTrialWT_完整备份.sql"

# 使用 mssql-scripter 工具（需要先安装）
# pip install mssql-scripter

mssql-scripter `
    -S $ServerInstance `
    -d $Database `
    --schema-and-data `
    --target-server-version "2019" `
    --file-path $OutputFile `
    --encoding "utf-8"

Write-Host "数据库导出完成: $OutputFile" -ForegroundColor Green
