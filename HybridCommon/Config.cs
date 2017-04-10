namespace HybridCommon
{
    public class Config
    {

        /// <summary>
        /// 数据库名称
        /// </summary>
        public static string DataBaseName
        {
            get { return "DataBase.db3"; }
        }

        public static int NetworkType = 0;

        public static string HtmlServerHost
        {
            get
            {
                if (NetworkType == 0)
                {
                    return "http://47.88.85.72:8056";//开发
                }
                return "http://192.168.1.175:802";//开发
            }
        }

        public static string APiServerHost
        {
            get
            {
                if (NetworkType == 0)
                {
                    return "http://192.168.1.175:802";//开发
                }
                return "http://192.168.1.175:802";//开发
            }
        }

    }
}
