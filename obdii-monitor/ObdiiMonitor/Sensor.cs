using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObdiiMonitor
{
    public class Sensor
    {
        string label;

        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        string pid;

        public string Pid
        {
            get { return pid; }
            set { pid = value; }
        }

        int bytes;

        public int Bytes
        {
            get { return bytes; }
            set { bytes = value; }
        }

        public Sensor(string label, string pid, int bytes)
        {
            this.label = label;
            this.pid = pid;
            this.bytes = bytes;
        }
    }
}
