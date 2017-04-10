using Android.App;
using HybridAndroid.Common;
using HybridCommon.Agreement;

namespace HybridAndroid.Chat
{
    public class ChatMain
    {
        public ChatMain(string parentPageId)
        {

            var prarm = new PageParam
            {
                IsRelative = false,
                PageName = "≤‚ ‘",
                PageUrl = "/Test",
                AllowBack = true,
                PageId = "≤‚ ‘",
                ParentPageId = parentPageId
            };
            X5WebViewActivity.ShowActivity(prarm);

        }
    }
}