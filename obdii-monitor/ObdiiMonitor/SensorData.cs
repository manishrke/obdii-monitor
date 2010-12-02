using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace ObdiiMonitor
{
    public class SensorData
    {
        private Controller controller;

        internal Controller Controller
        {
            set { controller = value; }
        }

        ArrayList pollResponses = new ArrayList();

        public ArrayList PollResponses
        {
            get { return pollResponses; }
        }

        internal void clearPollResponses()
        {
            if (pollResponses != null)
                pollResponses.Clear();
            else
                pollResponses = new ArrayList();
        }
    }
}
