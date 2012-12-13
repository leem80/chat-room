using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZqlChartObjects;
namespace ZqlChartInterfaces
{
    public class ClientCallBack
    {
        /// <summary>
        /// The callback to own client instance
        /// </summary>
        public IZqlChartServiceCallback Client
        {
            get;
            set;
        }

        /// <summary>
        /// The upd server for the call. 
        /// Note: this will assigned to the call initiator (caller)
        /// </summary>
        public UdpServer UdpCallServer
        {
            get;
            set;
        }

        /// <summary>
        /// The username of the client
        /// </summary>
        public ChatUser JoinChatUser
        {
            get;
            set;
        }

        /// <summary>
        /// The person who
        /// </summary>
        public IZqlChartServiceCallback Callee
        {
            get;
            set;
        }
    }
}
