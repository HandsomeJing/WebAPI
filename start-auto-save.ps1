# DearlerPlatform Project Auto-Save Service
# 每15分钟自动保存项目代码到Git（不包含脚本文件）

$env:PATH += ";D:\Program Files\Git\bin"

Write-Host "🚀 DearlerPlatform 项目自动保存启动" -ForegroundColor Green
Write-Host "📁 监控：DearlerPlatform 项目代码" -ForegroundColor Cyan  
Write-Host "⏰ 间隔：15分钟自动保存" -ForegroundColor Cyan
Write-Host "🛑 停止：按 Ctrl+C" -ForegroundColor Yellow
Write-Host ""

function Save-DearlerProject {
    try {
    # 只添加项目相关文件，排除脚本；同时纳入前端目录（dearler_platform_ui）
    git add "DearlerPlatform.*/" "dearler_platform_ui/" "*.sln" 2>$null
        
        # 检查是否有变更
        $status = git diff --cached --name-only 2>$null
        if ($status) {
            $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss" 
            $fileCount = ($status | Measure-Object).Count
            
            git commit -m "项目自动保存 - $timestamp"
            Write-Host "✅ [$timestamp] 保存了 $fileCount 个项目文件" -ForegroundColor Green
            
            # 显示保存的文件
            Write-Host "📋 保存的文件：" -ForegroundColor Cyan
            $status | ForEach-Object { Write-Host "   $_" -ForegroundColor Gray }
            Write-Host ""
        } else {
            $time = Get-Date -Format "HH:mm:ss"
            Write-Host "ℹ️  [$time] 项目无变更" -ForegroundColor Blue
        }
    }
    catch {
        Write-Host "❌ 保存出错：$($_.Exception.Message)" -ForegroundColor Red
    }
}

# 初始保存检查
Write-Host "🔄 执行初始检查..." -ForegroundColor Magenta
Save-DearlerProject

Write-Host "⏳ 定时器启动，每15分钟检查项目变更..." -ForegroundColor Green
Write-Host ""

# 主循环
try {
    while ($true) {
        Start-Sleep 900  # 15分钟
        Save-DearlerProject  
    }
}
catch [System.Management.Automation.PipelineStoppedException] {
    Write-Host ""
    Write-Host "🛑 自动保存服务已停止" -ForegroundColor Yellow
}
finally {
    # 最后一次保存
    Write-Host "🔄 执行最终保存..." -ForegroundColor Magenta
    Save-DearlerProject
    Write-Host "👋 DearlerPlatform 自动保存服务结束！" -ForegroundColor Cyan
}
