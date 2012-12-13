using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZqlChartObjects;

namespace ZqlChart
{
    partial class ZqlChartServiceClient
    {
        public static ZqlChartServiceClient Instance = null;

        public ZqlChartServiceClient
        (
            ChatUser chatUser,
            System.ServiceModel.InstanceContext callbackInstance, 
            string strEndPointConfigurationName, 
            System.ServiceModel.EndpointAddress remoteAddress
        ) :
            base(callbackInstance, strEndPointConfigurationName, remoteAddress)
        {
            m_chatUser = chatUser;
        }
    
        ChatUser m_chatUser;
        public ChatUser ChatUser
        {
            get { return m_chatUser; }
        }

        public bool Join()
        {
            if (m_chatUser == null)
            {
                return false;
            }
            return Join(m_chatUser);
        }

        public void Leave()
        {
            if (m_chatUser == null)
            {
                return;
            }
            Leave(m_chatUser);
        }
    }
}
