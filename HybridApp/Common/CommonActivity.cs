using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Com.Tencent.Smtt;
using Com.Tencent.Smtt.Export.External.Interfaces;
using HybridApp.Chat;
using Java.IO;
using Newtonsoft.Json;
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
    public class OpenPageParam
    {
        /// <summary>
        /// 是否是亲戚
        /// 如果是亲戚则和父窗口使用同一个WebView
        /// </summary>
        public bool IsRelative { get; set; }
        /// <summary>
        /// 是否保留在历史
        /// </summary>
        public bool IsHistory { get; set; }
        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName { get; set; }
        /// <summary>
        /// 传递过去当前的Fragment
        /// 如果是亲戚则和父窗口使用同一个WebView
        /// </summary>
        public string ParentWebViewKey { get; set; }

        /// <summary>
        /// 页面Id
        /// </summary>
        public string PageId { get; set; }

        /// <summary>
        /// 页面Url
        /// </summary>
        public string PageUrl { get; set; }
        /// <summary>
        /// 是否允许返回
        /// 决定是否有返回按钮显示
        /// </summary>
        public bool AllowBack { get; set; }
        /// <summary>
        /// 导航条右边按钮标题
        /// 如果按钮名字和处理Action都不为空则会启用导航条右边按钮
        /// 暂时仅支持按钮,后续扩展其他功能,或者使用自定义Activity
        /// </summary>
        public string BarRightTitle { get; set; }
        /// <summary>
        /// 导航条右边按钮点击事件委托
        /// 如果按钮名字和点击事件委托都不为null则会启用导航条右边按钮
        /// 暂时仅支持按钮,后续扩展其他功能,或者使用自定义Activity
        /// </summary>
        public Action<Activity, object, EventArgs> BarRightAction { get; set; }

    }

    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class CommonActivity : Activity
    {
        public OpenPageParam commonActivityParam { get; set; }
        public static void ShowActivity(Activity parentActivity, OpenPageParam param)
        {
            var intent = new Intent(parentActivity, typeof(CommonActivity));
            intent.PutExtra("OpenPageParam", JsonConvert.SerializeObject(param));
            parentActivity.StartActivity(intent);
        }

        protected CommonActivity(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CommonActivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.CommonActivityVw);

            var paramString = this.Intent.GetStringExtra("OpenPageParam");
            commonActivityParam = JsonConvert.DeserializeObject<OpenPageParam>(paramString);

            string webViewKey = null;
            if (commonActivityParam.IsRelative && !string.IsNullOrWhiteSpace(commonActivityParam.ParentWebViewKey) && X5WebViewHelper.WebViewAssets.ContainsKey(commonActivityParam.ParentWebViewKey))
            {
                webViewKey = X5WebViewHelper.WebViewInit(this, commonActivityParam.ParentWebViewKey, commonActivityParam.PageUrl);
            }
            else
            {
                if (X5WebViewHelper.PageWebViewKey.ContainsKey(commonActivityParam.PageId) && X5WebViewHelper.PageWebViewKey[commonActivityParam.PageId] != null)
                {
                    webViewKey = X5WebViewHelper.PageWebViewKey[commonActivityParam.PageId];
                }
                else
                {
                    webViewKey = X5WebViewHelper.WebViewInit(this, null, commonActivityParam.PageUrl);
                }
            }

            if (X5WebViewHelper.PageWebViewKey.ContainsKey(commonActivityParam.PageId))
            {
                X5WebViewHelper.PageWebViewKey[commonActivityParam.PageId] = webViewKey;
            }
            else
            {
                X5WebViewHelper.PageWebViewKey.Add(commonActivityParam.PageId, webViewKey);
            }

            var barTableTextView = FindViewById<TextView>(Resource.Id.BarTitle);
            barTableTextView.Text = commonActivityParam.PageName;

            var btnReturn = FindViewById<LinearLayout>(Resource.Id.btnReturn);
            btnReturn.Visibility = ViewStates.Visible;
            btnReturn.Click += (sender, args) =>
            {
                PageReturn();
                this.Finish();
            };

            if (!commonActivityParam.AllowBack)
            {
                //隐藏返回按钮
                btnReturn.Visibility = ViewStates.Gone;
            }

            var btnRight = FindViewById<LinearLayout>(Resource.Id.btnRight);
            if (!string.IsNullOrWhiteSpace(commonActivityParam.BarRightTitle) &&
                commonActivityParam.BarRightAction != null)
            {
                var txtRight = FindViewById<TextView>(Resource.Id.txtRight);
                txtRight.Text = commonActivityParam.BarRightTitle;

                btnRight.Click += (sender, args) =>
                {
                    commonActivityParam.BarRightAction(this, sender, args);
                };
            }
            else
            {
                btnRight.Visibility = ViewStates.Gone;
            }
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Up)
            {
                PageReturn();
                this.Finish();
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }

        private void PageReturn()
        {
            if (commonActivityParam.IsRelative && !string.IsNullOrWhiteSpace(commonActivityParam.ParentWebViewKey) && X5WebViewHelper.WebViewAssets.ContainsKey(commonActivityParam.ParentWebViewKey))
            {
                var webView = X5WebViewHelper.WebViewAssets[X5WebViewHelper.PageWebViewKey[commonActivityParam.PageId]];
                //webView.LoadUrl("javascript:window.history.go(-1)");
                webView.GoBack();
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            if (X5WebViewHelper.PageWebViewKey.ContainsKey(commonActivityParam.PageId))
            {
                var webViewContainer = this.Window.DecorView.FindViewById<RelativeLayout>(Resource.Id.webViewContainer);
                webViewContainer.AddView(X5WebViewHelper.WebViewAssets[X5WebViewHelper.PageWebViewKey[commonActivityParam.PageId]]);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (X5WebViewHelper.PageWebViewKey.ContainsKey(commonActivityParam.PageId))
            {
                var webViewContainer = this.Window.DecorView.FindViewById<RelativeLayout>(Resource.Id.webViewContainer);
                webViewContainer.RemoveView(X5WebViewHelper.WebViewAssets[X5WebViewHelper.PageWebViewKey[commonActivityParam.PageId]]);
            }
        }
    }


}