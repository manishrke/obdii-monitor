namespace ScanTool
{
    partial class ScanTool
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
            this.components = new System.ComponentModel.Container();
            this.buttonTransmit = new System.Windows.Forms.Button();
            this.textBoxTransmit = new System.Windows.Forms.TextBox();
            this.comboBoxMeasurement = new System.Windows.Forms.ComboBox();
            this.comboBoxComPort = new System.Windows.Forms.ComboBox();
            this.textBoxResults = new System.Windows.Forms.TextBox();
            this.buttonInitialize = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.SuspendLayout();
            // 
            // buttonTransmit
            // 
            this.buttonTransmit.Location = new System.Drawing.Point(795, 36);
            this.buttonTransmit.Name = "buttonTransmit";
            this.buttonTransmit.Size = new System.Drawing.Size(75, 23);
            this.buttonTransmit.TabIndex = 0;
            this.buttonTransmit.Text = "Transmit";
            this.buttonTransmit.UseVisualStyleBackColor = true;
            this.buttonTransmit.Click += new System.EventHandler(this.buttonTransmit_Click);
            // 
            // textBoxTransmit
            // 
            this.textBoxTransmit.Location = new System.Drawing.Point(12, 39);
            this.textBoxTransmit.Name = "textBoxTransmit";
            this.textBoxTransmit.Size = new System.Drawing.Size(777, 20);
            this.textBoxTransmit.TabIndex = 1;
            // 
            // comboBoxMeasurement
            // 
            this.comboBoxMeasurement.FormattingEnabled = true;
            this.comboBoxMeasurement.Items.AddRange(new object[] {
            "Metric",
            "US"});
            this.comboBoxMeasurement.Location = new System.Drawing.Point(12, 12);
            this.comboBoxMeasurement.Name = "comboBoxMeasurement";
            this.comboBoxMeasurement.Size = new System.Drawing.Size(121, 21);
            this.comboBoxMeasurement.TabIndex = 2;
            // 
            // comboBoxComPort
            // 
            this.comboBoxComPort.FormattingEnabled = true;
            this.comboBoxComPort.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8"});
            this.comboBoxComPort.Location = new System.Drawing.Point(139, 12);
            this.comboBoxComPort.Name = "comboBoxComPort";
            this.comboBoxComPort.Size = new System.Drawing.Size(121, 21);
            this.comboBoxComPort.TabIndex = 3;
            // 
            // textBoxResults
            // 
            this.textBoxResults.Location = new System.Drawing.Point(12, 65);
            this.textBoxResults.Multiline = true;
            this.textBoxResults.Name = "textBoxResults";
            this.textBoxResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxResults.Size = new System.Drawing.Size(777, 185);
            this.textBoxResults.TabIndex = 5;
            // 
            // buttonInitialize
            // 
            this.buttonInitialize.Location = new System.Drawing.Point(393, 10);
            this.buttonInitialize.Name = "buttonInitialize";
            this.buttonInitialize.Size = new System.Drawing.Size(86, 23);
            this.buttonInitialize.TabIndex = 6;
            this.buttonInitialize.Text = "Initialize";
            this.buttonInitialize.UseVisualStyleBackColor = true;
            this.buttonInitialize.Click += new System.EventHandler(this.buttonInitialize_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(502, 19);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(76, 13);
            this.labelStatus.TabIndex = 7;
            this.labelStatus.Text = "not connected";
            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.FormattingEnabled = true;
            this.comboBoxBaudRate.Items.AddRange(new object[] {
            "9600",
            "38400"});
            this.comboBoxBaudRate.Location = new System.Drawing.Point(266, 12);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(121, 21);
            this.comboBoxBaudRate.TabIndex = 8;
            // 
            // serialPort
            // 
            this.serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort_DataReceived);
            // 
            // ScanTool
            // 
            this.AcceptButton = this.buttonTransmit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(882, 262);
            this.Controls.Add(this.comboBoxBaudRate);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.buttonInitialize);
            this.Controls.Add(this.textBoxResults);
            this.Controls.Add(this.comboBoxComPort);
            this.Controls.Add(this.comboBoxMeasurement);
            this.Controls.Add(this.textBoxTransmit);
            this.Controls.Add(this.buttonTransmit);
            this.Name = "ScanTool";
            this.Text = "Scan Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScanTool_FormClosing);
            this.Load += new System.EventHandler(this.ScanTool_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonTransmit;
        private System.Windows.Forms.TextBox textBoxTransmit;
        private System.Windows.Forms.ComboBox comboBoxMeasurement;
        private System.Windows.Forms.ComboBox comboBoxComPort;
        private System.Windows.Forms.TextBox textBoxResults;
        private System.Windows.Forms.Button buttonInitialize;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ComboBox comboBoxBaudRate;
        private System.IO.Ports.SerialPort serialPort;
    }
}