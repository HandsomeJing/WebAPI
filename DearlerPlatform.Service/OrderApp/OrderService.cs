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
            IRepository<Stock> stockRepo,
            IRedisWorker redisWorker,
            IMapper mapper,
            LocalEventBus<SaleOrderDto> saleOrderDtoLocalEventBus
        )
        {
            OrderMasterRepo = saleOrderMasterRepo;
            OrderDetailRepo = saleOrderDetailRepo;
            OrderProgressrRepo = saleOrderProgressrRepo;
            StockRepo = stockRepo;
            RedisWorker = redisWorker;
            Mapper = mapper;
            this.SaleOrderDtoLocalEventBus = saleOrderDtoLocalEventBus;
        }

        public IRepository<SaleOrderMaster> OrderMasterRepo { get; }
        public IRepository<SaleOrderDetail> OrderDetailRepo { get; }
        public IRepository<SaleOrderProgress> OrderProgressrRepo { get; }
    public IRepository<Stock> StockRepo { get; }
        public IRedisWorker RedisWorker { get; }
        public IMapper Mapper { get; }
        public LocalEventBus<SaleOrderDto> SaleOrderDtoLocalEventBus { get; }


        /// <summary>
        /// 获得订单详情
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
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
                // 触发总线，获取开票人信息
                await SaleOrderDtoLocalEventBus.Publish(saleOrderDto);
            }
            return saleOrderDto ?? new();
        }

        public async Task<bool> BuyAgain(string SaleOrderNo)
        {
            using TransactionScope ts = new(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var newOrdeTime = DateTime.Now;
                var oldMaster = await OrderMasterRepo.GetAsync(m => m.SaleOrderNo == SaleOrderNo);
                if (oldMaster == null) return false;

                var oldDetails = await OrderDetailRepo.GetListAsync(m => m.SaleOrderNo == SaleOrderNo);
                var newSaleOrderNo = Guid.NewGuid().ToString();

                // 复制主订单，生成新单号与时间
                var newMaster = new SaleOrderMaster
                {
                    CustomerNo = oldMaster.CustomerNo,
                    DeliveryDate = newOrdeTime.AddDays(1),
                    EditUserNo = oldMaster.EditUserNo,
                    InputDate = newOrdeTime,
                    Remark = oldMaster.Remark,
                    InvoiceNo = oldMaster.InvoiceNo,
                    SaleOrderNo = newSaleOrderNo,
                    StockNo = oldMaster.StockNo
                };
                await OrderMasterRepo.InsertAsync(newMaster);

                // 复制明细
                foreach (var src in oldDetails)
                {
                    var d = new SaleOrderDetail
                    {
                        SaleOrderGuid = Guid.NewGuid().ToString(),
                        SaleOrderNo = newSaleOrderNo,
                        ProductNo = src.ProductNo,
                        ProductName = src.ProductName,
                        ProductPhotoUrl = src.ProductPhotoUrl,
                        CustomerNo = src.CustomerNo,
                        InputDate = newOrdeTime,
                        OrderNum = src.OrderNum,
                        BasePrice = src.BasePrice,
                        DiffPrice = src.DiffPrice,
                        SalePrice = src.SalePrice
                    };
                    await OrderDetailRepo.InsertAsync(d);
                }

                var progress = new SaleOrderProgress
                {
                    ProgressGuid = Guid.NewGuid().ToString(),
                    SaleOrderNo = newSaleOrderNo,
                    StepName = "下单",
                    StepSn = 1,
                    StepTime = newOrdeTime

                };
                await OrderProgressrRepo.InsertAsync(progress);
                ts.Complete();
                return true;
            }
            catch (System.Exception)
            {
                ts.Dispose();
                throw;
            }


        }

        /// <summary>
        /// 获取指定客户最近一笔订单号
        /// 按下单时间倒序获取第一条记录
        /// </summary>
        /// <param name="customerNo">客户编号</param>
        /// <returns>订单号，若没有则返回空字符串</returns>
        public async Task<string> GetLatestOrderNo(string customerNo)
        {
            var latest = (await OrderMasterRepo.GetListAsync(m => m.CustomerNo == customerNo))
                .OrderByDescending(m => m.InputDate)
                .FirstOrDefault();
            return latest?.SaleOrderNo ?? string.Empty;
        }
    }
}