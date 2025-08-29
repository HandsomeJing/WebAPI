using System;
using System.Collections.Generic;

#nullable disable

namespace DearlerPlatform.Domain
{
    public partial class ProductSaleAreaDiff : BaseEntity
    {
        public string SysNo { get; set; }
        public string ProductNo { get; set; }
        public string StockNo { get; set; }
        public string AreaNo { get; set; }
        public string FirstAreaNo { get; set; }
        public double DiffPrice { get; set; }
    }
}
