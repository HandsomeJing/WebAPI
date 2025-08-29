using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DearlerPlatform.Core.Repository;
using DearlerPlatform.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DearlerPlatform.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 仓储服务注册 - 自动注册Repository类型的服务
        /// 使用反射机制动态加载并注册泛型仓储接口
        /// </summary>
        public static IServiceCollection RepositoryRegister(this IServiceCollection services)
        {
            var asmCore = Assembly.Load("DearlerPlatform.Core");
            var implementationType = asmCore.GetTypes().FirstOrDefault(m =>
            m.Name == "Repository`1"
             );
            
            if (implementationType != null)
            {
                // 获取泛型接口定义，确保类型安全
                var interfaceType = implementationType.GetInterface("IRepository`1")?.GetGenericTypeDefinition();
                if (interfaceType != null)
                {
                    services.AddTransient(interfaceType, implementationType);
                }
            }
            return services;
        }
        /// <summary>
        /// 服务注册 - 自动注册带有IocTag标记的服务类
        /// 通过配置文件读取程序集列表，动态注册服务及其接口
        /// </summary>
        public static IServiceCollection ServicesRegister(this IServiceCollection services)
        {
            List<Assembly> assemblys = new();

            var provider  = services.BuildServiceProvider();
            var configuration = provider.GetService<IConfiguration>();
            
            // 确保配置不为空并且包含IocClasses设置
            var iocClassesConfig = configuration?["IocClasses"];
            if (string.IsNullOrEmpty(iocClassesConfig))
            {
                return services; // 如果配置为空，直接返回
            }
            
            List<string> classes = iocClassesConfig.Split(",").ToList();

            classes.ForEach(c=>{
                assemblys.Add(Assembly.Load(c));
            });
            
            foreach (var assembly in assemblys)
            {
                var implementationTypes = assembly.GetTypes().Where(
                    m => m.IsAssignableTo(typeof(IocTag)) &&
                    !m.IsAbstract &&
                    !m.IsInterface
                );
                foreach (var implementationType in implementationTypes)
                {
                    var interfaceType = implementationType.GetInterfaces().Where(m => m != typeof(IocTag)).FirstOrDefault();
                    // 确保接口类型不为空
                    if (interfaceType != null)
                    {
                        services.AddTransient(interfaceType, implementationType);
                    }
                }
            }
            return services;
        }
    }
}