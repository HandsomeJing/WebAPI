# Git Auto Save Script
# Usage: .\auto-git.ps1

$env:PATH += ";D:\Program Files\Git\bin"

Write-Host "Checking for changes..." -ForegroundColor Green

$status = git status --porcelain
if ($status) {
    Write-Host "Found changes:" -ForegroundColor Yellow
    git status --short
    
    git add .
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $commitMessage = "Auto save - $timestamp"
    
    git commit -m $commitMessage
    
    Write-Host "Project auto-saved!" -ForegroundColor Green
    Write-Host "Commit: $commitMessage" -ForegroundColor Cyan
} else {
    Write-Host "No changes to save" -ForegroundColor Blue
}

Write-Host "`nRecent commits:" -ForegroundColor Magenta
git log --oneline -5
