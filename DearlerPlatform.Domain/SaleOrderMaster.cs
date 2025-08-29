using System;
using System.Collections.Generic;

#nullable disable

namespace DearlerPlatform.Domain
{
    public partial class SaleOrderMaster : BaseEntity
    {
        public string SaleOrderNo { get; set; }
        public string CustomerNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InputDate { get; set; }
        public string StockNo { get; set; }
        public string EditUserNo { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Remark { get; set; }
    }
}
