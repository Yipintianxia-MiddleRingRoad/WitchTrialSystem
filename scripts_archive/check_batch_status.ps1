# 批次状态检查脚本
# 快速检查批次1和批次2的完整状态

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║   批次状态检查工具                    ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# 设置UTF-8编码
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# 数据库连接参数
$server = "localhost"
$database = "WitchTrialWT"

# 执行检查脚本
Write-Host "正在查询数据库..." -ForegroundColor Yellow
Write-Host ""

sqlcmd -S $server -d $database -E -i "check_batch_status.sql" -f 65001 -W

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ 检查完成！" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "❌ 检查过程中出现错误" -ForegroundColor Red
}

Write-Host ""
Write-Host "提示：查看上方输出了解详细信息" -ForegroundColor Yellow
Write-Host ""

