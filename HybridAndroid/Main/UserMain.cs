using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using HybridAndroid.Common;
using HybridCommon.Agreement;


namespace HybridAndroid.Main
{
    public class UserMain : Fragment
    {
        private static UserMain thisFragment;
        public string ParentPageId { get; set; }
        public string PageId { get; set; }

        public string ParentPageUrl { get; set; }
        public string PageUrl { get; set; }
        public string ParentWebViewKey { get; set; }
        public string WebViewKey { get; set; }
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
            barTableTextView.Text = "�ҵ�";
            PageUrl = "/User";
            PageId = "�ҵĸ�������";
            ParentWebViewKey = "";
            ParentPageId = "";
            ParentPageUrl = "";
            var pageParam = new PageParam()
            {
                PageUrl = PageUrl,
                PageId = PageId,
                ParentPageId = "",
                ParentPageUrl = ""
            };

            if (AnalyticAgreement.PageParam.ContainsKey(pageParam.PageId))
            {
                AnalyticAgreement.PageParam[pageParam.PageId] = pageParam;
            }
            else
            {
                AnalyticAgreement.PageParam.Add(pageParam.PageId, pageParam);
            }
            if (X5WebViewHelper.PageActivity.ContainsKey(pageParam.PageId))
            {
                X5WebViewHelper.PageActivity[pageParam.PageId] = this.Activity;
            }
            else
            {
                X5WebViewHelper.PageActivity.Add(PageId, this.Activity);
            }
            //webView����
            pageParam.WebViewKey = X5WebViewHelper.WebViewInit(PageId);
            WebViewKey = pageParam.WebViewKey;


            //var router = "{ path: '" + pageParam.PageUrl + "', params: { PageId: '" + PageId + "' }}";
            if (string.IsNullOrWhiteSpace(pageParam.OpenParam))
            {
                pageParam.OpenParam = "";
            }
            pageParam.OpenParam = pageParam.OpenParam.Replace("'", "```").Replace("\"", "~~~");
            var router = pageParam.PageUrl + "/" + pageParam.OpenParam;
            AnalyticAgreement.AgreementProvider.ExecuteJavaScript(pageParam.PageId, $@"window.AppBridge.Router.push(""{router}"")");
            AnalyticAgreement.AgreementProvider.ExecuteJavaScript(pageParam.PageId, $@"window.AppBridge.SetPageId(""{pageParam.ParentPageId}"",""{pageParam.PageId}"")");

            //���ص�����
            var rlTitle = RootLayout.FindViewById<RelativeLayout>(Resource.Id.rlTitle);
            rlTitle.Visibility = ViewStates.Gone;
            //���ط��ذ�ť
            var btnReturn = RootLayout.FindViewById<LinearLayout>(Resource.Id.btnReturn);
            btnReturn.Visibility = ViewStates.Gone;
            //�����Ҳఴť
            var txtRight = RootLayout.FindViewById<TextView>(Resource.Id.txtRight);
            txtRight.Visibility = ViewStates.Gone;

            return RootLayout;
        }


        public override void OnResume()
        {
            base.OnResume();
            var webViewContainer = RootLayout.FindViewById<RelativeLayout>(Resource.Id.webViewContainer);
            X5WebViewHelper.AttachedWebView(WebViewKey, webViewContainer);
        }

        public override void OnPause()
        {
            base.OnPause();
            var webViewContainer = RootLayout.FindViewById<RelativeLayout>(Resource.Id.webViewContainer);
            X5WebViewHelper.AttachedWebView(WebViewKey, webViewContainer);
        }
    }
}