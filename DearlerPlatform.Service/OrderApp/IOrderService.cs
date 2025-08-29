using System.Collections.Generic;
using System.Threading.Tasks;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.OrderApp.Dto;
using DearlerPlatform.Service.ShoppingCartApp.Dto;

namespace DearlerPlatform.Service.OrderApp
{
    /// <summary>
    /// 订单服务接口 - 定义订单处理相关操作
    /// 处理订单创建、查询、进度跟踪等核心业务逻辑
    /// </summary>
    public interface IOrderService : IocTag
    {
        Task<bool> AddOrder(
            string customerNo,
            OrderMasterInputDto input,
            List<ShoppingCartDto> carts
        );

        Task<SaleOrderDto> GetOrderInfoByOrderNo(string orderNo);
        Task<bool> BuyAgain(string SaleOrderNo);
    }
}