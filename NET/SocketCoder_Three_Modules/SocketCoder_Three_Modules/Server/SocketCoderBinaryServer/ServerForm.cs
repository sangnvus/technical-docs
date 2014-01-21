//
//                      (C) Fadi Abdelqader - SocketCoder.Com 2010
// SocketCoderBinaryServer Class is a part of SocketCoder Classes Project (C) SocketCoder.Com
//                   - This Project Is Created to Work Behind The NAT - 
//                - Just The Server Should be on a PC that has a public IP - 
//                              A Special Version For Silverlight 4
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;								
using System.Net;								
using System.Net.Sockets;						
using System.Collections;

namespace SocketCoder
{
    public partial class Form1 : Form
    {

        SocketCoderVoiceServer VoiceServer ;
        SocketCoderVideoServer VideoServer;
        SocketCoderDesktopServer DesktopServer;

        public void Add_Event(string MSG)
        {
            EventslistBox.Items.Add(MSG);
            EventslistBox.SelectedIndex = EventslistBox.Items.Count - 1;
        }

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            // Start The Policy Server
            PolicySocketServer StartPolicyServer = new PolicySocketServer();
            Thread th = new Thread(new ThreadStart(StartPolicyServer.StartSocketServer));
            th.IsBackground = true;
            th.Start();
            
        }
        string GetTime()
        {
            return " at " + DateTime.Now.ToString();
        }
        private void Conncet_Click(object sender, EventArgs e)
        {
            EventslistBox.Items.Clear();

            if (VoicecheckBox.Checked)
            {
                VoiceServer = new SocketCoderVoiceServer();
                string VoiceSTMSG = VoiceServer.Start_A_Server_On(4530) + " For Voice Service" + GetTime();
                Add_Event(VoiceSTMSG);
            }
            if (VideocheckBox.Checked)
            {
                VideoServer = new SocketCoderVideoServer();
                string VideoSTMSG = VideoServer.Start_A_Server_On(4531) + " For Video Service" + GetTime();
                Add_Event(VideoSTMSG);
            }
            if (DesktopcheckBox.Checked)
            {
                DesktopServer = new SocketCoderDesktopServer();
                string DesktopSTMSG = DesktopServer.Start_A_Server_On(4532) + " For Desktop Service" + GetTime();
                Add_Event(DesktopSTMSG);
            }

            ServicesPanel.Enabled = false;
            Conncet.Enabled = false;
            button_shutdown.Enabled = true;
        }

        private void button_shutdown_Click(object sender, EventArgs e)
        {
            if (VoicecheckBox.Checked)
            {
                VoiceServer = new SocketCoderVoiceServer();
                string VoiceSTMSG = VoiceServer.ShutDown() + " The Voice Service" + GetTime();
                Add_Event(VoiceSTMSG);
            }
            if (VideocheckBox.Checked)
            {
                VideoServer = new SocketCoderVideoServer();
                string VideoSTMSG = VideoServer.ShutDown() + " The Video Service" + GetTime();
                Add_Event(VideoSTMSG);
            }
            if (DesktopcheckBox.Checked)
            {
                DesktopServer = new SocketCoderDesktopServer();
                string DesktopSTMSG = DesktopServer.ShutDown() + " The Desktop Service" + GetTime();
                Add_Event(DesktopSTMSG);
            }

            Conncet.Enabled = true;
            button_shutdown.Enabled = false;
            ServicesPanel.Enabled = true;
        }
    }
}