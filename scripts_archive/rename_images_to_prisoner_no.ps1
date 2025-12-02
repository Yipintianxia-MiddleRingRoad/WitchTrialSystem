# ========================================
# Image Renaming Script
# From name-based to prisoner number-based
# ========================================

$ErrorActionPreference = "Stop"

# Mapping
$mapping = @{
    "ema.png"    = "658.png"
    "hiro.png"   = "659.png"
    "anan.png"   = "660.png"
    "noah.png"   = "661.png"
    "leia.png"   = "662.png"
    "miria.png"  = "663.png"
    "margo.png"  = "664.png"
    "nanoka.png" = "665.png"
    "alisa.png"  = "666.png"
    "sherry.png" = "667.png"
    "hanna.png"  = "668.png"
    "coco.png"   = "669.png"
    "meruru.png" = "670.png"
}

$imagesPath = "Images"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Start renaming image files" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check Images directory
if (-not (Test-Path $imagesPath)) {
    Write-Host "ERROR: Images directory not found!" -ForegroundColor Red
    exit 1
}

# Create backup
$backupPath = "Images_backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
Write-Host "Creating backup: $backupPath" -ForegroundColor Yellow
Copy-Item -Path $imagesPath -Destination $backupPath -Recurse
Write-Host "Backup completed" -ForegroundColor Green
Write-Host ""

# Rename files
$successCount = 0
$skipCount = 0
$errorCount = 0

foreach ($oldName in $mapping.Keys) {
    $newName = $mapping[$oldName]
    $oldPath = Join-Path $imagesPath $oldName
    $newPath = Join-Path $imagesPath $newName
    
    if (Test-Path $oldPath) {
        try {
            if (Test-Path $newPath) {
                Write-Host "SKIP: $newName already exists" -ForegroundColor Yellow
                $skipCount++
            } else {
                Rename-Item -Path $oldPath -NewName $newName
                Write-Host "OK: $oldName -> $newName" -ForegroundColor Green
                $successCount++
            }
        } catch {
            Write-Host "ERROR: $oldName - $($_.Exception.Message)" -ForegroundColor Red
            $errorCount++
        }
    } else {
        Write-Host "SKIP: $oldName not found" -ForegroundColor Yellow
        $skipCount++
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Renaming completed!" -ForegroundColor Cyan
Write-Host "Success: $successCount" -ForegroundColor Green
Write-Host "Skipped: $skipCount" -ForegroundColor Yellow
Write-Host "Failed: $errorCount" -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Backup location: $backupPath" -ForegroundColor White
Write-Host ""
