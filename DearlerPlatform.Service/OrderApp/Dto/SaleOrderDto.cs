using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Domain;

namespace DearlerPlatform.Service.OrderApp.Dto
{
    public class SaleOrderDto
    {
        public int Id { get; set; }
        public string SaleOrderNo { get; set; } = string.Empty;
        public string CustomerNo { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime InputDate { get; set; }
        public string StockNo { get; set; } = string.Empty;
        public string EditUserNo { get; set; } = string.Empty;
        public DateTime DeliveryDate { get; set; }
        public string Remark { get; set; } = string.Empty;
        public List<SaleOrderDetail> OrderDetails { get; set; } = new();
        public SaleOrderProgress OrderProgress { get; set; } = new();
        public CustomerInvoice CustomerInvoice { get; set; } = new();
    }
}