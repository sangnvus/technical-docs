namespace SocketCoder
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.EventslistBox = new System.Windows.Forms.ListBox();
            this.Conncet = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ServicesPanel = new System.Windows.Forms.Panel();
            this.VideocheckBox = new System.Windows.Forms.CheckBox();
            this.DesktopcheckBox = new System.Windows.Forms.CheckBox();
            this.VoicecheckBox = new System.Windows.Forms.CheckBox();
            this.button_shutdown = new System.Windows.Forms.Button();
            this.ServicesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // EventslistBox
            // 
            this.EventslistBox.FormattingEnabled = true;
            this.EventslistBox.HorizontalScrollbar = true;
            this.EventslistBox.Location = new System.Drawing.Point(4, 103);
            this.EventslistBox.Name = "EventslistBox";
            this.EventslistBox.Size = new System.Drawing.Size(448, 82);
            this.EventslistBox.TabIndex = 10;
            // 
            // Conncet
            // 
            this.Conncet.Location = new System.Drawing.Point(7, 9);
            this.Conncet.Name = "Conncet";
            this.Conncet.Size = new System.Drawing.Size(330, 36);
            this.Conncet.TabIndex = 7;
            this.Conncet.Text = "Start The Server";
            this.Conncet.UseVisualStyleBackColor = true;
            this.Conncet.Click += new System.EventHandler(this.Conncet_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(114, 188);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "A Special Version For Silverlight 4";
            // 
            // ServicesPanel
            // 
            this.ServicesPanel.Controls.Add(this.VideocheckBox);
            this.ServicesPanel.Controls.Add(this.DesktopcheckBox);
            this.ServicesPanel.Controls.Add(this.VoicecheckBox);
            this.ServicesPanel.Location = new System.Drawing.Point(4, 51);
            this.ServicesPanel.Name = "ServicesPanel";
            this.ServicesPanel.Size = new System.Drawing.Size(448, 49);
            this.ServicesPanel.TabIndex = 18;
            // 
            // VideocheckBox
            // 
            this.VideocheckBox.AutoSize = true;
            this.VideocheckBox.Checked = true;
            this.VideocheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.VideocheckBox.Location = new System.Drawing.Point(3, 26);
            this.VideocheckBox.Name = "VideocheckBox";
            this.VideocheckBox.Size = new System.Drawing.Size(90, 17);
            this.VideocheckBox.TabIndex = 3;
            this.VideocheckBox.Text = "Video Service";
            this.VideocheckBox.UseVisualStyleBackColor = true;
            // 
            // DesktopcheckBox
            // 
            this.DesktopcheckBox.AutoSize = true;
            this.DesktopcheckBox.Checked = true;
            this.DesktopcheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DesktopcheckBox.Location = new System.Drawing.Point(322, 3);
            this.DesktopcheckBox.Name = "DesktopcheckBox";
            this.DesktopcheckBox.Size = new System.Drawing.Size(103, 17);
            this.DesktopcheckBox.TabIndex = 2;
            this.DesktopcheckBox.Text = "Desktop Service";
            this.DesktopcheckBox.UseVisualStyleBackColor = true;
            // 
            // VoicecheckBox
            // 
            this.VoicecheckBox.AutoSize = true;
            this.VoicecheckBox.Checked = true;
            this.VoicecheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.VoicecheckBox.Location = new System.Drawing.Point(3, 3);
            this.VoicecheckBox.Name = "VoicecheckBox";
            this.VoicecheckBox.Size = new System.Drawing.Size(89, 17);
            this.VoicecheckBox.TabIndex = 0;
            this.VoicecheckBox.Text = "Voice Service";
            this.VoicecheckBox.UseVisualStyleBackColor = true;
            // 
            // button_shutdown
            // 
            this.button_shutdown.Enabled = false;
            this.button_shutdown.Location = new System.Drawing.Point(343, 9);
            this.button_shutdown.Name = "button_shutdown";
            this.button_shutdown.Size = new System.Drawing.Size(109, 36);
            this.button_shutdown.TabIndex = 19;
            this.button_shutdown.Text = "Shutdown";
            this.button_shutdown.UseVisualStyleBackColor = true;
            this.button_shutdown.Click += new System.EventHandler(this.button_shutdown_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 209);
            this.Controls.Add(this.button_shutdown);
            this.Controls.Add(this.ServicesPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EventslistBox);
            this.Controls.Add(this.Conncet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SocketCoder Web Conferencing Server";
            this.ServicesPanel.ResumeLayout(false);
            this.ServicesPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox EventslistBox;
        private System.Windows.Forms.Button Conncet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel ServicesPanel;
        private System.Windows.Forms.CheckBox VideocheckBox;
        private System.Windows.Forms.CheckBox DesktopcheckBox;
        private System.Windows.Forms.CheckBox VoicecheckBox;
        private System.Windows.Forms.Button button_shutdown;
    }
}

