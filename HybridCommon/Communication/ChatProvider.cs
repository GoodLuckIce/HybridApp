using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client;

namespace HybridCommon.Communication
{
    public class ChatProvider
    {
        public static void ChatInit()
        {

            var hubConnection = new HubConnection("http://192.168.2.103:801/");
            var chatHubProxy = hubConnection.CreateHubProxy("ChatHub");
            chatHubProxy.On<string, string>("broadcastMessage", (name, message) =>
            {

            });

            chatHubProxy.On<Guid, int>("getChatMsgLogList", GetChatMsgLogList);
            chatHubProxy.On< Guid , byte , byte , string> ("sendMsg", SendMsg);
            chatHubProxy.On<Guid>("setIsRead", SetIsRead);
            chatHubProxy.On<bool>("getFirend", GetFirend);

            hubConnection.Received += s =>
            {
                //在连接上接收到任何数据时触发。 提供接收的数据
            };
            hubConnection.ConnectionSlow += () =>
            {
                //在客户端检测到连接缓慢或频繁中断时触发
            };
            hubConnection.Reconnecting += () =>
            {
                //在底层传输开始重新连接时引发
            };
            hubConnection.Reconnected += () =>
            {
                //在底层传输已重新连接时引发
            };
            hubConnection.StateChanged += async change =>
            {
                if (change.NewState == ConnectionState.Connected)
                {
                    var result = await chatHubProxy.Invoke<dynamic>("Send", "JohnDoe", "Hello");
                    chatHubProxy.Invoke("Send", "q1q", result.name.ToString());
                }
                //在连接状态更改时触发。 提供旧状态和新状态
            };
            hubConnection.Error += exception =>
            {

            };
            // Start the connection
            hubConnection.Start();
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="receiverId">接收方Id(根据接收方类型判断是用户或者是讨论组/群</param>
        /// <param name="receiverType">接收方类型(EnumReceiverType:用户,讨论组/群)</param>
        /// <param name="dataType">记录类型(记录类型(EnumDataType:文本,文件/图片))</param>
        /// <param name="content">消息内容如果是文件则必须是FileModel的Json</param>
        public static void SendMsg(Guid receiverId, byte receiverType, byte dataType, string content)
        {
        }

        /// <summary>
        /// 设置已读
        /// </summary>
        /// <param name="userIdOrGroupId">用户Id或者讨论组Id</param>
        /// <returns></returns>
        public static void SetIsRead(Guid userIdOrGroupId)
        {
        }

        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <param name="isContacts">是否当前联系人</param>
        /// <returns></returns>
        public static void GetFirend(bool isContacts)
        {
        }

        /// <summary>
        /// 获取好友分组
        /// </summary>
        /// <returns></returns>
        public static void GetFirendGroup()
        {
        }

        /// <summary>
        /// 分页获取聊天记录
        /// </summary>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public static void GetChatMsgLogList(Guid userIdOrGroupId, int currentPage)
        {
        }

        /// <summary>
        /// 获取聊天记录
        /// </summary>
        /// <param name="userIdOrGroupId"></param>
        /// <param name="searchType"></param>
        /// <param name="searchContent"></param>
        /// <param name="searchDateTime"></param>
        /// <returns></returns>
        public static void GetChatMsgLogList(Guid userIdOrGroupId, int searchType, string searchContent, DateTime searchDateTime)
        {
        }

        /// <summary>
        /// 获取用户或讨论组的未读消息数量
        /// </summary>
        /// <param name="userIdOrGroupId"></param>
        /// <returns></returns>
        public static void GetUnreadNum(List<Guid> userIdOrGroupId)
        {
        }
    }
}