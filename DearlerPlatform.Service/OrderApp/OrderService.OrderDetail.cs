using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.ShoppingCartApp.Dto;

namespace DearlerPlatform.Service.OrderApp;

public partial class OrderService
{
    /// <summary>
    /// 批量写入订单明细：
    /// - 价格来自 ProductSale（当前无价差，DiffPrice=0）。
    /// - 冗余 ProductName/PhotoUrl 方便订单详情直接展示。
    /// </summary>
    private async Task AddOrderDetail(List<ShoppingCartDto> carts,
    string customerNo,
    string orderNo,
    DateTime inputDate)
    {
        foreach (var cart in carts)
        {
            SaleOrderDetail detail = new()
            {
                SaleOrderGuid = Guid.NewGuid().ToString(),
                SaleOrderNo = orderNo,
                ProductNo = cart.ProductNo,
                ProductName = cart.ProductDto.ProductName,
                ProductPhotoUrl = cart.ProductDto.ProductPhoto?.ProductPhotoUrl,
                CustomerNo = customerNo,
                InputDate = inputDate,
                OrderNum = cart.ProductNum,
                BasePrice = cart.ProductDto.ProductSale?.SalePrice ?? 0,
                DiffPrice = 0,
                SalePrice = cart.ProductDto.ProductSale?.SalePrice ?? 0
            };
            await OrderDetailRepo.InsertAsync(detail);
        }
    }
    /// <summary>
    /// 按订单号获取明细列表（用于详情聚合与汇总件数/金额）。
    /// </summary>
    private async Task<List<SaleOrderDetail>> GetOrderDetailsByOrderNo(string orderNo)
    {
        return await OrderDetailRepo.GetListAsync(m=>m.SaleOrderNo == orderNo);
    }
}