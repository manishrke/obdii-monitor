namespace ObdiiMonitor
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spreadsheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.troubleCodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelSensorSelection = new System.Windows.Forms.Panel();
            this.buttonCollect = new System.Windows.Forms.Button();
            this.labelSensorData = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.buttonInitialize = new System.Windows.Forms.Button();
            this.comboBoxComPort = new System.Windows.Forms.ComboBox();
            this.comboBoxMeasurement = new System.Windows.Forms.ComboBox();
            this.panelCollectData = new System.Windows.Forms.Panel();
            this.HelpText = new System.Windows.Forms.Label();
            this.MapButton = new System.Windows.Forms.Button();
            this.panelSensorGraphs = new System.Windows.Forms.Panel();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.textBoxStartTime = new System.Windows.Forms.TextBox();
            this.textBoxEndTime = new System.Windows.Forms.TextBox();
            this.buttonGet = new System.Windows.Forms.Button();
            this.labelStart = new System.Windows.Forms.Label();
            this.labelEnd = new System.Windows.Forms.Label();
            this.labelTotalMs = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip.SuspendLayout();
            this.panelCollectData.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(884, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.ConfigToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.LoadToolStripMenuItem_Click);
            // 
            // ConfigToolStripMenuItem
            // 
            this.ConfigToolStripMenuItem.Name = "ConfigToolStripMenuItem";
            this.ConfigToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.ConfigToolStripMenuItem.Text = "Save/Load config from SD Card";
            this.ConfigToolStripMenuItem.Click += new System.EventHandler(this.ConfigToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textFileToolStripMenuItem,
            this.spreadsheetToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.exportToolStripMenuItem.Text = "Export Data";
            // 
            // textFileToolStripMenuItem
            // 
            this.textFileToolStripMenuItem.Name = "textFileToolStripMenuItem";
            this.textFileToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.textFileToolStripMenuItem.Text = "Text File";
            this.textFileToolStripMenuItem.Click += new System.EventHandler(this.TextFileToolStripMenuItem_Click);
            // 
            // spreadsheetToolStripMenuItem
            // 
            this.spreadsheetToolStripMenuItem.Name = "spreadsheetToolStripMenuItem";
            this.spreadsheetToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.spreadsheetToolStripMenuItem.Text = "Spreadsheet";
            this.spreadsheetToolStripMenuItem.Click += new System.EventHandler(this.SpreadsheetToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.troubleCodesToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // troubleCodesToolStripMenuItem
            // 
            this.troubleCodesToolStripMenuItem.Name = "troubleCodesToolStripMenuItem";
            this.troubleCodesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.troubleCodesToolStripMenuItem.Text = "Trouble Codes";
            this.troubleCodesToolStripMenuItem.Click += new System.EventHandler(this.TroubleCodesToolStripMenuItem_Click);
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
            this.buttonCollect.Click += new System.EventHandler(this.ButtonCollect_Click);
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
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(164, 34);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(76, 13);
            this.labelStatus.TabIndex = 12;
            this.labelStatus.Text = "not connected";
            // 
            // buttonInitialize
            // 
            this.buttonInitialize.Location = new System.Drawing.Point(92, 26);
            this.buttonInitialize.Name = "buttonInitialize";
            this.buttonInitialize.Size = new System.Drawing.Size(66, 23);
            this.buttonInitialize.TabIndex = 11;
            this.buttonInitialize.Text = "Initialize";
            this.buttonInitialize.UseVisualStyleBackColor = true;
            this.buttonInitialize.Click += new System.EventHandler(this.ButtonInitialize_Click);
            // 
            // comboBoxComPort
            // 
            this.comboBoxComPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
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
            this.comboBoxComPort.Location = new System.Drawing.Point(9, 27);
            this.comboBoxComPort.Name = "comboBoxComPort";
            this.comboBoxComPort.Size = new System.Drawing.Size(77, 21);
            this.comboBoxComPort.TabIndex = 10;
            // 
            // comboBoxMeasurement
            // 
            this.comboBoxMeasurement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMeasurement.FormattingEnabled = true;
            this.comboBoxMeasurement.Items.AddRange(new object[] {
            "Metric",
            "US"});
            this.comboBoxMeasurement.Location = new System.Drawing.Point(778, 28);
            this.comboBoxMeasurement.Name = "comboBoxMeasurement";
            this.comboBoxMeasurement.Size = new System.Drawing.Size(94, 21);
            this.comboBoxMeasurement.TabIndex = 9;
            // 
            // panelCollectData
            // 
            this.panelCollectData.Controls.Add(this.HelpText);
            this.panelCollectData.Controls.Add(this.MapButton);
            this.panelCollectData.Controls.Add(this.buttonCollect);
            this.panelCollectData.Controls.Add(this.labelSensorData);
            this.panelCollectData.Location = new System.Drawing.Point(0, 52);
            this.panelCollectData.Name = "panelCollectData";
            this.panelCollectData.Size = new System.Drawing.Size(884, 32);
            this.panelCollectData.TabIndex = 14;
            // 
            // HelpText
            // 
            this.HelpText.Location = new System.Drawing.Point(417, 14);
            this.HelpText.Name = "HelpText";
            this.HelpText.Size = new System.Drawing.Size(455, 13);
            this.HelpText.TabIndex = 0;
            this.HelpText.Text = "HelpText";
            this.HelpText.Visible = false;
            // 
            // MapButton
            // 
            this.MapButton.Enabled = false;
            this.MapButton.Location = new System.Drawing.Point(255, 9);
            this.MapButton.Name = "MapButton";
            this.MapButton.Size = new System.Drawing.Size(75, 23);
            this.MapButton.TabIndex = 2;
            this.MapButton.Text = "Map";
            this.MapButton.UseVisualStyleBackColor = true;
            this.MapButton.Click += new System.EventHandler(this.MapButton_Click);
            // 
            // panelSensorGraphs
            // 
            this.panelSensorGraphs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSensorGraphs.AutoScroll = true;
            this.panelSensorGraphs.Location = new System.Drawing.Point(0, 86);
            this.panelSensorGraphs.Name = "panelSensorGraphs";
            this.panelSensorGraphs.Size = new System.Drawing.Size(884, 455);
            this.panelSensorGraphs.TabIndex = 0;
            // 
            // textBoxStartTime
            // 
            this.textBoxStartTime.Location = new System.Drawing.Point(400, 27);
            this.textBoxStartTime.Name = "textBoxStartTime";
            this.textBoxStartTime.Size = new System.Drawing.Size(65, 20);
            this.textBoxStartTime.TabIndex = 15;
            // 
            // textBoxEndTime
            // 
            this.textBoxEndTime.Location = new System.Drawing.Point(506, 27);
            this.textBoxEndTime.Name = "textBoxEndTime";
            this.textBoxEndTime.Size = new System.Drawing.Size(65, 20);
            this.textBoxEndTime.TabIndex = 16;
            // 
            // buttonGet
            // 
            this.buttonGet.Location = new System.Drawing.Point(577, 25);
            this.buttonGet.Name = "buttonGet";
            this.buttonGet.Size = new System.Drawing.Size(42, 23);
            this.buttonGet.TabIndex = 17;
            this.buttonGet.Text = "Get";
            this.buttonGet.UseVisualStyleBackColor = true;
            this.buttonGet.Click += new System.EventHandler(this.ButtonGet_Click);
            // 
            // labelStart
            // 
            this.labelStart.AutoSize = true;
            this.labelStart.Location = new System.Drawing.Point(362, 31);
            this.labelStart.Name = "labelStart";
            this.labelStart.Size = new System.Drawing.Size(32, 13);
            this.labelStart.TabIndex = 18;
            this.labelStart.Text = "Start:";
            // 
            // labelEnd
            // 
            this.labelEnd.AutoSize = true;
            this.labelEnd.Location = new System.Drawing.Point(471, 31);
            this.labelEnd.Name = "labelEnd";
            this.labelEnd.Size = new System.Drawing.Size(29, 13);
            this.labelEnd.TabIndex = 19;
            this.labelEnd.Text = "End:";
            // 
            // labelTotalMs
            // 
            this.labelTotalMs.AutoSize = true;
            this.labelTotalMs.Location = new System.Drawing.Point(626, 33);
            this.labelTotalMs.Name = "labelTotalMs";
            this.labelTotalMs.Size = new System.Drawing.Size(56, 13);
            this.labelTotalMs.TabIndex = 20;
            this.labelTotalMs.Text = "Total (ms):";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 542);
            this.Controls.Add(this.labelTotalMs);
            this.Controls.Add(this.labelEnd);
            this.Controls.Add(this.labelStart);
            this.Controls.Add(this.buttonGet);
            this.Controls.Add(this.textBoxEndTime);
            this.Controls.Add(this.textBoxStartTime);
            this.Controls.Add(this.panelSensorGraphs);
            this.Controls.Add(this.panelCollectData);
            this.Controls.Add(this.panelSensorSelection);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.comboBoxComPort);
            this.Controls.Add(this.comboBoxMeasurement);
            this.Controls.Add(this.buttonInitialize);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainWindow";
            this.Text = " ";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panelCollectData.ResumeLayout(false);
            this.panelCollectData.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spreadsheetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem troubleCodesToolStripMenuItem;
        private System.Windows.Forms.Panel panelSensorSelection;
        private System.Windows.Forms.Label labelSensorData;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button buttonInitialize;
        private System.Windows.Forms.ComboBox comboBoxComPort;
        private System.Windows.Forms.ComboBox comboBoxMeasurement;
        private System.Windows.Forms.Button buttonCollect;
        private System.Windows.Forms.Panel panelCollectData;
        private System.Windows.Forms.Panel panelSensorGraphs;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox textBoxStartTime;
        private System.Windows.Forms.TextBox textBoxEndTime;
        private System.Windows.Forms.Button buttonGet;
        private System.Windows.Forms.Label labelStart;
        private System.Windows.Forms.Label labelEnd;
        private System.Windows.Forms.Label labelTotalMs;
        private System.Windows.Forms.ToolStripMenuItem ConfigToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button MapButton;
        private System.Windows.Forms.Label HelpText;
    }
}