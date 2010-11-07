using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace ObdiiMonitor
{
    public class Serial
    {
        Controller controller;

        public Controller Controller
        {
            set { controller = value; }
        }

        private SerialPort serialPort = new SerialPort();

        bool initialized = false;

        public bool Initialized
        {
            get { return initialized; }
        }

        public Serial()
        {
        }

        public void initialize(string baudRate, string portName)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                initialized = false;
            }

            try
            {
                serialPort.BaudRate = int.Parse(baudRate);
                serialPort.PortName = portName;
                serialPort.DataBits = 8;
                serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "1");
                serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), "0");
                serialPort.ReadTimeout = 1000;
                serialPort.Open();
 /*               // initialize the elm
                serialPort.WriteLine("ATZ\r");
                while (serialPort.ReadChar() != '>')
                    ;
                // turn echo off
                serialPort.WriteLine("ATE0\r");
                while (serialPort.ReadChar() != '>')
                    ;
                serialPort.WriteLine("ATS0\r");
                while (serialPort.ReadChar() != '>')
                    ;
                // turn line feeds on
                serialPort.WriteLine("ATL1\r");*/
                initialized = true;
                serialPort.ReadTimeout = 300;
            }
            catch (Exception ex)
            {
                initialized = false;
                throw ex;
            }
        }

        public void sendCommand(string command)
        {
            if (!initialized)
                return;
            
            serialPort.WriteLine(command + "\r");
        }
        public void sendConfig()
        {
            if (!initialized)
                return;
            serialPort.WriteLine("w"+this.controller.Config + "\r");
        }
        public string dataReceived()
        {
            if (!initialized)
                return "";

            return serialPort.ReadLine();
        }
    }
}
