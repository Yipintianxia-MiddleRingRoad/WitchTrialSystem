# 执行数据库清理脚本
$serverInstance = "localhost"
$databaseName = "WitchTrialWT"
$sqlScriptPath = "e:\WitchTrialSystem\清理重复魔女数据.sql"

try {
    Write-Host "正在连接到数据库并执行清理脚本..."
    
    # 读取SQL脚本
    $sqlScript = Get-Content $sqlScriptPath -Raw
    
    # 执行SQL
    $connectionString = "Server=$serverInstance;Database=$databaseName;Integrated Security=True;"
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    $command = New-Object System.Data.SqlClient.SqlCommand($sqlScript, $connection)
    $command.CommandTimeout = 300  # 5分钟超时
    
    $result = $command.ExecuteNonQuery()
    
    $connection.Close()
    
    Write-Host "✅ 数据库清理脚本执行成功！"
    Write-Host "现在图鉴界面应该不再显示重复的缩略图了"
    Write-Host ""
    Write-Host "请重新启动程序查看效果"
    
} catch {
    Write-Host "❌ 执行失败: $($_.Exception.Message)"
    Write-Host "请检查SQL Server服务是否正在运行，以及连接字符串是否正确"
    Write-Host ""
    Write-Host "如果自动执行失败，请手动在SQL Server Management Studio中执行以下文件："
    Write-Host $sqlScriptPath
}