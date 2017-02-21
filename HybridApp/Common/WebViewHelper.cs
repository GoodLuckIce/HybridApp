using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Webkit;


namespace HybridApp.Common
{
    public class WebViewHelper
    {
        public static WebView WebViewInit(Activity activity, WebView webView, string url)
        {
            webView.ScrollBarStyle = ScrollbarStyles.InsideOverlay;
            webView.VerticalScrollBarEnabled = false;
            webView.HorizontalScrollBarEnabled = false;
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.UseWideViewPort = true;
            //缓存模式
            webView.Settings.CacheMode = CacheModes.Default;
            webView.Settings.DomStorageEnabled = true;
            webView.Settings.SetAppCacheEnabled(true);
            webView.Settings.SetAppCacheMaxSize(1024 * 1024 * 8);//设置缓冲大小，我设的是8M
            string appCacheDir = activity.GetDir("cache", FileCreationMode.Private).Path;
            webView.Settings.SetAppCachePath(appCacheDir);
            string appDatabaseDir = activity.GetDir("database", FileCreationMode.Private).Path;
            webView.Settings.DatabasePath = appDatabaseDir;
            webView.Settings.DatabaseEnabled = true;
            webView.Settings.AllowContentAccess = true;
            webView.Settings.AllowFileAccess = true;
            webView.Settings.AllowFileAccessFromFileURLs = true;
            webView.Settings.AllowUniversalAccessFromFileURLs = true;
            //网页自适应
            webView.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            webView.Settings.LoadWithOverviewMode = true;
            webView.Settings.DefaultTextEncodingName = "utf-8";
            webView.SetWebViewClient(new ExtWebViewClient(activity));
            webView.SetWebChromeClient(new ExtWebChromeClient(activity));

            //webView.LoadData("<html><body style='width:100%;height: 5000px;' ></body></html>", "text/html; charset=UTF-8", null);
            webView.LoadUrl(url);

            //var tempTimer = new Timer((obj =>
            //{
            //    Activity.RunOnUiThread(() =>
            //    {
            //        webView.LoadUrl("/");
            //    });
            //}), null, 200, Timeout.Infinite);

            return webView;
        }

        private class ExtWebChromeClient : WebChromeClient
        {
            Activity _parentActivity;
            public ExtWebChromeClient(Activity pa)
            {
                this._parentActivity = pa;
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