using System;
using System.Security.Cryptography;
using System.Text;

namespace DearlerPlatform.Common.Md5Module
{
    /// <summary>
    /// MD5加密工具类 - 用于密码加密等安全操作
    /// 注意：MD5已经不够安全，生产环境建议用更强的算法
    /// </summary>
    public static class Md5Helper
    {
        /// <summary>
        /// 将字符串转换为MD5哈希值
        /// 会自动加上"@HolyAce"作为盐值增加安全性
        /// </summary>
        public static string ToMd5(this string str)
        {
            using MD5 md5 = MD5.Create(); // 使用推荐的创建方式
            byte[] bytes = md5.ComputeHash(Encoding.Default.GetBytes(str + "@HolyAce"));
            var md5Str = BitConverter.ToString(bytes).Replace("-", "");
            return md5Str;
        }
    }
}