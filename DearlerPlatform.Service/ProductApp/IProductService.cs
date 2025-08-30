using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Core;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.ProductApp.Dto;
using JetBrains.Annotations;

namespace DearlerPlatform.Service.ProductApp
{
    public interface IProductService : IocTag
    {
    /// <summary>
    /// 按筛选条件查询商品列表（带分页/排序），并补齐图片与销售配置。
    /// </summary>
    Task<IEnumerable<ProductDto>> GetProductDto(string searchText, string productType, string belongTypeName,Dictionary<string, string> productProps, PageWithSortDto pageWithSortDto);

    /// <summary>
    /// 批量获取商品图片（通常用于列表一次性补齐封面）。
    /// </summary>
    Task<List<ProductPhoto>> GetProductPhotosByProductNo(params string[] productNos);

    /// <summary>
    /// 批量获取商品销售配置（价格/仓库等）。
    /// </summary>
    Task<List<ProductSale>> GetProductSalesByProductNo(params string[] productNos);

    /// <summary>
    /// 获取某归属分类下的产品类型（用于前端二级分类）。
    /// </summary>
    Task<IEnumerable<ProductTypeDto>> GetProductType(string belongTypeName);

    /// <summary>
    /// 获取一级归属分类列表（去重）。
    /// </summary>
    Task<List<BlongTypeDto>> GetBlongTypeDtosAsync();

    /// <summary>
    /// 获取一组可筛选属性的候选值（字典形式）。
    /// </summary>
    Task<Dictionary<string, IEnumerable<string>>> GetProductProps(string belongTypeName, string? typeNo);
    }
}