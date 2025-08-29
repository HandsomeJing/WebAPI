# Git定时自动保存服务
# 每15分钟自动保存项目变更，后台运行
# 使用方法：.\auto-save-timer.ps1

param(
    [int]$IntervalMinutes = 15  # 默认15分钟，可以修改
)

# 设置Git路径
$env:PATH += ";D:\Program Files\Git\bin"

Write-Host "🚀 启动Git定时自动保存服务" -ForegroundColor Green
Write-Host "📁 监控目录: $(Get-Location)" -ForegroundColor Cyan
Write-Host "⏰ 保存间隔: $IntervalMinutes 分钟" -ForegroundColor Cyan
Write-Host "🛑 按 Ctrl+C 停止服务" -ForegroundColor Yellow
Write-Host "💡 提示: 你可以随时运行 .\auto-save.ps1 手动保存" -ForegroundColor Gray
Write-Host ""

# 自动保存函数
function Invoke-AutoSave {
    try {
        $status = git status --porcelain 2>$null
        if ($status) {
            $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
            $changedFiles = ($status | Measure-Object).Count
            
            Write-Host "📝 [$timestamp] 发现 $changedFiles 个文件变更，正在保存..." -ForegroundColor Yellow
            
            git add . 2>$null
            $commitMessage = "定时自动保存 - $timestamp"
            git commit -m $commitMessage 2>$null
            
            Write-Host "✅ [$timestamp] 自动保存完成！" -ForegroundColor Green
            
            # 显示最近的提交
            Write-Host "📋 最近提交:" -ForegroundColor Cyan
            git log --oneline -3
            Write-Host ""
        } else {
            $timestamp = Get-Date -Format "HH:mm:ss"
            Write-Host "ℹ️  [$timestamp] 没有新的变更需要保存" -ForegroundColor Blue
        }
    }
    catch {
        $timestamp = Get-Date -Format "HH:mm:ss"
        Write-Host "❌ [$timestamp] 自动保存时出错: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# 立即执行一次保存
Write-Host "🔄 执行初始保存检查..." -ForegroundColor Magenta
Invoke-AutoSave

# 计算间隔秒数
$IntervalSeconds = $IntervalMinutes * 60

Write-Host "⏳ 定时器已启动，每 $IntervalMinutes 分钟自动保存一次..." -ForegroundColor Green
Write-Host ""

# 主循环
try {
    while ($true) {
        Start-Sleep $IntervalSeconds
        Invoke-AutoSave
    }
}
catch [System.Management.Automation.PipelineStoppedException] {
    Write-Host ""
    Write-Host "🛑 定时自动保存服务已停止" -ForegroundColor Yellow
}
catch {
    Write-Host ""
    Write-Host "❌ 服务异常停止: $($_.Exception.Message)" -ForegroundColor Red
}
finally {
    Write-Host "🧹 服务清理完成" -ForegroundColor Gray
    
    # 执行最后一次保存
    Write-Host "🔄 执行最终保存..." -ForegroundColor Magenta
    Invoke-AutoSave
    
    Write-Host "👋 感谢使用Git自动保存服务！" -ForegroundColor Cyan
}
