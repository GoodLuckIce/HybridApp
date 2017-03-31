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
                //�������Ͻ��յ��κ�����ʱ������ �ṩ���յ�����
            };
            hubConnection.ConnectionSlow += () =>
            {
                //�ڿͻ��˼�⵽���ӻ�����Ƶ���ж�ʱ����
            };
            hubConnection.Reconnecting += () =>
            {
                //�ڵײ㴫�俪ʼ��������ʱ����
            };
            hubConnection.Reconnected += () =>
            {
                //�ڵײ㴫������������ʱ����
            };
            hubConnection.StateChanged += async change =>
            {
                if (change.NewState == ConnectionState.Connected)
                {
                    var result = await chatHubProxy.Invoke<dynamic>("Send", "JohnDoe", "Hello");
                    chatHubProxy.Invoke("Send", "q1q", result.name.ToString());
                }
                //������״̬����ʱ������ �ṩ��״̬����״̬
            };
            hubConnection.Error += exception =>
            {

            };
            // Start the connection
            hubConnection.Start();
        }


        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="receiverId">���շ�Id(���ݽ��շ������ж����û�������������/Ⱥ</param>
        /// <param name="receiverType">���շ�����(EnumReceiverType:�û�,������/Ⱥ)</param>
        /// <param name="dataType">��¼����(��¼����(EnumDataType:�ı�,�ļ�/ͼƬ))</param>
        /// <param name="content">��Ϣ����������ļ��������FileModel��Json</param>
        public static void SendMsg(Guid receiverId, byte receiverType, byte dataType, string content)
        {
        }

        /// <summary>
        /// �����Ѷ�
        /// </summary>
        /// <param name="userIdOrGroupId">�û�Id����������Id</param>
        /// <returns></returns>
        public static void SetIsRead(Guid userIdOrGroupId)
        {
        }

        /// <summary>
        /// ��ȡ�����б�
        /// </summary>
        /// <param name="isContacts">�Ƿ�ǰ��ϵ��</param>
        /// <returns></returns>
        public static void GetFirend(bool isContacts)
        {
        }

        /// <summary>
        /// ��ȡ���ѷ���
        /// </summary>
        /// <returns></returns>
        public static void GetFirendGroup()
        {
        }

        /// <summary>
        /// ��ҳ��ȡ�����¼
        /// </summary>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public static void GetChatMsgLogList(Guid userIdOrGroupId, int currentPage)
        {
        }

        /// <summary>
        /// ��ȡ�����¼
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
        /// ��ȡ�û����������δ����Ϣ����
        /// </summary>
        /// <param name="userIdOrGroupId"></param>
        /// <returns></returns>
        public static void GetUnreadNum(List<Guid> userIdOrGroupId)
        {
        }
    }
}