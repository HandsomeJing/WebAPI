using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearlerPlatform.Service.CustomerApp.Dto
{
    public class InvoiceOfOrderConfirmDto
    {
        public int Id { get; set; } 
        public string CustomerNo { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
    }
}