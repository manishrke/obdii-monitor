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

        internal void saveData(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            Stream output = File.OpenWrite(fileName);

            foreach (PollResponse response in controller.SensorData.PollResponses)
            {
                Stream input = response.ToStream();

                byte[] buffer = new byte[8 * 64];
                int len;
                while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, len);
                }
            }

            output.Close();
        }
    }
}
