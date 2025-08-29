# Git文件监控自动提交脚本 - 监控文件变化并自动保存
# 使用方法：在PowerShell中运行 .\watch-git.ps1

# 设置Git路径到当前会话
$env:PATH += ";D:\Program Files\Git\bin"

Write-Host "🚀 启动Git文件监控服务..." -ForegroundColor Green
Write-Host "📁 监控目录：$(Get-Location)" -ForegroundColor Cyan
Write-Host "⏰ 检查间隔：30秒" -ForegroundColor Cyan
Write-Host "🛑 按 Ctrl+C 停止监控" -ForegroundColor Yellow
Write-Host ""

# 创建文件系统监控器
$watcher = New-Object System.IO.FileSystemWatcher
$watcher.Path = (Get-Location).Path
$watcher.IncludeSubdirectories = $true
$watcher.EnableRaisingEvents = $true

# 忽略的文件和目录
$ignorePatterns = @(
    "*.tmp", "*.log", "*.swp", "*~",
    "bin", "obj", ".vs", ".git",
    "node_modules", "packages"
)

# 检查是否应该忽略的函数
function Should-Ignore($path) {
    foreach ($pattern in $ignorePatterns) {
        if ($path -like "*$pattern*") {
            return $true
        }
    }
    return $false
}

# 自动提交函数
function Auto-Commit {
    try {
        $status = git status --porcelain 2>$null
        if ($status) {
            $changedFiles = ($status | Measure-Object).Count
            Write-Host "📝 检测到 $changedFiles 个文件变更，正在自动保存..." -ForegroundColor Yellow
            
            git add . 2>$null
            $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
            $commitMessage = "自动保存变更 - $timestamp"
            
            git commit -m $commitMessage 2>$null
            Write-Host "✅ 自动保存完成！($timestamp)" -ForegroundColor Green
        }
    }
    catch {
        Write-Host "❌ 自动保存时出错：$($_.Exception.Message)" -ForegroundColor Red
    }
}

# 注册事件处理器
Register-ObjectEvent -InputObject $watcher -EventName "Created" -Action {
    $path = $Event.SourceEventArgs.FullPath
    if (-not (Should-Ignore $path)) {
        Write-Host "📁 新建: $($Event.SourceEventArgs.Name)" -ForegroundColor Green
        Start-Sleep 2  # 等待文件操作完成
        Auto-Commit
    }
} | Out-Null

Register-ObjectEvent -InputObject $watcher -EventName "Changed" -Action {
    $path = $Event.SourceEventArgs.FullPath
    if (-not (Should-Ignore $path)) {
        Write-Host "📝 修改: $($Event.SourceEventArgs.Name)" -ForegroundColor Yellow
        Start-Sleep 2  # 等待文件操作完成
        Auto-Commit
    }
} | Out-Null

Register-ObjectEvent -InputObject $watcher -EventName "Deleted" -Action {
    $path = $Event.SourceEventArgs.FullPath
    if (-not (Should-Ignore $path)) {
        Write-Host "🗑️  删除: $($Event.SourceEventArgs.Name)" -ForegroundColor Red
        Start-Sleep 2  # 等待文件操作完成
        Auto-Commit
    }
} | Out-Null

Register-ObjectEvent -InputObject $watcher -EventName "Renamed" -Action {
    $oldPath = $Event.SourceEventArgs.OldFullPath
    $newPath = $Event.SourceEventArgs.FullPath
    if (-not (Should-Ignore $newPath)) {
        Write-Host "📂 重命名: $($Event.SourceEventArgs.OldName) → $($Event.SourceEventArgs.Name)" -ForegroundColor Cyan
        Start-Sleep 2  # 等待文件操作完成
        Auto-Commit
    }
} | Out-Null

Write-Host "✅ 文件监控已启动！每当你修改、新建、删除文件时，都会自动保存到Git。" -ForegroundColor Green

# 定期检查（备用机制）
try {
    while ($true) {
        Start-Sleep 30
        Auto-Commit
    }
}
catch {
    Write-Host "🛑 监控服务已停止" -ForegroundColor Yellow
}
finally {
    # 清理事件监听器
    Get-EventSubscriber | Unregister-Event
    $watcher.Dispose()
    Write-Host "🧹 清理完成" -ForegroundColor Gray
}
