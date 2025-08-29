using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.CustomerApp.Dto;
using DearlerPlatform.Service.OrderApp.Dto;
using DearlerPlatform.Service.ProductApp.Dto;
using DearlerPlatform.Service.ShoppingCartApp.Dto;
using Newtonsoft.Json;

namespace DearlerPlatform.Service
{
    /// <summary>
    /// AutoMapper配置文件 - 定义实体类与DTO之间的映射关系
    /// 用于简化对象之间的属性复制和转换
    /// </summary>
    public class DearlerPlatformProfile : Profile
    {
        
        public DearlerPlatformProfile()
        {
            // 产品相关映射 - 支持双向转换
            CreateMap<Product,ProductDto>().ReverseMap();
            CreateMap<ProductSale,ProductDto>().ReverseMap();
            CreateMap<ProductPhoto,ProductDto>().ReverseMap();
            CreateMap<ProductSaleAreaDiff,ProductDto>().ReverseMap();
            
            // 购物车映射 - 支持输入输出DTO
            CreateMap<ShoppingCart,ShoppingCartInputDto>().ReverseMap();
            CreateMap<ShoppingCart,ShoppingCartDto>().ReverseMap();
            
            // 订单相关映射
            CreateMap<CustomerInvoice,InvoiceOfOrderConfirmDto>().ReverseMap();
            CreateMap<SaleOrderMaster,SaleOrderDto>().ReverseMap();
            
            // 复杂对象映射 - 包含JSON序列化处理
            CreateMap<ProductDto,ProductCto>()
            .ForMember(cto=>cto.ProductPhoto,m=>m.MapFrom(dto=>JsonConvert.SerializeObject(dto.ProductPhoto)))
            .ForMember(cto=>cto.ProductSale,m=>m.MapFrom(dto=>JsonConvert.SerializeObject(dto.ProductSale)));

            CreateMap<ProductCto,ProductDto>()
            .ForMember(dto=>dto.ProductPhoto,m=>m.MapFrom(cto=>JsonConvert.DeserializeObject<ProductPhoto>(cto.ProductPhoto)))
            .ForMember(dto=>dto.ProductSale,m=>m.MapFrom(cto=>JsonConvert.DeserializeObject<ProductSale>(cto.ProductSale)));
        }
    }
}
