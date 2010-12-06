//-----------------------------------------------------------------------
// <copyright file="Sensor.cs" company="University of Louisville">
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
    /// Each sensor in the car is parameterized by the fields in this class.
    /// </summary>
    public class Sensor
    {
        /// <summary>
        /// The label for this sensor
        /// </summary>
        private string label;

        /// <summary>
        /// The pid (e.g., "AC")
        /// </summary>
        private string pid;

        /// <summary>
        /// Not currently implemented
        /// </summary>
        private int bytes;

        /// <summary>
        /// The units for this sensor
        /// </summary>
        private string units;

        /// <summary>
        /// The units met for this sensor
        /// </summary>
        private string unitsMet;

        /// <summary>
        /// The second label
        /// </summary>
        private string label2;

        /// <summary>
        /// Second component of units.
        /// </summary>
        private string units2;

        /// <summary>
        /// Second component of units met.
        /// </summary>
        private string unitsMet2;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sensor"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="pid">The pid (e.g., "AC").</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="units">The units.</param>
        public Sensor(string label, string pid, int bytes, string units)
        {
            this.label = label;
            this.pid = pid;
            this.bytes = bytes;
            this.units = units;
            this.label2 = null;
            this.units2 = null;
            this.unitsMet = units;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sensor"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="pid">The pid (e.g., "AC").</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="units">The units.</param>
        /// <param name="label2">The label2.</param>
        /// <param name="units2">The second component of units.</param>
        public Sensor(string label, string pid, int bytes, string units, string label2, string units2)
        {
            this.label = label;
            this.pid = pid;
            this.bytes = bytes;
            this.units = units;
            this.unitsMet = units;
            this.label2 = label2;
            this.units2 = units2;
            this.unitsMet2 = units2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sensor"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="pid">The pid (e.g., "AC").</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="unitsMet">The units met.</param>
        /// <param name="units">The units.</param>
        public Sensor(string label, string pid, int bytes, string unitsMet, string units)
        {
            this.label = label;
            this.pid = pid;
            this.bytes = bytes;
            this.units = units;
            this.label2 = null;
            this.units2 = null;
            this.unitsMet = unitsMet;
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label of the sensor.</value>
        public string Label
        {
            get { return this.label; }
            set { this.label = value; }
        }

        /// <summary>
        /// Gets or sets the pid
        /// </summary>
        /// <value>The pid (e.g., "AC").</value>
        public string Pid
        {
            get { return this.pid; }
            set { this.pid = value; }
        }

        /// <summary>
        /// Gets or sets the byte length of the data
        /// </summary>
        /// <value>The bytelength.</value>
        public int Bytes
        {
            get { return this.bytes; }
            set { this.bytes = value; }
        }

        /// <summary>
        /// Gets or sets the units for elements in this sensor.
        /// </summary>
        /// <value>The units.</value>
        public string Units
        {
            get { return this.units; }
            set { this.units = value; }
        }

        /// <summary>
        /// Gets or sets the units met.
        /// </summary>
        /// <value>The units met.</value>
        public string UnitsMet
        {
            get { return this.unitsMet; }
            set { this.unitsMet = value; }
        }

        /// <summary>
        /// Gets or sets the second label.
        /// </summary>
        /// <value>The second label.</value>
        public string Label2
        {
            get { return this.label2; }
            set { this.label2 = value; }
        }

        /// <summary>
        /// Gets or sets the second component of units.
        /// </summary>
        /// <value>The second unit component.</value>
        public string Units2
        {
            get { return this.units2; }
            set { this.units2 = value; }
        }

        /// <summary>
        /// Gets or sets the units met2.
        /// </summary>
        /// <value>The units met2.</value>
        public string UnitsMet2
        {
            get { return this.unitsMet2; }
            set { this.unitsMet2 = value; }
        }
    }
}
