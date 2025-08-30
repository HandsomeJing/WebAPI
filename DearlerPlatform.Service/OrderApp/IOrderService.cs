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
    /// <summary>
    /// 新增订单
    /// </summary>
    /// <param name="customerNo">客户编号</param>
    /// <param name="input">下单信息（交货日期、备注、发票）</param>
    /// <param name="carts">购物车中勾选的商品</param>
    /// <summary>
    /// 新增订单（含流程与明细落库，并清理购物车）。
    /// </summary>
    Task<bool> AddOrder(
            string customerNo,
            OrderMasterInputDto input,
            List<ShoppingCartDto> carts
        );

    /// <summary>
    /// 按订单号获取订单详情（含明细、进度、统计字段）
    /// </summary>
        /// <summary>
        /// 按订单号获取订单详情（含进度、明细与汇总字段）。
        /// </summary>
        Task<SaleOrderDto> GetOrderInfoByOrderNo(string orderNo);
    /// <summary>
    /// 获取指定客户最近一次下单生成的订单号
    /// </summary>
    /// <param name="customerNo">客户编号</param>
    /// <returns>订单号(若无则返回空字符串)</returns>
    Task<string> GetLatestOrderNo(string customerNo);

    /// <summary>
    /// 再次购买：把历史订单明细重新加入该客户购物车
    /// </summary>
    /// <param name="SaleOrderNo">历史订单号</param>
    /// <param name="customerNo">客户编号</param>
    Task<bool> BuyAgain(string SaleOrderNo, string customerNo);

    /// <summary>
    /// 获取客户最近订单列表（含汇总）
    /// </summary>
    Task<IEnumerable<SaleOrderDto>> GetOrdersByCustomer(string customerNo, int take = 20);
    /// <summary>
    /// 取消订单并清理相关明细与进度
    /// </summary>
    /// <param name="saleOrderNo">订单号</param>
    /// <returns>是否成功</returns>
    /// <summary>
    /// 取消订单：删除主/明细/进度，并清空该客户购物车
    /// </summary>
    Task<bool> CancelOrder(string saleOrderNo, string customerNo);
    }
}