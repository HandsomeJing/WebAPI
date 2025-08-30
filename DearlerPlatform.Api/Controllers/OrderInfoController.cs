using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Core.Consts;
using DearlerPlatform.Service.CustomerApp;
using DearlerPlatform.Service.OrderApp;
using DearlerPlatform.Service.OrderApp.Dto;
using DearlerPlatfrom.Api.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DearlerPlatfrom.Api.Controllers
{
    /// <summary>
    /// 订单信息接口 - 查询和显示订单详情
    /// 提供订单历史、状态跟踪等功能
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [CtmAuthorizationFilter]
    public class OrderInfoController : BaseController
    {
        public OrderInfoController(IOrderService orderService,ICustomerService customerService)
        {
            OrderService = orderService;
        }

        public IOrderService OrderService { get; }

        [HttpGet]
        public async Task<SaleOrderDto> GetSaleOrderDto(string orderNo)
        {
            // var customerNo = HttpContext.Items[HttpContextItemKeyName.CUSTOMER_NO].ToString();
           return await OrderService.GetOrderInfoByOrderNo(orderNo);
        }

        /// <summary>
        /// 订单列表（最近N条）
        /// </summary>
        /// <param name="take">返回条数，默认20</param>
        /// <returns></returns>
        [HttpGet("List")]
        public async Task<IEnumerable<SaleOrderDto>> List(int take = 20)
        {
            var customerNo = HttpContext.Items[HttpContextItemKeyName.CUSTOMER_NO]?.ToString() ?? string.Empty;
            return await OrderService.GetOrdersByCustomer(customerNo, take);
        }

        /// <summary>
        /// 再次购买
        /// 根据原订单号复制生成一笔新订单
        /// </summary>
        /// <param name="saleOrderNo">原订单号</param>
        /// <returns>是否成功</returns>
        [HttpGet("BuyAgain")]
        public async Task<bool> BuyAgain(string saleOrderNo)
        {
            var customerNo = HttpContext.Items[HttpContextItemKeyName.CUSTOMER_NO]?.ToString() ?? string.Empty;
            return await OrderService.BuyAgain(saleOrderNo, customerNo);
        }

        /// <summary>
        /// 取消订单
        /// 删除该订单的主表、明细、进度，并清理购物车选中项（如有关联）
        /// </summary>
        /// <param name="saleOrderNo">订单号</param>
        /// <returns>是否成功</returns>
        [HttpPost("Cancel")]
        public async Task<bool> Cancel(string saleOrderNo)
        {
            var customerNo = HttpContext.Items[HttpContextItemKeyName.CUSTOMER_NO]?.ToString() ?? string.Empty;
            return await OrderService.CancelOrder(saleOrderNo, customerNo);
        }
    }
}