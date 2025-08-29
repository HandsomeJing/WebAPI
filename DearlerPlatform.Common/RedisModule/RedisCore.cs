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
        public ConnectionMultiplexer Conn { get; set; }
        public IDatabase Db { get; set; }
        
        /// <summary>
        /// 初始化Redis连接 - 建立与Redis服务器的连接
        /// </summary>
        public RedisCore(IConfiguration configuration)
        {
            var redisConnectionStr = configuration["Redis"] ?? throw new ArgumentNullException("Redis连接字符串不能为空");
            ConfigurationOptions configurationOptions = ConfigurationOptions.Parse(redisConnectionStr);
            // 允许管理员权限，这样才能进行模糊搜索Key等高级操作
            configurationOptions.AllowAdmin = true;
            Conn = ConnectionMultiplexer.Connect(configurationOptions);
            Db = Conn.GetDatabase();
        }
        
        /// <summary>删除指定的Redis key</summary>
        public void RemoveKey(string key)
        {
            Db.KeyDelete(key);
        }
    }
}