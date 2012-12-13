using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.Threading;

namespace ZqlChart
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        private ZqlChartWindow m_mainWindow;
        public LoginControl(ZqlChartWindow mainWindow)
        {
            m_mainWindow = mainWindow;
            InitializeComponent();
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/background.jpg"));
            b.Stretch = Stretch.Fill;

            this.Background = b;
            this.Background.Opacity = 1;
        }
        public string UserName
        {
            get;
            set;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.chatTypeServer.IsChecked = true;
        }

        private void chatTypeServer_Checked(object sender, RoutedEventArgs e)
        {
            this.serverPanel.IsEnabled = false;
        }

        private void chatTypeClient_Checked(object sender, RoutedEventArgs e)
        {
            this.serverPanel.IsEnabled = true;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            EndpointAddress serverAddress;
            if (this.chatTypeServer.IsChecked == true)
            {
                ZqlChart.App.s_IsServer = true;
                serverAddress = new EndpointAddress("net.tcp://localhost:8000/ZqlChartService/service");
            }
            else
            {
                ZqlChart.App.StopServer();
                ZqlChart.App.s_IsServer = false;
                if (txtServer.Text.Length == 0)
                {
                    MessageBox.Show("Please enter server name");
                    return;
                }
                serverAddress = new EndpointAddress(string.Format("net.tcp://{0}:8000/ZqlChartService/service", txtServer.Text));
            }

            if (txtUserName.Text.Length == 0)
            {
                MessageBox.Show("Please enter username");
                return;
            }

            if (ZqlChartServiceClient.Instance == null)
            {
                UserName = txtUserName.Text;
                if (App.s_IsServer)
                {
                    ZqlChart.App.StartServer();
                }

                try
                {
                    ClientCallBack.Instance = new ClientCallBack(SynchronizationContext.Current, m_mainWindow);
                    ZqlChartServiceClient.Instance = new ZqlChartServiceClient
                                                    (
                                                        new ZqlChartObjects.ChatUser
                                                        (
                                                            txtUserName.Text,
                                                            System.Environment.UserName,
                                                            System.Environment.MachineName,
                                                            System.Diagnostics.Process.GetCurrentProcess().Id,
                                                            App.s_IsServer
                                                        ),
                                                        new InstanceContext(ClientCallBack.Instance),
                                                        "ZqlChartClientTcpBinding",
                                                        serverAddress
                                                    );
                    ZqlChartServiceClient.Instance.Open();
                    m_mainWindow.Title = m_mainWindow.Title+":"+UserName;
                }
                catch (System.Exception ex)
                {
                    ZqlChart.App.StopServer();
                    ZqlChartServiceClient.Instance = null;
                    MessageBox.Show(string.Format("Failed to connect to chat server, {0}", ex.Message),this.m_mainWindow.Title);
                    return;
                }
            }

            if (ZqlChartServiceClient.Instance.IsUserNameTaken(ZqlChartServiceClient.Instance.ChatUser.NickName))
            {
                ZqlChartServiceClient.Instance = null;
                MessageBox.Show("Username is already in use");
                return;
            }

            if (ZqlChartServiceClient.Instance.Join() == false)
            {
                MessageBox.Show("Failed to join chat room");
                ZqlChartServiceClient.Instance = null;
                ZqlChart.App.StopServer();
                return;
            }

            this.m_mainWindow.ChatMode();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.m_mainWindow.Close();
        }

    }
}
