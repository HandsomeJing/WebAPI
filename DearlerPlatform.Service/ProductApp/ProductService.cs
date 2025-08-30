using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using DearlerPlatform.Common.EventBusHelper;
using DearlerPlatform.Common.RedisModule;
using DearlerPlatform.Core;
using DearlerPlatform.Core.Consts;
using DearlerPlatform.Core.Repository;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.ProductApp.Dto;
using DearlerPlatform.Service.ShoppingCartApp.Dto;
using Microsoft.EntityFrameworkCore;
using JetBrains.Annotations;

namespace DearlerPlatform.Service.ProductApp
{
    /// <summary>
    /// 商品服务（聚合商品基本信息、图片、销售配置）
    /// 说明：
    /// - 被购物车、订单确认等场景调用，用于把“商品详情”补齐给前端/DTO。
    /// - 内置了一层 Redis 缓存读取，尽量减少数据库压力。
    /// </summary>
    public partial class ProductService : IProductService
    {
        public ProductService(
            IRepository<Product> productRepo,
            IRepository<ProductSale> ProductSale,
            IRepository<ProductSaleAreaDiff> ProductSaleAreaDiff,
            IRepository<ProductPhoto> ProductPhoto,
            IMapper mapper,
            DearlerPlatformDbContext context,
            LocalEventBus<List<ShoppingCartDto>> localEventBusShoppingCartDto,
            IRedisWorker redisWorker
            )
        {
            ProductRepo = productRepo;
            this.ProductSaleRepo = ProductSale;
            this.ProductSaleAreaDiffRepo = ProductSaleAreaDiff;
            this.ProductPhotoRepo = ProductPhoto;
            Mapper = mapper;
            Context = context;
            RedisWorker = redisWorker;
            localEventBusShoppingCartDto.localEventHandler += LocalEventHandler;
        }

        public IRepository<Product> ProductRepo { get; }
        public IRepository<ProductSale> ProductSaleRepo { get; }
        public IRepository<ProductSaleAreaDiff> ProductSaleAreaDiffRepo { get; }
        public IRepository<ProductPhoto> ProductPhotoRepo { get; }
        public IMapper Mapper { get; }
        public DearlerPlatformDbContext Context { get; }
        public IRedisWorker RedisWorker { get; }

    /// <summary>
    /// 购物车商品“补全”事件处理：
    /// - 监听本地事件总线，把购物车里的商品号批量查询并回填 ProductDto。
    /// - 优先从缓存读取，不足的再落库补齐（含图片与售价）。
    /// </summary>
        public async Task LocalEventHandler(List<ShoppingCartDto> dtos)
        {
            var nos = dtos.Select(d => d.ProductNo);
            // var productDtos = await GetProductByProductNos(nos.ToArray());
            var productDtos = await GetProductByProductNosInCache(nos.ToArray());
            dtos.ForEach(dto =>
            {
                var prodyctDto = productDtos.FirstOrDefault(m => m.ProductNo == dto.ProductNo);
                dto.ProductDto = prodyctDto ?? new();
            });
        }

    /// <summary>
    /// 综合查询商品（带筛选/分页/排序），并补充图片与销售配置。
    /// 用在商品列表页检索。
    /// </summary>
        public async Task<IEnumerable<ProductDto>> GetProductDto(
            string searchText,
            string productType,
            string belongTypeName,
            Dictionary<string, string> productProps,
            PageWithSortDto pageWithSortDto)
        {
            // int skipNum = (pageIndex - 1) * PageSize;

            pageWithSortDto.Sort ??= "ProductName";

            #region linq
            /* 
             linq语句查询数据并排序
              [from item in list]： 从list集合中获取每一条数据
              [ orderby p.GetType().GetProperty(sort).GetValue(p)]：排序，从item中通过反射获取列的值进行排序判断
              [ select item]：返回需要的数据，可以是item本身，也可以是另外重组值
            
            /*
            var products = (from p in (await ProductRepo.GetListAsync())
                            orderby p.GetType().GetProperty(sort).GetValue(p) descending
                            select p).Skip(skipNum).Take(PageSize).ToList();
            */
            #endregion

            var bzgg = productProps.ContainsKey("ProductBZGG") ? productProps["ProductBZGG"] : null;
            productProps.TryGetValue("ProductCd", out string? cd);
            productProps.TryGetValue("ProductPp", out string? pp);
            productProps.TryGetValue("ProductXh", out string? xh);
            productProps.TryGetValue("ProductCz", out string? cz);
            productProps.TryGetValue("ProductHb", out string? hb);
            productProps.TryGetValue("ProductHd", out string? hd);
            productProps.TryGetValue("ProductGy", out string? gy);
            productProps.TryGetValue("ProductHs", out string? hs);
            productProps.TryGetValue("ProductMc", out string? mc);
            productProps.TryGetValue("ProductDj", out string? dj);
            productProps.TryGetValue("ProductGg", out string? gg);
            productProps.TryGetValue("ProductYs", out string? ys);

            int skip = (pageWithSortDto.PageIndex - 1) * pageWithSortDto.PageSize;
            var query = ProductRepo.GetQueryable().Where(m =>
                m.BelongTypeName.ToLower() == belongTypeName.ToLower() &&
                (m.TypeNo == productType || string.IsNullOrWhiteSpace(productType)) &&
                (m.ProductName.Contains(searchText) || string.IsNullOrWhiteSpace(searchText)) &&
                (bzgg == null || m.ProductBzgg == bzgg)
                    && (cd == null || m.ProductCd == cd)
                    && (pp == null || m.ProductPp == pp)
                    && (xh == null || m.ProductXh == xh)
                    && (cz == null || m.ProductCz == cz)
                    && (hb == null || m.ProductHb == hb)
                    && (hd == null || m.ProductHd == hd)
                    && (gy == null || m.ProductGy == gy)
                    && (hs == null || m.ProductHs == hs)
                    && (mc == null || m.ProductMc == mc)
                    && (dj == null || m.ProductDj == dj)
                    && (gg == null || m.ProductGg == gg)
                    && (ys == null || m.ProductYs == ys)
            );
            
            var products = pageWithSortDto.Sort switch
            {
                "ProductName" => query.OrderBy(m => m.ProductName).Skip(skip).Take(pageWithSortDto.PageSize),
                "ProductNo" => query.OrderBy(m => m.ProductNo).Skip(skip).Take(pageWithSortDto.PageSize),
                "TypeName" => query.OrderBy(m => m.TypeName).Skip(skip).Take(pageWithSortDto.PageSize),
                _ => query.OrderBy(m => m.ProductName).Skip(skip).Take(pageWithSortDto.PageSize)
            };
            // 领域模型 转 视图模型
            var dtos = Mapper.Map<List<ProductDto>>(products);
            var productPhotos = await GetProductPhotosByProductNo(products.Select(m => m.ProductNo).ToArray());
            var productSales = await GetProductSalesByProductNo(products.Select(m => m.ProductNo).ToArray());
            dtos.ForEach(p =>
            {
                p.ProductPhoto = productPhotos.FirstOrDefault(m => m.ProductNo == p.ProductNo) ?? new();
                p.ProductSale = productSales.FirstOrDefault(m => m.ProductNo == p.ProductNo) ?? new();
                // var productSale = productSales.FirstOrDefault(m=>m.ProductNo == p.ProductNo);
            });
            return dtos;
        }

    /// <summary>
    /// 获取一级归属分类（去重）。
    /// 前端左侧分类筛选用。
    /// </summary>
        public async Task<List<BlongTypeDto>> GetBlongTypeDtosAsync()
        {
            return await Task.Run(() =>
            {
                var res = ProductRepo.GetQueryable().Select(m => new BlongTypeDto
                {
                    SysNo = m.SysNo,
                    BelongTypeName = m.BelongTypeName,
                }).Distinct().ToList();
                return res;
            });


        }

    /// <summary>
    /// 查询指定归属分类下的产品类型（TypeName/TypeNo 去重）。
    /// </summary>
        public async Task<IEnumerable<ProductTypeDto>> GetProductType(string belongTypeName)
        {
            return await Task.Run(() =>
            {
                var ProductType = ProductRepo.GetQueryable().Where(m => m.BelongTypeName == belongTypeName && !string.IsNullOrWhiteSpace(m.TypeNo) && !string.IsNullOrWhiteSpace(m.TypeName)).Select(m => new ProductTypeDto
                {
                    TypeName = m.TypeName,
                    TypeNo = m.TypeNo
                }).Distinct().ToList();

                return ProductType;
            });
        }

    /// <summary>
    /// 获取一组“可筛选属性”的候选值字典。
    /// Key 形如：字段|中文名，Value 为去重后的可选值列表。
    /// </summary>
        public async Task<Dictionary<string, IEnumerable<string>>> GetProductProps(string belongTypeName, string? typeNo)
        {
            Dictionary<string, IEnumerable<string>> dicProductType = new();
            // var products = Context.Products.Select(m=>new {
            //     PriductBzgg=m.ProductBzgg,
            //     ProductCd=m.ProductCd,})
            var products = await ProductRepo.GetListAsync(m => m.BelongTypeName == belongTypeName && (m.TypeNo == typeNo || string.IsNullOrWhiteSpace(typeNo)));
            dicProductType.Add("ProductBzgg|包装规格", products.Select(m => m.ProductBzgg).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)).ToList());
            dicProductType.Add("ProductCd|产地", products.Select(m => m.ProductCd).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)).ToList());
            dicProductType.Add("ProductCz|材质", products.Select(m => m.ProductCz).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)).ToList());
            dicProductType.Add("ProductDj|等级", products.Select(m => m.ProductDj).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)).ToList());
            dicProductType.Add("ProductGg|规格", products.Select(m => m.ProductGg).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)).ToList());
            dicProductType.Add("ProductGy|工艺", products.Select(m => m.ProductGy).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)).ToList());
            dicProductType.Add("ProductHb|环保", products.Select(m => m.ProductHb).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)).ToList());
            dicProductType.Add("ProductHd|厚度", products.Select(m => m.ProductHd).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)).ToList());
            dicProductType.Add("ProductHs|花色", products.Select(m => m.ProductHs).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)).ToList());
            dicProductType.Add("ProductMc|面材", products.Select(m => m.ProductMc).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)).ToList());
            dicProductType.Add("ProductPp|品牌", products.Select(m => m.ProductPp).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)).ToList());
            dicProductType.Add("ProductXh|型号", products.Select(m => m.ProductXh).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)).ToList());
            dicProductType.Add("ProductYs|颜色", products.Select(m => m.ProductYs).Distinct().Where(m => !string.IsNullOrWhiteSpace(m)).ToList());
            return dicProductType;
        }
        private static object lockObj = new object();
    /// <summary>
    /// 批量获取商品（带缓存）：
    /// - 先从 Redis 命中单品缓存，未命中则读库并回写缓存。
    /// - 同步补齐图片字段（避免老缓存缺图片导致前端空白）。
    /// </summary>
        public Task<List<ProductDto>> GetProductByProductNosInCache(params string[] postProductNos)
        {
            List<ProductCto> ctos = new();
            foreach (var productNo in postProductNos)
            {
                var res = RedisWorker.GetHashMemory<ProductCto>($"{RedisKeyName.PRODUCT_KEY}:{productNo}").FirstOrDefault();
                if (res == null)
                {
                    lock (lockObj)
                    {
                        res = RedisWorker.GetHashMemory<ProductCto>($"{RedisKeyName.PRODUCT_KEY}:{productNo}").FirstOrDefault();
                        if (res == null)
                        {
                            var productDto = GetProductByProductNos(productNo).Result.FirstOrDefault();
                            if (productDto != null)
                            {
                                res = Mapper.Map<ProductDto, ProductCto>(productDto);
                                RedisWorker.SetHashMemory<ProductCto>($"{RedisKeyName.PRODUCT_KEY}:{productNo}", res);
                            }
                        }
                    }

                }
                if (res != null)
                {
                    ctos.Add(res);
                }
            }
            // 映射回前端 DTO
            var dtos = Mapper.Map<List<ProductCto>, List<ProductDto>>(ctos);
            // 补齐缺失图片：当缓存老数据没有图片时，从库中加载并回写缓存
            var needPhotoNos = dtos.Where(d => string.IsNullOrWhiteSpace(d.ProductPhoto?.ProductPhotoUrl))
                                   .Select(d => d.ProductNo)
                                   .Distinct()
                                   .ToArray();
            if (needPhotoNos.Length > 0)
            {
                var photos = GetProductPhotosByProductNo(needPhotoNos).Result;
                foreach (var dto in dtos)
                {
                    if (string.IsNullOrWhiteSpace(dto.ProductPhoto?.ProductPhotoUrl))
                    {
                        var photo = photos.FirstOrDefault(p => p.ProductNo == dto.ProductNo);
                        if (photo != null)
                        {
                            dto.ProductPhoto = photo;
                            // 回写缓存，保持一致
                            var cto = Mapper.Map<ProductDto, ProductCto>(dto);
                            RedisWorker.SetHashMemory<ProductCto>($"{RedisKeyName.PRODUCT_KEY}:{dto.ProductNo}", cto);
                        }
                    }
                }
            }
            return Task.FromResult(dtos);
        }
    /// <summary>
    /// 直接从数据库按商品号批量获取，并补齐销售配置与图片（无缓存）。
    /// 适合后台批处理或缓存初始化。
    /// </summary>
        public async Task<List<ProductDto>> GetProductByProductNos(params string[] postProductNos)
        {
            var productNos = postProductNos.Distinct();
            var products = await ProductRepo.GetListAsync(m => productNos.Contains(m.ProductNo));
            var productDtos = Mapper.Map<List<Product>, List<ProductDto>>(products);
            var ProductSales = await GetProductSalesByProductNo(productDtos.Select(m => m.ProductNo).ToArray());
            var productPhotos = await GetProductPhotosByProductNo(productDtos.Select(m => m.ProductNo).ToArray());
            productDtos.ForEach(p =>
            {
                p.ProductSale = ProductSales.FirstOrDefault(m => m.ProductNo == p.ProductNo) ?? new();
                p.ProductPhoto = productPhotos.FirstOrDefault(m => m.ProductNo == p.ProductNo) ?? new();
            });
            return productDtos;
        }

    }
}