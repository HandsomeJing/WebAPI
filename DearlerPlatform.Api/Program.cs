using DearlerPlatform.Service.CustomerApp;
using DearlerPlatfrom.Api.Extensions;
using Microsoft.AspNetCore.Builder;

// 程序入口 - 配置和启动Web应用
var builder = WebApplication.CreateBuilder(args);

// 注册服务到容器
builder.Services.ServiceEnter();

var app = builder.Build();
// 配置HTTP请求管道

// 初始化中间件
app.InitEnter();

// 配置路由映射
app.InitMap();


app.Run();
