# DearlerPlatform 经销商平台

一个经销商B2B订货平台，包含后端 .NET 9 WebAPI 与前端 Vue3 的完整项目。

## 项目结构

### 后端服务
- `DearlerPlatform.Api/` - WebAPI 主项目（控制器、配置、启动）
- `DearlerPlatform.Core/` - 核心层（DbContext、数据访问、分页）
- `DearlerPlatform.Domain/` - 领域模型（实体类：Product、Order、Customer等）
- `DearlerPlatform.Service/` - 业务服务层（应用服务、业务逻辑）
- `DearlerPlatform.Common/` - 公共组件（Redis、Token、MD5、事件总线）
- `DearlerPlatform.Extensions/` - 依赖注入扩展和配置

### 前端应用
- `dearler_platform_ui/` - Vue3 前端（完整的B2B订货系统界面）

### 配置文件
- `DearlerPlatform.sln` - Visual Studio 解决方案文件
- `DearlerPlatform.code-workspace` - VS Code 工作区配置
- `docker-compose.yml` - Docker 服务编排（数据库等）

## 功能特性

### 核心业务功能
- 🔐 **用户认证** - 经销商登录、JWT Token认证
- 📦 **商品管理** - 商品分类、搜索、筛选、属性过滤
- 🛒 **购物车** - 添加商品、数量调整、选择结算
- 📋 **订单管理** - 下单确认、订单列表、订单详情
- 💰 **价格计算** - 实时价格计算、合计统计
- 🏷️ **开票管理** - 支持多开票人选择

### 技术特性
- 🏗️ **分层架构** - 清晰的DDD分层设计
- 📱 **响应式UI** - 移动端适配的Vue3界面
- 🔄 **状态管理** - Vuex状态管理购物车等数据
- 🌐 **API设计** - RESTful API设计规范
- 💾 **数据持久化** - Entity Framework Core + SQL Server
- ⚡ **性能优化** - Redis缓存、分页查询
- 🔧 **开发工具** - AutoMapper、Swagger文档

## 快速开始

### 环境要求
- .NET 9 SDK
- Node.js 16+
- SQL Server 2019+
- Redis（可选，用于缓存）

### 后端启动

```powershell
# 进入API项目目录
cd DearlerPlatform.Api

# 还原依赖包
dotnet restore

# 启动开发服务器
dotnet watch run
```

后端服务地址：`http://localhost:7032`
Swagger文档：`http://localhost:7032/swagger`

### 前端启动

```powershell
# 进入前端项目目录
cd dearler_platform_ui

# 安装依赖
npm install

# 启动开发服务器
npm run dev
```

前端访问地址：`http://localhost:3000`

## 开发说明

### API接口
- 所有接口都需要JWT Token认证
- 通过登录接口获取Token后，前端会自动在请求头中携带
- 接口文档可通过Swagger查看和测试

### 数据库配置
- 默认使用SQL Server数据库
- 连接字符串在 `appsettings.json` 中配置
- 首次运行会自动创建数据库和表结构

### 开发调试
- 后端支持热重载：使用 `dotnet watch run`
- 前端支持热更新：使用 `npm run dev`
- 建议同时启动后端和前端进行联调

## 项目特色

### 前端亮点
- 📱 移动端优先的响应式设计
- 🎨 仿移动电商的用户体验
- 🔄 实时购物车数量同步
- 📋 完整的订单流程
- 🏷️ 商品分类和属性筛选

### 后端亮点
- 🏗️ 清晰的分层架构设计
- 📦 完整的业务域建模
- 🔐 JWT Token安全认证
- 📊 支持分页和排序
- 🚀 高性能的数据访问层

## 技术栈

### 后端技术
- ASP.NET Core 9
- Entity Framework Core
- AutoMapper
- JWT Authentication
- Swagger/OpenAPI
- Redis (可选)

### 前端技术
- Vue 3 + Composition API
- Vite 构建工具
- Element Plus UI组件
- Vue Router 路由管理
- Vuex 状态管理
- Axios HTTP客户端

## 许可证

本项目仅供学习和参考使用。

1. 启动依赖（Docker）

- 在仓库根目录运行（需要 Docker）：

```powershell
docker compose up -d
```

 这通常会启动一个 SQL Server 实例和一个 Redis 实例（具体以仓库的 docker-compose.yml 为准）。

2. 初始化数据库（如果你要从迁移启动）：

```powershell
cd DearlerPlatform.Api
dotnet ef database update
```

3. 运行后端：

```powershell
dotnet watch run
```

4. 运行前端（可选）：按照上文的 Vue 运行步骤启动前端。

## 常见问题

- Redis 未启动：后端有降级逻辑，主流程仍能工作；若需要缓存功能请在 `appsettings` 中配置 Redis 连接。
- 端口占用：修改项目的 `launchSettings.json` 或后端监听地址，前端代理也可调整。
