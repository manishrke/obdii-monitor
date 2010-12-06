//-----------------------------------------------------------------------
// <copyright file="SaveController.cs" company="University of Louisville">
//     Copyright (c) 2010 All Rights Reserved
// </copyright>
//-----------------------------------------------------------------------
namespace ObdiiMonitor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Oversees saving to a log file
    /// </summary>
    public class SaveController
    {
        /// <summary>
        /// Reference to the controller in MVC design pattern.
        /// </summary>
        private Controller controller;

        /// <summary>
        /// Sets the controller reference.
        /// </summary>
        /// <value>The controller reference.</value>
        internal Controller Controller
        {
            set { this.controller = value; }
        }

        /// <summary>
        /// Converts, in order, the PollResponse objects in SensorData.PollResponses to byte arrays
        /// and write the byte arrays to a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        internal void SaveData(string fileName, int start, int end)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            Stream output = File.OpenWrite(fileName);

            for (int i = start; i < end; ++i)
            {
                output.Write(((PollResponse)this.controller.SensorData.PollResponses[i]).ToBytes(), 0, ((PollResponse)this.controller.SensorData.PollResponses[i]).ToBytes().Length);
            }

            output.Close();
        }
    }
}
