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
using System.Windows.Shapes;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZqlChart
{
    /// <summary>
    /// ActiveCallWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ActiveCallWindow : Window
    {

        private DateTime _startTime;
        public ActiveCallWindow()
        {
            InitializeComponent();
        }
        public ActiveCallWindow(string caller, string callee)
		{
	
			InitializeComponent();
            txtCaller.Text = caller;
            txtCallee.Text = callee;
		}

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            ZqlChartServiceClient.Instance.EndCall(); 
        }
        System.Windows.Threading.DispatcherTimer timer;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _startTime = DateTime.Now;
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);   //间隔1秒
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            TimeSpan duration = DateTime.Now - _startTime;
            lbTimer.Content = String.Format("{0}:{1}:{2}", duration.Hours.ToString("00"), duration.Minutes.ToString("00"), duration.Seconds.ToString("00"));
      
        }
        private void timerCallDuration_Tick(object sender, EventArgs e)
        {

        }

    }
}
