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
        Task<ShoppingCart> SetShoppingCart(ShoppingCartInputDto input);
        Task<List<ShoppingCartDto>> GetShoppingCartDtos(string customerNo);
        Task<string> UpdateCartSelected(ShoppingCartSelectedEditDto edit, string customerNo);
        Task<int> GetShoppingCartNum(string customerNo);
    }
}