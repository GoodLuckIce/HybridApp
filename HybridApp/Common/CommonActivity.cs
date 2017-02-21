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
        /// �Ƿ�������
        /// �����������͸�����ʹ��ͬһ��WebView
        /// </summary>
        public bool IsRelative { get; set; }
        /// <summary>
        /// �Ƿ�������ʷ
        /// </summary>
        public bool IsHistory { get; set; }
        /// <summary>
        /// ҳ������
        /// </summary>
        public string PageName { get; set; }
        /// <summary>
        /// ���ݹ�ȥ��ǰ��Fragment
        /// �����������͸�����ʹ��ͬһ��WebView
        /// </summary>
        public string ParentWebViewKey { get; set; }

        /// <summary>
        /// ҳ��Id
        /// </summary>
        public string PageId { get; set; }

        /// <summary>
        /// ҳ��Url
        /// </summary>
        public string PageUrl { get; set; }
        /// <summary>
        /// �Ƿ�������
        /// �����Ƿ��з��ذ�ť��ʾ
        /// </summary>
        public bool AllowBack { get; set; }
        /// <summary>
        /// �������ұ߰�ť����
        /// �����ť���ֺʹ���Action����Ϊ��������õ������ұ߰�ť
        /// ��ʱ��֧�ְ�ť,������չ��������,����ʹ���Զ���Activity
        /// </summary>
        public string BarRightTitle { get; set; }
        /// <summary>
        /// �������ұ߰�ť����¼�ί��
        /// �����ť���ֺ͵���¼�ί�ж���Ϊnull������õ������ұ߰�ť
        /// ��ʱ��֧�ְ�ť,������չ��������,����ʹ���Զ���Activity
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
                //���ط��ذ�ť
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