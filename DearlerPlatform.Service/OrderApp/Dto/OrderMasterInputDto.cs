using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearlerPlatform.Service.OrderApp.Dto
{
    public class OrderMasterInputDto
    {
        public DateTime DeliveryDate { get; set; }
        public string invoice { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
    }
}