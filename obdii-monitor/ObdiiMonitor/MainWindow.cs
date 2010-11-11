// <copyright file="MainWindow.cs" company="University of Louisville">
// Copyright (c) 2010 All Rights Reserved
// </copyright>
// <author>Bradley Schoch</author>
// <author>Nicholas Bell</author>
// <date>2010-10-17</date>
// <summary>Windows Forms frontend.</summary>
namespace ObdiiMonitor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using System.Windows.Forms.DataVisualization.Charting;

    /// <summary>
    /// Windows Forms frontend
    /// </summary>
    public partial class MainWindow : Form
    {
        public static int StartHeight = 4;
        public static int StartWidth = 4;
        public int height;
        private static string COLLECT = "g";
        private static string STOP = "s";
        private static string REQCONF = "r";
        private byte acGraphCount=0;
        private ArrayList sensornames;
        private ArrayList pids ;
        private ArrayList configs;
        Controller controller;

        Label[] labelsSensorGraphs;

        Label[] labelsSensorGraphsValues;

        Chart[] chartsSensorGraphs;

        internal Thread UpdateGraphPlots;

        delegate void SetResponseCallback(int i, PollResponse response);

        Queue graphQueue = new Queue();

        public Queue GraphQueue
        {
            get { return this.graphQueue; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.controller = new Controller();
            this.controller.MainWindow = this;
            InitializeComponent();
            sensornames = new ArrayList();
            pids = new ArrayList();
            configs = new ArrayList();
 //           this.PopulateSelectionWindow();
            panelSensorGraphs.Visible = false;
            comboBoxComPort.SelectedIndex = 3;
            comboBoxMeasurement.SelectedIndex = 1;
            this.Resize += new System.EventHandler(this.Resizing);
        }

        /// <summary>
        /// Populates the selection window.
        /// </summary>
        public void PopulateSelectionWindow()
        {
            this.panelSensorSelection.Controls.Clear();
            height = StartHeight;
            if (sensornames != null)
            {
                sensornames.Clear();
                pids.Clear();
            }
            for (int i = 0; i<this.controller.SensorController.Sensors.Length; i++)
            {
                byte add = 0;
                if (this.controller.SensorController.Sensors[i].Pid != "AC")
                {
                    add = (byte)(controller.Config[byte.Parse(this.controller.SensorController.Sensors[i].Pid, System.Globalization.NumberStyles.HexNumber)] >> 7);
                    if (add == 1)
                    {
                        if (this.controller.SensorController.Sensors[i].Label2 == null)
                            sensornames.Add(this.controller.SensorController.Sensors[i].Label);
                        else
                            sensornames.Add(this.controller.SensorController.Sensors[i].Label + "," +
                                            this.controller.SensorController.Sensors[i].Label2.Substring(this.controller.SensorController.Sensors[i].Label2.LastIndexOf(':') + 1));
                        pids.Add(this.controller.SensorController.Sensors[i].Pid);
                    }
                }
            }
            
            addConfigPanel();
        }

        /// <summary>
        /// This function will take the integers in numselected and add the a graph for each of the indexes that
        /// numsSelected represents  in the Sensors table of SensorController.
        /// This function assumes that the SelectedSensors member of SensorController has already been intialized and set to the indexes that
        /// numsSelected represents
        /// </summary>
        /// <param name="numsSelected">The indices of selected sensors to display.</param>
        internal void PopulateGraphWindow(ArrayList numsSelected)
        {
            this.panelSensorGraphs.Controls.Clear();
            this.labelsSensorGraphs = new Label[numsSelected.Count];
            this.labelsSensorGraphsValues = new Label[numsSelected.Count];
            this.chartsSensorGraphs = new Chart[numsSelected.Count];

            ChartArea[] chartAreas = new ChartArea[numsSelected.Count];
            Series[] seriesLines = new Series[numsSelected.Count];
            Series[] seriesPoints = new Series[numsSelected.Count];

            int height = StartHeight;
            int width = StartWidth;

            for (int i = 0; i < numsSelected.Count; ++i)
            {
                this.labelsSensorGraphs[i] = new Label();
                this.labelsSensorGraphs[i].Text = this.controller.SensorController.Sensors[(int)numsSelected[i]].Label;
                this.labelsSensorGraphs[i].Location = new Point(width, height + (200 * i));
                this.panelSensorGraphs.Controls.Add(this.labelsSensorGraphs[i]);

                this.labelsSensorGraphsValues[i] = new Label();
                this.labelsSensorGraphsValues[i].Text = "Value: ";
                this.labelsSensorGraphsValues[i].Location = new Point(width + this.labelsSensorGraphs[i].Size.Width + 5, height + (200 * i));
                this.panelSensorGraphs.Controls.Add(this.labelsSensorGraphsValues[i]);

                this.chartsSensorGraphs[i] = new Chart();
                chartAreas[i] = new ChartArea();
                chartAreas[i].AlignmentStyle = AreaAlignmentStyles.All;
                chartAreas[i].AxisX.IsReversed = true;
                chartAreas[i].AxisX.Minimum = 0;
                seriesLines[i] = new Series();

                seriesPoints[i] = new Series();

                this.chartsSensorGraphs[i].ChartAreas.Add(chartAreas[i]);
                seriesLines[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                seriesPoints[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                this.chartsSensorGraphs[i].Series.Add(seriesLines[i]);
                this.chartsSensorGraphs[i].Series.Add(seriesPoints[i]);
                this.chartsSensorGraphs[i].Location = new System.Drawing.Point(23, 50);
                this.chartsSensorGraphs[i].Size = new System.Drawing.Size(170, 170);
                this.chartsSensorGraphs[i].Location = new Point(width, height + (200 * i) + 25);
                this.panelSensorGraphs.Controls.Add(this.chartsSensorGraphs[i]);
            }
        }

        private void buttonCollect_Click(object sender, EventArgs e)
        {
            if (buttonCollect.Text == "Collect Data")
            {
                controller.reset();

                ArrayList numsSelected = new ArrayList();
                configs.RemoveAt(configs.Count - 1);
                for (int i = 0; i < this.configs.Count; ++i)
                {
                    ConfigPanel c = (ConfigPanel)configs[i];
                    for (int j = 0; j < this.controller.SensorController.Sensors.Length; j++)
                        if (c.Selected == this.controller.SensorController.Sensors[j].Pid)
                        {
                            numsSelected.Add(j);
                        }
                }

                this.controller.SensorController.initializeSelectedSensors(numsSelected);
                saveConfig();
                this.controller.Serial.sendConfig();
                this.controller.SensorController.initializeReceivingThreads();
                this.controller.Serial.sendCommand(COLLECT);
                this.PopulateGraphWindow(numsSelected);
                this.ShowSensorDataPanel();
                buttonCollect.Text = "Stop";
                this.StartGraphPlotThread();
            }
            else if (buttonCollect.Text == "Stop")
            {
                this.ShowResetButton();
                this.controller.Serial.sendCommand(STOP);
                this.controller.cancelAllThreads();
            }
            else if (buttonCollect.Text == "Reset")
            {
                this.PopulateSelectionWindow();
                buttonCollect.Text = "Collect Data";
                this.panelSensorSelection.Visible = true;
                this.panelSensorGraphs.Visible = false;
            }
        }

        internal void ShowResetButton()
        {
            buttonCollect.Text = "Reset";
            this.controller.cancelAllThreads();
        }

        internal void ShowSensorDataPanel()
        {
            this.panelSensorSelection.Visible = false;
            this.panelSensorGraphs.Visible = true;
        }

        internal void StartGraphPlotThread()
        {
            this.UpdateGraphPlots = new Thread(new ThreadStart(this.UpdateGraphs));
            this.UpdateGraphPlots.Name = "UpdateGraphs";
            this.UpdateGraphPlots.Start();
        }

        /// <summary>
        /// Updates the graphs.
        /// </summary>
        public void UpdateGraphs()
        {
            while (true)
            {
                if (this.graphQueue.Count != 0)
                {
                    PollResponse response = (PollResponse)this.graphQueue.Dequeue();

                    if (response.DataType == "OB")
                    {
                        for (int i = 0; i < this.controller.SensorController.SelectedSensors.Length; ++i)
                        {
                            try
                            {
                                if (response.Data.Substring(0, 2) == this.controller.SensorController.SelectedSensors[i].Pid)
                                {
                                    this.SetGraphPoint(i, response);
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the graph point.
        /// </summary>
        /// <param name="i">The point index.</param>
        /// <param name="response">The response from the microcontroller.</param>
        private void SetGraphPoint(int i, PollResponse response)
        {
            if (this.chartsSensorGraphs[i].InvokeRequired)
            {
                SetResponseCallback d = new SetResponseCallback(this.SetGraphPoint);
                this.Invoke(d, new object[] { i, response });
            }
            else
            {
                foreach (Series series in this.chartsSensorGraphs[i].Series)
                {
                    series.Points.Add(new DataPoint((double)response.Time, response.ConvertData()));
                    this.CreateDataPointToolTip(series.Points[series.Points.Count - 1]);
                    this.chartsSensorGraphs[i].ChartAreas[0].AxisX.Maximum = response.Time;
                }

                this.chartsSensorGraphs[i].Width += 10;
                this.labelsSensorGraphsValues[i].Text = "Value: " + response.ConvertData();
            }
        }

        private void buttonInitialize_Click(object sender, EventArgs e)
        {
            try
            {

                this.controller.Serial.initialize(comboBoxComPort.Text);
                labelStatus.Text = comboBoxComPort.Text + " now open.";
                this.controller.Serial.sendCommand(STOP);
                Thread.Sleep(300);
                controller.Serial.flush();
                this.controller.Serial.sendCommand(REQCONF);
                Thread.Sleep(300);

                byte[] response = controller.Serial.dataReceived();

                while (response == null)
                    response = controller.Serial.dataReceived();

                controller.SensorData.loadData(response);

                if (comboBoxMeasurement.SelectedIndex == 1)
                {
                    controller.US = true;
                }

                
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Could not connect. Try again.";
                MessageBox.Show(ex.Message);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.controller.cancelAllThreads();

            if ((this.controller.SensorData.PollResponses == null) || (this.controller.SensorData.PollResponses.Count == 0))
            {
                MessageBox.Show("No data to save.");
                return;
            }

            saveFileDialog.Reset();

            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName == string.Empty)
            {
                return;
            }

            if (!saveFileDialog.CheckPathExists)
            {
                MessageBox.Show("The path does not exist.");
                return;
            }

            this.controller.SaveController.saveData(saveFileDialog.FileName);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Reset();

            openFileDialog.ShowDialog();

            if (openFileDialog.FileName == string.Empty)
            {
                return;
            }

            if (!openFileDialog.CheckFileExists)
            {
                MessageBox.Show("File does not exist.");
                return;
            }

            this.controller.cancelAllThreads();
            if(comboBoxMeasurement.SelectedIndex==1)
                controller.US = true;
            try
            {
                this.controller.LoadController.LoadData(openFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error, invalid file format." + ex.Message + ex.StackTrace);
            }
        }
        private void troubleCodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.controller.TcWindow.Show();
        }
        private void ConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.controller.ConfigSave.Show();
        }


        /// <summary>
        /// Implemented in response to Issue 12.
        /// Creates the string used for the mouse hover text (ToolTip) of a point.
        /// </summary>
        /// <param name="pt">The datapoint for which to set the ToolTip property.</param>
        private void CreateDataPointToolTip(DataPoint pt)
        {
            pt.ToolTip = "Value:\t" + pt.YValues[0].ToString() + "\nTime:\t" + controller.TimeOfDayConverter.get(pt.XValue) + " GMT" + "\nGPS:\t" + controller.Gps.get((uint)pt.XValue);
        }

        /// <summary>
        /// this function will load the data in SensorData.PollResponses into the corresponding graphs
        /// that have already been created 
        /// </summary>
        internal void LoadDataIntoSensorGraphs()
        {
            foreach (PollResponse response in this.controller.SensorData.PollResponses)
            {
                if (response.DataType == "OB")
                {
                    for (int i = 0; i < this.controller.SensorController.SelectedSensors.Length; ++i)
                    {
                        if ((response.Data.Length > 2) && (this.controller.SensorController.SelectedSensors[i].Pid == response.Data.Substring(0, 2)))
                        {
                            this.chartsSensorGraphs[i].Width += 10;
                            foreach (Series series in this.chartsSensorGraphs[i].Series)
                            {
                                try
                                {
                                    series.Points.Add(new DataPoint(response.Time, response.ConvertData()));
                                    this.CreateDataPointToolTip(series.Points[series.Points.Count - 1]);
                                    this.chartsSensorGraphs[i].ChartAreas[0].AxisX.Maximum = response.Time;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }

                            break;
                        }
                    }
                }
                else if (response.DataType == "AC")
                {
                    if (++acGraphCount == 6)
                    {
                        acGraphCount = 0;
                        for (int i = 0; i < this.controller.SensorController.SelectedSensors.Length; ++i)
                        {
                            if (this.controller.SensorController.SelectedSensors[i].Pid == response.DataType)
                            {
                                this.chartsSensorGraphs[i].Width += 10;
                                foreach (Series series in this.chartsSensorGraphs[i].Series)
                                {
                                    try
                                    {
                                        series.Points.Add(new DataPoint(response.Time, response.ConvertData()));
                                        this.CreateDataPointToolTip(series.Points[series.Points.Count - 1]);
                                        this.chartsSensorGraphs[i].ChartAreas[0].AxisX.Maximum = response.Time;
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Aligns all graphs by time, by making all graphs the same size and setting same min/max values
        /// </summary>
        internal void AlignAllGraphs()
        {
            int maxWidth = 0;
            double maxX = 0.0;

            for (int i = 0; i < this.chartsSensorGraphs.Length; ++i)
            {
                if (maxWidth < this.chartsSensorGraphs[i].Width)
                {
                    maxWidth = this.chartsSensorGraphs[i].Width;
                    maxX = this.chartsSensorGraphs[i].ChartAreas[0].AxisX.Maximum;
                }
            }

            for (int i = 0; i < this.chartsSensorGraphs.Length; ++i)
            {
                this.chartsSensorGraphs[i].Width = maxWidth;
                this.chartsSensorGraphs[i].ChartAreas[0].AxisX.Maximum = maxX;
            }
        }

        /// <summary>
        /// Handles the Click event to export the data to a *.csv spreadsheet, which is a fairly universal format
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void spreadsheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.controller.SensorController.SelectedSensors != null)
            {
                SaveFileDialog saveTextFileDiag = new SaveFileDialog();
                saveTextFileDiag.Filter = "CSV File (*.csv)|*.csv";
                saveTextFileDiag.Title = "Save Collected Data";
                saveTextFileDiag.ShowDialog();

                // If the file name is not an empty string open it for saving.
                if (saveTextFileDiag.FileName != string.Empty)
                {
                    try
                    {
                        StreamWriter sw = new StreamWriter(saveTextFileDiag.OpenFile());

                        sw.WriteLine("Sensor, Time, Value, Coordinate");

                        for (int i = 0; i < this.controller.SensorController.SelectedSensors.Length; ++i)
                        {
                            foreach (Series series in this.chartsSensorGraphs[i].Series)
                            {
                                if (series.ChartType.ToString() == "Point")
                                {
                                    foreach (DataPoint point in series.Points)
                                    {
                                        sw.WriteLine(this.controller.SensorController.SelectedSensors[i].Label + "," +
                                            controller.TimeOfDayConverter.get(point.XValue) + "," + point.YValues[0] + "," + controller.Gps.get((uint)point.XValue));
                                    }
                                }
                            }
                        }

                        sw.Close();
                    }
                    catch (Exception ex)
                    {
                        // Expected for file access exceptions
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("No data exists to export", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
       }

        /// <summary>
        /// Handles the Click event to export the data to a text file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void textFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.controller.SensorController.SelectedSensors != null)
            {
                // Displays a SaveFileDialog so the user can save the collected data
                SaveFileDialog saveTextFileDiag = new SaveFileDialog();
                saveTextFileDiag.Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
                saveTextFileDiag.Title = "Save Collected Data";
                saveTextFileDiag.ShowDialog();

                // If the file name is not an empty string open it for saving.
                if (saveTextFileDiag.FileName != string.Empty)
                {
                    try
                    {
                        StreamWriter sw = new StreamWriter(saveTextFileDiag.OpenFile());
                        string separatorAsterisk = "********************************************************************************";
                        string separatorDash = "--------------------------------------------------------------------------------";

                        for (int i = 0; i < this.controller.SensorController.SelectedSensors.Length; ++i)
                        {
                            sw.WriteLine(separatorAsterisk);
                            sw.WriteLine(this.controller.SensorController.SelectedSensors[i].Label);
                            sw.WriteLine(separatorDash);
                            sw.WriteLine("Time\t\tValue");
                            sw.WriteLine(separatorDash);

                            foreach (Series series in this.chartsSensorGraphs[i].Series)
                            {
                                if (series.ChartType.ToString() == "Point")
                                {
                                    foreach (DataPoint point in series.Points)
                                    {
                                        sw.WriteLine(controller.TimeOfDayConverter.get(point.XValue) + "\t\t" + point.YValues[0]);
                                    }
                                }
                            }
                        }

                        // Add GPS coordinate data
                        sw.WriteLine(separatorAsterisk);
                        sw.WriteLine("GPS Coordinates");
                        sw.WriteLine(separatorDash);
                        sw.WriteLine("Time\t\tValue");
                        sw.WriteLine(separatorDash);
                        foreach (GPSCoordinate coordinate in controller.Gps.GpsList)
                        {
                            sw.WriteLine(controller.TimeOfDayConverter.get(coordinate.Time) + "\t\t" + coordinate);
                        }

                        sw.Close();
                    }
                    catch (Exception ex)
                    {
                        // Expected for file access exceptions
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("No data exists to export", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void addConfigPanel()
        {

            ConfigPanel config = new ConfigPanel();
            string[] names = (string[]) sensornames.ToArray(typeof(string));
            string[] pid = (string[]) pids.ToArray(typeof(string));
            
            config.Popcbo(names,pid);
            config.Location = new Point(0, height);
            config.Width = this.Width;
            config.Click += new System.EventHandler(ConfigPanel_Click);
            this.panelSensorSelection.Controls.Add(config);
            this.configs.Add(config);
            height += 25;
            
        }
        private void ConfigPanel_Click(object sender, EventArgs e)
        {
            int index;
            ConfigPanel current= (ConfigPanel)sender;
            current.Add_Click();
            if (current.Selected != "00" && !current.Remove)
            {
                index = pids.IndexOf((string)current.Selected);
                pids.RemoveAt(index);
                sensornames.RemoveAt(index);
                addConfigPanel();
            }
            else if (current.Remove)
            {
                pids.Add((string)current.Selected);
                sensornames.Add((string)current.name1);
                configs.Remove(current);
                this.panelSensorSelection.Controls.Remove(current);
                height = StartHeight;
                ConfigPanel[] list = (ConfigPanel[]) configs.ToArray(typeof(ConfigPanel));
                for(int i=0;i<list.Length;i++)
                {
                    list[i].Location = new Point(0, height);
                    height += 25;
                }
                string[] pid = (string[])pids.ToArray(typeof(string));
                list[list.Length-1].Popcbo(current.name1, pid);
            }
        }
        private void Resizing(object sender, EventArgs e)
        {
            panelSensorSelection.Width = this.Width;
            panelSensorSelection.Height = this.Height - (menuStrip.Height + comboBoxMeasurement.Height + labelSensorData.Height + 50);
            height = StartHeight;
            ConfigPanel[] list = (ConfigPanel[])configs.ToArray(typeof(ConfigPanel));
            for (int i = 0; i < list.Length; i++)
            {
                list[i].Location = new Point(0, height);
                height += 25;
            }
        }
        private void saveConfig()
        {
            int i = 0;
            ConfigPanel[] list = (ConfigPanel[])configs.ToArray(typeof(ConfigPanel));
            for (i = 0; i < controller.Config.Length; i++)
            {
                controller.Config[i] = (byte)(controller.Config[i] & 131);
            }
            for(i=0;i<list.Length;i++)
            {
                byte temptiming = (byte)(list[i].Timing << 2);
                controller.Config[byte.Parse(list[i].Selected)] = (byte)(temptiming | controller.Config[byte.Parse(list[i].Selected)]);
            }
        }

    }
}

