using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ObdiiMonitor
{
    class SaveController
    {
        private Controller controller;

        internal Controller Controller
        {
            set { controller = value; }
        }

        // the following function will convert in order the PollResponse objects in SensorData.PollResponses to byte arrays
        // and write the byte arrays to a file.
        internal void saveData(string fileName, int start, int end)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            Stream output = File.OpenWrite(fileName);

            for (int i = start; i < end; ++i)
            {
                output.Write(((PollResponse)controller.SensorData.PollResponses[i]).ToBytes(), 0, ((PollResponse)controller.SensorData.PollResponses[i]).ToBytes().Length);
            }

            output.Close();
        }
    }
}
