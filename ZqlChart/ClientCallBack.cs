using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using ZqlChartInterfaces;
using System.Windows;
namespace ZqlChart
{
    [
        CallbackBehavior
        (
            ConcurrencyMode = ConcurrencyMode.Single,
            UseSynchronizationContext = false
        )
    ]
    public class ClientCallBack : IZqlChartServiceCallback
    {
        public static ClientCallBack Instance;
        private SynchronizationContext m_uiSyncContext = null;
        private ZqlChartWindow m_mainWindow;

        public ClientCallBack(SynchronizationContext uiSyncContext, ZqlChartWindow mainWindow)
        {
            m_uiSyncContext = uiSyncContext;
            m_mainWindow = mainWindow;
        }

        public void OnInkStrokesUpdate(ZqlChartObjects.ChatUser chatUser, byte[] bytesStroke)
        {
            SendOrPostCallback callback =
                      delegate(object state)
                      {
                          m_mainWindow.OnInkStrokesUpdate(state as byte[] );
                      };

            m_uiSyncContext.Post(callback, bytesStroke);

            SendOrPostCallback callback2 =
                      delegate(object objchatUser)
                      {
                          m_mainWindow.LastUserDraw(objchatUser as ZqlChartObjects.ChatUser);
                      };
            m_uiSyncContext.Post(callback2, chatUser);
        }

        public void UpdateUsersList(List<ZqlChartObjects.ChatUser> listChatUsers)
        {
            SendOrPostCallback callback =
                     delegate(object objListChatUsers)
                     {
                         m_mainWindow.UpdateUsersList(objListChatUsers as List<ZqlChartObjects.ChatUser>);
                     };

            m_uiSyncContext.Post(callback, listChatUsers);
        }

        public void ServerDisconnected()
        {
            SendOrPostCallback callback =
                        delegate(object dummy)
                        {
                            m_mainWindow.ServerDisconnected();
                        };

            m_uiSyncContext.Post(callback, null);
        }
        public void NotifyMessage(string message)
        {
            SendOrPostCallback callback =
                              delegate(object dummy)
                              {
                                  m_mainWindow.NotifyMessage(message);
                              };

            m_uiSyncContext.Post(callback, message);
        }


        public bool AcceptCall(string username)
        {
            //调用线程必须为 STA，因为许多 UI 组件都需要。
            return MessageBox.Show(String.Format("Accep call from \"{0}\" ", username), "Incomming Call", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes;
        }

        /// <summary>
        /// End call on client
        /// </summary>

        public void EndCallClient()
        {
            SendOrPostCallback callback =
                      delegate(object dummy)
                      {
                          m_mainWindow.EndCallClient();
                      };

            m_uiSyncContext.Post(callback,null);
        }

        public void CallDetailes(System.Net.IPEndPoint endpoint, string caller, string callee)
        {

            SendOrPostCallback callback =
          delegate(object dummy)
          {
              m_mainWindow.CallDetailes(endpoint, caller, callee);
          };

            m_uiSyncContext.Post(callback,null);
        }



    }
}
