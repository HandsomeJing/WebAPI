using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearlerPlatform.Common.RedisModule
{
    public partial class RedisWorker
    {

        public void SetString(string key, string value, TimeSpan ts)
        {
            if (!RedisCore.IsAvailable || RedisCore.Db == null) return;
            RedisCore.Db.StringSet(key, value, ts);
        }
        /// <summary>异步设置字符串缓存，带过期时间</summary>
        public async Task SetStringAsync(string key, string value, TimeSpan ts)
        {
            if (!RedisCore.IsAvailable || RedisCore.Db == null) return;
            await RedisCore.Db.StringSetAsync(key, value, ts);
        }
        
        /// <summary>获取字符串缓存值</summary>
        public string GetString(string key)
        {
            if (!RedisCore.IsAvailable || RedisCore.Db == null) return string.Empty;
            var result = RedisCore.Db.StringGet(key);
            return result.HasValue ? result.ToString() : string.Empty;
        }
        
        /// <summary>异步获取字符串缓存值</summary>
        public async Task<string> GetStringAsync(string key)
        {
            if (!RedisCore.IsAvailable || RedisCore.Db == null) return string.Empty;
            var result = await RedisCore.Db.StringGetAsync(key);
            return result.HasValue ? result.ToString() : string.Empty;
        }
    }
}