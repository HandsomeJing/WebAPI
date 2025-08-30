using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using DearlerPlatform.Common.EventBusHelper;
using DearlerPlatform.Common.RedisModule;
using DearlerPlatform.Core.Repository;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.OrderApp.Dto;

namespace DearlerPlatform.Service.OrderApp
{
    /// <summary>
    /// 订单服务 - 处理订单相关的业务逻辑
    /// 包括订单创建、查询、状态更新等功能
    /// </summary>
    public partial class OrderService : IOrderService
    {
        public OrderService(
            IRepository<SaleOrderMaster> saleOrderMasterRepo,
            IRepository<SaleOrderDetail> saleOrderDetailRepo,
            IRepository<SaleOrderProgress> saleOrderProgressrRepo,
            IRepository<ProductSale> productSaleRepo,
            IRepository<Stock> stockRepo,
            IRedisWorker redisWorker,
            IMapper mapper,
            LocalEventBus<SaleOrderDto> saleOrderDtoLocalEventBus
        )
        {
            OrderMasterRepo = saleOrderMasterRepo;
            OrderDetailRepo = saleOrderDetailRepo;
            OrderProgressrRepo = saleOrderProgressrRepo;
            ProductSaleRepo = productSaleRepo;
            StockRepo = stockRepo;
            RedisWorker = redisWorker;
            Mapper = mapper;
            this.SaleOrderDtoLocalEventBus = saleOrderDtoLocalEventBus;
        }

        public IRepository<SaleOrderMaster> OrderMasterRepo { get; }
        public IRepository<SaleOrderDetail> OrderDetailRepo { get; }
    public IRepository<SaleOrderProgress> OrderProgressrRepo { get; }
    public IRepository<ProductSale> ProductSaleRepo { get; }
    public IRepository<Stock> StockRepo { get; }
        public IRedisWorker RedisWorker { get; }
        public IMapper Mapper { get; }
        public LocalEventBus<SaleOrderDto> SaleOrderDtoLocalEventBus { get; }


        /// <summary>
        /// 获得订单详情
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
    /// <summary>
    /// 获得订单详情（含汇总与仓库显示字段）
    /// </summary>
    /// <param name="orderNo">订单号</param>
    public async Task<SaleOrderDto> GetOrderInfoByOrderNo(string orderNo)
        {
            // 获取住订单
            var orderMaster = (await OrderMasterRepo.GetListAsync(m => m.SaleOrderNo == orderNo)).FirstOrDefault();
            // 转成Dto
            var saleOrderDto = Mapper.Map<SaleOrderDto>(orderMaster);
            if (saleOrderDto != null)
            {
                // 获取订单流程
                saleOrderDto.OrderProgress = (await GetProgressByOrderNos(saleOrderDto.SaleOrderNo)).FirstOrDefault() ?? new();
                // 根据住订单获取订单详情
                saleOrderDto.OrderDetails = await GetOrderDetailsByOrderNo(saleOrderDto.SaleOrderNo);
                // 统计件数和金额（为前端展示提供汇总数据）
                saleOrderDto.TotalNum = saleOrderDto.OrderDetails.Sum(d => d.OrderNum);
                saleOrderDto.TotalPrice = Math.Round(saleOrderDto.OrderDetails.Sum(d => d.OrderNum * d.SalePrice), 2);
                // 若主单未记录仓库，则依据首个商品的销售配置回填仓库
                if (string.IsNullOrWhiteSpace(saleOrderDto.StockNo))
                {
                    var firstProductNo = saleOrderDto.OrderDetails.FirstOrDefault()?.ProductNo;
                    if (!string.IsNullOrWhiteSpace(firstProductNo))
                    {
                        var ps = (await ProductSaleRepo.GetListAsync(x => x.ProductNo == firstProductNo)).FirstOrDefault();
                        if (ps != null) saleOrderDto.StockNo = ps.StockNo ?? string.Empty;
                    }
                }
                // 获取仓库信息
                if (!string.IsNullOrWhiteSpace(saleOrderDto.StockNo))
                {
                    var stock = (await StockRepo.GetListAsync(s => s.StockNo == saleOrderDto.StockNo)).FirstOrDefault();
                    if (stock != null)
                    {
                        saleOrderDto.StockName = stock.StockName ?? string.Empty;
                        saleOrderDto.StockLinkman = stock.StockLinkman ?? string.Empty;
                        saleOrderDto.StockPhone = string.IsNullOrWhiteSpace(stock.StockPhone) ? stock.StockTel ?? string.Empty : stock.StockPhone;
                    }
                }
                // 发货信息固定展示
                saleOrderDto.StockName = "西安仓库";
                saleOrderDto.StockLinkman = "井水田";
                saleOrderDto.StockPhone = "17765006056";
                // 触发总线，获取开票人信息
                await SaleOrderDtoLocalEventBus.Publish(saleOrderDto);
            }
            return saleOrderDto ?? new();
        }

    /// <summary>
    /// 再次购买：将历史订单的商品回填到指定客户的购物车
    /// </summary>
    /// <param name="SaleOrderNo">历史订单号</param>
    /// <param name="customerNo">客户编号</param>
    public async Task<bool> BuyAgain(string SaleOrderNo, string customerNo)
        {
            // 将该订单的商品重新加入当前用户的购物车（Redis）
            var oldDetails = await OrderDetailRepo.GetListAsync(m => m.SaleOrderNo == SaleOrderNo);
            if (oldDetails == null || oldDetails.Count == 0) return false;
            foreach (var src in oldDetails)
            {
                var cartGuid = Guid.NewGuid().ToString();
                var cart = new DearlerPlatform.Domain.ShoppingCart
                {
                    CartGuid = cartGuid,
                    CustomerNo = customerNo,
                    ProductNo = src.ProductNo,
                    ProductNum = src.OrderNum,
                    CartSelected = true
                };
                RedisWorker.SetHashMemory($"cart:{cartGuid}:{customerNo}", cart);
            }
            return true;
        }

        /// <summary>
        /// 获取指定客户最近一笔订单号
        /// 按下单时间倒序获取第一条记录
        /// </summary>
        /// <param name="customerNo">客户编号</param>
        /// <returns>订单号，若没有则返回空字符串</returns>
    /// <summary>
    /// 获取指定客户最近一笔订单号
    /// </summary>
    public async Task<string> GetLatestOrderNo(string customerNo)
        {
            var latest = (await OrderMasterRepo.GetListAsync(m => m.CustomerNo == customerNo))
                .OrderByDescending(m => m.InputDate)
                .FirstOrDefault();
            return latest?.SaleOrderNo ?? string.Empty;
        }

    /// <summary>
    /// 获取客户最近订单列表（带件数/金额汇总）
    /// </summary>
    public async Task<IEnumerable<SaleOrderDto>> GetOrdersByCustomer(string customerNo, int take = 20)
        {
            var masters = (await OrderMasterRepo.GetListAsync(m => m.CustomerNo == customerNo))
                .OrderByDescending(m => m.InputDate)
                .Take(take)
                .ToList();
            var orderNos = masters.Select(m => m.SaleOrderNo).ToArray();
            var details = await OrderDetailRepo.GetListAsync(d => orderNos.Contains(d.SaleOrderNo));
            var list = new List<SaleOrderDto>();
            foreach (var m in masters)
            {
                var dto = Mapper.Map<SaleOrderDto>(m);
                var ds = details.Where(d => d.SaleOrderNo == m.SaleOrderNo).ToList();
                dto.OrderDetails = ds;
                dto.TotalNum = ds.Sum(x => x.OrderNum);
                dto.TotalPrice = Math.Round(ds.Sum(x => x.OrderNum * x.SalePrice), 2);
                list.Add(dto);
            }
            return list;
        }

        /// <summary>
        /// 取消订单：删除订单主/明细/进度
        /// </summary>
    /// <summary>
    /// 取消订单：删除主/明细/进度，并清空该客户购物车
    /// </summary>
    public async Task<bool> CancelOrder(string saleOrderNo, string customerNo)
        {
            using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var master = await OrderMasterRepo.GetAsync(m => m.SaleOrderNo == saleOrderNo);
                if (master == null) return false;

                var details = await OrderDetailRepo.GetListAsync(d => d.SaleOrderNo == saleOrderNo);
                var progresses = await OrderProgressrRepo.GetListAsync(p => p.SaleOrderNo == saleOrderNo);

                foreach (var d in details)
                {
                    await OrderDetailRepo.DeleteAsync(d);
                }
                foreach (var p in progresses)
                {
                    await OrderProgressrRepo.DeleteAsync(p);
                }
                await OrderMasterRepo.DeleteAsync(master);

                // 清理该用户的购物车（选中项或全部：这里按全部键位清空）
                var keys = RedisWorker.GetKeys($"cart:*:{customerNo}");
                foreach (var key in keys)
                {
                    RedisWorker.RemoveKey(key);
                }

                ts.Complete();
                return true;
            }
            catch
            {
                ts.Dispose();
                throw;
            }
        }
    }
}