using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using ZqlChartObjects;
using System.IO;
using ZqlChartInterfaces;
using System.Net;
namespace ZqlChartServer
{
    delegate void EmtpyDelegate();
    [
        ServiceBehavior
        (
            ConcurrencyMode = ConcurrencyMode.Single, 
            InstanceContextMode = InstanceContextMode.Single
        )
    ]
    public class ZqlChartService : IZqlChartService
    {
        public static Dictionary<IZqlChartServiceCallback, ClientCallBack> s_dictCallbackToUser = new Dictionary<IZqlChartServiceCallback, ClientCallBack>();


        public ZqlChartService()
        { 
        }

        public bool Join(ChatUser chatUser)
        {
            if (s_dictCallbackToUser.Where(p => p.Value.JoinChatUser.NickName == chatUser.NickName).Count() == 0)
            {

                IZqlChartServiceCallback callbackChannel = OperationContext.Current.GetCallbackChannel<IZqlChartServiceCallback>();
                s_dictCallbackToUser.Add(callbackChannel, new ClientCallBack() { JoinChatUser = chatUser, Client = callbackChannel });

                foreach (IZqlChartServiceCallback callbackClient in s_dictCallbackToUser.Keys)
                {
                    callbackClient.UpdateUsersList(s_dictCallbackToUser.Values.Select(p => p.JoinChatUser).ToList());
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Leave(ChatUser chatUser)
        {
            IZqlChartServiceCallback client = OperationContext.Current.GetCallbackChannel<IZqlChartServiceCallback>();
            if (s_dictCallbackToUser.ContainsKey(client))
            {
                s_dictCallbackToUser.Remove(client);
            }

            foreach (IZqlChartServiceCallback callbackClient in s_dictCallbackToUser.Keys)
            {
                if (chatUser.IsServer)
                {
                    if (callbackClient != client)
                    {
                        //server user logout, disconnect clients
                        callbackClient.ServerDisconnected();
                    }
                }
                else
                {
                    //normal user logout
                    callbackClient.UpdateUsersList(s_dictCallbackToUser.Values.Select(p => p.JoinChatUser).ToList());
                }
            }

            if (chatUser.IsServer)
            {
                s_dictCallbackToUser.Clear();
            }
        }

        public bool IsUserNameTaken(string strNickName)
        {
            foreach (ChatUser chatUser in s_dictCallbackToUser.Values.Select(p => p.JoinChatUser).ToList())
            {
                if (chatUser.NickName.ToUpper().CompareTo(strNickName) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void SendInkStrokes(MemoryStream memoryStream)
        {
            IZqlChartServiceCallback client = OperationContext.Current.GetCallbackChannel<IZqlChartServiceCallback>();
            
            foreach (IZqlChartServiceCallback callbackClient in s_dictCallbackToUser.Keys)
            {
                if (callbackClient != OperationContext.Current.GetCallbackChannel<IZqlChartServiceCallback>())
                {
                    callbackClient.OnInkStrokesUpdate(s_dictCallbackToUser[client].JoinChatUser, memoryStream.GetBuffer());
                }
            }
        }

        public void SendBroadcastMessage(string clientName, string message)
        {
            IZqlChartServiceCallback client =
                OperationContext.Current.GetCallbackChannel<IZqlChartServiceCallback>();

            if (client != null)
            {

                foreach (IZqlChartServiceCallback callbackClient in s_dictCallbackToUser.Keys)
                {
                    if (callbackClient != OperationContext.Current.GetCallbackChannel<IZqlChartServiceCallback>())
                    {
                        callbackClient.NotifyMessage(clientName + ": " + message);
                    }
                }
            }
        }

        public bool InitiateCall(string username)
        {
            ClientCallBack clientCaller = s_dictCallbackToUser[OperationContext.Current.GetCallbackChannel<IZqlChartServiceCallback>()];
            ClientCallBack clientCalee = s_dictCallbackToUser.Values.Where(p => p.JoinChatUser.NickName == username).First();

            if (clientCaller.Callee != null || clientCalee.Callee != null) // callee or caller is in another call
            {
                return false;
            }

            if (clientCaller == clientCalee)
            {
                return false;
            }

            if (clientCalee.Client.AcceptCall(clientCaller.JoinChatUser.NickName))
            {
                clientCaller.Callee = clientCalee.Client;

                clientCalee.Callee = clientCaller.Client;

                clientCaller.UdpCallServer = new UdpServer();
                clientCaller.UdpCallServer.Start();

                EmtpyDelegate separateThread = delegate() // code need to  run a separate thread to avoid deadlock
                {
                    IPEndPoint endpoint = new IPEndPoint(clientCaller.UdpCallServer.ServerAddress,
                        ((IPEndPoint)clientCaller.UdpCallServer.UdpListener.Client.LocalEndPoint).Port);

                    clientCalee.Client.CallDetailes(endpoint, clientCaller.JoinChatUser.NickName, username);
                    clientCaller.Client.CallDetailes(endpoint, clientCaller.JoinChatUser.NickName, username);

                    foreach (var callback in s_dictCallbackToUser.Keys)
                    {
                        callback.NotifyMessage(String.Format("System:User \"{0}\" and user \"{1}\" have started a call",clientCaller.JoinChatUser.NickName, username));
                    }
                };
                separateThread.BeginInvoke(null, null);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void EndCall()
        {
            ClientCallBack clientCaller = s_dictCallbackToUser[OperationContext.Current.GetCallbackChannel<IZqlChartServiceCallback>()];
            ClientCallBack ClientCalee = s_dictCallbackToUser[clientCaller.Callee];

            if (clientCaller.UdpCallServer != null)
            {
                clientCaller.UdpCallServer.EndCall();
            }

            if (ClientCalee.UdpCallServer != null) 
            {
                ClientCalee.UdpCallServer.EndCall();
            }

            if (clientCaller.Callee != null)
            {
                foreach (var callback in s_dictCallbackToUser.Keys)
                {
                    callback.NotifyMessage(String.Format("System:User \"{0}\" and user \"{1}\" have ended the call", clientCaller.JoinChatUser.NickName, ClientCalee.JoinChatUser.NickName));
                }

                clientCaller.Callee.EndCallClient();
                clientCaller.Callee = null;
            }

            if (ClientCalee.Callee != null)
            {
                ClientCalee.Callee.EndCallClient();
                ClientCalee.Callee = null;
            }
        }

    }



}
