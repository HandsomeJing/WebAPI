using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace DearlerPlatform.Common.RedisModule
{
    /// <summary>
    /// 核心类，连接redis并获取redis数据库
    /// </summary>
    public class RedisCore
    {
    public ConnectionMultiplexer? Conn { get; set; }
    public IDatabase? Db { get; set; }
    public bool IsAvailable { get; private set; } = false;
        
        /// <summary>
        /// 初始化Redis连接 - 建立与Redis服务器的连接
        /// </summary>
        public RedisCore(IConfiguration configuration)
        {
            var redisConnectionStr = configuration["Redis"];
            if (string.IsNullOrWhiteSpace(redisConnectionStr))
            {
                // 没有配置Redis，直接降级为不可用状态
                IsAvailable = false;
                return;
            }
            try
            {
                var configurationOptions = ConfigurationOptions.Parse(redisConnectionStr);
                configurationOptions.AllowAdmin = true;
                // 连接失败不抛异常，进入重试/降级模式
                configurationOptions.AbortOnConnectFail = false;
                Conn = ConnectionMultiplexer.Connect(configurationOptions);
                Db = Conn.GetDatabase();
                IsAvailable = Conn != null;
            }
            catch
            {
                // 降级：后续通过 IsAvailable 判断，避免抛 500
                IsAvailable = false;
            }
        }
        
        /// <summary>删除指定的Redis key</summary>
        public void RemoveKey(string key)
        {
            if (!IsAvailable || Db == null) return;
            Db.KeyDelete(key);
        }
    }
}