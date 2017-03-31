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
                PageName = "��Ϣ",
                PageUrl = "/chat/ChatDetail",
                AllowBack = true,
                PageId = "��Ϣ",
                ParentPageId = parentPageId
            };
            X5WebViewActivity.ShowActivity(prarm);

        }
    }
}