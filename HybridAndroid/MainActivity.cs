using System;
using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using HybridAndroid.Main;


namespace HybridAndroid
{
    [Activity(Label = "", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity, BottonBar.OnItemChangedListener
    {
        public static Activity thisActivity { get; set; }
        private List<Fragment> FragmentList { get; set; }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.MainVw);
            thisActivity = this;
            //防止键盘挡住输入框
            //不希望遮挡设置activity属性 android: windowSoftInputMode = "adjustPan"
            //希望动态调整高度 android:windowSoftInputMode = "adjustResize"
            //Window.SetSoftInputMode(SoftInput.StateVisible | SoftInput.AdjustResize);

            FragmentList = new List<Fragment>();
            FragmentList.Add(HomeMain.GetThis);
            FragmentList.Add(OrderMain.GetThis);
            FragmentList.Add(UserMain.GetThis);
            var ft = FragmentManager.BeginTransaction();
            foreach (var item in FragmentList)
            {
                ft.Add(Resource.Id.details, item);
            }
            ft.Commit();

            var bottomBar = (BottonBar)FindViewById(Resource.Id.ll_bottom_bar);
            bottomBar.SetOnItemChangedListener(this);
            bottomBar.SetSelectedState(0);

            //Task.Run(() =>
            //{
            //    ChatProvider.ChatInit();
            //});
        }

        public void onItemChanged(int index)
        {
            //var fragmentcontent = FragmentManager.FindFragmentById(Resource.Id.fragmentcontent);

            var ft = FragmentManager.BeginTransaction();
            for (int i = 0; i < FragmentList.Count; i++)
            {
                var fa = FragmentList[i];
                if (index == i)
                {
                    ft.Show(fa);
                }
                else
                {
                    ft.Hide(fa);
                }
            }

            ft.SetTransition(FragmentTransit.None);
            ft.Commit();

        }

        DateTime _lastBackKeyDownTime;
        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                if (DateTime.Now - _lastBackKeyDownTime > new TimeSpan(0, 0, 2))
                {
                    Toast.MakeText(this.ApplicationContext, "再按一次退出程序", ToastLength.Short).Show();
                    _lastBackKeyDownTime = DateTime.Now;
                }
                else
                {
                    Process.KillProcess(Process.MyPid());
                }
                return true;
            }
            else
            {
                return base.OnKeyDown(keyCode, e);
            }

        }

    }
}

