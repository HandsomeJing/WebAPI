# DearlerPlatform

一个包含后端 .NET 9 WebAPI 与前端 Vue3 的一体化示例项目。

## 后端（DearlerPlatform.Api）

- 技术栈：ASP.NET Core 9、EF Core、AutoMapper、JWT、CORS、Swagger。
- 关键能力：
  - 购物车、下单、订单详情（含件数/合计/仓库信息）。
  - 下单后通过新增 GET 获取“最近一次订单号”，前端跳转详情。
  - “再次购买”接口支持把历史订单复制到购物车。
  - Redis 可选，断开时自动降级，不影响主流程。

启动：

- VS Code 任务：build / publish / watch。
- 或命令行：在 `DearlerPlatform.Api` 目录执行 `dotnet watch run`。
- 默认地址：<http://localhost:7032>（前端通过 /api 代理到此）。

## 前端（dearler_platform_ui 或 dealer-platform-web）

- 技术栈：Vue 3、Vite、Element Plus、Axios、Vue Router。
- 主要页面：登录、商品列表、购物车、下单确认、订单详情。
- 已移除：商品详情页（后续增强再启用）。

开发：

- 进入对应前端目录，执行 `npm i` 后 `npm run dev`。
- 本地开发使用 Vite 代理：`/api -> http://localhost:7032`。

## 常见问题

- Redis 未启动：后端会降级继续工作；需要时在 appsettings 设置连接串，默认 127.0.0.1:6379。
- 端口占用：修改 `launchSettings.json` 或 `Properties` 中配置，或改前端代理。

## 目录说明

- `DearlerPlatform.Api/` 后端 WebAPI
- `dearler_platform_ui/` 前端（较完整 UI）
- `dealer-platform-web/` 前端（早期目录，已忽略构建产物与依赖）

## 许可

内部示例项目，按需使用。
