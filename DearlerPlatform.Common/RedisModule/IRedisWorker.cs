using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DearlerPlatform.Domain;
using StackExchange.Redis;

namespace DearlerPlatform.Common.RedisModule
{
    /// <summary>
    /// Redis操作接口 - 负责缓存数据的存取，提升系统性能
    /// 主要用于缓存商品信息、购物车数据等热点数据
    /// </summary>
    public interface IRedisWorker : IocTag
    {
        /// <summary>删除指定的缓存key</summary>
        void RemoveKey(string key);
    /// <summary>
    /// 通过通配符扫描并返回匹配的所有键名
    /// 例如：cart:*:{customerNo}
    /// </summary>
    List<string> GetKeys(string key);
        /// <summary>获取字符串类型的缓存值</summary>
        string GetString(string key);
        /// <summary>异步获取字符串类型的缓存值</summary>
        Task<string> GetStringAsync(string key);
        /// <summary>设置字符串类型的缓存，带过期时间</summary>
        void SetString(string key, string value, TimeSpan ts);
        /// <summary>异步设置字符串类型的缓存，带过期时间</summary>
        Task SetStringAsync(string key, string value, TimeSpan ts);
        /// <summary>将对象存储为Hash结构到Redis</summary>
        void SetHashMemory<T>(string key, T entity, Type? type = null);
        /// <summary>批量将对象集合存储为Hash结构</summary>
        void SetHashMemory<T>(string key, IEnumerable<T> entities, Func<T, IEnumerable<string>> func);
        /// <summary>从Redis获取Hash结构的对象列表</summary>
        List<T> GetHashMemory<T>(string keyLike) where T : new();
    }
}