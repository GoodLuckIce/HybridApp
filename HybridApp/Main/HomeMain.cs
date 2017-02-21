using System.Collections.Generic;
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
using Mono.CSharp;

namespace HybridApp.Main
{
    public class HomeMain : Fragment
    {
        private static HomeMain thisFragment;
        private string PageId { get { return "��ҳ"; } }
        private View RootLayout { get; set; }

        public static HomeMain GetThis
        {
            get
            {
                if (thisFragment == null)
                {
                    thisFragment = new HomeMain();
                }
                return thisFragment;
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootLayout = inflater.Inflate(Resource.Layout.HomeMainVw, container, false);
            var barTableTextView = RootLayout.FindViewById<TextView>(Resource.Id.BarTitle);
            barTableTextView.Text = "��ҳ";
            //webView����
            var webViewKey = X5WebViewHelper.WebViewInit(this.Activity, null, "/");
            if (X5WebViewHelper.PageWebViewKey.ContainsKey(PageId))
            {
                X5WebViewHelper.PageWebViewKey[PageId] = webViewKey;
            }
            else
            {
                X5WebViewHelper.PageWebViewKey.Add(PageId, webViewKey);
            }
            //���ط��ذ�ť
            var btnReturn = RootLayout.FindViewById<LinearLayout>(Resource.Id.btnReturn);
            btnReturn.Visibility = ViewStates.Gone;
            //�Ҳఴť����
            var txtRight = RootLayout.FindViewById<TextView>(Resource.Id.txtRight);
            txtRight.Text = "��Ϣ";
            //�Ҳఴť��Ӧ
            var btnRight = RootLayout.FindViewById<LinearLayout>(Resource.Id.btnRight);
            btnRight.Click += (sender, args) =>
            {
                //var intent = new Intent(this.Activity, typeof(ChatMain));
                //intent.PutExtra("pram", "1");
                //StartActivity(intent);
                new ChatMain(this.Activity, webViewKey);
            };

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