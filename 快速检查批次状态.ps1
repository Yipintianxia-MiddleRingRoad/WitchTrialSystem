# 快速检查批次1和批次2的状态
$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║   快速批次状态检查                    ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$server = "localhost"
$database = "WitchTrialWT"

# 1. 批次信息
Write-Host "【批次信息】" -ForegroundColor Yellow
$batchInfo = sqlcmd -S $server -d $database -E -Q @"
SELECT 
    CAST(BatchID AS NVARCHAR) + ' | ' + 
    CAST(BatchNumber AS NVARCHAR) + ' | ' + 
    CAST(WitchCount AS NVARCHAR) AS Info
FROM wt.Batch
ORDER BY BatchID;
"@ -f 65001 -h -1 -W
Write-Host $batchInfo
Write-Host ""

# 2. 批次1魔女数量
Write-Host "【批次1 - 魔女】" -ForegroundColor Yellow
$batch1Witches = sqlcmd -S $server -d $database -E -Q "SELECT COUNT(*) FROM wt.Witch WHERE BatchID = 1;" -f 65001 -h -1 -W
Write-Host "  数量: $($batch1Witches.Trim())" -ForegroundColor White
$batch1List = sqlcmd -S $server -d $database -E -Q @"
SELECT PrisonerNo + ' - ' + Name AS Info
FROM wt.Witch
WHERE BatchID = 1
ORDER BY PrisonerNo;
"@ -f 65001 -h -1 -W
Write-Host $batch1List
Write-Host ""

# 3. 批次2魔女数量
Write-Host "【批次2 - 魔女】" -ForegroundColor Yellow
$batch2Witches = sqlcmd -S $server -d $database -E -Q "SELECT COUNT(*) FROM wt.Witch WHERE BatchID = 2;" -f 65001 -h -1 -W
Write-Host "  数量: $($batch2Witches.Trim())" -ForegroundColor White
$batch2List = sqlcmd -S $server -d $database -E -Q @"
SELECT PrisonerNo + ' - ' + Name AS Info
FROM wt.Witch
WHERE BatchID = 2
ORDER BY PrisonerNo;
"@ -f 65001 -h -1 -W
Write-Host $batch2List
Write-Host ""

# 4. 批次1用户数量
Write-Host "【批次1 - 用户账号】" -ForegroundColor Yellow
$batch1Users = sqlcmd -S $server -d $database -E -Q "SELECT COUNT(*) FROM wt.[User] WHERE BatchID = 1;" -f 65001 -h -1 -W
Write-Host "  数量: $($batch1Users.Trim())" -ForegroundColor White
Write-Host ""

# 5. 批次2用户数量
Write-Host "【批次2 - 用户账号】" -ForegroundColor Yellow
$batch2Users = sqlcmd -S $server -d $database -E -Q "SELECT COUNT(*) FROM wt.[User] WHERE BatchID = 2;" -f 65001 -h -1 -W
Write-Host "  数量: $($batch2Users.Trim())" -ForegroundColor White
Write-Host ""

# 6. 检查重复
Write-Host "【重复检查】" -ForegroundColor Yellow
$duplicates = sqlcmd -S $server -d $database -E -Q @"
SELECT PrisonerNo, COUNT(*) AS Cnt
FROM wt.Witch
GROUP BY PrisonerNo
HAVING COUNT(*) > 1;
"@ -f 65001 -h -1 -W
if ($duplicates.Trim() -eq "") {
    Write-Host "  ✓ 没有重复的囚犯编号" -ForegroundColor Green
} else {
    Write-Host "  ⚠️  发现重复:" -ForegroundColor Red
    Write-Host $duplicates
}
Write-Host ""

# 7. 关联检查
Write-Host "【用户-魔女关联】" -ForegroundColor Yellow
$batch1Assoc = sqlcmd -S $server -d $database -E -Q @"
SELECT COUNT(*) FROM wt.UserWitch uw
JOIN wt.[User] u ON uw.UserID = u.UserID
WHERE u.BatchID = 1;
"@ -f 65001 -h -1 -W
$batch2Assoc = sqlcmd -S $server -d $database -E -Q @"
SELECT COUNT(*) FROM wt.UserWitch uw
JOIN wt.[User] u ON uw.UserID = u.UserID
WHERE u.BatchID = 2;
"@ -f 65001 -h -1 -W
Write-Host "  批次1关联数: $($batch1Assoc.Trim())" -ForegroundColor White
Write-Host "  批次2关联数: $($batch2Assoc.Trim())" -ForegroundColor White
Write-Host ""

# 总结
Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║   总结                                ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Cyan

$totalWitches = sqlcmd -S $server -d $database -E -Q "SELECT COUNT(*) FROM wt.Witch;" -f 65001 -h -1 -W
$totalUsers = sqlcmd -S $server -d $database -E -Q "SELECT COUNT(*) FROM wt.[User];" -f 65001 -h -1 -W

Write-Host "  总魔女数: $($totalWitches.Trim())" -ForegroundColor White
Write-Host "  总用户数: $($totalUsers.Trim())" -ForegroundColor White
Write-Host ""

