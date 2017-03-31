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

        public static string ServerHost
        {
            get
            {
                if (NetworkType == 0)
                {
                    return "http://192.168.2.103";//开发
                }
                else if (NetworkType == 1)
                {
                    return "http://192.168.1.175";//测试
                }
                return "http://192.168.2.103";//开发
            }
        }
        
    }
}
