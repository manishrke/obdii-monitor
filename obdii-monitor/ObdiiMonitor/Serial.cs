using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

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

        public void initialize(string portName)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }

            try
            {
                serialPort.BaudRate = 38400;
                serialPort.PortName = portName;
                serialPort.DataBits = 8;
                serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "1");
                serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), "0");
                serialPort.ReadTimeout = 1000;
                serialPort.ReadBufferSize = 256;
                serialPort.Open();
                serialPort.ReadTimeout = 300;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void sendCommand(string command)
        {   
            if (serialPort.IsOpen)
                serialPort.WriteLine(command);
        }
        public void sendConfig()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write("w");
                Thread.Sleep(5);
                for (int i = 0; i < controller.Config.Length; ++i)
                {
                    serialPort.Write(controller.Config, i, 1);
                    Thread.Sleep(5);
                }
            }   
        }
        public byte[] dataReceived()
        {
            int num = 0;
            try
            {
                if ((num = serialPort.BytesToRead) > 0)
                {
                    byte[] bytes = new byte[num];

                    serialPort.Read(bytes, 0, num);

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
