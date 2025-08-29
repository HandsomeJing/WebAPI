using System;
using System.Threading.Tasks;
using AutoMapper;
using DearlerPlatform.Common.EventBusHelper;
using DearlerPlatform.Core.Repository;
using DearlerPlatform.Domain;
using DearlerPlatform.Service.OrderApp.Dto;

namespace DearlerPlatform.Service.CustomerApp
{
    /// <summary>
    /// 客户服务 - 管理客户信息和相关业务
    /// 处理客户账户、密码、发票信息等操作
    /// </summary>
    public partial class CustomerService : ICustomerService
    {
        public CustomerService(
            IRepository<Customer> customerRepo,
            IRepository<CustomerInvoice> customerInvoiceRepo,
            IRepository<CustomerPwd> customerPwdRepo,
            IMapper mapper,
            LocalEventBus<SaleOrderDto> saleOrderDtoLocalEventBus
            )
        {
            CustomerRepo = customerRepo;
            CustomerInvoiceRepo = customerInvoiceRepo;
            CustomerPwdRepo = customerPwdRepo;
            Mapper = mapper;
         saleOrderDtoLocalEventBus.localEventHandler += SaleOrderDtoLocalEventHandler;
        }

       

        public IRepository<Customer> CustomerRepo { get; }
        public IRepository<CustomerInvoice> CustomerInvoiceRepo { get; }
        public IRepository<CustomerPwd> CustomerPwdRepo { get; }
        public IMapper Mapper { get; }

        public async Task<Customer?> GetCustomerAsync(string customerNo){
            return await CustomerRepo.GetAsync(m=>m.CustomerNo == customerNo);
        }
    }
}