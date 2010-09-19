using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScanTool
{
    class SensorData
    {
        private Controller controller;

        internal Controller Controller
        {
            set { controller = value; }
        }

        public void parseData(string data)
        {
            Console.WriteLine(data);
        }
    }
}
