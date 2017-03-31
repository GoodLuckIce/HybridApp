using System;
using System.Collections.Generic;
using System.Linq;
using HybridCommon.Context;
using HybridCommon.SqlLite;
using HybridCommon.SqlLite.Model;

namespace HybridCommon.Utils
{
    public class AppLogHelper
    {

        public static void Init()
        {
            if (DeviceInfo.IsFirstLoad)
            {
                var db = DbConHelper.NewDbCon();
                db.CreateTable<AppLog>();
            }
            else
            {
            }
        }

        public static void AddAppLog(AppLog model)
        {
            var db = DbConHelper.NewDbCon();
            model.CreateTime = DateTime.Now.Ticks;
            db.Insert(model);
        }

        public static List<AppLog> GetAppLog()
        {
            var db = DbConHelper.NewDbCon();
            var result = db.Table<AppLog>().OrderByDescending(p => p.CreateTime).ToList();
            return result;
        }
    }
}
