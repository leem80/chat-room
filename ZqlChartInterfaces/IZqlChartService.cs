using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ZqlChartObjects;
using System.IO;

namespace ZqlChartInterfaces
{
    [
       ServiceContract
       (
           Name = "ZqlChartService",
           Namespace = "http://ZqlChart/ZqlChartService/",
           SessionMode = SessionMode.Required,
           CallbackContract = typeof(IZqlChartServiceCallback)
       )
    ]
    public interface IZqlChartService
    {
        [OperationContract()]
        bool Join(ChatUser chatUser);

        [OperationContract()]
        void Leave(ChatUser chatUser);

        [OperationContract()]
        bool IsUserNameTaken(string strUserName);

        [OperationContract()]
        void SendInkStrokes(MemoryStream memoryStream);


        [OperationContract]
        void SendBroadcastMessage(string strUserName, string message);

        [OperationContract(IsOneWay = false)]
        bool InitiateCall(string username);

        [OperationContract(IsOneWay = true)]
        void EndCall();

    }
}
