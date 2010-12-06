//-----------------------------------------------------------------------
// <copyright file="SensorData.cs" company="University of Louisville">
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

    /// <summary>
    /// Contains methods for managing poll responses in memory
    /// </summary>
    public class SensorData
    {
        /// <summary>
        /// Reference to controller in MVC design pattern
        /// </summary>
        private Controller controller;

        /// <summary>
        /// List of poll responses
        /// </summary>
        private ArrayList pollResponses = new ArrayList();

        /// <summary>
        /// Gets the poll responses.
        /// </summary>
        /// <value>The poll responses.</value>
        public ArrayList PollResponses
        {
            get { return this.pollResponses; }
        }

        /// <summary>
        /// Sets the controller reference.
        /// </summary>
        /// <value>The controller.</value>
        internal Controller Controller
        {
            set { this.controller = value; }
        }

        /// <summary>
        /// Clears the poll responses from memory
        /// </summary>
        internal void ClearPollResponses()
        {
            if (this.pollResponses != null)
            {
                this.pollResponses.Clear();
            }
            else
            {
                this.pollResponses = new ArrayList();
            }
        }
    }
}
