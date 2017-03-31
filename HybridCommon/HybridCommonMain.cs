using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HybridCommon.Agreement;
using HybridCommon.Context;
using HybridCommon.HttpClientHelper;
using HybridCommon.SqlLite;
using HybridCommon.SqlLite.Model;
using HybridCommon.Utils;
using Plugin.Connectivity;
using Plugin.DeviceInfo;
using Plugin.DeviceInfo.Abstractions;
using Plugin.Geolocator;
using Plugin.LocalNotifications;
using Plugin.Media;

namespace HybridCommon
{
    public class HybridCommonMain
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="agreement">协议</param>
        /// <param name="terminalType">终端类型:0=拼箱端</param>
        public static ResultObj Init(IAgreementProvider agreement, int terminalType)
        {
            AnalyticAgreement.AgreementProvider = agreement;

            DeviceInfo.TerminalType = terminalType;
            DeviceInfo.IsFirstLoad = true;
            DeviceInfo.RootFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string dataBasePath = Path.Combine(DeviceInfo.RootFolder, Config.DataBaseName);
            if (File.Exists(dataBasePath))
            {
                DeviceInfo.IsFirstLoad = false;
            }

            //初始化AppLog
            //AppLogHelper.Init();

            if (DeviceInfo.IsFirstLoad)
            {
                //如果第一次启动则创建数据库表
                var db = DbConHelper.NewDbCon();
                db.CreateTable<UserCertificate>();
                db.CreateTable<UserConfigure>();
            }
            else
            {
                //todo:检查用户状态是否正常
            }


            //var ss = CrossDeviceInfo.Current.Platform;
            //var sse = CrossConnectivity.Current;
            //var sese = CrossGeolocator.Current;
            //sese.AllowsBackgroundUpdates = true;
            //sese.StartListeningAsync(1, 90);
            //var a = sese.GetPositionAsync();
            //var sesea = a.Result;

            //sese.PositionChanged += (sender, args) =>
            //{
            //    var sse111 = args.Position;
            //    CrossLocalNotifications.Current.Show("坐标", sse111.Latitude + "," + sse111.Longitude, 0, DateTime.Now.AddSeconds(10));
            //};
            //var sesaae = CrossMedia.Current;
            return ResultObj.Ok();
        }

    }
}
