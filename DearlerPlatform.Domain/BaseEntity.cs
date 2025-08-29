namespace DearlerPlatform.Domain
{
    /// <summary>
    /// 基础实体类 - 所有数据库实体的基类
    /// 提供通用的Id主键字段，确保所有实体都有统一的标识符
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// 主键ID - 数据库自增长标识符
        /// </summary>
        public int Id { get; set; }
    }
}