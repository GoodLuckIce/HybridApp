using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using HybridApp.Chat;
using HybridApp.Common;

namespace HybridApp.Main
{
    public class UserMain : Fragment
    {
        private static UserMain thisFragment;
        private string PageId { get { return "我的"; } }
        private View RootLayout { get; set; }

        public static UserMain GetThis
        {
            get
            {
                if (thisFragment == null)
                {
                    thisFragment = new UserMain();
                }
                return thisFragment;
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootLayout = inflater.Inflate(Resource.Layout.HomeMainVw, container, false);
            var barTableTextView = RootLayout.FindViewById<TextView>(Resource.Id.BarTitle);
            barTableTextView.Text = "我的";
            //webView设置
            var webViewKey = X5WebViewHelper.WebViewInit(this.Activity, null, "/user/login");
            if (X5WebViewHelper.PageWebViewKey.ContainsKey(PageId))
            {
                X5WebViewHelper.PageWebViewKey[PageId] = webViewKey;
            }
            else
            {
                X5WebViewHelper.PageWebViewKey.Add(PageId, webViewKey);
            }
            //隐藏返回按钮
            var btnReturn = RootLayout.FindViewById<LinearLayout>(Resource.Id.btnReturn);
            btnReturn.Visibility = ViewStates.Gone;
            //隐藏右侧按钮
            var btnRight = RootLayout.FindViewById<LinearLayout>(Resource.Id.btnRight);
            btnRight.Visibility = ViewStates.Gone;

            return RootLayout;
        }

        public override void OnResume()
        {
            base.OnResume();
            if (X5WebViewHelper.PageWebViewKey.ContainsKey(PageId))
            {
                var webViewContainer = RootLayout.FindViewById<RelativeLayout>(Resource.Id.webViewContainer);
                var webView = X5WebViewHelper.WebViewAssets[X5WebViewHelper.PageWebViewKey[PageId]];
                webViewContainer.AddView(webView);
            }
        }

        public override void OnPause()
        {
            base.OnPause();
            if (X5WebViewHelper.PageWebViewKey.ContainsKey(PageId))
            {
                var webViewContainer = RootLayout.FindViewById<RelativeLayout>(Resource.Id.webViewContainer);
                webViewContainer.RemoveView(X5WebViewHelper.WebViewAssets[X5WebViewHelper.PageWebViewKey[PageId]]);
            }
        }
    }
}