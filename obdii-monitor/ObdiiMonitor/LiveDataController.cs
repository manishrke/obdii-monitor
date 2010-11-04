using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObdiiMonitor
{
    class LiveDataController
    {
        private Controller controller;

        public Controller Controller
        {
            set { controller = value; }
        }
    }
}
