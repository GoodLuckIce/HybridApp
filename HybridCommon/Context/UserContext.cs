using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridCommon.Context
{
    public class UserContext
    {

        /// <summary>
        /// 是否已登录
        /// </summary>
        public static bool IsLogin { get { return !string.IsNullOrWhiteSpace(UserId); } }

        /// <summary>
        /// 用户ID
        /// </summary>
        /// <value>The user I.</value>
        public static string UserId { get; set; }

        /// <summary>
        /// 登录SN,每次登录都会变
        /// </summary>
        /// <value>The user I.</value>
        public static Guid LoginSN { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        /// <value>The mobile.</value>
        public static string Mobile { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        /// <value>The username.</value>
        public static string UserName { get; set; }

        /// <summary>
        /// 登陆密码
        /// </summary>
        /// <value>The password.</value>
        public static string Password { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public static string Avatar { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        /// <value>The data status.</value>
        public static int DataStatus { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public static double Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public static double Latitude { get; set; }

        /// <summary>
        /// 极光设备Id
        /// </summary>
        public static string RegistrationID { get; set; }

        /// <summary>
        /// 推送号
        /// </summary>
        public static string PushCode { get; set; }

    }
}
