using System.Collections.Generic;
using ZqlChartObjects;
using System.ServiceModel;
using System.Net;
using ZqlChartObjects;

namespace ZqlChartInterfaces
{
    public interface IZqlChartServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateUsersList(List<ChatUser> listChatUsers);

        [OperationContract(IsOneWay = true)]
        void OnInkStrokesUpdate(ChatUser chatUser, byte[] bytesStroke);

        [OperationContract(IsOneWay = true)]
        void ServerDisconnected();
        [OperationContract(IsOneWay = true)]
        void NotifyMessage(string message);

        /// <summary>
        /// Accept call on client side
        /// </summary>
        /// <param name="username"> The caller </param>
        /// <returns> Aceepted or not </returns>
        [OperationContract(IsOneWay = false)]
        bool AcceptCall(string username);

        /// <summary>
        /// End call on client
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void EndCallClient();

        [OperationContract(IsOneWay = true)]
        void CallDetailes(IPEndPoint endpoint, string caller, string callee);

    }
   
}
