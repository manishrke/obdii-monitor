namespace ScanTool
{
    partial class MainWindow
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spreadsheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sensorDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.troubleCodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelSensorSelection = new System.Windows.Forms.Panel();
            this.buttonCollect = new System.Windows.Forms.Button();
            this.labelSensorData = new System.Windows.Forms.Label();
            this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.buttonInitialize = new System.Windows.Forms.Button();
            this.comboBoxComPort = new System.Windows.Forms.ComboBox();
            this.comboBoxMeasurement = new System.Windows.Forms.ComboBox();
            this.panelCollectData = new System.Windows.Forms.Panel();
            this.panelSensorGraphs = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.panelCollectData.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(884, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadToolStripMenuItem.Text = "Load";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textFileToolStripMenuItem,
            this.spreadsheetToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // textFileToolStripMenuItem
            // 
            this.textFileToolStripMenuItem.Name = "textFileToolStripMenuItem";
            this.textFileToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.textFileToolStripMenuItem.Text = "Text File";
            // 
            // spreadsheetToolStripMenuItem
            // 
            this.spreadsheetToolStripMenuItem.Name = "spreadsheetToolStripMenuItem";
            this.spreadsheetToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.spreadsheetToolStripMenuItem.Text = "Spreadsheet";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sensorDataToolStripMenuItem,
            this.troubleCodesToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // sensorDataToolStripMenuItem
            // 
            this.sensorDataToolStripMenuItem.Name = "sensorDataToolStripMenuItem";
            this.sensorDataToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.sensorDataToolStripMenuItem.Text = "Sensor Data";
            // 
            // troubleCodesToolStripMenuItem
            // 
            this.troubleCodesToolStripMenuItem.Name = "troubleCodesToolStripMenuItem";
            this.troubleCodesToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.troubleCodesToolStripMenuItem.Text = "Trouble Codes";
            // 
            // panelSensorSelection
            // 
            this.panelSensorSelection.AutoScroll = true;
            this.panelSensorSelection.Location = new System.Drawing.Point(0, 86);
            this.panelSensorSelection.Name = "panelSensorSelection";
            this.panelSensorSelection.Size = new System.Drawing.Size(884, 455);
            this.panelSensorSelection.TabIndex = 1;
            // 
            // buttonCollect
            // 
            this.buttonCollect.Location = new System.Drawing.Point(174, 9);
            this.buttonCollect.Name = "buttonCollect";
            this.buttonCollect.Size = new System.Drawing.Size(75, 23);
            this.buttonCollect.TabIndex = 1;
            this.buttonCollect.Text = "Collect Data";
            this.buttonCollect.UseVisualStyleBackColor = true;
            this.buttonCollect.Click += new System.EventHandler(this.buttonCollect_Click);
            // 
            // labelSensorData
            // 
            this.labelSensorData.AutoSize = true;
            this.labelSensorData.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSensorData.Location = new System.Drawing.Point(3, 1);
            this.labelSensorData.Name = "labelSensorData";
            this.labelSensorData.Size = new System.Drawing.Size(165, 31);
            this.labelSensorData.TabIndex = 0;
            this.labelSensorData.Text = "Sensor Data";
            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.FormattingEnabled = true;
            this.comboBoxBaudRate.Items.AddRange(new object[] {
            "9600",
            "38400"});
            this.comboBoxBaudRate.Location = new System.Drawing.Point(261, 29);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(121, 21);
            this.comboBoxBaudRate.TabIndex = 13;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(497, 36);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(76, 13);
            this.labelStatus.TabIndex = 12;
            this.labelStatus.Text = "not connected";
            // 
            // buttonInitialize
            // 
            this.buttonInitialize.Location = new System.Drawing.Point(388, 27);
            this.buttonInitialize.Name = "buttonInitialize";
            this.buttonInitialize.Size = new System.Drawing.Size(86, 23);
            this.buttonInitialize.TabIndex = 11;
            this.buttonInitialize.Text = "Initialize";
            this.buttonInitialize.UseVisualStyleBackColor = true;
            this.buttonInitialize.Click += new System.EventHandler(this.buttonInitialize_Click);
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
            this.comboBoxComPort.Location = new System.Drawing.Point(134, 29);
            this.comboBoxComPort.Name = "comboBoxComPort";
            this.comboBoxComPort.Size = new System.Drawing.Size(121, 21);
            this.comboBoxComPort.TabIndex = 10;
            // 
            // comboBoxMeasurement
            // 
            this.comboBoxMeasurement.FormattingEnabled = true;
            this.comboBoxMeasurement.Items.AddRange(new object[] {
            "Metric",
            "US"});
            this.comboBoxMeasurement.Location = new System.Drawing.Point(7, 29);
            this.comboBoxMeasurement.Name = "comboBoxMeasurement";
            this.comboBoxMeasurement.Size = new System.Drawing.Size(121, 21);
            this.comboBoxMeasurement.TabIndex = 9;
            // 
            // panelCollectData
            // 
            this.panelCollectData.Controls.Add(this.buttonCollect);
            this.panelCollectData.Controls.Add(this.labelSensorData);
            this.panelCollectData.Location = new System.Drawing.Point(0, 52);
            this.panelCollectData.Name = "panelCollectData";
            this.panelCollectData.Size = new System.Drawing.Size(884, 32);
            this.panelCollectData.TabIndex = 14;
            // 
            // panelSensorGraphs
            // 
            this.panelSensorGraphs.AutoScroll = true;
            this.panelSensorGraphs.Location = new System.Drawing.Point(0, 86);
            this.panelSensorGraphs.Name = "panelSensorGraphs";
            this.panelSensorGraphs.Size = new System.Drawing.Size(884, 455);
            this.panelSensorGraphs.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 542);
            this.Controls.Add(this.panelSensorGraphs);
            this.Controls.Add(this.panelCollectData);
            this.Controls.Add(this.comboBoxBaudRate);
            this.Controls.Add(this.panelSensorSelection);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.comboBoxComPort);
            this.Controls.Add(this.comboBoxMeasurement);
            this.Controls.Add(this.buttonInitialize);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = " ";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelCollectData.ResumeLayout(false);
            this.panelCollectData.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spreadsheetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sensorDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem troubleCodesToolStripMenuItem;
        private System.Windows.Forms.Panel panelSensorSelection;
        private System.Windows.Forms.Label labelSensorData;
        private System.Windows.Forms.ComboBox comboBoxBaudRate;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button buttonInitialize;
        private System.Windows.Forms.ComboBox comboBoxComPort;
        private System.Windows.Forms.ComboBox comboBoxMeasurement;
        private System.Windows.Forms.Button buttonCollect;
        private System.Windows.Forms.Panel panelCollectData;
        private System.Windows.Forms.Panel panelSensorGraphs;
    }
}