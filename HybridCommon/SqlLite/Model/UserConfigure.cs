using SQLite.Net.Attributes;

namespace HybridCommon.SqlLite.Model
{
	public class UserConfigure
	{
        /// <summary>
        /// 标识
        /// </summary>
        [PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
		public long UserId { get; set; }

		/// <summary>
		/// 是否接收系统消息通知  0 是不接收 1 是接收
		/// </summary>
		public  int IsGetNotify { get; set; }
	}
}

