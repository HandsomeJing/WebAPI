namespace DearlerPlatform.Common.TokenModule
{
    /// <summary>
    /// JWT令牌配置模型 - 存储Token生成和验证的相关参数
    /// </summary>
    public class JwtTokenModel
    {
        /// <summary>令牌发行者</summary>
        public string Issuer { get; set; } = string.Empty;
        /// <summary>令牌接收者</summary>
        public string Audience { get; set; } = string.Empty;
        /// <summary>令牌过期时间(分钟)</summary>
        public int Expires { get; set; }
        /// <summary>签名密钥</summary>
        public string Security { get; set; } = string.Empty;
        /// <summary>用户ID</summary>
        public int Id { get; set; }
        /// <summary>客户编号</summary>
        public string CustomerNo { get; set; } = string.Empty;
        /// <summary>客户名称</summary>
        public string CustomerName { get; set; } = string.Empty;
    }
}