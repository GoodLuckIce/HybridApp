using SQLite.Net.Attributes;

namespace HybridCommon.SqlLite.Model
{
    /// <summary>
    /// App日志
    /// </summary>
    public class AppLog
    {
        [PrimaryKey]
        public string LogId { get; set; }

        /// <summary>
        /// 是否错误
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// 提示
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 详细内容/数据结构内容
        /// </summary>
        public string ContentJson { get; set; }

        /// <summary>
        /// 创建时间
        /// yy-MM-dd HH:mm:ss
        /// </summary>
        public long CreateTime { get; set; }
    }
}
