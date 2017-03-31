using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HybridCommon.Agreement;
using Newtonsoft.Json;
using WebView = Com.Tencent.Smtt.Sdk.WebView;

namespace HybridAndroid.Common
{

    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class X5WebViewActivity : Activity
    {

        public string ParentPageId { get; set; }
        public string PageId { get; set; }

        public string ParentPageUrl { get; set; }
        public string PageUrl { get; set; }
        public string ParentWebViewKey { get; set; }
        public string WebViewKey { get; set; }
        


        public static void ShowActivity(PageParam param)
        {
            MainActivity.thisActivity.RunOnUiThread((() =>
            {
                var pageActivity = X5WebViewHelper.PageActivity;
                //var parentActivity = pageActivity[param.ParentPageId];
                var intent = new Intent(MainActivity.thisActivity, typeof(X5WebViewActivity));
                intent.PutExtra("OpenPageParam", JsonConvert.SerializeObject(param));
                MainActivity.thisActivity.StartActivity(intent);
            }));
        }



        protected X5WebViewActivity(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public X5WebViewActivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.CommonActivityVw);

            var paramString = this.Intent.GetStringExtra("OpenPageParam");
            var pageParam = JsonConvert.DeserializeObject<PageParam>(paramString);

            var parentPageAttributes = X5WebViewHelper.PageParam[pageParam.ParentPageId];
            if (parentPageAttributes != null)
            {
                pageParam.ParentPageUrl = parentPageAttributes.PageUrl;
                pageParam.ParentWebViewKey = parentPageAttributes.WebViewKey;
            }
            PageId = pageParam.PageId;
            ParentPageId = pageParam.ParentPageId;
            PageUrl = pageParam.PageUrl;
            ParentPageUrl = pageParam.ParentPageUrl;
            if (X5WebViewHelper.PageParam.ContainsKey(PageId))
            {
                X5WebViewHelper.PageParam[PageId] = pageParam;
            }
            else
            {
                X5WebViewHelper.PageParam.Add(PageId, pageParam);
            }
            if (X5WebViewHelper.PageActivity.ContainsKey(PageId))
            {
                X5WebViewHelper.PageActivity[PageId] = this;
            }
            else
            {
                X5WebViewHelper.PageActivity.Add(PageId, this);
            }

            pageParam.WebViewKey = X5WebViewHelper.WebViewInit(PageId);

            WebViewKey = pageParam.WebViewKey;
            ParentWebViewKey = pageParam.ParentWebViewKey;

            //var router = "{ path: '"+ pageParam.PageUrl + "', params: { PageId: '"+ PageId + "' }}";
            if (string.IsNullOrWhiteSpace(pageParam.OpenParam))
            {
                pageParam.OpenParam = "^";
            }
            pageParam.OpenParam = pageParam.OpenParam.Replace("'", "```").Replace("\"", "~~~");
            var router = pageParam.PageUrl + "/" + pageParam.OpenParam;
            AnalyticAgreement.AgreementProvider.ExecuteJavaScript(PageId, $@"window.AppBridge.Router.push(""{router}"")");
            AnalyticAgreement.AgreementProvider.ExecuteJavaScript(PageId, $@"window.AppBridge.SetPageId(""{pageParam.ParentPageId}"",""{pageParam.PageId}"")");

            var barTableTextView = FindViewById<TextView>(Resource.Id.BarTitle);
            barTableTextView.Text = pageParam.PageName;

            var btnReturn = FindViewById<LinearLayout>(Resource.Id.btnReturn);
            btnReturn.Visibility = ViewStates.Visible;
            btnReturn.Click += (sender, args) =>
            {
                PageBack();
            };

            if (!pageParam.AllowBack)
            {
                //Òþ²Ø·µ»Ø°´Å¥
                btnReturn.Visibility = ViewStates.Gone;
            }

            var btnRight = FindViewById<LinearLayout>(Resource.Id.btnRight);
            if (pageParam.AllowRightBtn
                && pageParam.RightBtnParam != null
                && !string.IsNullOrWhiteSpace(pageParam.RightBtnParam.BtnTitle)
                && pageParam.RightBtnParam.CallBackAction != null)
            {
                var txtRight = FindViewById<TextView>(Resource.Id.txtRight);
                txtRight.Text = pageParam.RightBtnParam.BtnTitle;

                btnRight.Click += (sender, args) =>
                {
                    pageParam.RightBtnParam.CallBackAction(sender, args);

                    AnalyticAgreement.AgreementProvider.OnRightBtn(PageId);
                };
            }
            else
            {
                btnRight.Visibility = ViewStates.Gone;
            }
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                PageBack();
                return true;
            }
            else
            {
                return base.OnKeyDown(keyCode, e);
            }
        }

        private void PageBack()
        {
            AnalyticAgreement.AgreementProvider.OnBackBtn(PageId, X5WebViewHelper.PageParam[PageId].IsRelative);
        }

        protected override void OnResume()
        {
            base.OnResume();
            var webViewContainer = this.Window.DecorView.FindViewById<RelativeLayout>(Resource.Id.webViewContainer);
            X5WebViewHelper.AttachedWebView(WebViewKey, webViewContainer);
        }

        protected override void OnPause()
        {
            base.OnPause();
            var webViewContainer = this.Window.DecorView.FindViewById<RelativeLayout>(Resource.Id.webViewContainer);
            X5WebViewHelper.AttachedWebView(WebViewKey, webViewContainer);
        }
    }


}