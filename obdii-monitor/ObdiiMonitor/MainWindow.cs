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
        private static string COLLECT = "s";
        private static string STOP = "g";

        Controller controller;

        CheckBox[] checkboxesSensorSelection;

        Label[] labelsSensorSelection;

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
            this.PopulateSelectionWindow();
            panelSensorGraphs.Visible = false;
            comboBoxBaudRate.SelectedIndex = 1;
            comboBoxComPort.SelectedIndex = 3;
        }

        /// <summary>
        /// Populates the selection window.
        /// </summary>
        private void PopulateSelectionWindow()
        {
            this.panelSensorSelection.Controls.Clear();

            this.labelsSensorSelection = new Label[this.controller.SensorController.Sensors.Length];
            this.checkboxesSensorSelection = new CheckBox[this.controller.SensorController.Sensors.Length];

            int height = StartHeight;
            int width = StartWidth;

            for (int i = 0; i < this.controller.SensorController.Sensors.Length; ++i)
            {
                this.labelsSensorSelection[i] = new Label();
                this.labelsSensorSelection[i].Text = this.controller.SensorController.Sensors[i].Label;
                this.labelsSensorSelection[i].Location = new Point(width + 20, height + (25 * i) + 5);
                this.panelSensorSelection.Controls.Add(this.labelsSensorSelection[i]);

                this.checkboxesSensorSelection[i] = new CheckBox();
                this.checkboxesSensorSelection[i].Location = new Point(width, height + (25 * i));
                this.checkboxesSensorSelection[i].Checked = true;
                this.panelSensorSelection.Controls.Add(this.checkboxesSensorSelection[i]);
            }
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
                ArrayList numsSelected = new ArrayList();
                for (int i = 0; i < this.checkboxesSensorSelection.Length; ++i)
                {
                    if (this.checkboxesSensorSelection[i].Checked)
                    {
                        numsSelected.Add(i);
                    }
                }
                
 //               this.controller.SensorController.initializeSelectedSensors(numsSelected);
                this.controller.Serial.sendCommand("r");
                this.controller.SensorController.initializePollingReceivingThreads();
                this.controller.SensorData.clearPollResponses();
                this.PopulateGraphWindow(numsSelected);
                this.ShowSensorDataPanel();
                buttonCollect.Text = "Stop";
                this.StartGraphPlotThread();
            }
            else if (buttonCollect.Text == "Stop")
            {
                this.ShowResetButton();
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
                }

                this.chartsSensorGraphs[i].Size = new System.Drawing.Size(this.chartsSensorGraphs[i].Size.Width + 15, this.chartsSensorGraphs[i].Size.Height);
                this.labelsSensorGraphsValues[i].Text = "Value: " + response.ConvertData();
            }
        }

        private void buttonInitialize_Click(object sender, EventArgs e)
        {
            try
            {
                this.controller.Serial.initialize(comboBoxBaudRate.Text, comboBoxComPort.Text);
                labelStatus.Text = comboBoxComPort.Text + " now open.";
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
                            this.chartsSensorGraphs[i].Size = new Size(this.chartsSensorGraphs[i].Size.Width + 10, this.chartsSensorGraphs[i].Size.Height);
                            foreach (Series series in this.chartsSensorGraphs[i].Series)
                            {
                                try
                                {
                                    series.Points.Add(new DataPoint(response.Time, response.ConvertData()));
                                    this.CreateDataPointToolTip(series.Points[series.Points.Count - 1]);
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
                    for (int i = 0; i < this.controller.SensorController.SelectedSensors.Length; ++i)
                    {
                        if (this.controller.SensorController.SelectedSensors[i].Pid == response.DataType)
                        {
                            this.chartsSensorGraphs[i].Size = new Size(this.chartsSensorGraphs[i].Size.Width + 10, this.chartsSensorGraphs[i].Size.Height);
                            foreach (Series series in this.chartsSensorGraphs[i].Series)
                            {
                                try
                                {
                                    series.Points.Add(new DataPoint(response.Time, response.ConvertData()));
                                        this.CreateDataPointToolTip(series.Points[series.Points.Count - 1]);
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
                    StreamWriter sw = new StreamWriter(saveTextFileDiag.OpenFile());

                    sw.WriteLine("Sensor, Time, Value");

                    for (int i = 0; i < this.controller.SensorController.SelectedSensors.Length; ++i)
                    {
                        foreach (Series series in this.chartsSensorGraphs[i].Series)
                        {
                            if (series.ChartType.ToString() == "Point")
                            {
                                foreach (DataPoint point in series.Points)
                                {
                                    sw.WriteLine(this.controller.SensorController.SelectedSensors[i].Label + "," +
                                        point.XValue + "," + point.YValues[0]);
                                }
                            }
                        }
                    }

                    sw.Close();
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
                                    sw.WriteLine(point.XValue + "\t\t" + point.YValues[0]);
                                }
                            }
                        }
                    }

                    sw.Close();
                }
            }
            else
            {
                MessageBox.Show("No data exists to export", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}

