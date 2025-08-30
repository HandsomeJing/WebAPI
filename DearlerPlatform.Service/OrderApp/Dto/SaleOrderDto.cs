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

    // 以下为便于前端展示的汇总与仓库显示字段
    // 件数合计（所有明细的数量之和）
    public int TotalNum { get; set; }
    // 金额合计（所有明细：数量*销售价 之和）
    public double TotalPrice { get; set; }
    // 发货仓库名称
    public string StockName { get; set; } = string.Empty;
    // 发货联系人
    public string StockLinkman { get; set; } = string.Empty;
    // 发货联系手机
    public string StockPhone { get; set; } = string.Empty;
    }
}