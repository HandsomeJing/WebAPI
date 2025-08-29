using System.Collections.Generic;
using System.Threading.Tasks;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.CustomerApp.Dto;
using DearlerPlatform.Service.OrderApp.Dto;

namespace DearlerPlatform.Service.CustomerApp
{
    /// <summary>
    /// 客户服务接口 - 定义客户相关业务操作
    /// 包括客户查询、密码验证、发票管理等功能
    /// </summary>
    public interface ICustomerService : IocTag
    {
        Task<Customer?> GetCustomerAsync(string customerNo);
        Task<bool> CheckPassword(CustomerLoginDto dto);
        Task<List<InvoiceOfOrderConfirmDto>> GetInvoicesByUser(string customerNo);
    }
}