using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Core;
using DearlerPlatform.Service.ProductApp;
using DearlerPlatform.Service.ProductApp.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DearlerPlatfrom.Api.Controllers;

/// <summary>
/// 商品控制器 - 处理商品相关的所有操作
/// 包括商品查询、分类管理、商品详情等功能
/// 需要JWT身份验证才能访问
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProductController : BaseController
{
    public ProductController(IProductService productService)
    {
        ProductService = productService;
    }

    public IProductService ProductService { get; }
    
    /// <summary>
    /// 获取商品列表 - 支持多条件搜索和分页
    /// 这是商城的核心查询接口，支持按名称、类型、属性等多维度筛选
    /// </summary>
    /// <param name="searchText">搜索关键词 - 按商品名称模糊搜索</param>
    /// <param name="productType">商品类型编号 - 精确匹配</param>
    /// <param name="belongTypeName">所属分类名称 - 大分类筛选</param>
    /// <param name="productProps">商品属性 - 格式: 属性名_属性值^属性名_属性值</param>
    /// <param name="sort">排序字段 - 如ProductName, ProductNo等</param>
    /// <param name="pageIndex">页码 - 从1开始</param>
    /// <param name="pageSize">每页数量 - 默认30条</param>
    /// <param name="orderType">排序方式 - 升序或降序</param>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IEnumerable<ProductDto>> GetProductDtosAsync(
        string? searchText,
        string? productType,
        string belongTypeName,
        string? productProps,
        string? sort,
        int pageIndex = 1,
        int pageSize = 30,
        OrderType orderType = OrderType.Asc)
    {
        // 解析商品属性参数 - 将字符串转换为字典格式方便查询
        Dictionary<string, string> dicProductProps = new();
        var arrProductProps = productProps?.Split("^") ?? new string[0];
        foreach (var item in arrProductProps)
        {
            var key = item.Split("_")[0];
            var value = item.Split("_")[1];
            dicProductProps.Add(key, value);
        }

        return await ProductService.GetProductDto(searchText ?? "", productType ?? "", belongTypeName,dicProductProps, new PageWithSortDto()
        {
            Sort = sort ?? "Id",
            PageIndex = pageIndex,
            PageSize = pageSize,
            OrderType = orderType
        });
    }

    [HttpGet("BlongType")]
    public async Task<List<BlongTypeDto>> GetBlongType()
    {
    return await ProductService.GetBlongTypeDtosAsync();
    }
    [HttpGet("type")]
    public async Task<IEnumerable<ProductTypeDto>> GetProductTypeDtosAsync(string belongTypeName)
    {
    return await ProductService.GetProductType(belongTypeName);
    }
    [HttpGet("props")]
    public async Task<Dictionary<string, IEnumerable<string>>> GetProductProps(string belongTypeName, string? typeNo)
    {
    return await ProductService.GetProductProps(belongTypeName, typeNo);
    }


}
