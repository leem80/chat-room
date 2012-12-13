using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

namespace ZqlChartInterfaces
{
    public class UdpServer
    {
        private Thread _listenerThread;

        private List<IPEndPoint> _users = new List<IPEndPoint>();

  
        private UdpClient _udpSender = new UdpClient();

        public IPAddress ServerAddress
        {
            get;
            set;
        }

        public UdpClient UdpListener
        {
            get;
            set;
        }

        public UdpServer()
        {
            try
            {
                ServerAddress = IPAddress.Parse("127.0.0.1");
            }
            catch
            {
                throw new Exception("Configuration not set propperly. View original source code");
            }
        }

        public void Start()
        {
            UdpListener = new System.Net.Sockets.UdpClient(0);
            _listenerThread = new Thread(new ThreadStart(listen));
            _listenerThread.IsBackground = true;
            _listenerThread.Start();
        }

        private void listen()
        {
            while (true)
            {
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                byte[] received = UdpListener.Receive(ref sender);

                if (!_users.Contains(sender))
                {
                    _users.Add(sender);
                }

                foreach (IPEndPoint endpoint in _users)
                {
                    if (!endpoint.Equals(sender))
                    {
                        _udpSender.Send(received, received.Length, endpoint);
                    }
                }
            }
        }

        public void EndCall()
        {
            _listenerThread.Abort();
        }
    }
}
