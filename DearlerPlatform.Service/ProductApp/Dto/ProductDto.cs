using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Domain;

namespace DearlerPlatform.Service.ProductApp.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string SysNo { get; set; } = string.Empty;
        public string ProductNo { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string TypeNo { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string ProductPp { get; set; } = string.Empty;
        public string ProductXh { get; set; } = string.Empty;
        public string ProductCz { get; set; } = string.Empty;
        public string ProductHb { get; set; } = string.Empty;
        public string ProductHd { get; set; } = string.Empty;
        public string ProductGy { get; set; } = string.Empty;
        public string ProductHs { get; set; } = string.Empty;
        public string ProductMc { get; set; } = string.Empty;
        public string ProductDj { get; set; } = string.Empty;
        public string ProductCd { get; set; } = string.Empty;
        public string ProductGg { get; set; } = string.Empty;
        public string ProductYs { get; set; } = string.Empty;
        public string UnitNo { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string ProductNote { get; set; } = string.Empty;
        public string ProductBzgg { get; set; } = string.Empty;
        public string BelongTypeNo { get; set; } = string.Empty;
        public string BelongTypeName { get; set; } = string.Empty;
        public ProductPhoto ProductPhoto { get; set; } = new();
        public ProductSale ProductSale { get; set; } = new();
    }
}