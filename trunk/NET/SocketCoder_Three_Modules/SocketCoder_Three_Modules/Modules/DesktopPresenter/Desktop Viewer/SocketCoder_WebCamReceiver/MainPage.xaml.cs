using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.IO;
using System.Windows.Media.Imaging;

namespace SocketCoder_WebCamReceiver
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }


        #region Global Declarations
        private Socket client_socket;
        private delegate void mydelegate(byte[] buffer);
        private delegate void ShowMessagedelegate(string MSG);
        #endregion Global Declarations
        #region Socket Methods
        void Conncet(string IP_Address)
        {

            client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs()
            {
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse(IP_Address), 4532)
            };
            socketEventArg.Completed += OnConncetCompleted;
            client_socket.ConnectAsync(socketEventArg);
        }
        void StartReceiving()
        {
            byte[] response = new byte[131072];
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.Completed += OnReceiveCompleted;
            socketEventArg.SetBuffer(response, 0, response.Length);
            client_socket.ReceiveAsync(socketEventArg);
        }
        void OnConncetCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                this.Dispatcher.BeginInvoke(new ShowMessagedelegate(ShowMessageBox), "Connceted Successfully!");
                StartReceiving();
            }
            else
            {
                this.Dispatcher.BeginInvoke(new ShowMessagedelegate(ShowMessageBox), e.SocketError.ToString());
            }

        }
        private void ShowMessageBox(string MSG)
        {
            MessageBox.Show(MSG);
        }
        void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new mydelegate(ViewReceivedImage), e.Buffer);
        }
         #endregion Socket Methods
        #region Decoding Methods
        private void ViewReceivedImage(byte[] buffer)
        {
            try
            {
                MemoryStream ms = new MemoryStream(buffer);
                BitmapImage bi = new BitmapImage();
                bi.SetSource(ms);
                MyImage.Source = bi;
                ms.Close();
            }
            catch (Exception) { }
            finally
            {
                StartReceiving();
            }
        }
        #endregion Decoding Methods

        private void ConnectBTN_Click(object sender, RoutedEventArgs e)
        {
            Conncet(IPAddress_TXT.Text);
        }
    }
}
