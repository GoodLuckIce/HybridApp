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
    public class OrderMain : Fragment
    {
        private static OrderMain thisFragment;
        private string PageId { get { return "∂©µ•"; } }
        private View RootLayout { get; set; }

        public static OrderMain GetThis
        {
            get
            {
                if (thisFragment == null)
                {
                    thisFragment = new OrderMain();
                }
                return thisFragment;
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootLayout = inflater.Inflate(Resource.Layout.HomeMainVw, container, false);
            var barTableTextView = RootLayout.FindViewById<TextView>(Resource.Id.BarTitle);
            barTableTextView.Text = "∂©µ•";
            //webView…Ë÷√
            var webViewKey = X5WebViewHelper.WebViewInit(this.Activity, null, "/order");
            if (X5WebViewHelper.PageWebViewKey.ContainsKey(PageId))
            {
                X5WebViewHelper.PageWebViewKey[PageId] = webViewKey;
            }
            else
            {
                X5WebViewHelper.PageWebViewKey.Add(PageId, webViewKey);
            }
            //“˛≤ÿ∑µªÿ∞¥≈•
            var btnReturn = RootLayout.FindViewById<LinearLayout>(Resource.Id.btnReturn);
            btnReturn.Visibility = ViewStates.Gone;
            //“˛≤ÿ”“≤‡∞¥≈•
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
                webViewContainer.AddView(X5WebViewHelper.WebViewAssets[X5WebViewHelper.PageWebViewKey[PageId]]);
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