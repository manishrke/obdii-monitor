//-----------------------------------------------------------------------
// <copyright file="AccelerometerConverter.cs" company="University of Louisville">
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
    /// Converts 3D accelerometer integer vector to an 1D double, to the appropriate units
    /// </summary>
    public class AccelerometerConverter
    {
        /// <summary>
        /// See Controller.cs
        /// </summary>
        private Controller controller;

        /// <summary>
        /// The intial reading from the accelerometer to calibrate other readings, this assumes that the device
        /// is facing forward and that the car is on a level surface.
        /// </summary>
        private int[] calibrationReading = null;

        /// <summary>
        /// Sets the controller.
        /// </summary>
        /// <value>The controller.</value>
        public Controller Controller
        {
            set { this.controller = value; }
        }

        /// <summary>
        /// sets the calbration reading to zero
        /// </summary>
        public void ResetCalibrationReading()
        {
            this.calibrationReading = null;
        }

        /// <summary>
        /// converts the three calibration integers into one acceleration value
        /// </summary>
        /// <param name="data">The data vector to convert.</param>
        /// <returns>Double representation of the input vector, in the appropriate units</returns>
        public double Convert(byte[] data)
        {
            if (data.Length != 3)
            {
                return -1;
            }

            if (this.calibrationReading == null)
            {
                this.calibrationReading = new int[3];

                for (int i = 0; i < this.calibrationReading.Length; ++i)
                {
                    if (data[i] > 127)
                    {
                        this.calibrationReading[i] = -256 + data[i];
                    }
                    else
                    {
                        this.calibrationReading[i] = data[0];
                    }
                }
            }

            int[] values = new int[3];

            for (int i = 0; i < values.Length; ++i)
            {
                if (data[i] > 127)
                {
                    values[i] = -256 + data[i];
                }
                else
                {
                    values[i] = data[0];
                }

                values[i] -= this.calibrationReading[i];

                values[i] *= 24;
            }

            // calculate the value for m/s^2 by squaring all adjusted values and taking square root
            double d = Math.Sqrt(((values[0] * values[0]) + (values[1] * values[1])) + (values[2] * values[2])) * (9.81 / 1000);
            
            // get rid of all freakishly high values
            if (d > 10)
            {
                return 0;
            }

            return d;
        }
    }
}
