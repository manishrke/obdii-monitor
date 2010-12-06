//-----------------------------------------------------------------------
// <copyright file="GPS.cs" company="University of Louisville">
//     Copyright (c) 2010 All Rights Reserved
// </copyright>
//-----------------------------------------------------------------------
namespace ObdiiMonitor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Contains methods for dealing with GPS Coordinates
    /// </summary>
    internal class GPS
    {
        /// <summary>
        /// See Controller.cs
        /// </summary>
        private Controller controller;

        /// <summary>
        /// The list of GPS Coordinates
        /// </summary>
        private ArrayList gpsList = new ArrayList();

        /// <summary>
        /// Sets the controller reference.
        /// </summary>
        /// <value>The controller reference.</value>
        public Controller Controller
        {
            set { this.controller = value; }
        }

        /// <summary>
        /// Gets the list of GPS Coordinates that exist in memory.
        /// </summary>
        /// <value>The list of GPS Coordinates.</value>
        public ArrayList GpsList
        {
            get { return this.gpsList; }
        }

        /// <summary>
        /// Gets the GPS Coordinate based on the specified timestamp
        /// </summary>
        /// <param name="time">The timestamp in ms.</param>
        /// <returns>The closest GPS coordinate less than the timestamp.</returns>
        public GPSCoordinate Get(uint time)
        {
            int length = this.gpsList.Count;

            for (int i = 0; i < length; ++i)
            {
                GPSCoordinate coord = (GPSCoordinate)this.gpsList[i];
                if (coord.Time < time)
                {
                    continue;
                }

                return coord;
            }

            return null;
        }
    }
}
