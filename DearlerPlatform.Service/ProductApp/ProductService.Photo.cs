using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Domain;

namespace DearlerPlatform.Service.ProductApp;

/// <summary>
/// 商品图片相关能力：按商品号批量查询图片信息。
/// </summary>
public partial class ProductService
{
    /// <summary>
    /// 根据商品号批量获取商品图片（通常用于列表批量补齐封面）。
    /// </summary>
    public async Task<List<ProductPhoto>> GetProductPhotosByProductNo(params string[] productNos)
    {
        return await ProductPhotoRepo.GetListAsync(m => productNos.Contains(m.ProductNo));
    }
}
