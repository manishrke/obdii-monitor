//-----------------------------------------------------------------------
// <copyright file="SensorController.cs" company="University of Louisville">
//     Copyright (c) 2010 All Rights Reserved
// </copyright>
//-----------------------------------------------------------------------
namespace ObdiiMonitor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Contains hard data on the sensors and will receive data from the elm327.
    /// </summary>
    public class SensorController
    {
        /// <summary>
        /// Receiving thread
        /// </summary>
        internal Thread Receiving;

        /// <summary>
        /// Reference to controller in MVC design pattern
        /// </summary>
        private Controller controller;

        /// <summary>
        /// Hard-coded information on each sensor
        /// </summary>
        private Sensor[] sensors = 
        {
            new Sensor("Acceleration:", "AC", 3, "m/s^2"),
            new Sensor("Engine RPM:", "0C", 2, "rpm"),
            new Sensor("Vehicle Speed:", "0D", 1, "km/h", "mph"),
            new Sensor("Calculated Load value:", "04", 1, "%"),
            new Sensor("Engine Coolant Temp:",   "05", 1, "°C", "°F"),
            new Sensor("Fuel Pressure:", "0A", 1, "kPa (gauge)", "psi"),
            new Sensor("Intake Manifold Abs. Pressure:", "0B", 1, "kPa (absolute)", "inHg"),
            new Sensor("Timing advance", "0E", 1, "° from #1 cylinder"),
            new Sensor("Intake air temperature", "0F", 1, "°C", "°F"),
            new Sensor("MAF air flow rate", "10", 2, "g/s", "lb/min"),
            new Sensor("Throttle position", "11", 1, "%"),
            new Sensor("Short term % trim Bank 1:", "06", 1, "%"),
            new Sensor("Long term % trim Bank 1:", "07", 1, "%"),
            new Sensor("Short term % trim Bank 2:", "08", 1, "%"),
            new Sensor("Long term % trim Bank 2:", "09", 1, "%"),
            new Sensor("Bank 1, Sensor 1: Oxygen sensor voltage", "14", 2, "Volts", "Bank 1, Sensor 1: Short term fuel trim,", "%"),
            new Sensor("Bank 1, Sensor 2: Oxygen sensor voltage", "15", 2, "Volts", "Bank 1, Sensor 2: Short term fuel trim,", "%"),
            new Sensor("Bank 1, Sensor 3: Oxygen sensor voltage", "16", 2, "Volts", "Bank 1, Sensor 3: Short term fuel trim,", "%"),
            new Sensor("Bank 1, Sensor 4: Oxygen sensor voltage", "17", 2, "Volts", "Bank 1, Sensor 4: Short term fuel trim,", "%"),
            new Sensor("Bank 2, Sensor 1: Oxygen sensor voltage", "18", 2, "Volts", "Bank 2, Sensor 1: Short term fuel trim,", "%"),
            new Sensor("Bank 2, Sensor 2: Oxygen sensor voltage", "19", 2, "Volts", "Bank 2, Sensor 2: Short term fuel trim,", "%"),
            new Sensor("Bank 2, Sensor 3: Oxygen sensor voltage", "1A", 2, "Volts", "Bank 2, Sensor 3: Short term fuel trim,", "%"),
            new Sensor("Bank 2, Sensor 4: Oxygen sensor voltage", "1B", 2, "Volts", "Bank 2, Sensor 4: Short term fuel trim,", "%"),
            new Sensor("Run time since engine start", "1F", 2, "seconds"),
            new Sensor("Distance traveled with malfunction indicator lamp (MIL) on", "21", 2, "km", "miles"),
            new Sensor("Fuel Rail Pressure (relative to manifold vacuum)", "22", 2, "kPa", "psi"),
            new Sensor("Fuel Rail Pressure (diesel)", "23", 2, "kPa (gauge)", "psi"),
            new Sensor("O2S1_WR_lambda(1): Equivalence Ratio", "24", 4, string.Empty, "O2S1_WR_lambda(1): Voltage", "V"),
            new Sensor("O2S2_WR_lambda(1): Equivalence Ratio", "25", 4, string.Empty, "O2S2_WR_lambda(1): Voltage", "V"),
            new Sensor("O2S3_WR_lambda(1): Equivalence Ratio", "26", 4, string.Empty, "O2S3_WR_lambda(1): Voltage", "V"),
            new Sensor("O2S4_WR_lambda(1): Equivalence Ratio", "27", 4, string.Empty, "O2S4_WR_lambda(1): Voltage", "V"),
            new Sensor("O2S5_WR_lambda(1): Equivalence Ratio", "28", 4, string.Empty, "O2S5_WR_lambda(1): Voltage", "V"),
            new Sensor("O2S6_WR_lambda(1): Equivalence Ratio", "29", 4, string.Empty, "O2S6_WR_lambda(1): Voltage", "V"),
            new Sensor("O2S7_WR_lambda(1): Equivalence Ratio", "2A", 4, string.Empty, "O2S7_WR_lambda(1): Voltage", "V"),
            new Sensor("O2S8_WR_lambda(1): Equivalence Ratio", "2B", 4, string.Empty, "O2S8_WR_lambda(1): Voltage", "V"),
            new Sensor("Commanded EGR", "2C", 1, "%"),
            new Sensor("EGR Error", "2D", 1, "%"),
            new Sensor("Commanded evaporative purge", "2E", 1, "%"),
            new Sensor("Fuel Level Input", "2F", 1, "%"),
            new Sensor("# of warm-ups since codes cleared", "30", 1, string.Empty),
            new Sensor("Distance traveled since codes cleared", "31", 2, "km", "miles"),
            new Sensor("Evap. System Vapor Pressure", "32", 2, "Pa", "H2O"),
            new Sensor("Barometric pressure", "33", 1, "kPa (Absolute)", "inHg"),
            new Sensor("O2S1_WR_lambda(1): Equivalence Ratio", "34", 4, string.Empty, "O2S1_WR_lambda(1): Current", "mA"),
            new Sensor("O2S2_WR_lambda(1): Equivalence Ratio", "35", 4, string.Empty, "O2S2_WR_lambda(1): Current", "mA"),
            new Sensor("O2S3_WR_lambda(1): Equivalence Ratio", "36", 4, string.Empty, "O2S3_WR_lambda(1): Current", "mA"),
            new Sensor("O2S4_WR_lambda(1): Equivalence Ratio", "37", 4, string.Empty, "O2S4_WR_lambda(1): Current", "mA"),
            new Sensor("O2S5_WR_lambda(1): Equivalence Ratio", "38", 4, string.Empty, "O2S5_WR_lambda(1): Current", "mA"),
            new Sensor("O2S6_WR_lambda(1): Equivalence Ratio", "39", 4, string.Empty, "O2S6_WR_lambda(1): Current", "mA"),
            new Sensor("O2S7_WR_lambda(1): Equivalence Ratio", "3A", 4, string.Empty, "O2S7_WR_lambda(1): Current", "mA"),
            new Sensor("O2S8_WR_lambda(1): Equivalence Ratio", "3B", 4, string.Empty, "O2S8_WR_lambda(1): Current", "mA"),
            new Sensor("Catalyst Temperature Bank 1, Sensor 1", "3C", 2, "°C", "°F"),
            new Sensor("Catalyst Temperature Bank 2, Sensor 1", "3D", 2, "°C", "°F"),
            new Sensor("Catalyst Temperature Bank 1, Sensor 2", "3E", 2, "°C", "°F"),
            new Sensor("Catalyst Temperature Bank 2, Sensor 2", "3F", 2, "°C", "°F"),
            new Sensor("Control module voltage", "42", 2, "V"),
            new Sensor("Absolute load value", "43", 2, "%"),
            new Sensor("Command equivalence ratio", "44", 2, string.Empty),
            new Sensor("Relative throttle position", "45", 1, "%"),
            new Sensor("Ambient air temperature", "46", 1, "°C", "°F"),
            new Sensor("Absolute throttle position B", "47", 1, "%"),
            new Sensor("Absolute throttle position C", "48", 1, "%"),
            new Sensor("Accelerator pedal position D", "49", 1, "%"),
            new Sensor("Accelerator pedal position E", "4A", 1, "%"),
            new Sensor("Accelerator pedal position F", "4B", 1, "%"),
            new Sensor("Commanded throttle actuator", "4C", 1, "%"),
            new Sensor("Time run with MIL on", "4D", 2, "minutes"),
            new Sensor("Time since trouble codes cleared", "4E", 2, "minutes"),
            new Sensor("Ethanol fuel %", "52", 1, "%"),
            new Sensor("Absoulute Evap system Vapour Pressure", "53", 2, "kpa", "psi"),                             
        };

        /// <summary>
        /// Array of sensors selected for data collection
        /// </summary>
        private Sensor[] selectedSensors;

        /// <summary>
        /// Gets the sensors.
        /// </summary>
        /// <value>The sensors.</value>
        public Sensor[] Sensors
        {
            get { return this.sensors; }
        }

        /// <summary>
        /// Gets the selected sensors.
        /// </summary>
        /// <value>The selected sensors.</value>
        public Sensor[] SelectedSensors
        {
            get { return this.selectedSensors; }
        }

        /// <summary>
        /// Sets the controller reference.
        /// </summary>
        /// <value>The controller reference.</value>
        internal Controller Controller
        {
            set { this.controller = value; }
        }

        /// <summary>
        /// initializes an array of the sensors that will either be polled or the results from prior polling
        /// will be displayed in graphs
        /// </summary>
        /// <param name="numsSelected">The nums selected.</param>
        public void InitializeSelectedSensors(ArrayList numsSelected)
        {
            this.selectedSensors = new Sensor[numsSelected.Count];

            for (int i = 0; i < numsSelected.Count; ++i)
            {
                this.selectedSensors[i] = this.sensors[(int)numsSelected[i]];
            }
        }

        /// <summary>
        /// Initializes the receiving threads.
        /// </summary>
        public void InitializeReceivingThreads()
        {
            this.Receiving = new Thread(new ThreadStart(this.BeginReceiving));

            this.Receiving.Name = "Polling";

            this.Receiving.Start();
        }

        /// <summary>
        /// Returns either the US or metric unit of index i of SelectedSensors
        /// </summary>
        /// <param name="i">the index of SelectedSensors whose unit is required</param>
        /// <returns>The unit in US or metric</returns>
        internal string ReturnUnit(int i)
        {
            if (this.controller.SensorController.SelectedSensors == null || this.controller.SensorController.SelectedSensors[i] == null)
            {
                return string.Empty;
            }

            if (!this.controller.US && this.controller.SensorController.SelectedSensors[i].UnitsMet != null)
            {
                return this.controller.SensorController.SelectedSensors[i].UnitsMet;
            }
            else if (this.controller.SensorController.SelectedSensors[i].Units != null)
            {
                return this.controller.SensorController.SelectedSensors[i].Units;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// This both polls and receives data from the elm 327.
        /// It will timeout after 300 ms and send a new command.
        /// </summary>
        private void BeginReceiving()
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            while (true)
            {
                try
                {
                    byte[] buffer = this.controller.Serial.DataReceived();

                    if (buffer == null)
                    {
                        continue;
                    }

                    while (buffer.Contains((byte)PollResponse.StartTag))
                    {
                        if (buffer.Length < 2)
                        {
                            break;
                        }

                        int index = Array.IndexOf(buffer, (byte)PollResponse.StartTag);

                        byte length = buffer[index + 1];

                        if (buffer.Length < length + 2)
                        {
                            break;
                        }

                        byte[] data = new byte[length + 2];
                        Array.Copy(buffer, index, data, 0, length + 2);
                        PollResponse response = new PollResponse(this.controller, data);
                        this.controller.SensorData.PollResponses.Add(response);
                        if (response.DataType == "MK")
                        {
                            this.controller.MainWindow.AddGraphHighlight(response.Time);
                        }

                        if (response.DataType == "TC")
                        {
                            this.controller.TcWindow.Set_Data(response.Time, response.Data);
                        }

                        this.controller.MainWindow.GraphQueue.Enqueue(response);
                        data = new byte[buffer.Length - length - 2];
                        Array.Copy(buffer, index + length + 2, data, 0, buffer.Length - length - 2 - index);
                        buffer = data;
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
