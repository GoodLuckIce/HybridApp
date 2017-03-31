namespace HybridCommon.HttpClientHelper
{
    public class PageParameterModel
    {
        public PageParameterModel()
        {
            CurrentPage = 1;
            PageCount = 20;
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 当前每页集合数量
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 数据总数 
        /// </summary>
        public int TotalCount { get; set; }


    }
}
