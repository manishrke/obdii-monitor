//-----------------------------------------------------------------------
// <copyright file="Serial.cs" company="University of Louisville">
//     Copyright (c) 2010 All Rights Reserved
// </copyright>
//-----------------------------------------------------------------------
namespace ObdiiMonitor
{
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Contains code to interface with the serial port
    /// </summary>
    public class Serial
    {
        /// <summary>
        /// Controller in MVC design pattern
        /// </summary>
        private Controller controller;

        /// <summary>
        /// Reference to serial port where data will be received
        /// </summary>
        private SerialPort serialPort = new SerialPort();

        /// <summary>
        /// Initializes a new instance of the <see cref="Serial"/> class.
        /// </summary>
        public Serial()
        {
        }

        /// <summary>
        /// Sets the controller reference.
        /// </summary>
        /// <value>The controller reference.</value>
        public Controller Controller
        {
            set { this.controller = value; }
        }

        /// <summary>
        /// Initializes the port using the specified port name.
        /// </summary>
        /// <param name="portName">Name of the port.</param>
        public void Initialize(string portName)
        {
            if (this.serialPort.IsOpen)
            {
                this.serialPort.Close();
            }

            try
            {
                this.serialPort.BaudRate = 38400;
                this.serialPort.PortName = portName;
                this.serialPort.DataBits = 8;
                this.serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "1");
                this.serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), "0");
                this.serialPort.ReadTimeout = 1000;
                this.serialPort.ReadBufferSize = 256;
                this.serialPort.Open();
                this.serialPort.ReadTimeout = 300;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sends the command over the port.
        /// </summary>
        /// <param name="command">The command to send.</param>
        public void SendCommand(string command)
        {
            if (this.serialPort.IsOpen)
            {
                this.serialPort.WriteLine(command);
            }
        }

        /// <summary>
        /// Sends the config file over the serial port
        /// </summary>
        public void SendConfig()
        {
            if (this.serialPort.IsOpen)
            {
                this.serialPort.Write("w");
                Thread.Sleep(5);
                for (int i = 0; i < this.controller.Config.Length; ++i)
                {
                    this.serialPort.Write(this.controller.Config, i, 1);
                    Thread.Sleep(5);
                }
            }   
        }

        /// <summary>
        /// Attempts to get received data
        /// </summary>
        /// <returns>The data received, if available. Null otherwise.</returns>
        public byte[] DataReceived()
        {
            int num = 0;
            try
            {
                if ((num = this.serialPort.BytesToRead) > 0)
                {
                    byte[] bytes = new byte[num];

                    this.serialPort.Read(bytes, 0, num);

                    return bytes;
                }

                    return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
            this.serialPort.ReadExisting();
        }
    }
}
