using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Domain;

namespace DearlerPlatform.Service.ProductApp
{
    /// <summary>
    /// 商品销售配置相关能力：按商品号批量查询销售配置（价格/仓库等）。
    /// </summary>
    public partial class ProductService
    {
        /// <summary>
        /// 根据商品号批量获取销售配置（用于价格展示与仓库回填）。
        /// </summary>
        public async Task<List<ProductSale>> GetProductSalesByProductNo(params string[] productNos)
        {
            return await ProductSaleRepo.GetListAsync(m => productNos.Contains(m.ProductNo));
        }
    }
}