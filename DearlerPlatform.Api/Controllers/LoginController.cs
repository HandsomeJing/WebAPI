using System.Threading.Tasks;
using DearlerPlatform.Common.TokenModule;
using DearlerPlatform.Service.CustomerApp;
using DearlerPlatform.Service.CustomerApp.Dto;
using DearlerPlatfrom.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DearlerPlatform.Api.Controllers
{
    /// <summary>
    /// 登录控制器 - 处理用户身份验证相关操作
    /// 主要功能：验证用户名密码，生成JWT令牌
    /// </summary>
    public class LoginController : BaseController
    {
        public LoginController(ICustomerService customerService, IConfiguration configuration)
        {
            CustomerService = customerService;
            Configuration = configuration;
        }

        public ICustomerService CustomerService { get; }
        public IConfiguration Configuration { get; }
        
        /// <summary>
        /// 用户登录验证 - 核心登录接口
        /// 验证用户名密码正确性，成功后返回JWT令牌用于后续接口调用
        /// </summary>
        [HttpPost]
        public async Task<string> CheckLogin(CustomerLoginDto dto)
        {
            // 基础参数校验 - 用户名和密码都不能为空
            if (string.IsNullOrWhiteSpace(dto.CustomerNo) || string.IsNullOrWhiteSpace(dto.Password))
            {
                HttpContext.Response.StatusCode = 400;
                return "NonLoginInfo";
            }
            
            // 验证密码是否正确 - 调用业务层进行密码MD5对比
            var isSuccess = await CustomerService.CheckPassword(dto);
            if (isSuccess)
            {
                // 获取完整的用户信息用于生成JWT
                var customer = await CustomerService.GetCustomerAsync(dto.CustomerNo);
                if (customer == null)
                {
                    HttpContext.Response.StatusCode = 400;
                    return "NonUser";
                }
                // 生成JWT令牌返回给前端
                return GetToken(customer.Id, customer.CustomerNo, customer.CustomerName);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
                return "NonUser";
            }
        }

        /// <summary>
        /// 生成JWT令牌 - 根据用户信息创建访问凭证
        /// 包含用户基本信息，用于后续API调用的身份验证
        /// </summary>
        private string GetToken(int customerId, string customerNo, string customerName)
        {
            var token = Configuration.GetSection("Jwt").Get<JwtTokenModel>();
            if (token == null)
            {
                throw new InvalidOperationException("JWT configuration not found");
            }
            token.Id = customerId;
            token.CustomerNo = customerNo;
            token.CustomerName = customerName;
            return TokenHelper.CreateToken(token);
        }
    }
}