using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HybridCommon.Context;
using HybridCommon.SqlLite;
using HybridCommon.SqlLite.Model;
using Newtonsoft.Json;

namespace HybridCommon.Agreement
{
    /// <summary>
    /// 解析协议
    /// </summary>
    public class AnalyticAgreement
    {
        /// <summary>
        /// 协议提供者
        /// </summary>
        public static IAgreementProvider AgreementProvider { get; set; }

        public static Dictionary<string, PageParam> PageParam { get; set; }


        public static bool HandleAgreement(string url)
        {
            var hybridScheme = "hybridagreement";
            url = Uri.UnescapeDataString(url);
            var scheme = url.Split(':')[0];
            if (scheme == hybridScheme)
            {
                url = url.Replace(hybridScheme + "://", "");
                var strList = url.Split('?');
                var s1 = new Dictionary<string, string>();
                if (strList.Length > 1 && !String.IsNullOrWhiteSpace(strList[1]))
                {
                    s1 = strList[1].Split('&').ToDictionary(p => p.Split('=')[0], p1 => p1.Split('=')[1]);
                }

                var handleStr = strList[0].Replace("/", "");
                if (handleStr == "JumpPage")
                {
                    var strJson = s1["p"];
                    var openPageParam = JsonConvert.DeserializeObject<PageParam>(strJson);
                    openPageParam.PageId = openPageParam.PageId.Replace("/", ">");
                    AgreementProvider.JumpPage(openPageParam);
                }
                else if (handleStr == "PageBack")
                {
                    var parentPageId = s1["currentPageId"].Replace("/", ">");
                    var pageId = s1["pageId"].Replace("/", ">"); ;
                    var param = s1["p"];
                    AgreementProvider.PageBack(parentPageId, pageId, param);
                }
                else if (handleStr == "PageFinished")
                {
                    var pageId = s1["pageId"].Replace("/", ">"); ;
                    AgreementProvider.PageFinished(pageId);
                }
                else if (handleStr == "SaveUserContext")
                {
                    var userId = s1["userId"];
                    var userName = s1["userName"];
                    var avatar = s1["avatar"];

                    UserContext.UserId = userId;
                    UserContext.UserName = userName;
                    UserContext.Avatar = avatar;

                    var db = DbConHelper.NewDbCon();
                    db.Table<UserCertificate>().Delete(p => p.UserId == userId);

                    if (!String.IsNullOrWhiteSpace(userId))
                    {
                        var userCertificate = new UserCertificate();
                        userCertificate.LoginTime = DateTime.Now;
                        userCertificate.UserId = userId;
                        userCertificate.UserName = userName;
                        userCertificate.Avatar = avatar;
                        db.Insert(userCertificate);
                    }

                    var model = new
                    {
                        UserId = UserContext.UserId,
                        UserName = UserContext.UserName,
                        Avatar = UserContext.Avatar
                    };
                    var modelJson = JsonConvert.SerializeObject(model).Replace("\"", "\\\"");
                    foreach (var item in PageParam)
                    {
                        AgreementProvider.ExecuteJavaScript(item.Key, $@"window.AppBridge.SetUserContext(""{modelJson}"")");
                    }

                }
                else if (handleStr == "SetPageIdComplete")
                {
                    var parentPageId = s1["parentPageId"].Replace("/", ">");
                    var pageId = s1["pageId"].Replace("/", ">");
                    AgreementProvider.SetPageIdComplete(parentPageId, pageId);
                }
                return false;
            }
            return true;
        }
    }

}
