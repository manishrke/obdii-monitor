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

        string units;

        public string Units
        {
            get { return units; }
            set { units = value; }
        }

        string unitsMet;

        public string UnitsMet
        {
            get { return unitsMet; }
            set { unitsMet = value; }
        }

        string label2;

        public string Label2
        {
            get { return label2; }
            set { label2 = value; }
        }

        string units2;

        public string Units2
        {
            get { return units2; }
            set { units2 = value; }
        }

        string unitsMet2;

        public string UnitsMet2
        {
            get { return unitsMet2; }
            set { unitsMet2 = value; }
        }

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
        public Sensor(string label, string pid, int bytes, string units, string unitsMet)
        {
            this.label = label;
            this.pid = pid;
            this.bytes = bytes;
            this.units = units;
            this.label2 = null;
            this.units2 = null;
            this.unitsMet = unitsMet;
        }
    }
}
