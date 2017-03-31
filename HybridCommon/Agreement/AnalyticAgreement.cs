using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
                if (strList.Length > 1 && !string.IsNullOrWhiteSpace(strList[1]))
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
                    var parentPageId = s1["currentPageId"];
                    var pageId = s1["pageId"].Replace("/", ">"); ;
                    var param = s1["p"];
                    AgreementProvider.PageBack(parentPageId, pageId, param);
                }
                else if (handleStr == "PageFinished")
                {
                    var pageId = s1["pageId"].Replace("/", ">"); ;
                    AgreementProvider.PageFinished(pageId);
                }
                return false;
            }
            return true;
        }


    }

}
