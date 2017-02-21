using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Widget;
using Android.OS;
using Android.Views;
using HybridApp.Chat;
using HybridApp.Main;
using Android.Runtime;
using Android.Util;
using Android.Views.InputMethods;
using Com.Tencent.Smtt.Sdk;
using HybridApp.Common;


namespace HybridApp
{
    [Activity(Label = "长帆App", MainLauncher = true, Theme = "@style/AppSplash", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, NoHistory = true)]
    public class LoadMain : Activity, QbSdk.IPreInitCallback, ITbsListener
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.LoadMainVw);
            

            QbSdk.InitX5Environment(this.ApplicationContext,this);
            QbSdk.SetTbsListener(this);

            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);

            X5WebViewHelper.PageWebViewKey = new Dictionary<string, string>();
        }

        protected override void OnPause()
        {
            base.OnPause();
            //this.Finish();
        }

        public void OnCoreInitFinished()
        {
            Console.WriteLine("OnCoreInitFinished");
        }

        public void OnViewInitFinished(bool p0)
        {
            Console.WriteLine("OnViewInitFinished " + p0);
        }

        public void OnDownloadFinish(int p0)
        {
            Console.WriteLine("OnDownloadFinish " + p0);
        }

        public void OnDownloadProgress(int p0)
        {
            Console.WriteLine("OnDownloadProgress " + p0);
        }

        public void OnInstallFinish(int p0)
        {
            Console.WriteLine("OnInstallFinish " + p0);
        }
    }
}

