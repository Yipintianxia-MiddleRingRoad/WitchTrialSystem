param(
    [string]$Server = "localhost",
    [string]$Database = "WitchTrialWT",
    [int[]]$BatchIds = @(2),
    [string]$Password = "123456"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Batch password reset" -ForegroundColor Cyan
Write-Host "Target batches: $($BatchIds -join ', ')" -ForegroundColor Yellow
Write-Host "========================================`n" -ForegroundColor Cyan

function New-HexSalt([int]$bytes = 16) {
    $buffer = New-Object byte[] $bytes
    [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($buffer)
    return ($buffer | ForEach-Object { $_.ToString("X2") }) -join ''
}

function Get-Sha256Hex([string]$text) {
    $sha = [System.Security.Cryptography.SHA256]::Create()
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($text)
    $hash = $sha.ComputeHash($bytes)
    return ($hash | ForEach-Object { $_.ToString("X2") }) -join ''
}

$connString = "Server=$Server;Database=$Database;Trusted_Connection=True;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection $connString
$conn.Open()

$batchList = ($BatchIds | ForEach-Object { $_ }) -join ","
$selectCmd = $conn.CreateCommand()
$selectCmd.CommandText = "SELECT UserID, Username, BatchID FROM wt.[User] WHERE BatchID IN ($batchList) ORDER BY BatchID, Username;"
$reader = $selectCmd.ExecuteReader()
$users = @()
while ($reader.Read()) {
    $users += [pscustomobject]@{
        UserID = $reader["UserID"]
        Username = $reader["Username"]
        BatchID = $reader["BatchID"]
    }
}
$reader.Close()

if ($users.Count -eq 0) {
    Write-Host "No matching users found." -ForegroundColor Yellow
    $conn.Close()
    exit 0
}

$updateCmd = $conn.CreateCommand()
$updateCmd.CommandText = "UPDATE wt.[User] SET Salt = @Salt, PasswordHash = @Hash WHERE UserID = @UserID;"
$saltParam = $updateCmd.Parameters.Add("@Salt", [System.Data.SqlDbType]::VarChar, 64)
$hashParam = $updateCmd.Parameters.Add("@Hash", [System.Data.SqlDbType]::VarChar, 128)
$userParam = $updateCmd.Parameters.Add("@UserID", [System.Data.SqlDbType]::Int)

foreach ($user in $users) {
    $salt = New-HexSalt 16
    $hash = Get-Sha256Hex ($Password + $salt)

    $saltParam.Value = $salt
    $hashParam.Value = $hash
    $userParam.Value = $user.UserID

    $updateCmd.ExecuteNonQuery() | Out-Null
    Write-Host ("User {0} (batch {1}) password reset" -f $user.Username, $user.BatchID) -ForegroundColor Green
}

$conn.Close()

Write-Host "`nCompleted password reset for $($users.Count) users. Default password: $Password" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

