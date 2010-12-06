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
    using System.Collections.ObjectModel;
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
        /// <summary>
        /// Updating graph plot thread
        /// </summary>
        internal Thread UpdateGraphPlots;

        /// <summary>
        /// Maximum length of time in ms that live data can be collected in one run.
        /// </summary>
        private const int LiveGraphLength = 300000;

        /// <summary>
        /// Start height of form windows
        /// </summary>
        private static int startHeight = 4;

        /// <summary>
        /// Start width of form windows
        /// </summary>
        private static int startWidth = 4;

        /// <summary>
        /// String to send the microprocessor code to start collecting
        /// </summary>
        private static string collectCommand = "g";

        /// <summary>
        /// String to send the microprocessor code to stop collecting
        /// </summary>
        private static string stopCommand = "s";

        /// <summary>
        /// String to send the microprocessor to request the config file
        /// </summary>
        private static string requestConfigCommand = "r";

        /// <summary>
        /// Height attribute when altering sensors contained on config panel
        /// </summary>
        private int heightConfigPanel;

        /// <summary>
        /// Sensor names for the config panel
        /// </summary>
        private ArrayList sensorNames;

        /// <summary>
        /// pids for the config panel
        /// </summary>
        private ArrayList pids;

        /// <summary>
        /// List of sensors specified on the config panel
        /// </summary>
        private ArrayList configPanelSensors;

        /// <summary>
        /// Reference to the Controller in the MVC design pattern.
        /// </summary>
        private Controller controller;

        /// <summary>
        /// Array of labels for the sensor graphs.
        /// </summary>
        private Label[] labelsSensorGraphs;

        /// <summary>
        /// Array of labels for the values in each sensor graph.
        /// </summary>
        private Label[] labelsSensorGraphsValues;

        /// <summary>
        /// Array of sensor graph charts
        /// </summary>
        private Chart[] chartsSensorGraphs;

        /// <summary>
        /// The graph queue
        /// </summary>
        private Queue graphQueue = new Queue();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.controller = new Controller();
            this.controller.MainWindow = this;
            InitializeComponent();
            this.sensorNames = new ArrayList();
            this.pids = new ArrayList();
            this.configPanelSensors = new ArrayList();
            panelSensorGraphs.Visible = false;
            comboBoxComPort.SelectedIndex = 3;
            comboBoxMeasurement.SelectedIndex = 0;
            this.Resize += new System.EventHandler(this.Resizing);
            this.troubleCodesToolStripMenuItem.Enabled = false;
            buttonGet.Enabled = false;
            this.DisableTcWindow();
        }

        /// <summary>
        /// Delegate for maintaining thread safety while adding graph highlights.
        /// </summary>
        /// <param name="i">The graph number currently being higlighted.</param>
        /// <param name="highlightToBeAdded">The actual highlight.</param>
        private delegate void SetResponseCallbackHighlight(int i, StripLine highlightToBeAdded);

        /// <summary>
        /// Delegate for maintaining thread safety while setting a graph point.
        /// </summary>
        /// <param name="i">The iterator.</param>
        /// <param name="response">The response.</param>
        private delegate void SetResponseCallbackGraph(int i, PollResponse response);

        /// <summary>
        /// Gets the graph queue.
        /// </summary>
        /// <value>The graph queue.</value>
        public Queue GraphQueue
        {
            get { return this.graphQueue; }
        }

        /// <summary>
        /// Populates the selection window.
        /// </summary>
        public void PopulateSelectionWindow()
        {
            this.panelSensorSelection.Controls.Clear();
            this.heightConfigPanel = startHeight;
            if (this.sensorNames != null)
            {
                this.sensorNames.Clear();
                this.pids.Clear();
            }

            for (int i = 0; i < this.controller.SensorController.Sensors.Length; i++)
            {
                byte add = 0;
                if (this.controller.SensorController.Sensors[i].Pid != "AC")
                {
                    add = (byte)(this.controller.Config[byte.Parse(this.controller.SensorController.Sensors[i].Pid, System.Globalization.NumberStyles.HexNumber)] >> 7);
                    if (add == 1)
                    {
                        if (this.controller.SensorController.Sensors[i].Label2 == null)
                        {
                            this.sensorNames.Add(this.controller.SensorController.Sensors[i].Label);
                        }
                        else
                        {
                            this.sensorNames.Add(this.controller.SensorController.Sensors[i].Label + "," +
                                            this.controller.SensorController.Sensors[i].Label2.Substring(this.controller.SensorController.Sensors[i].Label2.LastIndexOf(':') + 1));
                        }

                        this.pids.Add(this.controller.SensorController.Sensors[i].Pid);
                    }
                }
            }
            
            this.AddConfigPanel();
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
                    else if (response.DataType == "AC")
                    {
                        for (int i = 0; i < this.controller.SensorController.SelectedSensors.Length; ++i)
                        {
                            try
                            {
                                if (response.DataType == this.controller.SensorController.SelectedSensors[i].Pid)
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
        /// Adds a graph highlight to all graphs.
        /// </summary>
        /// <param name="time">The time to display the highlight.</param>
        public void AddGraphHighlight(uint time)
        {
            StripLine highlightToBeAdded = new StripLine();

            for (int i = 0; i < this.controller.SensorController.SelectedSensors.Length; ++i)
            {
                // A new StripLine is required for each graph; reusing the same StripLine eventually leads to display problems
                highlightToBeAdded = new StripLine();
                highlightToBeAdded.BackColor = Color.Yellow;
                highlightToBeAdded.IntervalOffset = time;
                highlightToBeAdded.StripWidth = 1000;
                this.AddGraphHighlightAuxilliary(i, highlightToBeAdded);
            }
        }

        /// <summary>
        /// This function will take the integers in numselected and add the a graph for each of the indexes that
        /// numsSelected represents  in the Sensors table of SensorController.
        /// This function assumes that the SelectedSensors member of SensorController has already been intialized and set to the indexes that
        /// numsSelected represents
        /// </summary>
        internal void PopulateGraphWindow()
        {
            this.panelSensorGraphs.Controls.Clear();
            this.labelsSensorGraphs = new Label[this.controller.SensorController.SelectedSensors.Length];
            this.labelsSensorGraphsValues = new Label[this.controller.SensorController.SelectedSensors.Length];
            this.chartsSensorGraphs = new Chart[this.controller.SensorController.SelectedSensors.Length];

            ChartArea[] chartAreas = new ChartArea[this.controller.SensorController.SelectedSensors.Length];
            Series[] seriesLines = new Series[this.controller.SensorController.SelectedSensors.Length];
            Series[] seriesPoints = new Series[this.controller.SensorController.SelectedSensors.Length];

            int heightP = startHeight;
            int widthP = startWidth;

            TickMark tickMark = new TickMark();

            for (int i = 0; i < this.controller.SensorController.SelectedSensors.Length; ++i)
            {
                this.labelsSensorGraphs[i] = new Label();
                this.labelsSensorGraphs[i].AutoSize = true;
                this.labelsSensorGraphs[i].Text = this.controller.SensorController.SelectedSensors[i].Label + " ( " + this.controller.SensorController.ReturnUnit(i) + " ) ";
                this.labelsSensorGraphs[i].Location = new Point(widthP, heightP + (200 * i));
                this.panelSensorGraphs.Controls.Add(this.labelsSensorGraphs[i]);

                this.labelsSensorGraphsValues[i] = new Label();
                this.labelsSensorGraphsValues[i].AutoSize = true;
                this.labelsSensorGraphsValues[i].Text = string.Empty;
                this.labelsSensorGraphsValues[i].Location = new Point(widthP + this.labelsSensorGraphs[i].Size.Width + 5, heightP + (200 * i));
                this.panelSensorGraphs.Controls.Add(this.labelsSensorGraphsValues[i]);

                this.chartsSensorGraphs[i] = new Chart();
                chartAreas[i] = new ChartArea();
                chartAreas[i].Name = "Chart" + i;
                chartAreas[i].AlignmentStyle = AreaAlignmentStyles.All;
                chartAreas[i].AxisX.IsReversed = true;
                chartAreas[i].CursorX.IsUserEnabled = true;
                chartAreas[i].CursorX.Position = 0;

                // While "InsideArea" may seem like the reverse of what we want, the Double.MaxValue Crossing reverses the meaning of Inside and Outside area.
                chartAreas[i].AxisX.Crossing = Double.MaxValue;
                tickMark = chartAreas[i].AxisY.MajorTickMark;
                tickMark.TickMarkStyle = TickMarkStyle.InsideArea;
                tickMark.Size = 0;
                chartAreas[i].AxisY.MajorTickMark = tickMark;

                // add units to the graphs
                chartAreas[i].AxisX.Title = "ms (starting at zero) (hover mouse over point for time of day)";
                chartAreas[i].AxisY.Title = this.controller.SensorController.ReturnUnit(i);
                chartAreas[i].AxisX.TitleAlignment = StringAlignment.Near;

                seriesLines[i] = new Series();

                seriesPoints[i] = new Series();

                this.chartsSensorGraphs[i].ChartAreas.Add(chartAreas[i]);
                seriesLines[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                seriesPoints[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                this.chartsSensorGraphs[i].Series.Add(seriesLines[i]);
                this.chartsSensorGraphs[i].Series.Add(seriesPoints[i]);
                this.chartsSensorGraphs[i].Location = new System.Drawing.Point(23, 50);
                this.chartsSensorGraphs[i].Size = new System.Drawing.Size(170, 170);
                this.chartsSensorGraphs[i].Location = new Point(widthP, heightP + (200 * i) + 25);
                this.panelSensorGraphs.Controls.Add(this.chartsSensorGraphs[i]);
            }

            for (int i = 0; i < this.chartsSensorGraphs.Length; i++)
            {
                this.chartsSensorGraphs[i].MouseClick += new MouseEventHandler(this.OnChart_MouseClick);
            }
        }

        /// <summary>
        /// Shows the reset button.
        /// </summary>
        internal void ShowResetButton()
        {
            buttonCollect.Text = "Reset";
            this.controller.CancelAllThreads();
        }

        /// <summary>
        /// Shows the sensor selection panel.
        /// </summary>
        internal void ShowSensorSelectionPanel()
        {
            MapButton.Enabled = false;
            buttonGet.Enabled = false;
            this.panelSensorGraphs.Controls.Clear();
            this.panelSensorGraphs.Visible = false;
            this.panelSensorSelection.Visible = true;
        }

        /// <summary>
        /// Shows the sensor data panel.
        /// </summary>
        internal void ShowSensorDataPanel()
        {
            this.panelSensorSelection.Visible = false;
            this.panelSensorGraphs.Visible = true;
        }

        /// <summary>
        /// Starts the graph plot thread.
        /// </summary>
        internal void StartGraphPlotThread()
        {
            this.UpdateGraphPlots = new Thread(new ThreadStart(this.UpdateGraphs));
            this.UpdateGraphPlots.Name = "UpdateGraphs";
            this.UpdateGraphPlots.Start();
        }

        /// <summary>
        /// this function will load the data in SensorData.PollResponses into the corresponding graphs
        /// that have already been created
        /// </summary>
        internal void LoadDataIntoSensorGraphs()
        {
            this.ResetGraphs();

            foreach (PollResponse response in this.controller.SensorData.PollResponses)
            {
                if (response.DataType == "OB" && response.Data.Length > 2)
                {
                    for (int i = 0; i < this.controller.SensorController.SelectedSensors.Length; ++i)
                    {
                        if (this.controller.SensorController.SelectedSensors[i].Pid == response.Data.Substring(0, 2))
                        {
                            this.chartsSensorGraphs[i].Width += 10;
                            foreach (Series series in this.chartsSensorGraphs[i].Series)
                            {
                                try
                                {
                                    series.Points.Add(new DataPoint(response.Time, response.ConvertData()));
                                    this.CreateDataPointToolTip(series.Points[series.Points.Count - 1], this.controller.SensorController.ReturnUnit(i));
                                    this.chartsSensorGraphs[i].ChartAreas[0].AxisX.Maximum = response.Time;
                                }
                                catch (Exception e)
                                {
                                }
                            }

                            break;
                        }
                    }
                }
                else if (response.DataType == "AC")
                {
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
                                    this.CreateDataPointToolTip(series.Points[series.Points.Count - 1], this.controller.SensorController.ReturnUnit(i));
                                    this.chartsSensorGraphs[i].ChartAreas[0].AxisX.Maximum = response.Time;
                                }
                                catch (Exception e)
                                {
                                }
                            }

                            break;
                        }
                    }
                }
                else if (response.DataType == "MK")
                {
                    this.AddGraphHighlight(response.Time);
                }
            }

            buttonGet.Enabled = true;
        }

        /// <summary>
        /// Sets the displayed X-axis graph range for all graphs.
        /// This method also causes all graphs to line up.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        internal void SetDisplayedGraphRange(uint startTime, uint endTime)
        {
            // This value inversely affects the spacing between graph points. Set lower to increase spacing, set higher for the reverse.
            const int Scale = 90;

            // The largest between minimumWidth/scale and the calculated appropriate width based on the timespan will be the width of all graphs
            const int MinimumWidth = 70000;

            for (int i = 0; i < this.chartsSensorGraphs.Length; ++i)
            {
                this.chartsSensorGraphs[i].ChartAreas[0].AxisX.Minimum = startTime;
                this.chartsSensorGraphs[i].ChartAreas[0].AxisX.Maximum = endTime;
                this.chartsSensorGraphs[i].Width = (int)Math.Max(endTime - startTime, MinimumWidth) / Scale;
            }
        }

        /// <summary>
        /// Sets the total ms label.
        /// </summary>
        /// <param name="p">The value in ms.</param>
        internal void SetTotalMsLabel(uint p)
        {
            labelTotalMs.Text = "Total (ms): " + p;
        }

        /// <summary>
        /// Sets the start time and end time displayed on all visible graphs.
        /// </summary>
        /// <param name="startTime">The start time in ms.</param>
        /// <param name="endTime">The end time in ms.</param>
        internal void SetStartTimeEndTime(uint startTime, uint endTime)
        {
            textBoxStartTime.Text = string.Empty + startTime;
            textBoxEndTime.Text = string.Empty + endTime;
        }

        /// <summary>
        /// Gets the name of the directory to store a split log file.
        /// </summary>
        /// <returns>The path name.</returns>
        internal string GetSplitDirectoryName()
        {
            MessageBox.Show("Multiple sessions exist, press ok to split up the file.");
            folderBrowserDialog.ShowDialog();

            return folderBrowserDialog.SelectedPath;
        }

        /// <summary>
        /// Called when any chart is clicked.
        /// Current intention is to clear the cursorX on all of the charts that were not clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void OnChart_MouseClick(object sender, MouseEventArgs e)
        {
            foreach (Chart chart in this.chartsSensorGraphs)
            {
                if (sender != chart)
                {
                    chart.ChartAreas[0].CursorX.Position = 0.0;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the ButtonCollect control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonCollect_Click(object sender, EventArgs e)
        {
            if (buttonCollect.Text == "Collect Data")
            {
                if (comboBoxMeasurement.SelectedIndex == 1)
                {
                    this.controller.US = true;
                }
                else
                {
                    this.controller.US = false;
                }

                buttonGet.Enabled = false;
                troubleCodesToolStripMenuItem.Enabled = false;
                this.controller.TcWindow.Hide();
                comboBoxMeasurement.Enabled = false;
                this.controller.Reset();

                ArrayList numsSelected = new ArrayList();

                if (this.configPanelSensors.Count == 0)
                {
                    MessageBox.Show("Please configure the sensors you want polled.");
                    return;
                }

                this.configPanelSensors.RemoveAt(this.configPanelSensors.Count - 1);
                for (int i = 0; i < this.configPanelSensors.Count; ++i)
                {
                    ConfigPanel c = (ConfigPanel)this.configPanelSensors[i];
                    for (int j = 0; j < this.controller.SensorController.Sensors.Length; j++)
                    {
                        if (c.Selected == this.controller.SensorController.Sensors[j].Pid)
                        {
                            numsSelected.Add(j);
                        }
                    }
                }

                for (int i = 0; i < this.controller.SensorController.Sensors.Length; ++i)
                {
                    if (this.controller.SensorController.Sensors[i].Pid == "AC")
                    {
                        // add a default AC graph
                        numsSelected.Add(i);
                        break;
                    }
                }

                this.controller.SensorController.InitializeSelectedSensors(numsSelected);
                this.SaveConfig();
                this.controller.Serial.SendConfig();
                this.controller.SensorController.InitializeReceivingThreads();
                this.controller.Serial.SendCommand(collectCommand);
                this.PopulateGraphWindow();
                this.ShowSensorDataPanel();
                this.buttonCollect.Text = "Stop";
                this.MapButton.Enabled = false;
                this.StartGraphPlotThread();
            }
            else if (buttonCollect.Text == "Stop")
            {
                troubleCodesToolStripMenuItem.Enabled = true;
                comboBoxMeasurement.Enabled = true;
                this.ShowResetButton();
                this.controller.Serial.SendCommand(stopCommand);
                this.controller.CancelAllThreads();
                this.ResetGraphs();
                if ((this.controller.SensorData.PollResponses != null) && (this.controller.SensorData.PollResponses.Count != 0))
                {
                    uint endTime = ((PollResponse)this.controller.SensorData.PollResponses[this.controller.SensorData.PollResponses.Count - 1]).Time;
                    this.SetTotalMsLabel(endTime);
                    if (endTime > LoadController.DefaultEndTime)
                    {
                        endTime = LoadController.DefaultEndTime;
                    }

                    this.LoadDataIntoSensorGraphs();

                    // ensure that the size of all graphs are the same so they line up
                    this.controller.MainWindow.SetDisplayedGraphRange(0, endTime);
                    this.SetStartTimeEndTime(0, endTime);
                    this.MapButton.Enabled = true;
                }
            }
            else if (buttonCollect.Text == "Reset")
            {
                this.controller.Serial.SendCommand(requestConfigCommand);
                Thread.Sleep(300);
                this.configPanelSensors.Clear();
                if (comboBoxMeasurement.SelectedIndex == 1)
                {
                    this.controller.US = true;
                }
                else
                {
                    this.controller.US = false;
                }

                this.PopulateSelectionWindow();
                buttonCollect.Text = "Collect Data";
                this.ShowSensorSelectionPanel();
            }
        }

        /// <summary>
        /// Sets the graph point.
        /// </summary>
        /// <param name="i">The point index.</param>
        /// <param name="response">The response from the microcontroller.</param>
        private void SetGraphPoint(int i, PollResponse response)
        {
            if (!this.UpdateGraphPlots.IsAlive)
            {
                return;
            }

            if (this.chartsSensorGraphs[i].InvokeRequired)
            {
                SetResponseCallbackGraph d = new SetResponseCallbackGraph(this.SetGraphPoint);
                this.Invoke(d, new object[] { i, response });
            }
            else
            {
                if (response.ConvertData() != null)
                {
                    foreach (Series series in this.chartsSensorGraphs[i].Series)
                    {
                        while ((series.Points.Count > 0) && ((series.Points[series.Points.Count - 1].XValue - series.Points[0].XValue) > LiveGraphLength))
                        {
                            series.Points.RemoveAt(0);
                        }

                        series.Points.Add(new DataPoint((double)response.Time, response.ConvertData()));
                        this.CreateDataPointToolTip(series.Points[series.Points.Count - 1], this.controller.SensorController.ReturnUnit(i));
                        this.chartsSensorGraphs[i].ChartAreas[0].AxisX.Maximum = response.Time;
                    }

                    this.chartsSensorGraphs[i].Width = (this.chartsSensorGraphs[i].Series[0].Points.Count * 10) + 170;
                    this.labelsSensorGraphsValues[i].Text = "Value: " + response.ConvertData();
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the ButtonInitialize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonInitialize_Click(object sender, EventArgs e)
        {
            try
            {
                this.controller.Serial.Initialize(comboBoxComPort.Text);
                labelStatus.Text = comboBoxComPort.Text + " now open.";
                this.controller.Serial.SendCommand(stopCommand);
                Thread.Sleep(300);
                this.controller.Serial.Flush();
                this.controller.Serial.SendCommand(requestConfigCommand);
                Thread.Sleep(300);

                byte[] response = this.controller.Serial.DataReceived();
                int loop = 0;
                while (response == null)
                {
                    response = this.controller.Serial.DataReceived();
                    Thread.Sleep(300);
                    if (loop > 10)
                    {
                        MessageBox.Show("Could not connect with the ObdiiMonitor.  Please press reset and wait for the lights to stop counting.");
                        labelStatus.Text = comboBoxComPort.Text + " could not connect.";
                        return;
                    }

                    loop++;
                }

                this.controller.LoadController.LoadData(response);

                labelStatus.Text = comboBoxComPort.Text + " successfully connected.";

                if (comboBoxMeasurement.SelectedIndex == 1)
                {
                    this.controller.US = true;
                }

                this.EnableTcWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not connect with the ObdiiMonitor.  Please press reset and wait for the lights to stop counting.");
                labelStatus.Text = comboBoxComPort.Text + " could not connect.";
                return;
            }
        }

        /// <summary>
        /// Handles the Click event of the SaveToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.controller.CancelAllThreads();

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

            this.controller.SaveController.SaveData(saveFileDialog.FileName, 0, this.controller.SensorData.PollResponses.Count);
        }

        /// <summary>
        /// Handles the Click event of the LoadToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
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

            this.controller.CancelAllThreads();
            if (comboBoxMeasurement.SelectedIndex == 1)
            {
                this.controller.US = true;
            }
            else
            {
                this.controller.US = false;
            }

            try
            {
                this.controller.LoadController.LoadData(openFileDialog.FileName);
                this.MapButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error, invalid file format." + ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// Handles the Click event of the TroubleCodesToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TroubleCodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.controller.TcWindow.Show();
        }

        /// <summary>
        /// Handles the Click event of the ConfigToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.controller.ConfigSave.Show();
        }

        /// <summary>
        /// Implemented in response to Issue 12.
        /// Creates the string used for the mouse hover text (ToolTip) of a point.
        /// </summary>
        /// <param name="pt">The datapoint for which to set the ToolTip property.</param>
        /// <param name="unit">The unit (e.g., s, km, etc.)</param>
        private void CreateDataPointToolTip(DataPoint pt, string unit)
        {
            pt.ToolTip = "Value:\t" + pt.YValues[0].ToString() + " " + unit + "\nTime:\t" + this.controller.TimeOfDayConverter.Get(pt.XValue) + " GMT" + "\nGPS:\t" + this.controller.Gps.Get((uint)pt.XValue);
        }

        /// <summary>
        /// Ensures thread safety when highlighting multiple maps
        /// </summary>
        /// <param name="i">The graph number currently being highlighted.</param>
        /// <param name="highlightToBeAdded">The highlight to be added.</param>
        private void AddGraphHighlightAuxilliary(int i, StripLine highlightToBeAdded)
        {
            if (this.chartsSensorGraphs[i].InvokeRequired)
            {
                SetResponseCallbackHighlight d = new SetResponseCallbackHighlight(this.AddGraphHighlightAuxilliary);
                this.Invoke(d, new object[] { i, highlightToBeAdded });
            }
            else
            {
                this.chartsSensorGraphs[i].ChartAreas[0].AxisX.StripLines.Add(highlightToBeAdded);
            }
        }

        /// <summary>
        /// Handles the Click event to export the data to a *.csv spreadsheet, which is a fairly universal format
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SpreadsheetToolStripMenuItem_Click(object sender, EventArgs e)
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
                                            this.controller.TimeOfDayConverter.Get(point.XValue) + "," + point.YValues[0] + "," + this.controller.Gps.Get((uint)point.XValue));
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
        private void TextFileToolStripMenuItem_Click(object sender, EventArgs e)
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
                                        sw.WriteLine(this.controller.TimeOfDayConverter.Get(point.XValue) + "\t\t" + point.YValues[0]);
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
                        foreach (GPSCoordinate coordinate in this.controller.Gps.GpsList)
                        {
                            sw.WriteLine(this.controller.TimeOfDayConverter.Get(coordinate.Time) + "\t\t" + coordinate);
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
        /// Adds the config panel.
        /// </summary>
        private void AddConfigPanel()
        {
            ConfigPanel config = new ConfigPanel();
            string[] names = (string[])this.sensorNames.ToArray(typeof(string));
            string[] pid = (string[])this.pids.ToArray(typeof(string));

            config.Popcbo(names, pid);
            config.Location = new Point(0, this.heightConfigPanel);
            config.Width = this.Width;
            config.Click += new System.EventHandler(this.ConfigPanel_Click);
            this.panelSensorSelection.Controls.Add(config);
            this.configPanelSensors.Add(config);
            this.heightConfigPanel += 25;
        }

        /// <summary>
        /// Handles the Click event of the ConfigPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ConfigPanel_Click(object sender, EventArgs e)
        {
            int index;
            ConfigPanel current = (ConfigPanel)sender;
            current.Add_Click();
            if (current.Selected != "00" && !current.Remove)
            {
                index = this.pids.IndexOf((string)current.Selected);
                this.pids.RemoveAt(index);
                this.sensorNames.RemoveAt(index);
                this.AddConfigPanel();
            }
            else if (current.Remove)
            {
                this.pids.Add((string)current.Selected);
                this.sensorNames.Add((string)current.name1);
                this.configPanelSensors.Remove(current);
                this.panelSensorSelection.Controls.Remove(current);
                this.heightConfigPanel = startHeight;
                ConfigPanel[] list = (ConfigPanel[])this.configPanelSensors.ToArray(typeof(ConfigPanel));
                for (int i = 0; i < list.Length; i++)
                {
                    list[i].Location = new Point(0, this.heightConfigPanel);
                    this.heightConfigPanel += 25;
                }

                string[] pid = (string[])this.pids.ToArray(typeof(string));
                list[list.Length - 1].Popcbo(current.name1, pid);
            }
        }

        /// <summary>
        /// Called when the window is resized
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Resizing(object sender, EventArgs e)
        {
            panelSensorSelection.Width = this.Width;
            panelSensorSelection.Height = this.Height - (menuStrip.Height + comboBoxMeasurement.Height + labelSensorData.Height + 50);
            this.heightConfigPanel = startHeight;
            ConfigPanel[] list = (ConfigPanel[])this.configPanelSensors.ToArray(typeof(ConfigPanel));
            for (int i = 0; i < list.Length; i++)
            {
                list[i].Location = new Point(0, this.heightConfigPanel);
                this.heightConfigPanel += 25;
            }
        }

        /// <summary>
        /// Saves the config file
        /// </summary>
        private void SaveConfig()
        {
            int i = 0;
            ConfigPanel[] list = (ConfigPanel[])this.configPanelSensors.ToArray(typeof(ConfigPanel));
            for (i = 0; i < this.controller.Config.Length; i++)
            {
                this.controller.Config[i] = (byte)(this.controller.Config[i] & 131);
            }

            for (i = 0; i < list.Length; i++)
            {
                byte temptiming = (byte)(list[i].Timing << 2);
                this.controller.Config[byte.Parse(list[i].Selected, System.Globalization.NumberStyles.HexNumber)] = (byte)(temptiming | this.controller.Config[byte.Parse(list[i].Selected, System.Globalization.NumberStyles.HexNumber)]);
            }
        }

        /// <summary>
        /// Handles the Click event of the ButtonGet control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonGet_Click(object sender, EventArgs e)
        {
            try 
            {
                uint startTime = uint.Parse(textBoxStartTime.Text), endTime = uint.Parse(textBoxEndTime.Text);

                if (endTime <= startTime)
                {
                    MessageBox.Show("The start time must be less than the end time.");
                    return;
                }

                this.SetDisplayedGraphRange(startTime, endTime);
            }
            catch 
            {
                MessageBox.Show("Please enter integers values between 0 and the maximum (ms) time.");
            }
        }

        /// <summary>
        /// Resets the graphs.
        /// </summary>
        private void ResetGraphs() 
        {
            foreach (Chart chart in this.chartsSensorGraphs)
            {
                chart.Size = new System.Drawing.Size(170, 170);
                foreach (Series series in chart.Series)
                {
                    series.Points.Clear();
                }
            }
        }

        /// <summary>
        /// Enables the Trouble Code window.
        /// </summary>
        private void EnableTcWindow()
        {
            troubleCodesToolStripMenuItem.Enabled = true;
            buttonCollect.Enabled = true;
            buttonInitialize.Enabled = false;
            comboBoxComPort.Enabled = false;
            this.controller.TcWindow.Enable();
        }

        /// <summary>
        /// Disables the Trouble Code window.
        /// </summary>
        private void DisableTcWindow()
        {
            buttonCollect.Enabled = false;
            buttonInitialize.Enabled = true;
            comboBoxComPort.Enabled = true;
            this.controller.TcWindow.Disable();
        }

        /// <summary>
        /// Standard "exit" functionality -- exits the form
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the MapButton control.
        /// Will map the point in time determined by the CursorX (horizontal cursor) of a visible graph.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MapButton_Click(object sender, EventArgs e)
        {
            foreach (Chart chart in this.chartsSensorGraphs)
            {
                if (chart.ChartAreas[0].CursorX.Position != 0.0)
                {
                    this.MapGpsCoordinate(this.controller.Gps.Get((uint)chart.ChartAreas[0].CursorX.Position));
                }
            }
        }

        /// <summary>
        /// Maps the GPS coordinate using Google Maps
        /// </summary>
        /// <param name="g">The GPS Coordinate.</param>
        private void MapGpsCoordinate(GPSCoordinate g)
        {
            if (g != null)
            {
                // Create URL.
                string url = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + g.ToString();

                // Open URL in default browser.
                System.Diagnostics.Process.Start(url);
            }
            else
            {
                MessageBox.Show("No GPS Coordinate present to Map.");
            }

            return;
        }
    }
}

