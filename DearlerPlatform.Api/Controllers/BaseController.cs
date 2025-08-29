using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DearlerPlatfrom.Api.Controllers
{
    /// <summary>
    /// 控制器基类 - 所有API控制器的父类
    /// 统一配置路由和API行为
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class BaseController : ControllerBase
    {

    }
}