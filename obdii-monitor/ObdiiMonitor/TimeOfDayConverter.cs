//-----------------------------------------------------------------------
// <copyright file="TimeOfDayConverter.cs" company="University of Louisville">
//     Copyright (c) 2010 All Rights Reserved
// </copyright>
//-----------------------------------------------------------------------
namespace ObdiiMonitor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Converts the millisecond count to the actual date and time, given a base time maintained by the class
    /// </summary>
    public class TimeOfDayConverter
    {
        /// <summary>
        /// Controller in MVC Design Pattern
        /// </summary>
        private Controller controller;

        /// <summary>
        /// DateTime instance used to convert
        /// </summary>
        private DateTime dateTime = new DateTime();

        /// <summary>
        /// Sets the controller reference.
        /// </summary>
        /// <value>The controller reference.</value>
        public Controller Controller
        {
            set { this.controller = value; }
        }

        /// <summary>
        /// Sets the base time.
        /// </summary>
        /// <param name="time">The time already elapsed.</param>
        /// <param name="data">The data directly from the microcontroller containing current time.</param>
        public void SetBaseTime(uint time, string data)
        {
            if (data.Length != 12)
            {
                return;
            }

            int hour, minutes, seconds, month, day, year;

            try
            {
                hour = int.Parse(data.Substring(0, 2));
                minutes = int.Parse(data.Substring(2, 2));
                seconds = int.Parse(data.Substring(4, 2));
                month = int.Parse(data.Substring(6, 2));
                day = int.Parse(data.Substring(8, 2));
                year = int.Parse(data.Substring(10, 2));
            }
            catch 
            {
                return;
            }

            this.dateTime = new DateTime(year + 2000, month, day, hour, minutes, seconds);

            this.dateTime = this.dateTime.Subtract(new TimeSpan(0, 0, 0, 0, (int)time));
        }

        /// <summary>
        /// Gets a human readable representation of the specified time.
        /// </summary>
        /// <param name="time">The time in ms.</param>
        /// <returns>The human readable representation of the time</returns>
        public string Get(double time)
        {
            return this.dateTime.AddMilliseconds(time).ToString("MM/dd/yyyy HH:mm:ss.fff");
        }
    }
}
