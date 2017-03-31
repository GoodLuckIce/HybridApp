using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HybridCommon.Agreement;
using HybridCommon.Context;
using HybridCommon.SqlLite;
using HybridCommon.SqlLite.Model;
using Newtonsoft.Json;

namespace HybridAndroid.Common
{
    public class AgreementProvider : IAgreementProvider
    {
        #region Js触发的方法
        /// <summary>
        /// 跳转页面
        /// </summary>
        /// <param name="prarm">跳转页面需要传递的参数</param>
        public void JumpPage(PageParam prarm)
        {
            X5WebViewActivity.ShowActivity(prarm);
        }

        /// <summary>
        /// 返回到指定页面
        /// </summary>
        /// <param name="currentPageId">当前页面PageId</param>
        /// <param name="pageId">需要返回的指定PageId,如果为空则返回上一个页面</param>
        /// <param name="param">需要传递的参数</param>
        public void PageBack(string currentPageId, string pageId, string param)
        {
            if (!string.IsNullOrWhiteSpace(pageId)
                && "undefined" != pageId
                && X5WebViewHelper.PageActivity.ContainsKey(pageId)
                && X5WebViewHelper.PageParam.ContainsKey(pageId))
            {
                X5WebViewHelper.PageParam[pageId].BackParam = param;
                //var router = "{ path: '" + X5WebViewHelper.PageParam[pageId].PageUrl + "', params: { PageId: '" + pageId + "' }}";

                var router = X5WebViewHelper.PageParam[pageId].PageUrl + "/" + pageId + "/^";
                if (X5WebViewHelper.PageParam[currentPageId].IsRelative)
                {
                    ExecuteJavaScript(pageId, $@"window.AppBridge.SetPageId(""{X5WebViewHelper.PageParam[pageId].ParentPageId}"",""{X5WebViewHelper.PageParam[pageId].PageId}"")");
                    ExecuteJavaScript(pageId, $@"window.AppBridge.Router.push(""{router}"")");
                }
                AnalyticAgreement.AgreementProvider.OnResume(pageId, param);
                var pageIdList = new List<string>();
                pageIdList = GetParentPageIdList(pageIdList, currentPageId, pageId);

                MainActivity.thisActivity.RunOnUiThread((() =>
                {
                    foreach (var item in pageIdList)
                    {
                        var pageActivity = X5WebViewHelper.PageActivity;
                        var pageParam = X5WebViewHelper.PageParam;
                        var webViewAssets = X5WebViewHelper.WebViewAssets;
                        pageActivity[item].Finish();
                        pageActivity[item] = null;
                        pageActivity.Remove(item);
                        if (!X5WebViewHelper.PageParam[item].IsRelative)
                        {
                            webViewAssets[pageParam[item].WebViewKey].RemoveAllViews();
                            webViewAssets[pageParam[item].WebViewKey].Destroy();
                            webViewAssets[pageParam[item].WebViewKey] = null;
                            webViewAssets.Remove(pageParam[item].WebViewKey);
                        }
                        pageParam.Remove(currentPageId);
                    }
                }));

            }
            else
            {
                var pa = X5WebViewHelper.PageParam[X5WebViewHelper.PageParam[currentPageId].ParentPageId];
                X5WebViewHelper.PageParam[pa.PageId].BackParam = param;
                if (X5WebViewHelper.PageParam[currentPageId].IsRelative)
                {
                    ExecuteJavaScript(pa.PageId, $@"window.AppBridge.SetPageId(""{pa.ParentPageId}"",""{pa.PageId}"")");
                    ExecuteJavaScript(pa.PageId, $@"window.AppBridge.Router.go(-1)");
                }
                AnalyticAgreement.AgreementProvider.OnResume(pa.PageId, param);


                MainActivity.thisActivity.RunOnUiThread((() =>
                {
                    var pageActivity = X5WebViewHelper.PageActivity;
                    var pageParam = X5WebViewHelper.PageParam;
                    var webViewAssets = X5WebViewHelper.WebViewAssets;
                    pageActivity[currentPageId].Finish();
                    pageActivity[currentPageId] = null;
                    pageActivity.Remove(currentPageId);

                    if (!X5WebViewHelper.PageParam[currentPageId].IsRelative)
                    {
                        webViewAssets[pageParam[currentPageId].WebViewKey].RemoveAllViews();
                        webViewAssets[pageParam[currentPageId].WebViewKey].Destroy();
                        webViewAssets[pageParam[currentPageId].WebViewKey] = null;
                        webViewAssets.Remove(pageParam[currentPageId].WebViewKey);
                    }
                    pageParam.Remove(currentPageId);
                }));

            }
        }

        List<string> GetParentPageIdList(List<string> pageIdList, string currentPageId, string pageId)
        {
            var pp = X5WebViewHelper.PageParam[currentPageId];
            if (X5WebViewHelper.PageActivity.ContainsKey(pp.PageId))
            {
                pageIdList.Add(pp.PageId);
            }
            if (pp.ParentPageId == pageId)
            {
                return pageIdList;
            }
            else
            {
                return GetParentPageIdList(pageIdList, pp.ParentPageId, pageId);
            }
        }
        public void SaveUserContext(string userId, string username)
        {
            var db = DbConHelper.NewDbCon();
            var model = new UserCertificate();
            model.UserId = userId;
            model.UserName = username;
            db.Insert(model);
        }


        /// <summary>
        /// 执行JavaScript
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="js"></param>
        public void ExecuteJavaScript(string pageId, string js)
        {
            Task.Run((() =>
            {
                var temp = true;
                while (temp)
                {
                    if (X5WebViewHelper.PageParam[pageId].IsLoadFinished)
                    {
                        temp = false;
                        var x5WebView = X5WebViewHelper.WebViewAssets[X5WebViewHelper.PageParam[pageId].WebViewKey];
                        //js = js.Replace("'", "\\'");
                        //js = $@"executeJs(""{js}"")";
                        js = $"javascript:{js}";
                        MainActivity.thisActivity.RunOnUiThread((() =>
                        {
                            x5WebView.LoadUrl(js);
                        }));
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
            }));
        }


        /// <summary>
        /// 页面加载完毕时调用的方法
        /// </summary>
        /// <param name="pageId"></param>
        public void PageFinished(string pageId)
        {
            if (X5WebViewHelper.PageParam.ContainsKey(pageId))
            {
                var model = new
                {
                    ParentPageId = X5WebViewHelper.PageParam[pageId].ParentPageId,
                    PageId = pageId,
                    UserContext = new
                    {
                        UserID = UserContext.UserId,
                        UserName = UserContext.Username
                    },
                    DeviceInfo = new
                    {
                        PlatformType = "Android"
                    }
                };
                var modelJson = JsonConvert.SerializeObject(model).Replace("\"", "\\\"");
                ExecuteJavaScript(pageId, $@"window.AppBridge.DataInit(""{modelJson}"")");

                OnCreate(X5WebViewHelper.PageParam[pageId].ParentPageId, pageId, X5WebViewHelper.PageParam[pageId].OpenParam);


                X5WebViewHelper.PageParam[pageId].IsLoadFinished = true;

            }
        }

        #endregion

        #region App触发的方法


        /// <summary>
        /// 点击返回按钮时触发
        /// </summary>
        /// <param name="isRelative"></param>
        public void OnBackBtn(string pageId, bool isRelative)
        {
            ExecuteJavaScript(pageId, $@"window.AppBridge.OnBackBtn(""{isRelative}"")");
        }

        /// <summary>
        /// 点击右侧按钮时触发
        /// </summary>
        public void OnRightBtn(string pageId)
        {
            ExecuteJavaScript(pageId, $@"window.AppBridge.OnRightBtn(""{pageId}"")");
        }

        /// <summary>
        /// 从其他页面返回过来时触发
        /// </summary>
        public void OnResume(string pageId, string prarm)
        {
            ExecuteJavaScript(pageId, $@"window.AppBridge.OnResume(""{pageId}"",""{prarm}"")");
        }

        /// <summary>
        /// 页面打开时触发
        /// </summary>
        /// <param name="parentPageId"></param>
        /// <param name="pageId"></param>
        /// <param name="prarm">传递参数</param>
        public void OnCreate(string parentPageId, string pageId, string prarm)
        {
            ExecuteJavaScript(pageId, $@"window.AppBridge.OnCreate(""{parentPageId}"",""{pageId}"",""{prarm}"")");
        }

        #endregion
    }
}