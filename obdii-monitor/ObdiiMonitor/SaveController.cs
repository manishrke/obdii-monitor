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
        internal void saveData(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            Stream output = File.OpenWrite(fileName);

            foreach (PollResponse response in controller.SensorData.PollResponses)
            {
                output.Write(response.ToBytes(), 0, response.ToBytes().Length);
            }

            output.Close();
        }
    }
}
