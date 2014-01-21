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
using System.Windows.Media.Imaging;
using System.IO;

using FluxJpeg.Core;
using FluxJpeg.Core.Encoder;
using System.Net.Sockets;

namespace SocketCoder_WebCamSender
{

    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            saveFileDlg = new SaveFileDialog { DefaultExt = ".jpg", Filter = "JPEG Images (*jpeg *.jpg)|*.jpeg;*.jpg", };
        }

        #region Global Declarations
        private CaptureSource _capture = new CaptureSource();
        private SaveFileDialog saveFileDlg;
        private Socket client_socket;
        private delegate void mydelegate(byte[] buffer);
        private delegate void ShowMessagedelegate(string MSG);
        private delegate void Enabledelegate(bool value);
        private bool StartSending = false;

        #endregion Global Declarations

        #region Socket Methods
        void Conncet(string IP_Address)
        {

            client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs()
            {
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse(IP_Address), 4531)
            };
            socketEventArg.Completed += OnConncetCompleted;
            client_socket.ConnectAsync(socketEventArg);
        }

        void Send_Bytes(byte[] buffer)
        {

            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.Completed += OnSendCompleted;
            socketEventArg.SetBuffer(buffer, 0, buffer.Length);
            client_socket.SendAsync(socketEventArg);

        }
        void StartReceiving()
        {
            byte[] response = new byte[1280000];
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
                this.Dispatcher.BeginInvoke(new Enabledelegate(EnableControl), true);
                StartReceiving();
            }
            else
            {
                this.Dispatcher.BeginInvoke(new ShowMessagedelegate(ShowMessageBox), e.SocketError.ToString());
                this.Dispatcher.BeginInvoke(new Enabledelegate(EnableControl), false);
            }

        }
        void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            // Show Message or something ...
            //this.Dispatcher.BeginInvoke(new ShowMessagedelegate(ShowMessageBox), "Sent Successfully!");
        }
        void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new mydelegate(ViewReceivedImage), e.Buffer);
        }
        #endregion Socket Methods

        #region Encoding/Decoding Methods
        public static void EncodeJpeg(WriteableBitmap bmp, Stream dstStream)
        {
            // Init buffer in FluxJpeg format
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int[] p = bmp.Pixels;
            byte[][,] pixelsForJpeg = new byte[3][,]; // RGB colors
            pixelsForJpeg[0] = new byte[w, h];
            pixelsForJpeg[1] = new byte[w, h];
            pixelsForJpeg[2] = new byte[w, h];

            // Copy WriteableBitmap data into buffer for FluxJpeg
            int i = 0;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int color = p[i++];
                    pixelsForJpeg[0][x, y] = (byte)(color >> 16); // R
                    pixelsForJpeg[1][x, y] = (byte)(color >> 8);  // G
                    pixelsForJpeg[2][x, y] = (byte)(color);       // B
                }
            }
            //Encode Image as JPEG
            var jpegImage = new FluxJpeg.Core.Image(new ColorModel { colorspace = ColorSpace.RGB }, pixelsForJpeg);
            var encoder = new JpegEncoder(jpegImage, 95, dstStream);
            encoder.Encode();
        }
        public static WriteableBitmap DecodeJpeg(Stream srcStream)
        {
            // Decode JPEG
            var decoder = new FluxJpeg.Core.Decoder.JpegDecoder(srcStream);
            var jpegDecoded = decoder.Decode();
            var img = jpegDecoded.Image;
            img.ChangeColorSpace(ColorSpace.RGB);

            // Init Buffer
            int w = img.Width;
            int h = img.Height;
            var result = new WriteableBitmap(w, h);
            int[] p = result.Pixels;
            byte[][,] pixelsFromJpeg = img.Raster;
            // Copy FluxJpeg buffer into WriteableBitmap
            int i = 0;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    p[i++] = (0xFF << 24) // A
                                | (pixelsFromJpeg[0][x, y] << 16) // R
                                | (pixelsFromJpeg[1][x, y] << 8)  // G
                                | pixelsFromJpeg[2][x, y];       // B
                }
            }

            return result;
        }
        private void SaveSnapshot(Stream dstStream)
        {
            try
            {
                // Encoding The Snapshot
                WriteableBitmap bmp = new WriteableBitmap(rectVideo, null);
                EncodeJpeg(bmp, dstStream);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error saving snapshot", MessageBoxButton.OK);
            }
        }
        private void StartEncoding()
        {
            try
            {
                WriteableBitmap bmp = new WriteableBitmap(rectVideo, null);
                MemoryStream ms = new MemoryStream();
                EncodeJpeg(bmp, ms);
                Send_Bytes(ms.GetBuffer());
            }
            catch (Exception) { }
        }
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
        #endregion Encoding/Decoding Methods

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Start The Camera
            if (_capture != null)
            {
                _capture.Stop();
                _capture.VideoCaptureDevice = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();
                VideoBrush videoBrush = new VideoBrush();
                videoBrush.SetSource(_capture);
                rectVideo.Fill = videoBrush;
                if (CaptureDeviceConfiguration.AllowedDeviceAccess || CaptureDeviceConfiguration.RequestDeviceAccess())
                {
                    _capture.Start();
                }
            }
        }
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            // Take Snapshot
            if (saveFileDlg.ShowDialog().Value)
            {
                using (Stream dstStream = saveFileDlg.OpenFile())
                {
                    SaveSnapshot(dstStream);
                }
            }
        }
        private void ConnectBTN_Click(object sender, RoutedEventArgs e)
        {
            Conncet(IPAddress_TXT.Text);
            StartTimer(sender, e);
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            StartEncoding();
        }

        // Timer
        public void StartTimer(object o, RoutedEventArgs sender)
        {
            System.Windows.Threading.DispatcherTimer myDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000); 
            myDispatcherTimer.Tick += new EventHandler(Each_Tick);
            myDispatcherTimer.Start();
        }
        public void Each_Tick(object o, EventArgs sender)
        {
            if (StartSending)
            {
                StartEncoding();
            }
        }
        //
        private void ShowMessageBox(string MSG)
        {
            MessageBox.Show(MSG);
        }
        private void EnableControl(bool value)
        {
            button2.IsEnabled = value;
            StartSendingBTN.IsEnabled = value;
            StopSendingBT.IsEnabled = value;
        }

        private void StopSendingBT_Click(object sender, RoutedEventArgs e)
        {
            StartSending = false;
        }

        private void StartSendingBTN_Click(object sender, RoutedEventArgs e)
        {
            StartSending = true;
        }

    }
}
