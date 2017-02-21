using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using HybridApp.Chat;

namespace HybridApp.Common
{
    public class CommonFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            var myInflater = (LayoutInflater)Activity.GetSystemService(Context.LayoutInflaterService);
            var layout = myInflater.Inflate(Resource.Layout.CommonFragmentVw, container, false);

            //接收传来的参数
            if (this.Arguments != null)
            {
                var url = this.Arguments.GetString("url");
                var webView = layout.FindViewById<WebView>(Resource.Id.webView);
                WebViewHelper.WebViewInit(this.Activity, webView, url);
            }
            return layout;
        }
        
    }
}