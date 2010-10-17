// <copyright file="LoadController.cs" company="University of Louisville">
// Copyright (c) 2010 All Rights Reserved
// </copyright>
// <author>Bradley Schoch</author>
// <author>Nicholas Bell</author>
// <date>2010-10-16</date>
// <summary>Contains functions for loading log data from a file and displaying this data to a graph</summary>
namespace ObdiiMonitor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Contains functions for loading log data from a file and displaying this data to a graph
    /// </summary>
    public class LoadController
    {
        /// <summary>
        /// See Controller.cs
        /// TODO: Possibly explain what this actually does.
        /// </summary>
        private Controller controller;

        /// <summary>
        /// Sets the controller.
        /// </summary>
        /// <value>The controller.</value>
        internal Controller Controller
        {
            set { this.controller = value; }
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
            // clear the pollResponses ArrayList member in SensorData to begin loading in new data
            this.controller.SensorData.clearPollResponses();

            // Clear the GraphQueue, not sure if the GraphQueue will stick around.
            this.controller.MainWindow.GraphQueue.Clear();

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
                this.controller.SensorData.loadData(data);
            }

            // now that the data has been loaded into the software
            // create a series of graphs to represent the data
            this.CreateGraphs(this.controller.SensorData.PollResponses);

            // load the data into the graphs just created
            this.controller.MainWindow.loadDataIntoSensorGraphs();

            // now that the data has all been loaded, show the panel
            this.controller.MainWindow.showSensorDataPanel();

            // show the Reset text on the action button of the window
            this.controller.MainWindow.showResetButton();
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

            // initialize the selectedSensors member of SensorController
            this.controller.SensorController.initializeSelectedSensors(numsSelected);

            // create the initial graphs based off the sensors in selectedSensor member of SensorController
            this.controller.MainWindow.populateGraphWindow(numsSelected);
        }
    }
}
