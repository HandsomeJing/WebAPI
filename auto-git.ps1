# Git自动提交脚本 - 实现项目变更的自动保存
# 使用方法：在PowerShell中运行 .\auto-git.ps1

# 设置Git路径到当前会话
$env:PATH += ";D:\Program Files\Git\bin"

Write-Host "🔄 开始检查项目变更..." -ForegroundColor Green

# 检查是否有变更
$status = git status --porcelain
if ($status) {
    Write-Host "📝 发现以下变更：" -ForegroundColor Yellow
    git status --short
    
    # 添加所有变更
    git add .
    
    # 生成提交信息（包含时间戳）
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $commitMessage = "自动保存 - $timestamp"
    
    # 提交变更
    git commit -m $commitMessage
    
    Write-Host "✅ 项目已自动保存到Git！" -ForegroundColor Green
    Write-Host "提交信息：$commitMessage" -ForegroundColor Cyan
} else {
    Write-Host "ℹ️  当前没有需要保存的变更" -ForegroundColor Blue
}

Write-Host "`n📊 最近5次提交记录：" -ForegroundColor Magenta
git log --oneline -5

Write-Host "`n💡 提示：你可以用 'git log' 查看完整提交历史" -ForegroundColor Gray
Write-Host "💡 提示：你可以用 'git checkout 提交ID' 回到任何历史版本" -ForegroundColor Gray
