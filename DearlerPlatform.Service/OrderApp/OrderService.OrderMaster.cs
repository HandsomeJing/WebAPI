using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.OrderApp.Dto;
using DearlerPlatform.Service.ShoppingCartApp.Dto;

namespace DearlerPlatform.Service.OrderApp
{
    public partial class OrderService
    {
    /// <summary>
    /// 新增订单：写主表、流程与明细；成功后清理购物车中已下单的项。
    /// 仓库默认取首个商品的销售配置；订单号用 Guid。
    /// </summary>
    public async Task<bool> AddOrder(
            string customerNo,
            OrderMasterInputDto input,
            List<ShoppingCartDto> carts
        )
        {
            using TransactionScope ts = new(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // 添加主订单
                DateTime inputDate = DateTime.Now;
                string orderNo = Guid.NewGuid().ToString();
                // 按照业务：使用第一个商品的销售配置中的仓库作为发货仓库
                var firstStockNo = carts.FirstOrDefault()?.ProductDto?.ProductSale?.StockNo ?? string.Empty;
                SaleOrderMaster master = new SaleOrderMaster
                {
                    CustomerNo = customerNo,
                    DeliveryDate = input.DeliveryDate,
                    EditUserNo = customerNo,
                    InputDate = inputDate,
                    Remark = input.Remark,
                    InvoiceNo = input.invoice,
                    SaleOrderNo = orderNo,
                    StockNo = firstStockNo
                };
                await OrderMasterRepo.InsertAsync(master);
                // 添加流程（记录“下单”步骤）
                await AddProgress(orderNo, inputDate);
                // 添加订单详情（逐条写入）
                await AddOrderDetail(carts, customerNo, orderNo, inputDate);
                // 提交事务
                ts.Complete();
                // 删除Redis中的购物车数据（已下单的勾选项）
                foreach (var cart in carts)
                {
                    RedisWorker.RemoveKey($"cart:{cart.CartGuid}:{customerNo}");
                }
                return true;
            }
            catch (System.Exception)
            {
                ts.Dispose();
                throw;
            }

        }
    }
}