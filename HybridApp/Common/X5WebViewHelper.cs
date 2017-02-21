using System;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
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


namespace HybridApp.Common
{
    public class X5WebViewHelper
    {
        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length = 20, bool useNum = true, bool useLow = true, bool useUpp = true, bool useSpe = false, string custom = "")
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        public static Dictionary<string, WebView> WebViewAssets { get; set; }
        public static Dictionary<string, string> PageWebViewKey{ get; set; }
        public static string WebViewInit(Activity activity, string webViewAssetsKey, string url)
        {
            url = "192.168.2.103:8080/#/" + url;
            url = url.Replace("//", "/");
            url = "http://" + url;


            if (WebViewAssets == null)
            {
                WebViewAssets = new Dictionary<string, WebView>();
            }


            if (string.IsNullOrWhiteSpace(webViewAssetsKey) || !WebViewAssets.ContainsKey(webViewAssetsKey))
            {
                var x5WebView = new WebView(activity.Application.ApplicationContext);
                if (string.IsNullOrWhiteSpace(webViewAssetsKey))
                {
                    webViewAssetsKey = GetRandomString();
                }
                WebViewAssets.Add(webViewAssetsKey, x5WebView);

            }
            WebViewAssets[webViewAssetsKey].ScrollBarStyle = ScrollbarStyles.InsideOverlay;
            WebViewAssets[webViewAssetsKey].VerticalScrollBarEnabled = false;
            WebViewAssets[webViewAssetsKey].HorizontalScrollBarEnabled = false;

            WebViewAssets[webViewAssetsKey].Settings.JavaScriptEnabled = true;
            WebViewAssets[webViewAssetsKey].Settings.UseWideViewPort = true;
            //缓存模式
            WebViewAssets[webViewAssetsKey].Settings.DomStorageEnabled = true;
            WebViewAssets[webViewAssetsKey].Settings.SetAppCacheMaxSize(1024 * 1024 * 8);//设置缓冲大小，我设的是8M
            string appCacheDir = activity.GetDir("appcache", FileCreationMode.Private).Path;
            WebViewAssets[webViewAssetsKey].Settings.SetAppCachePath(appCacheDir);
            WebViewAssets[webViewAssetsKey].Settings.SetAppCacheEnabled(true);
            string appDatabaseDir = activity.GetDir("databases", FileCreationMode.Private).Path;
            WebViewAssets[webViewAssetsKey].Settings.DatabasePath = appDatabaseDir;
            WebViewAssets[webViewAssetsKey].Settings.DatabaseEnabled = true;
            string appGeolocationDir = activity.GetDir("geolocation", FileCreationMode.Private).Path;
            WebViewAssets[webViewAssetsKey].Settings.SetGeolocationDatabasePath(appGeolocationDir);
            WebViewAssets[webViewAssetsKey].Settings.SetGeolocationEnabled(true);
            WebViewAssets[webViewAssetsKey].Settings.AllowContentAccess = true;
            WebViewAssets[webViewAssetsKey].Settings.AllowFileAccess = true;
            //网页自适应
            WebViewAssets[webViewAssetsKey].Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            WebViewAssets[webViewAssetsKey].Settings.LoadWithOverviewMode = true;
            WebViewAssets[webViewAssetsKey].Settings.DefaultTextEncodingName = "utf-8";
            WebViewAssets[webViewAssetsKey].SetWebViewClient(new ExtWebViewClient(activity));
            WebViewAssets[webViewAssetsKey].SetWebChromeClient(new ExtWebChromeClient(activity));

            WebViewAssets[webViewAssetsKey].Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.NarrowColumns);
            WebViewAssets[webViewAssetsKey].Settings.SetSupportZoom(false);
            WebViewAssets[webViewAssetsKey].Settings.SetSupportMultipleWindows(false);

            WebViewAssets[webViewAssetsKey].SetDownloadListener(new DownloadListener());

            //webView.LoadData("<html><body style='width:100%;height: 5000px;' ></body></html>", "text/html; charset=UTF-8", null);
            WebViewAssets[webViewAssetsKey].LoadUrl(url);

            //var tempTimer = new Timer((obj =>
            //{
            //    Activity.RunOnUiThread(() =>
            //    {
            //        webView.LoadUrl("/");
            //    });
            //}), null, 200, Timeout.Infinite);


            CookieSyncManager.CreateInstance(activity.Application.ApplicationContext);
            CookieSyncManager.Instance.Sync();

            WebViewAssets[webViewAssetsKey].AddJavascriptInterface(new WebViewJavaScriptFunction(), "Android");

            return webViewAssetsKey;
        }

        private class ExtWebChromeClient : WebChromeClient
        {
            Activity _parentActivity;
            public ExtWebChromeClient(Activity pa)
            {
                this._parentActivity = pa;
            }

            public override bool OnJsConfirm(WebView p0, string p1, string p2, IJsResult p3)
            {
                return base.OnJsConfirm(p0, p1, p2, p3);
            }

            View myVideoView;
            View myNormalView;
            IX5WebChromeClientCustomViewCallback callback;
            public override void OnShowCustomView(View view, int p1, IX5WebChromeClientCustomViewCallback customViewCallback)
            {
                base.OnShowCustomView(view, p1, customViewCallback);
                var normalView = (FrameLayout)_parentActivity.FindViewById(Resource.Id.details);
                ViewGroup viewGroup = (ViewGroup)normalView.Parent;
                viewGroup.RemoveView(normalView);
                viewGroup.AddView(view);
                myVideoView = view;
                myNormalView = normalView;
                callback = customViewCallback;
            }

            public override void OnHideCustomView()
            {
                base.OnHideCustomView();
                if (callback != null)
                {
                    callback.OnCustomViewHidden();
                    callback = null;
                }
                if (myVideoView != null)
                {
                    ViewGroup viewGroup = (ViewGroup)myVideoView.Parent;
                    viewGroup.RemoveView(myVideoView);
                    viewGroup.AddView(myNormalView);
                }
            }

            public override bool OnShowFileChooser(WebView arg0, IValueCallback arg1, FileChooserParams arg2)
            {
                Console.WriteLine("onShowFileChooser");
                return base.OnShowFileChooser(arg0, arg1, arg2);
            }

            public override void OpenFileChooser(IValueCallback uploadFile, string acceptType, string captureType)
            {
                base.OpenFileChooser(uploadFile, acceptType, captureType);
            }

            public override bool OnJsAlert(WebView p0, string p1, string p2, IJsResult p3)
            {
                //这里写入你自定义的window alert
                return base.OnJsAlert(p0, p1, p2, p3);
            }

            //对应js 的通知弹框 ，可以用来实现js 和 android之间的通信
            public override void OnReceivedTitle(WebView p0, string p1)
            {
                base.OnReceivedTitle(p0, p1);
            }

            public override void OnReachedMaxAppCacheSize(long requiredStorage, long quota, WebStorage.IQuotaUpdater quotaUpdater)
            {
                quotaUpdater.UpdateQuota(requiredStorage * 2);
            }


        }

        private class ExtWebViewClient : WebViewClient
        {
            Activity _parentActivity;


            public ExtWebViewClient(Activity pa)
            {
                this._parentActivity = pa;
            }
            public override void OnPageFinished(WebView view, string url)
            {
                var tempTimer = new Timer((obj =>
                {
                }), null, 200, Timeout.Infinite);
            }
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                view.LoadUrl(url);
                return true;
            }

            public override WebResourceResponse ShouldInterceptRequest(WebView p0, IWebResourceRequest request)
            {
                Console.WriteLine("should request.getUrl().toString() is " + request.Url);
                return base.ShouldInterceptRequest(p0, request);
            }
        }

        public class DownloadListener : IDownloadListener
        {

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public IntPtr Handle { get; }
            public void OnDownloadStart(string p0, string p1, string p2, string p3, long p4)
            {
                throw new NotImplementedException();
            }


        }

        public class WebViewJavaScriptFunction : Java.Lang.Object
        {
            [JavascriptInterface]
            public void onJsFunctionCalled(String tag)
            {

            }
            public void onSubmit(String s)
            {
                Console.WriteLine("onSubmit ");
            }
        }

        public class Agreement
        {
            /// <summary>
            /// 获取跳转新页面协议
            /// </summary>
            /// <returns></returns>
            public static string GetToPageAgreement()
            {
                return "";
            }

            /// <summary>
            /// 获取协议桥
            /// </summary>
            /// <returns></returns>
            public static string GetBridgeAgreement()
            {
                return @"

window.CallAppBridge  =  function (url) {
  /*window.location = url;*/
  var ua = window.navigator.userAgent,os={};
  var android = ua.match(/(Android);?[\s\/]+([\d.]+)?/),
    /*osx = !!ua.match(/\(Macintosh\; Intel /),*/
    ipad = ua.match(/(iPad).*OS\s([\d_]+)/),
    ipod = ua.match(/(iPod)(.*OS\s([\d_]+))?/),
    iphone = !ipad && ua.match(/(iPhone\sOS)\s([\d_]+)/);
  if (android){ os.android = true, os.version = android[2];}
  if (iphone && !ipod) {os.ios = os.iphone = true, os.version = iphone[2].replace(/_/g, '.');}
  if (ipad){ os.ios = os.ipad = true, os.version = ipad[2].replace(/_/g, '.');}
  if (ipod){ os.ios = os.ipod = true, os.version = ipod[3] ? ipod[3].replace(/_/g, '.') : null;}
  if (os.ios) {
    window.location = url;
  } else {
    var ifr = document.createElement('iframe');
    ifr.setAttribute('style','display: none;');
    ifr.setAttribute('src',url);
    document.querySelector('body').appendChild(ifr);

    setTimeout(function ()
    {
      ifr.parentNode.removeChild(ifr);
    }, 1000);
  }
};
window.route_getHybridUrl = function(address,callback) {
  if(callback){
    address += '&cb='+callback
  }

  return address;
};
window.routeRequestHybrid = function(address,callback)
{
  callback = window.routeMadeCallBack(callback);
  window.CallAppBridge(window.route_getHybridUrl(address,callback));
};

window.routeMadeCallBack = function(callback,q)
{
  /*生成唯一执行函数，执行后销毁*/
  var time = (new Date().getTime());
  var t = 'hybrid_' + time + (q||'');
  var tmpFn;

  /*处理有回调的情况*/
  if (callback) {
    tmpFn = callback;
    callback = t;
    window.H5Api[t] = function(data) {
      tmpFn(data);
      delete window.Hybrid[t];
    }
  }
  return callback;
};

function GetRandomNum(Min,Max)
{
  var Range = Max - Min;
  var Rand = Math.random();
  return(Min + Math.round(Rand * Range));
}

window.CallAppBridgeAsyn  =  function (url) {
  if (window.CallAppBridgeData == undefined)
  {
    window.CallAppBridgeData = [];
  }
  window.CallAppBridgeData.push(url)

  if(window.CallAppBridgeInterval ==undefined)
  {
    window.CallAppBridgeInterval = setInterval(function() {

      var tempUrl = window.CallAppBridgeData.shift();
      if(tempUrl != undefined)
      {
        window.CallAppBridge(tempUrl);
      }
    }, 100);
  }
};

";
            }
        }
    }
}