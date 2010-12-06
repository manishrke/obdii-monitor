//-----------------------------------------------------------------------
// <copyright file="LoadController.cs" company="University of Louisville">
//     Copyright (c) 2010 All Rights Reserved
// </copyright>
//-----------------------------------------------------------------------
namespace ObdiiMonitor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Contains functions for loading log data from a file and displaying this data to a graph
    /// </summary>
    public class LoadController
    {
        /// <summary>
        /// Be default, only the first DefaultEndTime ms will be displayed when loaded.  More can be shown by the user after load.
        /// </summary>
        private static uint defaultEndTime = 600000;

        /// <summary>
        /// See Controller.cs
        /// TODO: Possibly explain what this actually does.
        /// </summary>
        private Controller controller;

        /// <summary>
        /// Gets the default end time.
        /// </summary>
        /// <value>The default end time.</value>
        public static uint DefaultEndTime
        {
            get { return defaultEndTime; }
        }

        /// <summary>
        /// Sets the controller.
        /// </summary>
        /// <value>The controller.</value>
        internal Controller Controller
        {
            set { this.controller = value; }
        }

        /// <summary>
        /// Loads the data from the file, interpreting control codes as it reads
        /// </summary>
        /// <param name="data">The data file stream.</param>
        public void LoadData(byte[] data)
        {
            try
            {
                PollResponse response = new PollResponse(this.controller, data);
                this.controller.SensorData.PollResponses.Add(response);
                if (response.DataType == "GP")
                {
                    this.controller.Gps.GpsList.Add(new GPSCoordinate(response));
                }
                else if (response.DataType == "TC")
                {
                    this.controller.TcWindow.Set_Data(response.Time, response.Data);
                }
                else if (response.DataType == "GT")
                {
                    this.controller.TimeOfDayConverter.setBaseTime(response.Time, response.Data);
                }
                else if (response.DataType == "CF")
                {
                    this.controller.Config = response.Data2;
                    this.controller.MainWindow.PopulateSelectionWindow();
                }
            }
            catch (Exception e)
            {
                return;
            }
        }

        /// <summary>
        /// The following function will attempt to open fileName 
        /// and attempt to find sections of the stream that start with with the chosen start tag defined
        /// in PollResponse.cs. If so it subsequently looks for a single byte length and copies the 
        /// bytes from the beginning of the start tag to the the length plus a defined  length of bytes
        /// that will always be there. It then sends that data to be converted into a PollResponse object
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        internal void LoadData(string fileName)
        {
            ConvertSensorData.US = this.controller.US;

            this.controller.Reset();

            // open the FileStream of the fileName
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            // while more to read
            while (fileStream.Position < fileStream.Length)
            {
                // read until it finds the start tag
                while ((fileStream.Position < fileStream.Length) && ((char)fileStream.ReadByte() != PollResponse.StartTag))
                {
                    // Waiting
                }

                // if it's the end of the file break from while l0op
                if (fileStream.Position >= fileStream.Length)
                {
                    break;
                }

                // record the start position of the start tag, this may be changed to index the position right after the start tag
                long start = fileStream.Position - 1;

                // record a single byte integer from the file stream which should be the length of the data portion
                int length = fileStream.ReadByte();

                // initialize a byte array that should hold all the data required to create a PollResponse
                byte[] data = new byte[length + PollResponse.StartTagLength];

                fileStream.Position = start;

                // copy the data starting from the start tag to a length of the length byte plus a defined constant number of bytes
                fileStream.Read(data, 0, length + PollResponse.StartTagLength);

                // send what should be a single poll response to the SensorData.loadData function that will construct a PollResponse object
                // and store them in order in the pollRespnses member, any error checking of a bad data parameter will occur there
                this.controller.LoadController.LoadData(data);
            }

            ArrayList nsIndex = new ArrayList();

            // scan through entire list of sensors looking for multiple NS tags
            for (int i = 0; i < this.controller.SensorData.PollResponses.Count; ++i)
            {
                if (((PollResponse)this.controller.SensorData.PollResponses[i]).DataType == "NS")
                {
                    nsIndex.Add(i);
                }
            }

            // if nsIndex has a count of more than one, separate into separate files.
            if (nsIndex.Count > 1)
            {
                string directoryName = this.controller.MainWindow.GetSplitDirectoryName();

                if (directoryName == string.Empty)
                {
                    return;
                }

                int start = (int)nsIndex[0];
                int fileNme = 0;
                for (int i = 0; i < nsIndex.Count; ++i)
                {
                    while (File.Exists(directoryName + "/" + fileNme))
                    {
                        ++fileNme;
                    }

                    if (i < nsIndex.Count - 1)
                    {
                        this.controller.SaveController.saveData(directoryName + "/" + fileNme, start, (int)nsIndex[i + 1]);
                        start = (int)nsIndex[i + 1];
                    }
                    else
                    {
                        this.controller.SaveController.saveData(directoryName + "/" + fileNme, start, this.controller.SensorData.PollResponses.Count);
                    }
                }

                MessageBox.Show("Data file successfully split up to the path " + directoryName);

                return;
            }

            this.controller.MainWindow.Cursor = Cursors.WaitCursor;

            uint startTime = 0, endTime = 0, totalMs = ((PollResponse)this.controller.SensorData.PollResponses[this.controller.SensorData.PollResponses.Count - 1]).Time;

            if (totalMs < defaultEndTime)
            {
                endTime = totalMs;
            }
            else
            {
                endTime = defaultEndTime;
            }

            this.controller.MainWindow.SetTotalMsLabel(totalMs);
            this.controller.MainWindow.SetStartTimeEndTime(startTime, endTime);

            // now that the data has been loaded into the software
            // create a series of graphs to represent the data
            this.CreateGraphs(this.controller.SensorData.PollResponses);

            // load the data into the graphs just created
            this.controller.MainWindow.LoadDataIntoSensorGraphs();

            // ensure that the size of all graphs are the same so they line up
            this.controller.MainWindow.SetDisplayedGraphRange(startTime, endTime);

            // now that the data has all been loaded, show the panel
            this.controller.MainWindow.ShowSensorDataPanel();

            // show the Reset text on the action button of the window
            this.controller.MainWindow.ShowResetButton();

            this.controller.MainWindow.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Creates the graphs.
        /// </summary>
        /// <param name="pollResponses">The poll responses.</param>
        private void CreateGraphs(ArrayList pollResponses)
        {
            HashSet<string> set = new HashSet<string>();
            ArrayList numsSelected = new ArrayList();

            // get a set of all the pid's in pollResponses that should correspond to a table in SensorController
            foreach (PollResponse response in pollResponses)
            {
                // if the data type is a sensor response from the elm327
                if (response.DataType == "OB")
                {
                    // Add the first two characters of data that should represent the sensor type to a set of types
                    // to generate a graph, one for each sensor type
                    if (response.Data.Length > 1)
                    {
                        set.Add(response.Data.Substring(0, 2));
                    }
                }
                else if (response.DataType == "AC")
                {   
                    // if the data type is the accelerometer
                    set.Add(response.DataType);
                }
            }

            // now compile a list of the index of the pids from the sensors table in SensorController 
            foreach (string str in set)
            {
                for (int i = 0; i < this.controller.SensorController.Sensors.Length; ++i)
                {
                    if (this.controller.SensorController.Sensors[i].Pid == str)
                    {
                        numsSelected.Add(i);
                    }
                }
            }

            numsSelected.Sort();

            // initialize the selectedSensors member of SensorController
            this.controller.SensorController.initializeSelectedSensors(numsSelected);

            // create the initial graphs based off the sensors in selectedSensor member of SensorController
            this.controller.MainWindow.PopulateGraphWindow();
        }
    }
}
