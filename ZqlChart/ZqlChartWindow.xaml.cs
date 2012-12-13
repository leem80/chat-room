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
using System.Xml;
using System.Windows.Markup;
using System.IO;
using System.Windows.Ink;
using System.Threading;
using System.ServiceModel;
using System.Configuration;

namespace ZqlChart
{
    public partial class ZqlChartWindow : Window
    {
        public static readonly DependencyProperty FillColorProperty =
           DependencyProperty.Register
           ("FillColor", typeof(Color), typeof(ZqlChartWindow),
           new PropertyMetadata(Colors.Aquamarine));
        LoginControl loginCtrl;
        ActiveCallWindow _activeCallForm;
        CallManager _callManager;
        public ZqlChartWindow()
        {
            InitializeComponent();
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/background_win.jpg"));
            b.Stretch = Stretch.Fill;
            this.Background = b;
            this.Opacity = 0.9;
             loginCtrl = new LoginControl(this);
            this.loginCanvas.Children.Add(loginCtrl);

        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ExitChat();
            base.OnClosing(e);
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SignInMode();

            this.inkCanv.DefaultDrawingAttributes.StylusTip = StylusTip.Ellipse;
            this.inkCanv.DefaultDrawingAttributes.Width = 10;
            this.inkCanv.DefaultDrawingAttributes.Height = 10;
            this.inkCanv.DefaultDrawingAttributes.Color = FillColor;
        }

        private void ShowChatControls(Visibility visibility)
        {
            this.BorderEditingType.Visibility = visibility;
            this.BorderInkCanvas.Visibility = visibility;
            this.BorderUsersList.Visibility = visibility;
            this.BorderInkMessage.Visibility = visibility;
        }

        private void SignInMode()
        {

            this.lvUsers.Items.Clear();
            this.inkCanv.Strokes.Clear();
            ShowChatControls(Visibility.Hidden);
            this.loginCanvas.Visibility = Visibility.Visible;
            this.btnLeave.Visibility = Visibility.Hidden;

        }
        
        public void ChatMode()
        {
            this.btnLeave.Visibility = Visibility.Visible;
            loginCanvas.Visibility = Visibility.Hidden;
            ShowChatControls(Visibility.Visible);
        }

        private void btnLeave_Click(object sender, RoutedEventArgs e)
        {
            ExitChat();
            this.SignInMode();
        }

        public void ServerDisconnected()
        {
            ZqlChartServiceClient.Instance = null;
            this.SignInMode();
        }

        public void CallDetailes(System.Net.IPEndPoint endpoint, string caller, string callee)
        {
            _activeCallForm = new ActiveCallWindow(caller, callee);
            _activeCallForm.Show();
            _callManager = new CallManager(endpoint);
            _callManager.Start();
        }

        public void EndCallClient()
        {
            _activeCallForm.Close();
            _callManager.Stop();
        }

        public void NotifyMessage(string message)
        {
            txtAllMessage.AppendText(message + "\r\n");
        
        }

        #region Send and receive strokes

        public void OnInkStrokesUpdate(byte[] bytesStroke)
        {
            try
            {
                System.IO.MemoryStream memoryStream = new MemoryStream(bytesStroke);
                this.inkCanv.Strokes = new StrokeCollection(memoryStream);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Title);
            }
        }

        private void SaveGesture()
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();

                this.inkCanv.Strokes.Save(memoryStream);
                   
                memoryStream.Flush();

                ZqlChartServiceClient.Instance.SendInkStrokes(memoryStream);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Title);
            }
        }

        private void inkCanv_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            SaveGesture();
        }

        private void inkCanv_StrokeErasing(object sender, InkCanvasStrokeErasingEventArgs e)
        {
            SaveGesture();
        }

        private void inkCanv_StrokeErased(object sender, RoutedEventArgs e)
        {
            SaveGesture();
        }

        private void ExitChat()
        {
            if (ZqlChartServiceClient.Instance != null)
            {
                ZqlChartServiceClient.Instance.Leave();
                ZqlChartServiceClient.Instance.Close();
                ZqlChartServiceClient.Instance = null;
            }

            if (App.s_IsServer)
            {
                App.StopServer();
            }
        }

        #endregion SendAndReceiveStrokes

        #region Update Chat Status
        public void UpdateUsersList(List<ZqlChartObjects.ChatUser> listChatUsers)
        {
            lvUsers.Items.Clear();
            foreach (ZqlChartObjects.ChatUser chatUser in listChatUsers)
            {
                lvUsers.Items.Add(chatUser.NickName);
            }
        }

        public void LastUserDraw(ZqlChartObjects.ChatUser chatUser)
        {
  
        }
        #endregion Update Chat Status

        #region Update ink

        private void OnSetFill(object sender, RoutedEventArgs e)
        {
            Microsoft.Samples.CustomControls.ColorPickerDialog cPicker = new Microsoft.Samples.CustomControls.ColorPickerDialog();
            cPicker.StartingColor = FillColor;
            cPicker.Owner = this;

            bool? dialogResult = cPicker.ShowDialog();
            if (dialogResult != null && (bool)dialogResult == true)
            {
                FillColor = cPicker.SelectedColor;
                this.inkCanv.DefaultDrawingAttributes.Color = FillColor;
            }
        }

        private Color FillColor
        {
            get
            {
                return (Color)GetValue(FillColorProperty);
            }
            set
            {
                SetValue(FillColorProperty, value);
            }
        }

        private void rbInkType_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rad = sender as RadioButton;
            this.inkCanv.EditingMode = (InkCanvasEditingMode)rad.Tag;
        }

        #endregion

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            ZqlChartServiceClient.Instance.SendBroadcastMessage(loginCtrl.UserName, txtMessage.Text);
            txtAllMessage.AppendText("我："+txtMessage.Text + "\r\n");
            txtMessage.Text = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = lvUsers.SelectedValue.ToString();
            if (!ZqlChartServiceClient.Instance.InitiateCall(username))
            {
                MessageBox.Show(String.Format("User \"{0}\" Refused call", username), "Initiate call failed");
            }
        }
    }
}
