using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using HybridApp.Common;
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Com.Tencent.Smtt;
using Com.Tencent.Smtt.Export.External.Interfaces;
using CookieSyncManager = Com.Tencent.Smtt.Sdk.CookieSyncManager;
using IDownloadListener = Com.Tencent.Smtt.Sdk.IDownloadListener;
using IValueCallback = Com.Tencent.Smtt.Sdk.IValueCallback;
using IWebResourceRequest = Com.Tencent.Smtt.Export.External.Interfaces.IWebResourceRequest;
using WebChromeClient = Com.Tencent.Smtt.Sdk.WebChromeClient;
using WebResourceResponse = Com.Tencent.Smtt.Export.External.Interfaces.WebResourceResponse;
using WebSettings = Com.Tencent.Smtt.Sdk.WebSettings;
using WebStorage = Com.Tencent.Smtt.Sdk.WebStorage;
using WebView = Com.Tencent.Smtt.Sdk.WebView;
using WebViewClient = Com.Tencent.Smtt.Sdk.WebViewClient;

namespace HybridApp.Chat
{
    public class ChatMain
    {
        public ChatMain(Activity parentActivity, string parentWebViewKey)
        {

            var prarm = new OpenPageParam
            {
                IsRelative = false,
                IsHistory = true,
                PageName = "消息",
                ParentWebViewKey = parentWebViewKey,
                PageUrl = "/Chat",
                AllowBack = true,
                BarRightTitle = null,
                BarRightAction = null,
                PageId = "消息"
            };
            CommonActivity.ShowActivity(parentActivity, prarm);

        }
    }
}