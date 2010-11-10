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

        public Serial()
        {
        }

        public void initialize(string baudRate, string portName)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }

            try
            {
                serialPort.BaudRate = int.Parse(baudRate);
                serialPort.PortName = portName;
                serialPort.DataBits = 8;
                serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "1");
                serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), "0");
                serialPort.ReadTimeout = 1000;
                serialPort.ReadBufferSize = 256;
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
                serialPort.ReadTimeout = 300;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void sendCommand(string command)
        {   
            serialPort.WriteLine(command);
        }
        public void sendConfig()
        {
            serialPort.WriteLine("w"+this.controller.Config);
        }
        public byte[] dataReceived()
        {
            try
            {
                if (serialPort.BytesToRead > 0)
                {
                    byte[] bytes = new byte[serialPort.BytesToRead];

                    serialPort.Read(bytes, 0, serialPort.BytesToRead);

                    return bytes;
                }
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public void flush()
        {
            serialPort.ReadExisting();
        }
    }
}
