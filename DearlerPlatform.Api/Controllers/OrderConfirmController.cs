using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Core.Consts;
using DearlerPlatform.Core.Repository;
using DearlerPlatform.Service.OrderApp;
using DearlerPlatform.Service.OrderApp.Dto;
using DearlerPlatform.Service.ProductApp;
using DearlerPlatform.Service.ShoppingCartApp;
using DearlerPlatform.Service.ShoppingCartApp.Dto;
using DearlerPlatfrom.Api.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DearlerPlatfrom.Api.Controllers
{
    /// <summary>
    /// 订单确认接口 - 处理订单确认和提交流程
    /// 整合购物车、产品、订单服务完成下单操作
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [CtmAuthorizationFilter]
    public class OrderConfirmController : BaseController
    {
        public OrderConfirmController(
            IShoppingCartAppService shoppingCartAppService,
            IProductService productService,
            IOrderService orderService)
        {
            ShoppingCartAppService = shoppingCartAppService;
            ProductService = productService;
            OrderService = orderService;
        }

        public IShoppingCartAppService ShoppingCartAppService { get; }
        public IProductService ProductService { get; }
        public IOrderService OrderService { get; }

        [HttpGet]
        public async Task<IEnumerable<ShoppingCartDto>> Get()
        {
          var customerNo =  HttpContext.Items[HttpContextItemKeyName.CUSTOMER_NO]?.ToString() ?? "";
          var carts = (await ShoppingCartAppService.GetShoppingCartDtos(customerNo)).Where(m=>m.CartSelected);
          return carts;
        }

        /// <summary>
        /// 获取当前客户最近订单号
        /// 用于前端下单成功后跳转到订单详情页
        /// </summary>
        /// <returns>最近订单号（无则返回空字符串）</returns>
        [HttpGet("LatestOrderNo")]
        public async Task<string> GetLatestOrderNo()
        {
            var customerNo = HttpContext.Items[HttpContextItemKeyName.CUSTOMER_NO]?.ToString() ?? string.Empty;
            return await OrderService.GetLatestOrderNo(customerNo);
        }
        [HttpPost]
        public async Task<bool> Add(OrderMasterInputDto input)
        {
             var customerNo =  HttpContext.Items[HttpContextItemKeyName.CUSTOMER_NO]?.ToString() ?? "";
             var carts = (await ShoppingCartAppService.GetShoppingCartDtos(customerNo)).Where(m=>m.CartSelected).ToList();
             return await OrderService.AddOrder(customerNo,input,carts);
        }
    }
}