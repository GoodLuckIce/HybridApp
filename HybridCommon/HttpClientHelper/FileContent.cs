using System.IO;

namespace HybridCommon.HttpClientHelper
{
    public class FileContent
    {
        /// <summary>
        /// 表单字段名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 文件名(包含扩展名)
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件流
        /// </summary>
        public Stream FileStream { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType { get; set; }
    }
}
