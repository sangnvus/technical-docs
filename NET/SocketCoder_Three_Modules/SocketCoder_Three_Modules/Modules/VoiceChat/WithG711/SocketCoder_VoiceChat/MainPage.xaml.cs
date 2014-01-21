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
using VoiceEncoder;
using System.IO;
using System.Threading;

namespace SocketCoder_VoiceChat
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

       #region Global Declarations
        private CaptureSource _source = new CaptureSource();
        private PcmToWave _pcm = new PcmToWave();
        private MemoryAudioSink _sink;
        private bool _isRecording;
        private bool _isConnected = false;
        private Socket _client_socket;
        private delegate void mydelegate(byte[] buffer);
        private delegate void ShowMessagedelegate(string MSG);
        private delegate void Enabledelegate(bool value);

        private int BufferSize = 4000;
        #endregion Global Declarations

       #region Socket Methods
        void Conncet(string IP_Address)
        {
            _client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs()
            {
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse(IP_Address), 4530)
            };
            socketEventArg.Completed += OnConncetCompleted;
            _client_socket.ConnectAsync(socketEventArg);
        }

        void Send_Bytes(byte[] buffer)
        {
            _client_socket.NoDelay = true;
                
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.Completed += OnSendCompleted;
            socketEventArg.SetBuffer(buffer, 0, buffer.Length);
            _client_socket.SendAsync(socketEventArg);
            
        }

        void StartReceiving()
        {
            byte[] response = new byte[BufferSize];
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.Completed += OnReceiveCompleted;
            socketEventArg.SetBuffer(response, 0, response.Length);
            _client_socket.ReceiveAsync(socketEventArg);
        }
        void OnConncetCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                this.Dispatcher.BeginInvoke(new ShowMessagedelegate(ShowMessageBox), "Connceted Successfully!");
                this.Dispatcher.BeginInvoke(new Enabledelegate(EnableControl), true);
                
            }
            else
            {
                this.Dispatcher.BeginInvoke(new ShowMessagedelegate(ShowMessageBox), e.SocketError.ToString());
                this.Dispatcher.BeginInvoke(new Enabledelegate(EnableControl), false);
            }

        }
        void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {

        }
        void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new mydelegate(PlayReceivedBuffer), e.Buffer);
            
        }
        #endregion Socket Methods

       #region Encoding/Decoding Methods

        void StartRecording()
        {
            try
            {
                if (!_isRecording)
                {
                    _source = new CaptureSource();
                    if ((CaptureDeviceConfiguration.AllowedDeviceAccess | CaptureDeviceConfiguration.RequestDeviceAccess()) & (_source.State == CaptureState.Stopped))
                    {

                        _sink = new MemoryAudioSink();

                        AudioFormat desiredAudioFormat = null;
                        foreach (AudioFormat audioFormat in _source.AudioCaptureDevice.SupportedFormats)
                        {
                            if (audioFormat.SamplesPerSecond == 8000 && audioFormat.BitsPerSample == 16 && audioFormat.Channels == 1 && audioFormat.WaveFormat == WaveFormatType.Pcm)
                            {
                                desiredAudioFormat = audioFormat;
                            }
                        }

                        if (desiredAudioFormat == null)
                        {
                            ShowMessageBox("Your sound device may not found or in use also please make sure that it's support (16 bits,8000 samples,1 channel) please tell your system administrator about this problem!");
                            return;
                        }

                        _sink.OnBufferFulfill += new EventHandler(SendVoiceBuffer);
                        _source.AudioCaptureDevice.DesiredFormat = desiredAudioFormat;
                        _sink.CaptureSource = _source;
                        _source.Start();
                        _isRecording = true;
                    }

                }
            }
            catch (Exception ex)
            {
                ShowMessageBox("Your sound device may in use! " + ex.Message);
            }
        }

        void SendVoiceBuffer(object VoiceBuffer, EventArgs e)
        {

            byte[] PCM_Buffer = (byte[])VoiceBuffer;
            byte[] Encoded = VoiceEncoder.G711Audio.ALawEncoder.ALawEncode(PCM_Buffer);

            Send_Bytes(Encoded);
            this.Dispatcher.BeginInvoke(new ShowMessagedelegate(ShowBufferSize), Encoded.Length.ToString());

        }
        void StopRecording()
        {
            if (_source.State == CaptureState.Started)
            {
                _source.Stop();
                _isRecording = false;
                _sink.StartSending = false;
                StartSendingBTN.IsEnabled = true;
                StopSendingBT.IsEnabled = false;
            }
        }
        void OpenWavFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog().Value)
            {
                Stream s = ofd.File.OpenRead();
                WaveMediaStreamSource wavMss = new WaveMediaStreamSource(s);
                mediaElement1.SetSource(wavMss);
                mediaElement1.Play();
            }
        }

        void SaveTheVoiceSession() 
        {
            // To DO In the Next Version: to record the voice session
            // Just Need to create a new stream object to 
            // collect the incoming/outgoing voice streams

            //    SaveFileDialog SaveDialog = new SaveFileDialog();

            //    SaveDialog.Filter = "Audio Files (*.wav)|*.wav";

            //    SaveDialog.ShowDialog();

            //    try
            //    {
            //        using (Stream filestream = SaveDialog.OpenFile())
            //        {

            //            _pcm.SavePcmToWav(_sink._stream, filestream,_sink._format);
            //        }
            //   
            //    catch (Exception){}
            //}
        }

        private void PlayReceivedBuffer(byte[] Encodedbuffer)
        {
            if (MuteCheckBox.IsChecked == false)
                try
                {
                    //Decode to Wave Format Then Play
                    byte[] DecodedBuffer = new byte[Encodedbuffer.Length * 2];
                    VoiceEncoder.G711Audio.ALawDecoder.ALawDecode(Encodedbuffer, out DecodedBuffer); ;
                    PlayWave(DecodedBuffer);
                    
                }
                catch (Exception) { }

                StartReceiving();
        }

        void PlayWave(byte[] PCMBytes)
        {
            MemoryStream ms_PCM = new MemoryStream(PCMBytes);
            MemoryStream ms_Wave = new MemoryStream();

            _pcm.SavePcmToWav(ms_PCM, ms_Wave,16,8000,1);

            WaveMediaStreamSource WaveStream = new WaveMediaStreamSource(ms_Wave);
            mediaElement1.SetSource(WaveStream);
            mediaElement1.Play();
        }
        private void ShowBufferSize(string Buffer_Size)
        {
            BufferSizeLB.Content = Buffer_Size;
        }
        #endregion Encoding/Decoding Methods

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;
            button3.IsEnabled = true;

            StartRecording();
        }
        private void ConnectBTN_Click(object sender, RoutedEventArgs e)
        {
            if (IPAddress_TXT.Text != "")
            {
                Conncet(IPAddress_TXT.Text);
                
            }
            else ShowMessageBox("The Server IP is required!");
        }
        private void ShowMessageBox(string MSG)
        {
            MessageBox.Show(MSG);
        }
        private void EnableControl(bool value)
        {
            button1.IsEnabled = value;
            ListeningBTN.IsEnabled = value;
            _isConnected = value;
            MuteCheckBox.IsEnabled = value;
        }

        private void StopSendingBT_Click(object sender, RoutedEventArgs e)
        {
            _sink.StartSending = false;
            StartSendingBTN.IsEnabled = true;
            StopSendingBT.IsEnabled = false;
        }

        private void StartSendingBTN_Click(object sender, RoutedEventArgs e)
        {
            if (_isRecording)
            {
                BufferSizeLB.Content = (BufferSize).ToString();
                _sink.StartSending = true;
                StartSendingBTN.IsEnabled = false;
                StopSendingBT.IsEnabled = true;
            }
            else ShowMessageBox("Please Press On Start The Microphone Firstly!");
        }
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = true;
            StopRecording();
        }

        private void ListeningBTN_Click(object sender, RoutedEventArgs e)
        {
            if (_isConnected)
            {
                StartReceiving();
                ListeningBTN.IsEnabled = false;
                BufferSizeLB.Content = BufferSize.ToString();
                StartSendingBTN.IsEnabled = true;
            }
            else ShowMessageBox("Sorry but you are not Connected!");

        }

    }
}
