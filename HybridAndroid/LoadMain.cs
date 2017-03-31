using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Com.Tencent.Smtt.Sdk;
using HybridAndroid.Common;
using HybridCommon;
using HybridCommon.Agreement;
using HybridCommon.Context;
using Plugin.DeviceInfo;
using Object = Java.Lang.Object;

namespace HybridAndroid
{
    [Activity(Label = "长帆App", MainLauncher = true, Theme = "@style/AppSplash", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, NoHistory = true)]
    public class LoadMain : Activity, QbSdk.IPreInitCallback, ITbsListener
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.LoadMainVw);


            QbSdk.InitX5Environment(this.ApplicationContext, this);
            QbSdk.SetTbsListener(this);

            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            X5WebViewHelper.PageParam = new Dictionary<string, PageParam>();
            X5WebViewHelper.PageActivity = new Dictionary<string, Activity>();

            //异步初始化操作
            var th = new Thread((() => { HybridCommonMain.Init(new AgreementProvider(), 0); }));
            th.Start();
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

