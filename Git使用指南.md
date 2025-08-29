# Git自动保存使用指南

## 🎯 功能说明
这套Git配置可以让你的项目实现类似"自动保存"的功能，确保代码不会意外丢失。

## 📋 已为你配置的功能

### 1. 基础Git仓库
- ✅ 已初始化Git仓库
- ✅ 已创建.gitignore文件（自动忽略临时文件）
- ✅ 已完成初始提交，保存当前所有代码

### 2. 自动保存脚本

#### 手动触发保存：`auto-git.ps1`
```powershell
.\auto-git.ps1
```
- 检查项目变更
- 自动提交所有改动
- 显示提交历史

#### 实时监控保存：`watch-git.ps1`
```powershell
.\watch-git.ps1
```
- 实时监控文件变化
- 自动保存新建、修改、删除的文件
- 后台运行，按Ctrl+C停止

## 🚀 推荐使用方式

### 方案1：开发时启动监控（推荐）
1. 打开PowerShell，进入项目目录
2. 运行：`.\watch-git.ps1`
3. 正常开发，系统会自动保存所有变更
4. 工作结束时按Ctrl+C停止

### 方案2：定期手动保存
1. 开发一段时间后
2. 运行：`.\auto-git.ps1`
3. 查看保存结果

## 📚 常用Git命令

### 查看历史
```powershell
git log --oneline        # 查看提交历史
git log --graph          # 图形化显示分支
```

### 版本回退
```powershell
git log --oneline                    # 先查看提交ID
git checkout [提交ID]               # 回到指定版本
git checkout main                    # 返回最新版本
```

### 查看变更
```powershell
git status              # 查看当前状态
git diff                # 查看具体变更内容
```

### 分支管理
```powershell
git branch feature-xxx  # 创建新分支
git checkout feature-xxx # 切换分支
git merge feature-xxx   # 合并分支
```

## 🛡️ 数据安全保障

### 本地备份
- 所有变更都保存在本地Git仓库
- 可以随时回退到任何历史版本
- 即使误删文件也能完全恢复

### 建议的远程备份
1. 在GitHub/Gitee创建远程仓库
2. 添加远程仓库：
   ```powershell
   git remote add origin [仓库地址]
   git push -u origin main
   ```
3. 定期推送：`git push`

## ⚡ 快速恢复技巧

### 恢复删除的文件
```powershell
git checkout HEAD -- [文件名]    # 恢复单个文件
git checkout HEAD -- .           # 恢复所有文件
```

### 撤销最近的提交
```powershell
git reset --soft HEAD~1          # 撤销提交但保留变更
git reset --hard HEAD~1          # 完全撤销（慎用）
```

## 🎉 现在你可以放心开发了！
- 文件不会意外丢失
- 可以随时回到任何历史版本
- 支持实时自动保存
- 完整的变更记录追踪
