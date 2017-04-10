using System;
using SQLite.Net.Attributes;

namespace HybridCommon.SqlLite.Model
{
    public class UserCertificate
    {
        /// <summary>
        /// 标识
        /// </summary>
        [PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string Avatar { get; set; }
        
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

    }
}
