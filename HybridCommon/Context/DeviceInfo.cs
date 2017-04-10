using Plugin.DeviceInfo.Abstractions;

namespace HybridCommon.Context
{
    public class DeviceInfo
    {
        /// <summary>
        /// 是否第一次加载
        /// </summary>
        public static bool IsFirstLoad { get; set; }

        /// <summary>
        /// 文件根目录
        /// </summary>
        public static string RootFolder { get; set; }

        /// <summary>
        /// Html文件目录
        /// </summary>
        public static string HtmlFolder { get { return RootFolder + "/Html"; } }
    }
}
