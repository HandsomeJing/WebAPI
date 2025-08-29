using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearlerPlatform.Domain
{
    /// <summary>
    /// IoC容器标记接口 - 用于自动服务注册
    /// 实现此接口的类会被ServiceCollectionExtensions自动注册到DI容器中
    /// 无需手动配置依赖注入，简化服务注册流程
    /// </summary>
    public interface IocTag
    {
        
    }
}