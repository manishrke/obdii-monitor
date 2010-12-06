//-----------------------------------------------------------------------
// <copyright file="GPSCoordinate.cs" company="University of Louisville">
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
    /// Contains information for using Coordinates read from a GPS.
    /// </summary>
    internal class GPSCoordinate
    {
        /// <summary>
        /// The string representation of the horizontal position on Earth (In degrees West or East)
        /// </summary>
        private string horizontal;

        /// <summary>
        /// The string representation of the horizontal position on Earth (In degrees North or South)
        /// </summary>
        private string vertical;

        /// <summary>
        /// Millisecond representation of the time
        /// </summary>
        private uint time;

        /// <summary>
        /// Initializes a new instance of the <see cref="GPSCoordinate"/> class.
        /// </summary>
        /// <param name="time">The time in ms.</param>
        /// <param name="horizontal">The formatted horizontal position in degrees W or E.</param>
        /// <param name="vertical">The formatted vertical position in degrees N or S.</param>
        public GPSCoordinate(uint time, string horizontal, string vertical)
        {
            this.time = time;
            this.horizontal = horizontal;
            this.vertical = vertical;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GPSCoordinate"/> class.
        /// </summary>
        /// <param name="time">The time in ms.</param>
        /// <param name="coordinates">The coordinates as read from the microcontroller.</param>
        public GPSCoordinate(uint time, string coordinates)
        {
            this.vertical = Convert.ToString(Math.Round(Convert.ToDouble(coordinates.Substring(0, 2)) + (Convert.ToDouble(coordinates.Substring(2, 7)) / 60.0), 6)) + coordinates.Substring(9, 1);

            this.horizontal = Convert.ToString(Math.Round(Convert.ToDouble(coordinates.Substring(10, 3)) + (Convert.ToDouble(coordinates.Substring(13, 7)) / 60.0), 6)) + coordinates.Substring(20, 1);

            this.time = time;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GPSCoordinate"/> class.
        /// </summary>
        /// <param name="response">The response from polling.</param>
        public GPSCoordinate(PollResponse response)
            : this(response.Time, response.Data)
        {
        }
        
        /// <summary>
        /// Gets a string represntation of the horizontal GPS position.
        /// </summary>
        /// <value>The horizontal position.</value>
        public string Horizontal
        {
            get { return this.horizontal; }
        }

        /// <summary>
        /// Gets a string represntation of the vertical GPS position.
        /// </summary>
        /// <value>The vertical position.</value>
        public string Vertical
        {
            get { return this.vertical; }
        }

        /// <summary>
        /// Gets the time in milliseconds of this GPS coordinate.
        /// </summary>
        /// <value>The time in ms.</value>
        public uint Time
        {
            get { return this.time; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// Overloaded to print Vertical and Horizontal position with a space between.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.vertical + " " + this.horizontal;
        }
    }
}
