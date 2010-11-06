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

        double max;

        public double Max
        {
            get { return max; }
            set { max = value; }
        }

        double min;

        public double Min
        {
            get { return min; }
            set { min = value; }
        }

        string units;

        public string Units
        {
            get { return units; }
            set { units = value; }
        }

        string label2;

        public string Label2
        {
            get { return label2; }
            set { label2 = value; }
        }


        double max2;

        public double Max2
        {
            get { return max2; }
            set { max2 = value; }
        }

        double min2;

        public double Min2
        {
            get { return min2; }
            set { min2 = value; }
        }

        string units2;

        public string Units2
        {
            get { return units2; }
            set { units2 = value; }
        }

        public Sensor(string label, string pid, int bytes, double min, double max, string units)
        {
            this.label = label;
            this.pid = pid;
            this.bytes = bytes;
            this.min = min;
            this.max = max;
            this.units = units;
            this.label2 = null;
            this.units2 = null;
        }
        public Sensor(string label, string pid, int bytes, double min, double max, string units, string label2, double min2, double max2, string units2)
        {
            this.label = label;
            this.pid = pid;
            this.bytes = bytes;
            this.min = min;
            this.max = max;
            this.units = units;
            this.label2 = label2;
            this.min2 = min2;
            this.max2 = max2;
            this.units2 = units2;
        }
    }
}
