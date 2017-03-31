using HybridCommon.Context;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;
using SQLite.Net.Platform.Generic;

namespace HybridCommon.SqlLite
{
    public class DbConHelper
    {
        private class SqLitePlatform : ISQLitePlatform
        {
            public ISQLiteApi SQLiteApi { get; set; }
            public IStopwatchFactory StopwatchFactory { get; set; }
            public IReflectionService ReflectionService { get; set; }
            public IVolatileService VolatileService { get; set; }
        }

        private static SqLitePlatform _sqLitePlatformNeiBu { get; set; }
        private static SqLitePlatform _sqLitePlatform
        {
            get
            {
                if (_sqLitePlatformNeiBu == null)
                {
                    _sqLitePlatformNeiBu = new SqLitePlatform
                    {
                        SQLiteApi = new SQLiteApiGeneric(),
                        StopwatchFactory = new StopwatchFactoryGeneric(),
                        ReflectionService = new ReflectionServiceGeneric(),
                        VolatileService = new VolatileServiceGeneric()
                    };
                }
                return _sqLitePlatformNeiBu;
            }
        }

        public static SQLiteConnection NewDbCon()
        {
            return NewDbCon(Config.DataBaseName);
        }

        public static SQLiteConnection NewDbCon(string dataBaseName)
        {
            if (!string.IsNullOrWhiteSpace(dataBaseName) && !string.IsNullOrWhiteSpace(DeviceInfo.RootFolder))
            {
                return new SQLiteConnection(_sqLitePlatform, DeviceInfo.RootFolder + "/" + dataBaseName);
            }
            return null;
        }

        public static SQLiteAsyncConnection NewDbAsyncCon()
        {
            return NewDbAsyncCon(Config.DataBaseName);
        }

        public static SQLiteAsyncConnection NewDbAsyncCon(string dataBaseName)
        {
            if (!string.IsNullOrWhiteSpace(dataBaseName) && !string.IsNullOrWhiteSpace(DeviceInfo.RootFolder))
            {
                return new SQLiteAsyncConnection((() => { return new SQLiteConnectionWithLock(_sqLitePlatform, new SQLiteConnectionString(DeviceInfo.RootFolder + "/" + dataBaseName, false)); }));
            }
            return null;
        }
    }
}
