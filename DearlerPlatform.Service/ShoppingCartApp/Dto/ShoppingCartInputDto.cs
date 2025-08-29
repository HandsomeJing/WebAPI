using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearlerPlatform.Service.ShoppingCartApp.Dto
{
    public class ShoppingCartInputDto
    {
        public string CustomerNo { get; set; } = string.Empty;
        public string ProductNo { get; set; } = string.Empty;
        public int ProductNum { get; set; }
    }
}