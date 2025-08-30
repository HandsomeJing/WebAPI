using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DearlerPlatform.Core.Repository;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.CustomerApp.Dto;
using DearlerPlatform.Service.OrderApp.Dto;

namespace DearlerPlatform.Service.CustomerApp
{
    public partial class CustomerService
    {
    /// <summary>
    /// 订单详情组装时的事件处理：
    /// - 根据订单上的发票号补齐发票抬头信息（用于前端展示“开票人”）。
    /// </summary>
        private async Task SaleOrderDtoLocalEventHandler(SaleOrderDto saleOrderDto)
        {
            saleOrderDto.CustomerInvoice = await GetCustomerInvoiceAsync(saleOrderDto.InvoiceNo) ?? new();
        }
    /// <summary>
    /// 获取客户所有发票抬头（用于订单确认页下拉）。
    /// </summary>
        public async Task<List<InvoiceOfOrderConfirmDto>> GetInvoicesByUser(string customerNo)
        {
            var invoices = await CustomerInvoiceRepo.GetListAsync(m => m.CustomerNo == customerNo);
            return Mapper.Map<List<CustomerInvoice>, List<InvoiceOfOrderConfirmDto>>(invoices);
        }

    /// <summary>
    /// 按发票号获取发票抬头实体。
    /// </summary>
        private async Task<CustomerInvoice?> GetCustomerInvoiceAsync(string invoiceNo)
        {
            return await CustomerInvoiceRepo.GetAsync(m => m.InvoiceNo == invoiceNo);
        }
    }
}