using System.Collections.Generic;

namespace HybridCommon.HttpClientHelper
{
    public class PageResultModel<T>
    {
        public PageResultModel()
        {
            CurrentPage = 1;
            Data = new List<T>();
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

        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> Data { get; set; }
    }
}
