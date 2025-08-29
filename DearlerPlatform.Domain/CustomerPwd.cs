using System;
using System.Collections.Generic;

#nullable disable

namespace DearlerPlatform.Domain
{
    public partial class CustomerPwd : BaseEntity
    {
        public string CustomerNo { get; set; }
        public string CustomerPwd1 { get; set; }
    }
}
