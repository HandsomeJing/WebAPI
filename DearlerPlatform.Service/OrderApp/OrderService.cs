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
            IRedisWorker redisWorker,
            IMapper mapper,
            LocalEventBus<SaleOrderDto> saleOrderDtoLocalEventBus
        )
        {
            OrderMasterRepo = saleOrderMasterRepo;
            OrderDetailRepo = saleOrderDetailRepo;
            OrderProgressrRepo = saleOrderProgressrRepo;
            RedisWorker = redisWorker;
            Mapper = mapper;
            this.SaleOrderDtoLocalEventBus = saleOrderDtoLocalEventBus;
        }

        public IRepository<SaleOrderMaster> OrderMasterRepo { get; }
        public IRepository<SaleOrderDetail> OrderDetailRepo { get; }
        public IRepository<SaleOrderProgress> OrderProgressrRepo { get; }
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
                var master = await OrderMasterRepo.GetAsync(m => m.SaleOrderNo == SaleOrderNo);
                if (master == null) return false;
                
                var details = await OrderDetailRepo.GetListAsync(m => m.SaleOrderNo == SaleOrderNo);
                var newSaleOrderNo = Guid.NewGuid().ToString();
                master.SaleOrderNo = newSaleOrderNo;
                master.InputDate = newOrdeTime;
                master.DeliveryDate = newOrdeTime.AddDays(1);
                await OrderMasterRepo.InsertAsync(master);

                details.ForEach(async d =>
                {
                    d.SaleOrderNo = newSaleOrderNo;
                    d.SaleOrderGuid = Guid.NewGuid().ToString();
                    d.InputDate = newOrdeTime;
                    await OrderDetailRepo.InsertAsync(d);
                });

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
                return false;
            }
            catch (System.Exception)
            {
                ts.Dispose();
                throw;
            }


        }
    }
}