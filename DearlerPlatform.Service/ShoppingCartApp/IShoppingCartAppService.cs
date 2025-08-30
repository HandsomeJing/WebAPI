using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.ShoppingCartApp.Dto;

namespace DearlerPlatform.Service.ShoppingCartApp
{
    /// <summary>
    /// 购物车服务接口 - 定义购物车相关操作
    /// 管理商品添加、删除、选择状态、数量统计等功能
    /// </summary>
    public interface IShoppingCartAppService : IocTag
    {
    /// <summary>
    /// 加入购物车（存在则数量+1，不存在则新建并默认勾选）。
    /// </summary>
        Task<ShoppingCart> SetShoppingCart(ShoppingCartInputDto input);
    /// <summary>
    /// 获取指定用户的购物车列表（聚合 Redis 并补齐商品详情）。
    /// </summary>
        Task<List<ShoppingCartDto>> GetShoppingCartDtos(string customerNo);
    /// <summary>
    /// 更新购物车某条目的勾选与数量（数量小于等于 0 视为删除）。
    /// </summary>
        Task<string> UpdateCartSelected(ShoppingCartSelectedEditDto edit, string customerNo);
    /// <summary>
    /// 获取购物车总件数（已选/未选都会计入）。
    /// </summary>
        Task<int> GetShoppingCartNum(string customerNo);
    }
}